using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SVGMeshUnity
{
    public class SVGMesh : MonoBehaviour
    {
        // https://github.com/mattdesl/svg-path-contours
        // https://github.com/mattdesl/adaptive-bezier-curve

        public float Scale = 1f;

        public float PathDistanceEpsilon = 1f;
        public int RecursionLimit = 8;
        public float FLTEpsilon = 1.19209290e-7f;

        public float AngleEpsilon = 0.01f;
        public float AngleTolerance = 0f;
        public float CuspLimit = 0f; 
        
        private static List<Vector3> WorkVertices = new List<Vector3>(32);
        private static int UsedWorkVerticesSize;
        
        private List<Vector3> Vertices = new List<Vector3>();
        private int UsedVerticesSize;
        private List<int> Triangles = new List<int>();
        private Mesh Mesh;
        
        public void Fill(SVGData svg)
        {
            UsedWorkVerticesSize = 0;
            UsedVerticesSize = 0;
            
            FillVertices(svg);
            
            Triangles.Clear();
            for (var i = 0; i < UsedVerticesSize - 2; ++i)
            {
                Triangles.Add(i + 0);
                Triangles.Add(i + 1);
                Triangles.Add(i + 2);
            }
            
            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.MarkDynamic();
            }
            
            Mesh.SetVertices(Vertices);
            Mesh.SetTriangles(Triangles, 0);
            Mesh.RecalculateBounds();
            Mesh.RecalculateNormals();

            var filter = GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.sharedMesh = Mesh;
            }
        }

        private void FillVertices(SVGData svg)
        {
            var pen = Vector2.zero;
            
            var curves = svg.Curves;
            var l = curves.Count;
            for (var i = 0; i < l; ++i)
            {
                var curve = curves[i];
                if (curve.IsMove)
                {
                    EmitWorkVerticesIfNeeded();
                }
                else
                {
                    FillBezier(pen, curve.InControl, curve.OutControl, curve.Position);
                }
                pen = curve.Position;
            }
            EmitWorkVerticesIfNeeded();
        }

        private void EmitWorkVerticesIfNeeded()
        {
            if (UsedWorkVerticesSize == 0)
            {
                return;
            }
            
            // TODO: Simplify

            var size = UsedVerticesSize + UsedWorkVerticesSize;
            if (size > Vertices.Count)
            {
                Vertices.AddRange(Enumerable.Repeat(Vector3.zero, size - Vertices.Count));
            }

            for (var i = 0; i < UsedWorkVerticesSize; ++i)
            {
                Vertices[UsedVerticesSize + i] = WorkVertices[i];
            }

            UsedVerticesSize += UsedWorkVerticesSize;
            UsedWorkVerticesSize = 0;
        }

        private void PushWorkVertex(ref Vector2 v)
        {
            if (WorkVertices.Count == UsedWorkVerticesSize)
            {
                WorkVertices.Add(v);
            }
            else
            {
                WorkVertices[UsedWorkVerticesSize] = v;
            }

            ++UsedWorkVerticesSize;
        }

        ////// Based on:
        ////// https://github.com/pelson/antigrain/blob/master/agg-2.4/src/agg_curves.cpp

        private void FillBezier(Vector2 start, Vector2 c1, Vector2 c2, Vector2 end)
        {
            var distanceTolerance = PathDistanceEpsilon / Scale;
            distanceTolerance *= distanceTolerance;
            BeginFillBezier(start, c1, c2, end, distanceTolerance);
        }
        
        private void BeginFillBezier(Vector2 start, Vector2 c1, Vector2 c2, Vector2 end, float distanceTolerance)
        {
            PushWorkVertex(ref start);
            RecursiveFillBezier(start, c1, c2, end, distanceTolerance, 0);
            PushWorkVertex(ref end);
        }

        private void RecursiveFillBezier(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4, float distanceTolerance, int level)
        {
            if (level > RecursionLimit)
            {
                return;
            }

            var pi = Mathf.PI;

            // Calculate all the mid-points of the line segments
            //----------------------
            var v12 = (v1 + v2) / 2f;
            var v23 = (v2 + v3) / 2f;
            var v34 = (v3 + v4) / 2f;
            var v123 = (v12 + v23) / 2f;
            var v234 = (v23 + v34) / 2f;
            var v1234 = (v123 + v234) / 2f;
            
            // Enforce subdivision first time
            if (level > 0)
            {
                // Try to approximate the full cubic curve by a single straight line
                //------------------
                var d = v4 - v1;

                var d2 = Mathf.Abs((v2.x - v4.x) * d.y - (v2.y - v4.y) * d.x);
                var d3 = Mathf.Abs((v3.x - v4.x) * d.y - (v3.y - v4.y) * d.x);

                if (d2 > FLTEpsilon && d3 > FLTEpsilon)
                {
                    // Regular care
                    //-----------------
                    if ((d2 + d3) * (d2 + d3) <= distanceTolerance * (d.x * d.x + d.y * d.y))
                    {
                        // If the curvature doesn't exceed the distanceTolerance value
                        // we tend to finish subdivisions.
                        //----------------------
                        if (AngleTolerance < AngleEpsilon)
                        {
                            PushWorkVertex(ref v1234);
                            return;
                        }

                        // Angle & Cusp Condition
                        //----------------------
                        var a23 = Mathf.Atan2(v3.y - v2.y, v3.x - v2.x);
                        var da1 = Mathf.Abs(a23 - Mathf.Atan2(v2.y - v1.y, v2.x - v1.x));
                        var da2 = Mathf.Abs(Mathf.Atan2(v4.y - v3.y, v4.x - v3.x) - a23);

                        if (da1 >= pi)
                        {
                            da1 = 2 * pi - da1;
                        }

                        if (da2 >= pi)
                        {
                            da2 = 2 * pi - da2;
                        }

                        if (da1 + da2 < AngleTolerance)
                        {
                            // Finally we can stop the recursion
                            //----------------------
                            PushWorkVertex(ref v1234);
                            return;
                        }

                        if (CuspLimit > 0f)
                        {
                            if (da1 > CuspLimit)
                            {
                                PushWorkVertex(ref v2);
                                return;
                            }

                            if (da2 > CuspLimit)
                            {
                                PushWorkVertex(ref v3);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (d2 > FLTEpsilon)
                    {
                        // p1,p3,p4 are collinear, p2 is considerable
                        //----------------------
                        if (d2 * d2 <= distanceTolerance * (d.x * d.x + d.y * d.y))
                        {
                            if (AngleTolerance < AngleEpsilon)
                            {
                                PushWorkVertex(ref v1234);
                                return;
                            }

                            // Angle Condition
                            //----------------------
                            var da1 = Mathf.Abs(Mathf.Atan2(v3.y - v2.y, v3.x - v2.x) -
                                                Mathf.Atan2(v2.y - v1.y, v2.x - v1.x));
                            if (da1 >= pi)
                            {
                                da1 = 2 * pi - da1;
                            }

                            if (da1 < AngleTolerance)
                            {
                                PushWorkVertex(ref v2);
                                PushWorkVertex(ref v3);
                                return;
                            }

                            if (CuspLimit > 0f)
                            {
                                if (da1 > CuspLimit)
                                {
                                    PushWorkVertex(ref v2);
                                    return;
                                }
                            }
                        }
                    }
                    else if (d3 > FLTEpsilon)
                    {
                        // p1,p2,p4 are collinear, p3 is considerable
                        //----------------------
                        if (d3 * d3 <= distanceTolerance * (d.x * d.x + d.y * d.y))
                        {
                            if (AngleTolerance < AngleEpsilon)
                            {
                                PushWorkVertex(ref v1234);
                                return;
                            }

                            // Angle Condition
                            //----------------------
                            var da1 = Mathf.Abs(Mathf.Atan2(v4.y - v3.y, v4.x - v3.x) -
                                                Mathf.Atan2(v3.y - v2.y, v3.x - v2.x));
                            if (da1 >= pi)
                            {
                                da1 = 2 * pi - da1;
                            }

                            if (da1 < AngleTolerance)
                            {
                                PushWorkVertex(ref v2);
                                PushWorkVertex(ref v3);
                                return;
                            }

                            if (CuspLimit > 0f)
                            {
                                if (da1 > CuspLimit)
                                {
                                    PushWorkVertex(ref v3);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Collinear case
                        //-----------------
                        var dx = v1234.x - (v1.x + v4.x) / 2f;
                        var dy = v1234.y - (v1.y + v4.y) / 2f;
                        if (dx * dx + dy * dy <= distanceTolerance)
                        {
                            PushWorkVertex(ref v1234);
                            return;
                        }
                    }
                }
            }

            // Continue subdivision
            //----------------------
            RecursiveFillBezier(v1, v12, v123, v1234, distanceTolerance, level + 1);
            RecursiveFillBezier(v1234, v234, v34, v4, distanceTolerance, level + 1);
        }
    }
}