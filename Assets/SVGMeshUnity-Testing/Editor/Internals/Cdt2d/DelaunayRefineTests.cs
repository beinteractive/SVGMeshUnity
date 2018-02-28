using NUnit.Framework;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class DelaunayRefineTests
    {
        private DelaunayRefine DelaunayRefine;

        [SetUp]
        public void SetUp()
        {
            DelaunayRefine = new DelaunayRefine();
            DelaunayRefine.WorkBufferPool = new WorkBufferPool();
        }

        [Test]
        public void RefineTriangles()
        {
            var mesh = new MeshData();
            
            mesh.Vertices.AddRange(Fixtures.TwitterBirdVertices);
            mesh.Edges.AddRange(Fixtures.TwitterBirdEdges);
            mesh.Triangles.AddRange(Fixtures.TwitterBirdMonotoneTriangles);
            
            var triangles = new Triangles(mesh);
            
            DelaunayRefine.RefineTriangles(triangles);
            
            triangles.Fill(mesh.Triangles);
            
            Assert.AreEqual(Fixtures.TwitterBirdDelaunayRefinedTriangles, mesh.Triangles);
        }
    }
}