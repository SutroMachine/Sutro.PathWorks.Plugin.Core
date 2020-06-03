using g3;
using gs;
using gs.FillTypes;
using Sutro.Core.Models.GCode;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public abstract class DecompilerBase<TPrintVertex> where TPrintVertex : IToolpathVertex
    {
        protected int currentLayerIndex = 0;
        protected IFillType currentFillType;
        protected TPrintVertex currentVertex;
        protected TPrintVertex previousVertex;
        protected LinearToolpath3<PrintVertex> toolpath;
        protected bool extruderRelativeCoordinates = false;

        public event Action<IToolpath> OnToolpathComplete;

        public event Action<int> OnNewLayer;

        protected abstract Dictionary<string, IFillType> FillTypes { get; }

        protected Regex fillTypeLabelPattern => new Regex(@"feature (.+)$");

        public abstract void Begin();

        protected void EmitNewLayer(int layerIndex)
        {
            OnNewLayer?.Invoke(layerIndex);
        }

        protected void SetExtrusionCoordinateMode(GCodeLine line)
        {
            if (line.Type == LineType.MCode)
            {
                if (line.Code == 82)
                {
                    extruderRelativeCoordinates = false;
                }
                else if (line.Code == 83)
                {
                    extruderRelativeCoordinates = true;
                }
            }
        }

        protected virtual Vector2d ExtractDimensions(GCodeLine line, Vector2d dimensions)
        {
            double width = dimensions.x;
            double height = dimensions.y;

            if (line.Comment != null && line.Comment.Contains("tool"))
            {
                foreach (var word in line.Comment.Split(' '))
                {
                    int i = word.IndexOf('W');
                    if (i >= 0)
                        width = double.Parse(word.Substring(i + 1));
                    i = word.IndexOf('H');
                    if (i >= 0)
                        height = double.Parse(word.Substring(i + 1));
                }
            }

            return new Vector2d(width, height);
        }

        protected virtual double ExtractExtrusion(GCodeLine line, double previousExtrusion)
        {
            if (line.Parameters != null)
            {
                foreach (var param in line?.Parameters)
                {
                    if (param.Identifier == "E")
                        return param.DoubleValue;
                }
            }
            return previousExtrusion;
        }

        protected virtual double ExtractFeedRate(GCodeLine line, double previousFeedrate)
        {
            if (line.Parameters != null)
            {
                foreach (var param in line?.Parameters)
                {
                    if (param.Identifier == "F")
                        return param.DoubleValue;
                }
            }
            return previousFeedrate;
        }

        protected virtual Vector3d ExtractPosition(GCodeLine line, Vector3d previousPosition)
        {
            double x = previousPosition.x;
            double y = previousPosition.y;
            double z = previousPosition.z;

            if (line.Parameters != null)
            {
                foreach (var param in line?.Parameters)
                {
                    switch (param.Identifier)
                    {
                        case "X":
                            x = param.DoubleValue;
                            break;

                        case "Y":
                            y = param.DoubleValue;
                            break;

                        case "Z":
                            z = param.DoubleValue;
                            break;
                    }
                }
            }
            return new Vector3d(x, y, z);
        }

        protected bool LineIsNewFillTypeComment(GCodeLine line, out IFillType fillType)
        {
            if (line.Comment != null)
            {
                var match = fillTypeLabelPattern.Match(line.Comment);
                if (match.Success)
                {
                    if (!FillTypes.TryGetValue(match.Groups[1].Captures[0].Value, out fillType))
                        fillType = new DefaultFillType();
                    return true;
                }
            }

            fillType = null;
            return false;
        }

        protected static bool LineIsEmpty(GCodeLine line)
        {
            return line == null || line.Type == LineType.Blank;
        }

        protected static bool LineIsNewLayerComment(GCodeLine line)
        {
            return line.Comment != null && line.Comment.Contains("layer") && !line.Comment.Contains("feature");
        }

        public void End()
        {
            FinishToolpath();
        }

        protected abstract LinearToolpath3<TPrintVertex> FinishToolpath();

        protected bool LineIsTravel(GCodeLine line)
        {
            return line?.Comment?.Contains("Travel") ?? false;
        }

        protected void EmitToolpath(LinearToolpath3<PrintVertex> travel)
        {
            OnToolpathComplete(travel);
        }

        public abstract void ProcessGCodeLine(GCodeLine line);
    }

    public class DecompilerFFF : DecompilerBase<PrintVertex>
    {
        protected override Dictionary<string, IFillType> FillTypes => new Dictionary<string, IFillType>()
        {
            {DefaultFillType.Label, new DefaultFillType() },
            {OuterPerimeterFillType.Label, new OuterPerimeterFillType(new SingleMaterialFFFSettings()) },
            {InnerPerimeterFillType.Label, new InnerPerimeterFillType(new SingleMaterialFFFSettings()) },
            {OpenShellCurveFillType.Label, new OpenShellCurveFillType()},
            {SolidFillType.Label, new SolidFillType(new SingleMaterialFFFSettings().SolidFillSpeedX)},
            {SparseFillType.Label, new SparseFillType()},
            {SupportFillType.Label, new SupportFillType(new SingleMaterialFFFSettings())},
            {BridgeFillType.Label, new BridgeFillType(new SingleMaterialFFFSettings())},
        };

        public override void Begin()
        {
            previousVertex = new PrintVertex(Vector3d.Zero, 0, Vector2d.Zero);
        }

        public override void ProcessGCodeLine(GCodeLine line)
        {
            if (LineIsEmpty(line))
                return;

            SetExtrusionCoordinateMode(line);
            currentVertex = UpdatePrintVertex(line, previousVertex);

            if (LineIsNewLayerComment(line))
            {
                toolpath = FinishToolpath();
                EmitNewLayer(currentLayerIndex++);
            }

            if (LineIsTravel(line))
            {
                CloseToolpathAndAddTravel(previousVertex, currentVertex);
            }

            if (LineIsNewFillTypeComment(line, out var newFillType))
            {
                toolpath = FinishToolpath();
                toolpath.Type = ToolpathTypes.Deposition;
                toolpath.FillType = newFillType;
            }

            if (line.Comment?.Contains("Plane Change") ?? false)
            {
                toolpath.Type = ToolpathTypes.PlaneChange;
            }

            if (line.Type == LineType.GCode)
            {
                if (VertexHasNegativeOrZeroExtrusion(previousVertex, currentVertex))
                {
                    CloseToolpathAndAddTravel(previousVertex, currentVertex);
                }
                else if (toolpath != null)
                {
                    AppendVertexToCurrentToolpath(currentVertex);
                }
            }

            previousVertex = currentVertex;
        }

        private void AppendVertexToCurrentToolpath(PrintVertex vertex)
        {
            if (toolpath.VertexCount == 1)
            {
                var modifiedFirstVertex = new PrintVertex(toolpath.StartPosition, vertex.FeedRate, vertex.Dimensions, vertex.Extrusion.x);
                modifiedFirstVertex.ExtendedData = vertex.ExtendedData;
                modifiedFirstVertex.Source = vertex.Source;
                toolpath.UpdateVertex(0, modifiedFirstVertex);
            }
            toolpath.AppendVertex(vertex, TPVertexFlags.None);

        }

        private void CloseToolpathAndAddTravel(PrintVertex previousVertex, PrintVertex currentVertex)
        {
            toolpath = FinishToolpath();

            if (currentVertex == null)
                return;

            CreateTravelToolpath(previousVertex, currentVertex);
            toolpath = StartNewToolpath(toolpath, currentVertex);
        }

        private bool VertexHasNegativeOrZeroExtrusion(PrintVertex previousVertex, PrintVertex currentVertex)
        {
            if (previousVertex == null || currentVertex == null)
                return false;

            if (extruderRelativeCoordinates)
            {
                // TODO: Update this for relative vs. absolute extruder coords
                return currentVertex.Extrusion.x <= 0;
            }
            else
            {
                return currentVertex.Extrusion.x <= previousVertex.Extrusion.x;
            }
        }

        private LinearToolpath3<PrintVertex> StartNewToolpath(LinearToolpath3<PrintVertex> toolpath, PrintVertex currentVertex)
        {
            var newToolpath = new LinearToolpath3<PrintVertex>(toolpath.Type);
            newToolpath.FillType = toolpath.FillType;
            newToolpath.AppendVertex(currentVertex, TPVertexFlags.IsPathStart);
            return newToolpath;
        }

        private void CreateTravelToolpath(PrintVertex vertexStart, PrintVertex vertexEnd)
        {
            var travel = new LinearToolpath3<PrintVertex>(ToolpathTypes.Travel);
            travel.AppendVertex(vertexStart, TPVertexFlags.IsPathStart);
            travel.AppendVertex(vertexEnd, TPVertexFlags.None);
            EmitToolpath(travel);
        }

        protected override LinearToolpath3<PrintVertex> FinishToolpath()
        {
            if (toolpath == null)
                return new LinearToolpath3<PrintVertex>();

            toolpath.Type = SetTypeFromVertexExtrusions(toolpath);
            EmitToolpath(toolpath);

            // TODO: Simplify with "CopyProperties" method on LinearToolpath3
            var newToolpath = new LinearToolpath3<PrintVertex>(toolpath.Type);
            newToolpath.IsHole = toolpath.IsHole;
            newToolpath.FillType = toolpath.FillType;
            newToolpath.AppendVertex(toolpath.End, TPVertexFlags.IsPathStart);

            return newToolpath;
        }

        private ToolpathTypes SetTypeFromVertexExtrusions(LinearToolpath3<PrintVertex> toolpath)
        {
            for (int i = 1; i < toolpath.VertexCount; i++)
            {
                if (!VertexHasNegativeOrZeroExtrusion(toolpath[i - 1], toolpath[i]))
                    return ToolpathTypes.Deposition;
            }
            return ToolpathTypes.Travel;
        }

        private PrintVertex UpdatePrintVertex(GCodeLine line, PrintVertex previousVertex)
        {
            return new PrintVertex(
                ExtractPosition(line, previousVertex.Position),
                ExtractFeedRate(line, previousVertex.FeedRate),
                ExtractDimensions(line, previousVertex.Dimensions),
                ExtractExtrusion(line, previousVertex.Extrusion.x));
        }
    }
}