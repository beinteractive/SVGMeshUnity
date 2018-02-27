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

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            Triangles.Clear();
        }

        public void AddVertices(WorkBuffer<Vector2> buffer)
        {
            var firstEdgeIdx = -1;

            for (var i = 0; i < buffer.UsedSize; ++i)
            {
                var v = buffer.Data[i];
                var idx = Vertices.IndexOf(v);
                if (idx == -1)
                {
                    Vertices.Add(v);
                    idx = Vertices.Count - 1;
                }
                
                Edges.Add(idx);

                if (i == 0)
                {
                    firstEdgeIdx = idx;
                }
            }
            
            Edges.Add(firstEdgeIdx);
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