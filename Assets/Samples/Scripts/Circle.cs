using SVGMeshUnity;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private SVGMesh Mesh;

    private SVGData SVG;

    void Start()
    {
        SVG = new SVGData();
    }

    void Update()
    {
        SVG.Clear();

        for (var i = 0; i < 4; ++i)
        {
            var angle0 = Mathf.PI * 0.5f * (i + 0);
            var angle1 = Mathf.PI * 0.5f * (i + 1);

            var r = 3f;
            var x0 = Mathf.Cos(angle0) * r;
            var y0 = Mathf.Sin(angle0) * r;
            var x1 = Mathf.Cos(angle1) * r;
            var y1 = Mathf.Sin(angle1) * r;

            var a = r * (4f / 3f) * Mathf.Tan((angle1 - angle0) / 4f);
            var inAngle = angle0 + Mathf.PI * 0.5f;
            var inX = x0 + Mathf.Cos(inAngle) * a;
            var inY = y0 + Mathf.Sin(inAngle) * a;
            var outAngle = angle1 - Mathf.PI * 0.5f;
            var outX = x1 + Mathf.Cos(outAngle) * a;
            var outY = y1 + Mathf.Sin(outAngle) * a;

            if (i == 0)
            {
                SVG.Move(x0, y0);
            }
            
            SVG.Curve(inX, inY, outX, outY, x1, y1);
        }
        
        Mesh.Fill(SVG);
    }
}