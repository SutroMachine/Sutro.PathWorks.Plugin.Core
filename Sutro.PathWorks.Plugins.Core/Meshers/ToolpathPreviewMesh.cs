using Sutro.PathWorks.Plugins.API.Visualizers;

namespace Sutro.PathWorks.Plugins.Core.Meshers
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
}