using NUnit.Framework;
using UnityEngine;
using UnityEditor;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class RobustTests
    {
        [Test]
        public void OrientationWorks()
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
    }
}