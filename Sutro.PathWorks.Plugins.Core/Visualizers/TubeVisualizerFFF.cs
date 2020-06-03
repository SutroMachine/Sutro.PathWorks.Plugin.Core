using g3;
using gs;
using gs.FillTypes;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.Decompilers;
using Sutro.PathWorks.Plugins.Core.Meshers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public static class TubeVisualizerFFF
    {
        public static IEnumerable<Tuple<string, VisualizerFillType>> GetFillTypes()
        {
            return new Tuple<string, VisualizerFillType>[]
            {
                Tuple.Create(InnerPerimeterFillType.Label, new VisualizerFillType("Inner Perimeter", new Vector3f(1, 0, 0))),
                Tuple.Create(OuterPerimeterFillType.Label, new VisualizerFillType("Outer Perimeter", new Vector3f(1, 1, 0))),
                Tuple.Create(OpenShellCurveFillType.Label, new VisualizerFillType("Open Mesh Curve", new Vector3f(0, 1, 1))),
                Tuple.Create(SolidFillType.Label, new VisualizerFillType("Solid Fill", new Vector3f(0, 0.5f, 1))),
                Tuple.Create(SparseFillType.Label, new VisualizerFillType("Sparse Fill", new Vector3f(0.5f, 0, 1))),
                Tuple.Create(SupportFillType.Label, new VisualizerFillType("Support", new Vector3f(1, 0, 1))),
                Tuple.Create(BridgeFillType.Label, new VisualizerFillType("Bridge", new Vector3f(0, 0, 1))),
                Tuple.Create(DefaultFillType.Label, new VisualizerFillType("Unknown", new Vector3f(0.5, 0.5, 0.5))),
            };
        }

        public static VisualizerCore Create()
        {
            return new VisualizerCore(
                "Tube",
                new FillTypeMapper(GetFillTypes()),
                new DecompilerFFF(),
                new TubeMesher<PrintVertex>());
        }
    }
}