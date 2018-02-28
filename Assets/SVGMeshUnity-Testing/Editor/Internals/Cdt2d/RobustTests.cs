using NUnit.Framework;
using UnityEngine;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class RobustTests
    {
        [Test]
        public void Orientation()
        {
            Assert.IsTrue(Robust.Orientation(
                              new Vector2(0.1f, 0.1f),
                              new Vector2(0.1f, 0.1f),
                              new Vector2(0.3f, 0.7f)
                          ) == 0f);

            Assert.IsTrue(Robust.Orientation(
                              new Vector2(0f, 0f),
                              new Vector2(-1e-32f, 0f),
                              new Vector2(0f, 1f)
                          ) > 0f);

            Assert.IsTrue(Robust.Orientation(
                              new Vector2(0f, 0f),
                              new Vector2(1e-32f, 1e-32f),
                              new Vector2(1f, 1f)
                          ) == 0f);

            Assert.IsTrue(Robust.Orientation(
                              new Vector2(0f, 0f),
                              new Vector2(1e-32f, 0f),
                              new Vector2(0f, 1f)
                          ) < 0f);

            var x = 1e-32f;
            for (var i = 0; i < 32; ++i)
            {
                Assert.IsTrue(Robust.Orientation(
                                  new Vector2(-x, 0f),
                                  new Vector2(0f, 1f),
                                  new Vector2(x, 0f)
                              ) > 0f);
                Assert.IsTrue(Robust.Orientation(
                                  new Vector2(-x, 0f),
                                  new Vector2(0f, 0f),
                                  new Vector2(x, 0f)
                              ) == 0f);
                Assert.IsTrue(Robust.Orientation(
                                  new Vector2(-x, 0f),
                                  new Vector2(0f, -1f),
                                  new Vector2(x, 0f)
                              ) < 0f, "x=" + x);
                Assert.IsTrue(Robust.Orientation(
                                  new Vector2(0f, 1f),
                                  new Vector2(0f, 0f),
                                  new Vector2(x, x)
                              ) < 0f, "x=" + x);
                x *= 10f;
            }
        }

        [Test]
        public void InSphere()
        {
            Assert.IsTrue(Robust.InSphere(
                              new Vector2(0f, -1f),
                              new Vector2(1f, 0f),
                              new Vector2(0f, 1f),
                              new Vector2(-0.5f, 0f)) > 0f);

            Assert.IsTrue(Robust.InSphere(
                              new Vector2(0f, -1f),
                              new Vector2(1f, 0f),
                              new Vector2(0f, 1f),
                              new Vector2(-1f, 0f)) == 0f);

            Assert.IsTrue(Robust.InSphere(
                              new Vector2(0f, -1f),
                              new Vector2(1f, 0f),
                              new Vector2(0f, 1f),
                              new Vector2(-1.5f, 0f)) < 0f);

            var x = 1e-4f;
            for(var i = 0; i < 8; ++i)
            {
                Assert.IsTrue(Robust.InSphere(
                                  new Vector2(0f, x),
                                  new Vector2(-x, -x),
                                  new Vector2(x, -x),
                                  new Vector2(0f, 0f)) > 0f, "sphere test:" + x);
                Assert.IsTrue(Robust.InSphere(
                                  new Vector2(0f, x),
                                  new Vector2(-x, -x),
                                  new Vector2(x, -x),
                                  new Vector2(0f, 2f * x)) < 0f, "sphere test:" + x);
                Assert.IsTrue(Robust.InSphere(
                                  new Vector2(0f, x),
                                  new Vector2(-x, -x),
                                  new Vector2(x, -x),
                                  new Vector2(0f, x)) == 0f, "sphere test:" + x);
                x *= 10f;
            }
            
        }
    }
}