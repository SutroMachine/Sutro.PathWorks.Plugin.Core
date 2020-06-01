using g3;
using gs;
using gs.FillTypes;
using Sutro.Core.Models.GCode;
using System;
using System.Linq;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class Decompiler
    {
        protected int currentLayerIndex = 0;
        protected IFillType currentFillType;
        protected PrintVertex currentVertex;
        protected PrintVertex previousVertex;
        protected LinearToolpath3<PrintVertex> toolpath;

        public event Action<IToolpath> OnToolpathComplete;
        public event Action<int> OnNewLayer;

        public void Begin()
        {
            previousVertex = new PrintVertex(Vector3d.Zero, 0, Vector2d.Zero);
        }

        public virtual void ProcessGCodeLine(GCodeLine line)
        {
            if (LineIsEmpty(line))
                return;

            if (LineIsNewLayerComment(line))
            {
                toolpath = FinishToolpath();
                OnNewLayer?.Invoke(currentLayerIndex++);
            }

            if (LineIsNewFillTypeComment(line, out var newFillType))
            {
                toolpath = FinishToolpath();
                toolpath.Type = ToolpathTypes.Deposition;
                toolpath.FillType = newFillType;
            }

            currentVertex = UpdatePrintVertex(line, previousVertex);

            if (line.Comment?.Contains("Plane Change") ?? false)
            {
                toolpath.Type = ToolpathTypes.PlaneChange;
            }

            if (line.Type == LineType.GCode)
            {
                if (toolpath == null)
                {
                    if (currentVertex.Extrusion.x > previousVertex.Extrusion.x)
                    {
                        toolpath = FinishToolpath();
                        toolpath.Append(previousVertex);
                        toolpath.Append(currentVertex);
                    }
                    else
                    {
                        //RaiseLineGenerated(new List<Vector3d>() { previousVertex.Position, vertex.Position }, layerIndex);
                    }
                }
                else
                {
                    toolpath.AppendVertex(currentVertex, TPVertexFlags.None);
                    if (currentVertex.Extrusion.x <= previousVertex.Extrusion.x)
                    {
                        FinishToolpath();
                    }
                }
            }

            previousVertex = currentVertex;
        }

        public void End()
        {
            FinishToolpath();
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
        protected LinearToolpath3<PrintVertex> FinishToolpath()
        {
            if (toolpath == null)
                return new LinearToolpath3<PrintVertex>();

            // TODO: Simplify with "CopyProperties" method on LinearToolpath3
            var newToolpath = new LinearToolpath3<PrintVertex>(toolpath.Type);
            newToolpath.IsHole = toolpath.IsHole;
            newToolpath.FillType = toolpath.FillType;
            newToolpath.AppendVertex(toolpath.End, TPVertexFlags.IsPathStart);

            OnToolpathComplete?.Invoke(toolpath);

            return newToolpath;
        }

        protected bool LineIsNewFillTypeComment(GCodeLine line, out IFillType fillType)
        {
            fillType = new DefaultFillType();
            if (line?.Comment?.Contains("feature") ?? false)
            {
                return true;
            }
            return false;
        }

        private static bool LineIsEmpty(GCodeLine line)
        {
            return line == null || line.Type == LineType.Blank;
        }

        private static bool LineIsNewLayerComment(GCodeLine line)
        {
            return line.Comment != null && line.Comment.Contains("layer") && !line.Comment.Contains("feature");
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