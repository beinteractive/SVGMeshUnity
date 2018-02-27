using System.Collections.Generic;
using UnityEngine;

namespace SVGMeshUnity.Internals
{
    public class MeshData
    {
        public MeshData()
        {
            Vertices = new List<Vector3>();
            Edges = new List<int>();
            Triangles = new List<int>();
        }
        
        public List<Vector3> Vertices { get; private set; }
        public List<int> Edges { get; private set; }
        public List<int> Triangles { get; private set; }
        
        public int EdgeCount
        {
            get { return Edges.Count / 2; }
        }

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            Triangles.Clear();
        }

        public void AddVertices(WorkBuffer<Vector2> buffer)
        {
            var firstEdgeIdx = -1;
            var prevEdgeidx = -1;

            for (var i = 0; i < buffer.UsedSize; ++i)
            {
                var v = buffer.Data[i];
                var idx = Vertices.IndexOf(v);
                if (idx == -1)
                {
                    Vertices.Add(v);
                    idx = Vertices.Count - 1;
                }

                if (idx == prevEdgeidx)
                {
                    continue;
                }
                
                if (i == 0)
                {
                    firstEdgeIdx = idx;
                }
                else
                {
                    Edges.Add(prevEdgeidx);
                    Edges.Add(idx);
                }

                prevEdgeidx = idx;
            }

            if (prevEdgeidx != firstEdgeIdx)
            {
                Edges.Add(prevEdgeidx);
                Edges.Add(firstEdgeIdx);
            }
        }

        public int GetEdgeA(int i)
        {
            return Edges[i * 2 + 0];
        }

        public int GetEdgeB(int i)
        {
            return Edges[i * 2 + 1];
        }

        public void Flip()
        {
            var l = Vertices.Count;
            for (var i = 0; i < l; ++i)
            {
                Vertices[i] *= -1f;
            }
        }

        public void Upload(Mesh m)
        {
            m.SetVertices(Vertices);
            m.SetTriangles(Triangles, 0);
            m.RecalculateBounds();
            m.RecalculateNormals();
        }
    }
}