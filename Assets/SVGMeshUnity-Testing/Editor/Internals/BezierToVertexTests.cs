using NUnit.Framework;
using UnityEngine.TestTools.Utils;

namespace SVGMeshUnity.Internals
{
    public class BezierToVertexTests
    {
        private BezierToVertex BezierToVertex;

        [SetUp]
        public void SetUp()
        {
            BezierToVertex = new BezierToVertex();
            BezierToVertex.WorkBufferPool = new WorkBufferPool();
        }

        [Test]
        public void GetContours()
        {
            var svg = new SVGData();
            svg.Path(Fixtures.TwitterBirdPathCurve);
            
            var mesh = new MeshData();
            
            BezierToVertex.Scale = 10f;
            BezierToVertex.GetContours(svg, mesh);
            
            Assert.That(mesh.Vertices, Is.EqualTo(Fixtures.TwitterBirdPathCurveVertices).Using(Vector3EqualityComparer.Instance));
            Assert.That(mesh.Edges, Is.EqualTo(Fixtures.TwitterBirdPathCurveEdges));
        }
    }
}