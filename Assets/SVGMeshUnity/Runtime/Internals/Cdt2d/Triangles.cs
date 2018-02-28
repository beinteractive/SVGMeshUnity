using System;
using System.Collections.Generic;
using UnityEngine;

namespace SVGMeshUnity.Internals.Cdt2d
{
    public class Triangles
    {
        public Triangles(MeshData data)
        {
            PrivateVertices = data.Vertices;
            CreateStars(data.Vertices.Count);
            CreateEdges(data.Edges);
            CreateTriangles(data.Triangles);
        }

        public List<Vector3> Vertices
        {
            get { return PrivateVertices; }
        }

        public WorkBuffer<Vector2Int>[] Stars
        {
            get { return PrivateStars; }
        }

        private List<Vector3> PrivateVertices;
        private WorkBuffer<Vector2Int>[] PrivateStars;
        private Vector2Int[] Edges;

        private void CreateStars(int n)
        {
            var stars = new WorkBuffer<Vector2Int>[n];
            for (var i = 0; i < n; ++i)
            {
                stars[i] = new WorkBuffer<Vector2Int>(16);
            }
            PrivateStars = stars;
        }

        private void CreateEdges(List<Vector2Int> source)
        {
            var l = source.Count;
            var edges = new Vector2Int[l];
            for (var i = 0; i < l; ++i)
            {
                var edge = source[i];
                if (edge.y < edge.x)
                {
                    var x = edge.x;
                    edge.x = edge.y;
                    edge.y = x;
                }
                edges[i] = edge;
            }
            Array.Sort(edges, CompareEdge);
            Edges = edges;
        }

        private static int CompareEdge(Vector2Int a, Vector2Int b)
        {
            var d = a.x - b.x;
            if (d != 0) return d;
            return a.y - b.y;
        }
 
        private void CreateTriangles(List<int> source)
        {
            var l = source.Count;
            for (var i = 0; i < l; i += 3)
            {
                AddTriangle(source[i + 0], source[i + 1], source[i + 2]);
            }
        }
        
        public void AddTriangle(int i, int j, int k)
        {
            var jk = new Vector2Int(j, k);
            var ki = new Vector2Int(k, i);
            var ij = new Vector2Int(i, j);
            PrivateStars[i].Push(ref jk);
            PrivateStars[j].Push(ref ki);
            PrivateStars[k].Push(ref ij);
        }
        
        public void RemoveTriangle(int i, int j, int k)
        {
            RemovePair(PrivateStars[i], j, k);
            RemovePair(PrivateStars[j], k, i);
            RemovePair(PrivateStars[k], i, j);
        }

        private void RemovePair(WorkBuffer<Vector2Int> list, int j, int k)
        {
            var n = list.UsedSize;
            var data = list.Data;
            for (var i = 0; i < n; ++i)
            {
                var s = data[i];
                if (s.x == j && s.y == k)
                {
                    data[i] = data[n - 1];
                    list.RemoveLast(1);
                    return;
                }
            }
        }
        
        public int Opposite(int j, int i)
        {
            var list = PrivateStars[i];
            var n = list.UsedSize;
            var data = list.Data;
            for (var k = 0; k < n; ++k)
            {
                if (data[k].y == j)
                {
                    return data[k].x;
                }
            }
            return -1;
        }
        
        public void Flip(int i, int j)
        {
            var a = Opposite(i, j);
            var b = Opposite(j, i);
            RemoveTriangle(i, j, a);
            RemoveTriangle(j, i, b);
            AddTriangle(i, b, a);
            AddTriangle(j, a, b);
        }

        public bool IsConstraint(int i, int j)
        {
            var e = new Vector2Int(Mathf.Min(i, j), Mathf.Max(i, j));
            return BinarySearch.EQ(Edges, e, CompareEdge, 0, Edges.Length) >= 0;
        }

        public void Fill(List<int> triangles)
        {
            var n = PrivateStars.Length;

            if (triangles.Capacity < n)
            {
                triangles.Capacity = n;
            }
            triangles.Clear();
            
            for(var i = 0; i < n; ++i)
            {
                var list = PrivateStars[i];
                var data = list.Data;
                var m = list.UsedSize;
                for(var j = 0; j < m; ++j)
                {
                    var s = data[j];
                    if(i < Mathf.Min(s.x, s.y))
                    {
                        triangles.Add(i);
                        triangles.Add(s.x);
                        triangles.Add(s.y);
                    }
                }
            }
        }
    }
}