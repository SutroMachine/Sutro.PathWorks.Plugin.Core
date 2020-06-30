using Sutro.PathWorks.Plugins.API.Visualizers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public class ToolpathPreviewMesh
    {
        private readonly List<int> triangles = new List<int>();
        private readonly List<ToolpathPreviewVertex> vertices = new List<ToolpathPreviewVertex>();

        public ReadOnlyCollection<int> Triangles => triangles.AsReadOnly();
        public ReadOnlyCollection<ToolpathPreviewVertex> Vertices => vertices.AsReadOnly();

        public void AddTriangle(int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }

        public int AddVertex(ToolpathPreviewVertex vertex)
        {
            vertices.Add(vertex);
            return vertices.Count - 1;
        }
    }
}