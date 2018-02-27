﻿using SVGMeshUnity.Internals;
using SVGMeshUnity.Internals.Cdt2d;
using UnityEngine;

namespace SVGMeshUnity
{
    public class SVGMesh : MonoBehaviour
    {
        // https://github.com/mattdesl/adaptive-bezier-curve

        public float Scale = 1f;
        
        private static WorkBufferPool WorkBufferPool = new WorkBufferPool();
        
        private MeshData MeshData = new MeshData();
        private Mesh Mesh;

        private BezierToVertex BezierToVertex;
        private Triangulation Triangulation;

        private void Awake()
        {
            BezierToVertex = new BezierToVertex();
            BezierToVertex.WorkBufferPool = WorkBufferPool;
            
            Triangulation = new Triangulation();
            Triangulation.WorkBufferPool = WorkBufferPool;
        }
        
        public void Fill(SVGData svg)
        {
            MeshData.Clear();
            
            // convert curves into discrete points
            BezierToVertex.Scale = Scale;
            BezierToVertex.GetContours(svg, MeshData);
            
            // triangulate mesh
            Triangulation.BuildTriangles(MeshData);
            
            if (Mesh == null)
            {
                Mesh = new Mesh();
                Mesh.MarkDynamic();
            }
            
            MeshData.Flip();
            MeshData.Upload(Mesh);

            var filter = GetComponent<MeshFilter>();
            if (filter != null)
            {
                filter.sharedMesh = Mesh;
            }
        }
    }
}