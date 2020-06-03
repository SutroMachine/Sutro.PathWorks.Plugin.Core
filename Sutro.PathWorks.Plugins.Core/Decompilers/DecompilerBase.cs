﻿using g3;
using gs;
using gs.FillTypes;
using Sutro.Core.Models.GCode;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sutro.PathWorks.Plugins.Core.Decompilers
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
}