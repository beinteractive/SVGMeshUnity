using System;
using System.Collections.Generic;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public static class BinarySearch
    {
        // https://github.com/mikolalysenko/binary-search-bounds

        public static int GE<G, E>(List<G> a, E y, Func<G, E, int> c, int l, int h)
        {
            var i = h + 1;
            while (l <= h)
            {
                var m = (int) (uint) (l + h) >> 1;
                var x = a[m];
                if (c(x, y) >= 0)
                {
                    i = m;
                    h = m - 1;
                }
                else
                {
                    l = m + 1;
                }
            }

            return i;
        }

        public static int GT<G, E>(List<G> a, E y, Func<G, E, int> c, int l, int h)
        {
            var i = h + 1;
            while (l <= h)
            {
                var m = (int) (uint) (l + h) >> 1;
                var x = a[m];
                if (c(x, y) > 0)
                {
                    i = m;
                    h = m - 1;
                }
                else
                {
                    l = m + 1;
                }
            }

            return i;
        }

        public static int LT<G, E>(List<G> a, E y, Func<G, E, int> c, int l, int h)
        {
            var i = l - 1;
            while (l <= h)
            {
                var m = (int) (uint) (l + h) >> 1;
                var x = a[m];
                if (c(x, y) < 0)
                {
                    i = m;
                    l = m + 1;
                }
                else
                {
                    h = m - 1;
                }
            }

            return i;
        }

        public static int LE<G, E>(List<G> a, E y, Func<G, E, int> c, int l, int h)
        {
            var i = l - 1;
            while (l <= h)
            {
                var m = (int) (uint) (l + h) >> 1;
                var x = a[m];
                if (c(x, y) <= 0)
                {
                    i = m;
                    l = m + 1;
                }
                else
                {
                    h = m - 1;
                }
            }

            return i;
        }

        public static int EQ<G, E>(List<G> a, E y, Func<G, E, int> c, int l, int h)
        {
            while (l <= h)
            {
                var m = (int) (uint) (l + h) >> 1;
                var x = a[m];
                var p = c(x, y);
                if (p == 0)
                {
                    return m;
                }

                if (p <= 0)
                {
                    l = m + 1;
                }
                else
                {
                    h = m - 1;
                }
            }

            return -1;
        }
    }
}