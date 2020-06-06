using g3;
using gs.FillTypes;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class FillTypeMapper
    {
        public FillTypeMapper(IEnumerable<Tuple<string, VisualizerFillType>> fillTypes)
        {
            foreach (var tuple in fillTypes)
            {
                Add(tuple.Item1, tuple.Item2);
            }
        }

        private int fillTypeCount = 0;

        private readonly Dictionary<string, int> fillTypeIntegerId = new Dictionary<string, int>();
        private readonly Dictionary<int, string> fillTypeStringInt = new Dictionary<int, string>();

        public Dictionary<int, VisualizerFillType> VisualizerFillTypes { get; } = new Dictionary<int, VisualizerFillType>();

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

        protected void Add(string label, VisualizerFillType visualizerFillType)
        {
            fillTypeIntegerId.Add(label, fillTypeCount);
            fillTypeStringInt.Add(fillTypeCount, label);
            VisualizerFillTypes.Add(fillTypeCount, visualizerFillType);
            fillTypeCount++;
        }
    }
}