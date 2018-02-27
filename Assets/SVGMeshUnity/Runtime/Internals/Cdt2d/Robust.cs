using UnityEngine;
using UnityEngine.Assertions;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public static class Robust
    {
        // https://github.com/mikolalysenko/robust-orientation
        // https://github.com/mikolalysenko/robust-sum
        // https://github.com/mikolalysenko/robust-subtract
        // https://github.com/mikolalysenko/two-product

        private static readonly float Epsilon = 1.1102230246251565e-16f;
        private static readonly float Errbound3 = (3.0f + 16.0f * Epsilon) * Epsilon;

        private static readonly float Splitter = +(Mathf.Pow(2f, 27f) + 1.0f);


        public static float Orientation(Vector2 a, Vector2 b, Vector2 c)
        {
            var l = (a[1] - c[1]) * (b[0] - c[0]);
            var r = (a[0] - c[0]) * (b[1] - c[1]);
            var det = l - r;
            var s = 0f;
            if (l > 0)
            {
                if (r <= 0)
                {
                    return det;
                }
                else
                {
                    s = l + r;
                }
            }
            else if (l < 0)
            {
                if (r >= 0)
                {
                    return det;
                }
                else
                {
                    s = -(l + r);
                }
            }
            else
            {
                return det;
            }

            var tol = Errbound3 * s;
            if (det >= tol || det <= -tol)
            {
                return det;
            }
            
            var m0 = a;
            var m1 = b;
            var m2 = c;

            var p = Sum(Sum(Prod(m1[1], m2[0]), Prod(-m2[1], m1[0])), Sum(Prod(m0[1], m1[0]), Prod(-m1[1], m0[0])));
            var n = Sum(Prod(m0[1], m2[0]), Prod(-m2[1], m0[0]));
            var d = Sub(p, n);
            
            return d[d.Length - 1];
        }

        private static float[] Prod(float a, float b)
        {
            var x = a * b;

            var c = Splitter * a;
            var abig = c - a;
            var ahi = c - abig;
            var alo = a - ahi;

            var d = Splitter * b;
            var bbig = d - b;
            var bhi = d - bbig;
            var blo = b - bhi;

            var err1 = x - (ahi * bhi);
            var err2 = err1 - (alo * bhi);
            var err3 = err2 - (ahi * blo);

            var y = alo * blo - err3;

            return new[] { y, x };
        }

        private static float[] Sum(float[] e, float[] f)
        {
            var ne = e.Length;
            var nf = f.Length;
            if(ne == 1 && nf == 1)
            {
                return ScalarScalar(e[0], f[0]);
            }
            var n = ne + nf;
            var g = new float[n];
            var count = 0;
            var eptr = 0;
            var fptr = 0;
            var ei = e[eptr];
            var ea = Mathf.Abs(ei);
            var fi = f[fptr];
            var fa = Mathf.Abs(fi);
            var a = 0f;
            var b = 0f;
            if (ea < fa)
            {
                b = ei;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                    ea = Mathf.Abs(ei);
                }
            }
            else
            {
                b = fi;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = f[fptr];
                    fa = Mathf.Abs(fi);
                }
            }

            if ((eptr < ne && ea < fa) || (fptr >= nf))
            {
                a = ei;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                    ea = Mathf.Abs(ei);
                }
            }
            else
            {
                a = fi;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = f[fptr];
                    fa = Mathf.Abs(fi);
                }
            }

            var x = a + b;
            var bv = x - a;
            var y = b - bv;
            var q0 = y;
            var q1 = x;
            var _x = 0f;
            var _bv = 0f;
            var _av = 0f;
            var _br = 0f;
            var _ar = 0f;
            while (eptr < ne && fptr < nf)
            {
                if (ea < fa)
                {
                    a = ei;
                    eptr += 1;
                    if (eptr < ne)
                    {
                        ei = e[eptr];
                        ea = Mathf.Abs(ei);
                    }
                }
                else
                {
                    a = fi;
                    fptr += 1;
                    if (fptr < nf)
                    {
                        fi = f[fptr];
                        fa = Mathf.Abs(fi);
                    }
                }

                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
            }

            while (eptr < ne)
            {
                a = ei;
                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                }
            }

            while (fptr < nf)
            {
                a = fi;
                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = f[fptr];
                }
            }

            if (q0 != 0f)
            {
                g[count++] = q0;
            }

            if (q1 != 0f)
            {
                g[count++] = q1;
            }

            if (count == 0)
            {
                g[count++] = 0f;
            }

            if (g.Length != count)
            {
                var g_ = new float[count];
                for (var i = 0; i < count; ++i)
                {
                    g_[i] = g[i];
                }

                g = g_;
            }

            return g;
        }

        private static float[] Sub(float[] e, float[] f)
        {
            var ne = e.Length;
            var nf = f.Length;
            if (ne == 1 && nf == 1)
            {
                return ScalarScalar(e[0], -f[0]);
            }

            var n = ne + nf;
            var g = new float[n];
            var count = 0;
            var eptr = 0;
            var fptr = 0;
            var ei = e[eptr];
            var ea = Mathf.Abs(ei);
            var fi = -f[fptr];
            var fa = Mathf.Abs(fi);
            var a = 0f;
            var b = 0f;
            if (ea < fa)
            {
                b = ei;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                    ea = Mathf.Abs(ei);
                }
            }
            else
            {
                b = fi;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = -f[fptr];
                    fa = Mathf.Abs(fi);
                }
            }

            if ((eptr < ne && ea < fa) || (fptr >= nf))
            {
                a = ei;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                    ea = Mathf.Abs(ei);
                }
            }
            else
            {
                a = fi;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = -f[fptr];
                    fa = Mathf.Abs(fi);
                }
            }

            var x = a + b;
            var bv = x - a;
            var y = b - bv;
            var q0 = y;
            var q1 = x;
            var _x = 0f;
            var _bv = 0f;
            var _av = 0f;
            var _br = 0f;
            var _ar = 0f;
            while (eptr < ne && fptr < nf)
            {
                if (ea < fa)
                {
                    a = ei;
                    eptr += 1;
                    if (eptr < ne)
                    {
                        ei = e[eptr];
                        ea = Mathf.Abs(ei);
                    }
                }
                else
                {
                    a = fi;
                    fptr += 1;
                    if (fptr < nf)
                    {
                        fi = -f[fptr];
                        fa = Mathf.Abs(fi);
                    }
                }

                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
            }

            while (eptr < ne)
            {
                a = ei;
                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
                eptr += 1;
                if (eptr < ne)
                {
                    ei = e[eptr];
                }
            }

            while (fptr < nf)
            {
                a = fi;
                b = q0;
                x = a + b;
                bv = x - a;
                y = b - bv;
                if (y != 0f)
                {
                    g[count++] = y;
                }

                _x = q1 + x;
                _bv = _x - q1;
                _av = _x - _bv;
                _br = x - _bv;
                _ar = q1 - _av;
                q0 = _ar + _br;
                q1 = _x;
                fptr += 1;
                if (fptr < nf)
                {
                    fi = -f[fptr];
                }
            }

            if (q0 != 0f)
            {
                g[count++] = q0;
            }

            if (q1 != 0f)
            {
                g[count++] = q1;
            }

            if (count == 0)
            {
                g[count++] = 0.0f;
            }

            if (g.Length != count)
            {
                var g_ = new float[count];
                for (var i = 0; i < count; ++i)
                {
                    g_[i] = g[i];
                }

                g = g_;
            }

            return g;
        }

        //Easy case: Add two scalars
        private static float[] ScalarScalar(float a, float b)
        {
            var x = a + b;
            var bv = x - a;
            var av = x - bv;
            var br = b - bv;
            var ar = a - av;
            var y = ar + br;
            if (y != 0f)
            {
                return new[] {y, x};
            }
            return new[] { x };
        }
    }
}