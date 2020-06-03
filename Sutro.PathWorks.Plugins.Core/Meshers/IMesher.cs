using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public interface IToolpathPreviewMesher<TPrintVertex>
            where TPrintVertex : IToolpathVertex
    {
        ToolpathPreviewMesh Generate(
            LinearToolpath3<TPrintVertex> toolpath,
            Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory);
    }
}