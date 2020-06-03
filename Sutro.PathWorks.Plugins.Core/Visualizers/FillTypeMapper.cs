using g3;
using gs.FillTypes;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class FillTypeMapper
    {
        private int fillTypeCount = 0;

        private readonly Dictionary<string, int> fillTypeIntegerId = new Dictionary<string, int>();
        private readonly Dictionary<int, string> fillTypeStringInt = new Dictionary<int, string>();

        public Dictionary<int, VisualizerFillType> VisualizerFillTypes { get; } = new Dictionary<int, VisualizerFillType>();

        public FillTypeMapper()
        {
            AddFillTypes();
        }

        public Vector3f GetColor(int fillTypeIndex)
        {
            if (VisualizerFillTypes.TryGetValue(fillTypeIndex, out var value))
                return value.Color;

            return VisualizerFillTypes[GetIntegerFromLabel(DefaultFillType.Label)].Color;
        }

        public int GetIntegerFromLabel(string label)
        {
            if (fillTypeIntegerId.TryGetValue(label, out int value))
                return value;
            return 0;
        }

        protected virtual void AddFillTypes()
        {
            Add(InnerPerimeterFillType.Label, new VisualizerFillType("Inner Perimeter", new Vector3f(1, 0, 0)));
            Add(OuterPerimeterFillType.Label, new VisualizerFillType("Outer Perimeter", new Vector3f(1, 1, 0)));
            Add(OpenShellCurveFillType.Label, new VisualizerFillType("Open Mesh Curve", new Vector3f(0, 1, 1)));
            Add(SolidFillType.Label, new VisualizerFillType("Solid Fill", new Vector3f(0, 0.5f, 1)));
            Add(SparseFillType.Label, new VisualizerFillType("Sparse Fill", new Vector3f(0.5f, 0, 1)));
            Add(SupportFillType.Label, new VisualizerFillType("Support", new Vector3f(1, 0, 1)));
            Add(BridgeFillType.Label, new VisualizerFillType("Bridge", new Vector3f(0, 0, 1)));
            Add(DefaultFillType.Label, new VisualizerFillType("Unknown", new Vector3f(0.5, 0.5, 0.5)));
        }

        private void Add(string label, VisualizerFillType visualizerFillType)
        {
            fillTypeIntegerId.Add(label, fillTypeCount);
            fillTypeStringInt.Add(fillTypeCount, label);
            VisualizerFillTypes.Add(fillTypeCount, visualizerFillType);
            fillTypeCount++;
        }
    }
}