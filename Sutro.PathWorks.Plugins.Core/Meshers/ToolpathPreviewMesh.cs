using g3;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public class ToolpathPreviewMesh
    {
        private readonly List<int> triangles = new List<int>();
        public ReadOnlyCollection<int> Triangles => triangles.AsReadOnly();

        private readonly List<ToolpathPreviewVertex> vertices = new List<ToolpathPreviewVertex>();
        public ReadOnlyCollection<ToolpathPreviewVertex> Vertices => vertices.AsReadOnly();

        public int AddVertex(ToolpathPreviewVertex vertex)
        {
            vertices.Add(vertex);
            return vertices.Count - 1;
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
    }
}