using NUnit.Framework;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class MonotoneTriangulationTests
    {
        private MonotoneTriangulation MonotoneTriangulation;

        [SetUp]
        public void SetUp()
        {
            MonotoneTriangulation = new MonotoneTriangulation();
            MonotoneTriangulation.WorkBufferPool = new WorkBufferPool();
        }
        
        [Test]
        public void BuildTriangles()
        {
            var mesh = new MeshData();
            
            mesh.Vertices.AddRange(Fixtures.TwitterBirdVertices);
            mesh.Edges.AddRange(Fixtures.TwitterBirdEdges);
            
            MonotoneTriangulation.BuildTriangles(mesh);
            
            Assert.AreEqual(Fixtures.TwitterBirdMonotoneTriangles, mesh.Triangles);
        }
    }
}