using g3;
using gs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.Meshers;
using System.Collections.Generic;
using System.IO;

namespace Sutro.PathWorks.Plugins.Core.Tests
{
    [TestClass]
    public class TubeMesherTests
    {
        [TestMethod]
        public void SharpAngleLeft()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 1, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "SharpAngleLeft.stl");
        }

        [TestMethod]
        public void SharpAngleRight()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, -1, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "SharpAngleRight.stl");
        }

        private void ExportMesh(ToolpathPreviewMesh mesh, string filePath)
        {
            var dmesh3 = new DMesh3();
            foreach (var vertex in mesh.Vertices)
                dmesh3.AppendVertex(vertex.Point);

            for (int i = 0; i < mesh.Triangles.Length; i += 3)
            {
                dmesh3.AppendTriangle(mesh.Triangles[i], mesh.Triangles[i + 1], mesh.Triangles[i + 2]);
            }

            var writeMesh = new WriteMesh(dmesh3, "Name");
            var stlWriter = new STLWriter();

            using var writer = File.CreateText(filePath);
            stlWriter.Write(writer, new List<WriteMesh>() { writeMesh }, WriteOptions.Defaults);
        }

        private static ToolpathPreviewVertex VertexF(PrintVertex vertex, Vector3d position, float brightness)
        {
            return new ToolpathPreviewVertex(position, 0, 0, Vector3f.AxisX, brightness, new CustomColorData());
        }

        [TestMethod]
        public void SharpAngle_ShortSegment()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0.001, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 1, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "SharpAngleShortSegment.stl");
        }

        [TestMethod]
        public void ShortJog()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0.001, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(20, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "ShortJog.stl");
        }

        [TestMethod]
        public void Square()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 10, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 10, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "Square.stl");
        }


        [TestMethod]
        public void ShortBackJogLeft()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(9, 0.02, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "ShortBackJogLeft.stl");
        }

        [TestMethod]
        public void ShortBackJogRight()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(0, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(10, 0, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);
            toolpath.AppendVertex(new PrintVertex(new Vector3d(9, -0.02, 0), 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "ShortBackJogRight.stl");
        }


        [TestMethod]
        public void GearTooth()
        {
            var mesher = new TubeMesher<PrintVertex>();
            var toolpath = new LinearToolpath3<PrintVertex>(ToolpathTypes.Deposition);

            var vertices = new Vector3d[]
            {
                new Vector3d(-2.8213, -4.9295, 0),
                new Vector3d(-1.6718, -4.7474, 0),
                new Vector3d(-1.6102, -4.6931, 0),
                new Vector3d(-1.5441, -4.605, 0),
                new Vector3d(-1.5069, -4.5174, 0),
                new Vector3d(-1.4839, -4.432, 0),
                new Vector3d(-1.4391, -4.0986, 0),
                new Vector3d(-1.2555, -2.966, 0),
                new Vector3d(-1.2323, -2.8077, 0),
                new Vector3d(-1.2138, -2.6866, 0),
                new Vector3d(-1.1305, -2.3684, 0),
                new Vector3d(-0.992, -1.981, 0),
                new Vector3d(-0.9011, -1.7072, 0),
                new Vector3d(-0.7117, -1.3611, 0),
                new Vector3d(-0.567, -1.1083, 0),
                new Vector3d(-0.4772, -1.0075, 0),
                new Vector3d(-0.3867, -0.91, 0),
                new Vector3d(-0.2536, -0.8363, 0),
                new Vector3d(-0.0808, -0.7455, 0),
                new Vector3d(-0.0492, -0.7425, 0),
                new Vector3d(-0.0037, -0.7384, 0),
                new Vector3d(0.1313, -0.7881, 0),
                new Vector3d(0.3004, -0.8472, 0),
                new Vector3d(0.4753, -1.04, 0),
                new Vector3d(0.622, -1.1958, 0),
                new Vector3d(0.8513, -1.654, 0),
                new Vector3d(0.9476, -1.8397, 0),
                new Vector3d(1.08, -2.2337, 0),
                new Vector3d(1.1071, -2.3109, 0),
                new Vector3d(1.2461, -2.9749, 0),
                new Vector3d(1.3606, -3.4863, 0),
                new Vector3d(1.3801, -3.5576, 0),
                new Vector3d(1.3939, -3.6692, 0),
                new Vector3d(1.4124, -3.8724, 0),
                new Vector3d(1.4764, -4.432, 0),
                new Vector3d(1.4907, -4.4721, 0),
                new Vector3d(1.5005, -4.5269, 0),
                new Vector3d(1.5356, -4.6163, 0),
                new Vector3d(1.6208, -4.7383, 0),
                new Vector3d(1.6669, -4.7837, 0),
                new Vector3d(2.4775, -4.9121, 0),
                new Vector3d(4.2755, -5.161, 0),
            };

            foreach (var vertex in vertices)
                toolpath.AppendVertex(new PrintVertex(vertex, 1000, new Vector2d(0.4, 0.2)), TPVertexFlags.None);

            var mesh = mesher.Generate(toolpath, VertexF);

            ExportMesh(mesh, "GearTooth.stl");
        }
    }
}