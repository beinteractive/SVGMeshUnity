using UnityEngine;
using UnityEngine.Assertions;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public static class Robust
    {
        // https://github.com/mikolalysenko/robust-orientation
        // https://github.com/mikolalysenko/robust-in-sphere
        // https://github.com/mikolalysenko/robust-sum
        // https://github.com/mikolalysenko/robust-subtract
        // https://github.com/mikolalysenko/robust-scale
        // https://github.com/mikolalysenko/two-product
        // https://github.com/mikolalysenko/two-sum

        private static readonly float Epsilon = 1.1102230246251565e-16f;
        private static readonly float Errbound3 = (3.0f + 16.0f * Epsilon) * Epsilon;

        private static readonly float Splitter = +(Mathf.Pow(2f, 27f) + 1.0f);

        public static float Orientation(Vector2 a, Vector2 b, Vector2 c)
        {
            var l = (a.y - c.y) * (b.x - c.x);
            var r = (a.x - c.x) * (b.y - c.y);
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

            var p = Sum(Sum(TwoProd(m1.y, m2.x), TwoProd(-m2.y, m1.x)), Sum(TwoProd(m0.y, m1.x), TwoProd(-m1.y, m0.x)));
            var n = Sum(TwoProd(m0.y, m2.x), TwoProd(-m2.y, m0.x));
            var d = Sub(p, n);
            
            return d[d.Length - 1];
        }

        public static float InSphere(Vector2 m0, Vector2 m1, Vector2 m2, Vector2 m3)
        {
            var w0 = Sum(TwoProd(m0[0], m0[0]), TwoProd(m0[1], m0[1]));
            var w0m1 = Scale(w0, m1[0]);
            var w0m2 = Scale(w0, m2[0]);
            var w0m3 = Scale(w0, m3[0]);
            var w1 = Sum(TwoProd(m1[0], m1[0]), TwoProd(m1[1], m1[1]));
            var w1m0 = Scale(w1, m0[0]);
            var w1m2 = Scale(w1, m2[0]);
            var w1m3 = Scale(w1, m3[0]);
            var w2 = Sum(TwoProd(m2[0], m2[0]), TwoProd(m2[1], m2[1]));
            var w2m0 = Scale(w2, m0[0]);
            var w2m1 = Scale(w2, m1[0]);
            var w2m3 = Scale(w2, m3[0]);
            var w3 = Sum(TwoProd(m3[0], m3[0]), TwoProd(m3[1], m3[1]));
            var w3m0 = Scale(w3, m0[0]);
            var w3m1 = Scale(w3, m1[0]);
            var w3m2 = Scale(w3, m2[0]);
            var p =
                Sum(
                    Sum(Scale(Sub(w3m2, w2m3), m1[1]),
                        Sum(Scale(Sub(w3m1, w1m3), -m2[1]), Scale(Sub(w2m1, w1m2), m3[1]))),
                    Sum(Scale(Sub(w3m1, w1m3), m0[1]),
                        Sum(Scale(Sub(w3m0, w0m3), -m1[1]), Scale(Sub(w1m0, w0m1), m3[1]))));
            var n =
                Sum(
                    Sum(Scale(Sub(w3m2, w2m3), m0[1]),
                        Sum(Scale(Sub(w3m0, w0m3), -m2[1]), Scale(Sub(w2m0, w0m2), m3[1]))),
                    Sum(Scale(Sub(w2m1, w1m2), m0[1]),
                        Sum(Scale(Sub(w2m0, w0m2), -m1[1]), Scale(Sub(w1m0, w0m1), m2[1]))));
            var d = Sub(p, n);

            return d[d.Length - 1];
        }

        private static float[] TwoProd(float a, float b, float[] result = null)
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
            
            if (result != null)
            {
                result[0] = y;
                result[1] = x;
                return result;
            }
            return new[] { y, x };
        }

        private static float[] TwoSum(float a, float b, float[] result = null)
        {
            var x = a + b;
            var bv = x - a;
            var av = x - bv;
            var br = b - bv;
            var ar = a - av;
            if (result != null)
            {
                result[0] = ar + br;
                result[1] = x;
                return result;
            }
            return new[] { ar + br, x };
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

        private static float[] Scale(float[] e, float scale)
        {
            var n = e.Length;
            if (n == 1)
            {
                var ts = TwoProd(e[0], scale);
                if (ts[0] != 0f)
                {
                    return ts;
                }

                return new[] {ts[1]};
            }

            var g = new float[2 * n];
            var q = new[] {0.1f, 0.1f};
            var t = new[] {0.1f, 0.1f};
            var count = 0;
            TwoProd(e[0], scale, q);
            if (q[0] != 0f)
            {
                g[count++] = q[0];
            }

            for (var i = 1; i < n; ++i)
            {
                TwoProd(e[i], scale, t);
                var pq = q[1];
                TwoSum(pq, t[0], q);
                if (q[0] != 0f)
                {
                    g[count++] = q[0];
                }

                var a = t[1];
                var b = q[1];
                var x = a + b;
                var bv = x - a;
                var y = b - bv;
                q[1] = x;
                if (y != 0f)
                {
                    g[count++] = y;
                }
            }

            if (q[1] != 0f)
            {
                g[count++] = q[1];
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