using System.Collections.Generic;
using SVGMeshUnity.Internals;
using UnityEngine;

namespace SVGMeshUnity
{
    public class SVGMesh : MonoBehaviour
    {
        // https://github.com/mattdesl/adaptive-bezier-curve

        public float Scale = 1f;
        
        private static WorkBuffer<Vector2> WorkVertices = new WorkBuffer<Vector2>(32);
        
        private MeshData MeshData = new MeshData();
        private Mesh Mesh;

        private BezierToVertex BezierToVertex;

        private void Awake()
        {
            BezierToVertex = new BezierToVertex();
            BezierToVertex.MeshData = MeshData;
            BezierToVertex.WorkVertices = WorkVertices;
        }
        
        public void Fill(SVGData svg)
        {
            WorkVertices.Clear();
            MeshData.Clear();
            
            // convert curves into discrete points
            BezierToVertex.Scale = Scale;
            BezierToVertex.GetContours(svg);
            
            for (var i = 0; i < MeshData.Vertices.Count - 2; ++i)
            {
                MeshData.Triangles.Add(i + 0);
                MeshData.Triangles.Add(i + 1);
                MeshData.Triangles.Add(i + 2);
            }
            
            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.MarkDynamic();
            }
            
            MeshData.Upload(Mesh);

            var filter = GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.sharedMesh = Mesh;
            }
        }
    }
}