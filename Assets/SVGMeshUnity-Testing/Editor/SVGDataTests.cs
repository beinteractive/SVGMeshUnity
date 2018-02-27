using NUnit.Framework;

namespace SVGMeshUnity
{
    public class SVGDataTests
    {
        [Test]
        public void ParseSVGPath()
        {
            var svg = new SVGData();
            svg.Path(Fixtures.TwitterBirdPathSource);
            Assert.AreEqual(Fixtures.TwitterBirdPathCurve, svg.Dump());
        }
    }
}