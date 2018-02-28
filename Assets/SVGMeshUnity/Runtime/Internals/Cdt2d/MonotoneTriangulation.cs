using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class MonotoneTriangulation
    {
        private static readonly bool Verbose = false;
        
        public WorkBufferPool WorkBufferPool;

        private enum EventType
        {
            Point = 0,
            End = 1,
            Start = 2,
        }
        
        //An event in the sweep line procedure
        private class Event
        {
            public Vector2 A;
            public Vector2 B;
            public EventType Type;
            public int Index;
        }
        
        //A partial convex hull fragment, made of two unimonotone polygons
        private class PartialHull
        {
            public Vector2 A;
            public Vector2 B;
            public int Index;
            public List<int> LowerIds = new List<int>();
            public List<int> UpperIds = new List<int>();
        }

        public void BuildTriangles(MeshData data)
        {
            var numPoints = data.Vertices.Count;
            var numEdges = data.EdgeCount;
            
            var events = WorkBufferPool.Get<Event>();
            if (events.NewForClass == null)
            {
                events.NewForClass = () => new Event();
            }

            //Create point events
            for (var i = 0; i < numPoints; ++i)
            {
                var e = events.Push();
                e.A = data.Vertices[i];
                e.B = Vector2.zero;
                e.Type = EventType.Point;
                e.Index = i;
            }

            //Create edge events
            for(var i=0; i<numEdges; ++i) {
                var a = data.Vertices[data.GetEdgeA(i)];
                var b = data.Vertices[data.GetEdgeB(i)];
                if(a[0] < b[0])
                {
                    {
                        var e = events.Push();
                        e.A = a;
                        e.B = b;
                        e.Type = EventType.Start;
                        e.Index = i;
                    }
                    {
                        var e = events.Push();
                        e.A = b;
                        e.B = a;
                        e.Type = EventType.End;
                        e.Index = i;
                    }
                }
                else if(a[0] > b[0])
                {
                    {
                        var e = events.Push();
                        e.A = b;
                        e.B = a;
                        e.Type = EventType.Start;
                        e.Index = i;
                    }
                    {
                        var e = events.Push();
                        e.A = a;
                        e.B = b;
                        e.Type = EventType.End;
                        e.Index = i;
                    }
                }
            }

            //Sort events
            events.Sort(CompareEvent);

            if (Verbose)
            {
                DumpEvents(events);
            }

            //Initialize hull
            var minX = events.Data[0].A[0] - 1f;
            var hulls = WorkBufferPool.Get<PartialHull>();
            if (hulls.NewForClass == null)
            {
                hulls.NewForClass = () => new PartialHull();
            }
            var h = hulls.Push();
            h.A = new Vector2(minX, 1f);
            h.B = new Vector2(minX, 0f);
            h.Index = -1;
            h.LowerIds.Clear();
            h.UpperIds.Clear();

            //Process events in order
            var numEvents = events.UsedSize;
            for (var i = 0; i < numEvents; ++i)
            {
                var e = events.Data[i];

                if (Verbose)
                {
                    Debug.Log("");
                    Debug.Log(i);
                    DumpEvent(e);
                }
                
                switch (e.Type)
                {
                    case EventType.Point:
                        AddPoint(data.Triangles, hulls, data.Vertices, e.A, e.Index);
                        break;
                    case EventType.Start:
                        SplitHulls(hulls, e);
                        break;
                    case EventType.End:
                        MergeHulls(hulls, e);
                        break;
                }

                if (Verbose)
                {
                    Debug.Log("");
                }
            }
            
            WorkBufferPool.Release(ref hulls);
            WorkBufferPool.Release(ref events);
        }
        
        //This is used to compare events for the sweep line procedure
        // Points are:
        //  1. sorted lexicographically
        //  2. sorted by type  (point < end < start)
        //  3. segments sorted by winding order
        //  4. sorted by index
        private int CompareEvent(Event a, Event b)
        {
            var d = 0;

            d = Sign(a.A[0] - b.A[0]);
            if (d != 0) return d;
            
            d = Sign(a.A[1] - b.A[1]);
            if (d != 0) return d;

            d = a.Type - b.Type;
            if (d != 0) return d;
            
            if (a.Type != EventType.Point)
            {
                d = Sign(Robust.Orientation(a.A, a.B, b.B));
                if (d != 0) return d;
            }

            return a.Index - b.Index;
        }

        private int Sign(float n)
        {
            if (n < 0f)
            {
                return -1;
            }

            if (n > 0f)
            {
                return 1;
            }

            return 0;
        }

        private void AddPoint(List<int> cells, WorkBuffer<PartialHull> hulls, List<Vector3> points, Vector2 p, int idx)
        {
            var lo = BinarySearch.LT(hulls.Data, p, TestPoint, 0, hulls.UsedSize - 1);
            var hi = BinarySearch.GT(hulls.Data, p, TestPoint, 0, hulls.UsedSize - 1);
            for (var i = lo; i < hi; ++i)
            {
                var hull = hulls.Data[i];

                //Insert p into lower hull
                {
                    var lowerIds = hull.LowerIds;
                    var m = lowerIds.Count;
                    while (m > 1 && Robust.Orientation(points[lowerIds[m - 2]], points[lowerIds[m - 1]], p) > 0f)
                    {
                        cells.Add(lowerIds[m - 1]);
                        cells.Add(lowerIds[m - 2]);
                        cells.Add(idx);
                        m -= 1;
                    }

                    if (m < lowerIds.Count)
                    {
                        lowerIds.RemoveRange(m, lowerIds.Count - m);
                    }

                    lowerIds.Add(idx);
                }

                //Insert p into upper hull
                {
                    var upperIds = hull.UpperIds;
                    var m = upperIds.Count;
                    while (m > 1 && Robust.Orientation(points[upperIds[m - 2]], points[upperIds[m - 1]], p) < 0f)
                    {
                        cells.Add(upperIds[m - 2]);
                        cells.Add(upperIds[m - 1]);
                        cells.Add(idx);
                        m -= 1;
                    }

                    if (m < upperIds.Count)
                    {
                        upperIds.RemoveRange(m, upperIds.Count - m);
                    }

                    upperIds.Add(idx);
                }
            }

            if (Verbose)
            {
                Debug.Log("Add");
                DumpHulls(hulls);
                hulls.Dump();
            }
        }

        private void SplitHulls(WorkBuffer<PartialHull> hulls, Event e)
        {
            var splitIdx = BinarySearch.LE(hulls.Data, e, FindSplit, 0, hulls.UsedSize - 1);
            var hull = hulls.Data[splitIdx];
            var upperIds = hull.UpperIds;
            var x = upperIds[upperIds.Count - 1];
            hull.UpperIds = new List<int>() { x };
            var h = hulls.Insert(splitIdx + 1);
            h.A = e.A;
            h.B = e.B;
            h.Index = e.Index;
            h.LowerIds.Clear();
            h.LowerIds.Add(x);
            h.UpperIds = upperIds;

            if (Verbose)
            {
                Debug.Log("Split: " + splitIdx);
                DumpHulls(hulls);
                hulls.Dump();
            }
        }

        private void MergeHulls(WorkBuffer<PartialHull> hulls, Event e)
        {
            //Swap pointers for merge search
            var tmp = e.A;
            e.A = e.B;
            e.B = tmp;
            var mergeIdx = BinarySearch.EQ(hulls.Data, e, FindSplit, 0, hulls.UsedSize - 1);
            var upper = hulls.Data[mergeIdx];
            var lower = hulls.Data[mergeIdx - 1];
            lower.UpperIds = upper.UpperIds;
            hulls.RemoveAt(mergeIdx);

            if (Verbose)
            {
                Debug.Log("Merge: " + mergeIdx);
                DumpHulls(hulls);
                hulls.Dump();
            }
        }

        private int TestPoint(PartialHull hull, Vector2 p)
        {
            return Sign(Robust.Orientation(hull.A, hull.B, p));
        }
        
        private int FindSplit(PartialHull hull, Event edge)
        {
            var d = 0;
            
            if (hull.A[0] < edge.A[0])
            {
                d = Sign(Robust.Orientation(hull.A, hull.B, edge.A));
            } else
            {
                d = Sign(Robust.Orientation(edge.B, edge.A, hull.A));
            }

            if (d != 0) return d;
            
            if (edge.B[0] < hull.B[0])
            {
                d = Sign(Robust.Orientation(hull.A, hull.B, edge.B));
            } else
            {
                d = Sign(Robust.Orientation(edge.B, edge.A, hull.B));
            }

            if (d != 0) return d;

            return hull.Index - edge.Index;
        }


        #region Debug

        private void DumpEvent(Event _)
        {
            Debug.Log(string.Format("{{ a: {0}, b: {1}, type: {2}, idx: {3} }}", _.A, _.B, _.Type, _.Index));
        }

        private void DumpEvents(WorkBuffer<Event> events)
        {
            Debug.Log(events.Data.Take(events.UsedSize)
                .Select(_ => string.Format("{{ a: {0}, b: {1}, type: {2}, idx: {3} }}", _.A, _.B, _.Type, _.Index))
                .Aggregate("", (_, s) => _ + s + "\n"));
        }

        private void DumpHulls(WorkBuffer<PartialHull> hulls)
        {
            Debug.Log(hulls.Data.Take(hulls.UsedSize)
                .Select(_ => string.Format("{{ a: {0}, b: {1}, idx: {2}, lowerIds: [ {3} ], upperIds: [ {4} ] }}", _.A, _.B, _.Index, ToString(_.LowerIds), ToString(_.UpperIds)))
                .Aggregate("", (_, s) => _ + s + "\n"));
        }

        private string ToString(List<int> list)
        {
            return string.Join(", ", list.Select(_ => _.ToString()).ToArray());
        }

        #endregion
    }
}