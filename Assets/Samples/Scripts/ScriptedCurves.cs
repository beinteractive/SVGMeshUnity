using SVGMeshUnity;
using UnityEngine;

public class ScriptedCurves : MonoBehaviour
{
    [SerializeField] private SVGMesh Mesh;

    private SVGData SVG;

    void Start()
    {
        SVG = new SVGData();
    }
    
    /**
    void Update()
    {
        SVG.Clear();

        var r = 3f;
        
        for (var i = 0; i < 4; ++i)
        {
            var t = Time.time * 2f;
            
            var i0 = i;
            var i1 = (i + 1) % 4;

            var a = Mathf.PI * 0.25f; // (t / (Mathf.PI * 2f)) * Mathf.PI;
            var angle0 = a + Mathf.PI * 0.5f * i0;
            var angle1 = a + Mathf.PI * 0.5f * i1;

            var x0 = Mathf.Cos(angle0) * r;
            var y0 = Mathf.Sin(angle0) * r;
            var x1 = Mathf.Cos(angle1) * r;
            var y1 = Mathf.Sin(angle1) * r;

            var cx = x0 + (x1 - x0) * 0.5f;
            var cy = y0 + (y1 - y0) * 0.5f;
            var ca = Mathf.Atan2(cy, cx);
            var cr = 0.75f + Mathf.Sin(t) * 1.5f;
            cx += Mathf.Cos(ca) * cr;
            cy += Mathf.Sin(ca) * cr;

            if (i == 0)
            {
                SVG.Move(x0, y0);
            }
            
            SVG.Curve(cx, cy, cx, cy, x1, y1);
        }
        
        Mesh.Fill(SVG);
    }
    /**/

    /**/
    void Update()
    {
        SVG.Clear();

        var resolution = 5;
        var radius = 3f;
        
        SVG.Move(NoisedR(radius, 0), 0f);
        
        for (var i = 0; i < resolution; ++i)
        {
            var i0 = i;
            var i1 = (i + 1) % resolution;
            
            var angle0 = Mathf.PI * 2f * ((float) i0 / resolution);
            var angle1 = Mathf.PI * 2f * ((float) i1 / resolution);

            var r0 = NoisedR(radius, i0);
            var r1 = NoisedR(radius, i1);
            var x0 = Mathf.Cos(angle0) * r0;
            var y0 = Mathf.Sin(angle0) * r0;
            var x1 = Mathf.Cos(angle1) * r1;
            var y1 = Mathf.Sin(angle1) * r1;

            var cx = x0 + (x1 - x0) * 0.5f;
            var cy = y0 + (y1 - y0) * 0.5f;
            var ca = Mathf.Atan2(cy, cx);
            var cr = 0.3f + (Mathf.PerlinNoise(Time.time, i * -100f) - 0.5f) * 1.15f;
            cx += Mathf.Cos(ca) * cr;
            cy += Mathf.Sin(ca) * cr;
            
            SVG.Curve(cx, cy, cx, cy, x1, y1);
        }
        
        Mesh.Fill(SVG);
    }

    private float NoisedR(float r, float randomize)
    {
        return r + (Mathf.PerlinNoise(Time.time, randomize * 10f) - 0.5f) * 0.5f;
    }
    /**/
}