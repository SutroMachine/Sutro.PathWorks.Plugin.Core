using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class ToolpathPreviewMesh
    {
        public ToolpathPreviewVertex[] Vertices;
        public int[] Triangles;

        public ToolpathPreviewMesh(ToolpathPreviewVertex[] vertices, int[] triangles)
        {
            Vertices = vertices;
            Triangles = triangles;
        }
    }

    public interface IToolpathPreviewMesher<TPrintVertex>
            where TPrintVertex : IToolpathVertex
    {
        ToolpathPreviewMesh Generate(
            LinearToolpath3<TPrintVertex> toolpath,
            Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory);
    }
}