using NUnit.Framework;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class FilterTests
    {
        private MeshData Mesh;
        private Triangles Triangles;
        private Filter Filter;
        
        [SetUp]
        public void SetUp()
        {
            Mesh = new MeshData();
            Mesh.Vertices.AddRange(Fixtures.TwitterBirdVertices);
            Mesh.Edges.AddRange(Fixtures.TwitterBirdEdges);
            Mesh.Triangles.AddRange(Fixtures.TwitterBirdMonotoneTriangles);
            
            Triangles = new Triangles(Mesh);
            
            var delaunay = new DelaunayRefine();
            delaunay.WorkBufferPool = new WorkBufferPool();
            delaunay.RefineTriangles(Triangles);
            
            Filter = new Filter();
            Filter.WorkBufferPool = new WorkBufferPool();
        }

        [Test]
        public void ExteriorFilter()
        {
            Filter.Target = -1;
            Filter.Do(Triangles, Mesh.Triangles);
            
            Assert.AreEqual(Fixtures.TwitterBirdExteriorFilteredTriangles, Mesh.Triangles);
        }

        [Test]
        public void InteriorFilter()
        {
            Filter.Target = 1;
            Filter.Do(Triangles, Mesh.Triangles);
            
            Assert.AreEqual(Fixtures.TwitterBirdInteriorFilteredTriangles, Mesh.Triangles);
        }
    }
}