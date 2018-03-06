using SVGMeshUnity;
using UnityEngine;

public class MetaballBehaviour : MonoBehaviour
{
    [SerializeField] private SVGMesh Circle1Mesh;
    [SerializeField] private SVGMesh Circle2Mesh;
    [SerializeField] private SVGMesh MetaballMesh;

    private SVGData Circle1SVG;
    private SVGData Circle2SVG;
    private SVGData MetaballSVG;

    void Start()
    {
        Circle1SVG = new SVGData();
        Circle2SVG = new SVGData();
        MetaballSVG = new SVGData();
    }

    void Update()
    {
        Circle1SVG.Clear();
        Circle2SVG.Clear();
        MetaballSVG.Clear();

        var c1 = (Vector2) Circle1Mesh.transform.localPosition;
        var c2 = (Vector2) Circle2Mesh.transform.localPosition;

        var c1R = 1f;
        var c2R = 1.2f;
        
        var v = Mathf.Max(0.5f, 1.3f - Mathf.Clamp01((c2 - c1).magnitude / 8f));
        
        Circle(Circle1SVG, Vector2.zero, c1R);
        Circle(Circle2SVG, Vector2.zero, c2R);
        Metaball(MetaballSVG, c1, c1R, c2, c2R, v);
        
        Circle1Mesh.Fill(Circle1SVG);
        Circle2Mesh.Fill(Circle2SVG);
        MetaballMesh.Fill(MetaballSVG);
    }

    private void Circle(SVGData svg, Vector2 c, float r)
    {
        for (var i = 0; i < 4; ++i)
        {
            var angle0 = Mathf.PI * 0.5f * (i + 0);
            var angle1 = Mathf.PI * 0.5f * (i + 1);

            var x0 = c.x + Mathf.Cos(angle0) * r;
            var y0 = c.y - Mathf.Sin(angle0) * r;
            var x1 = c.x + Mathf.Cos(angle1) * r;
            var y1 = c.y - Mathf.Sin(angle1) * r;

            var a = r * (4f / 3f) * Mathf.Tan((angle1 - angle0) / 4f);
            var inAngle = angle0 + Mathf.PI * 0.5f;
            var inX = x0 + Mathf.Cos(inAngle) * a;
            var inY = y0 - Mathf.Sin(inAngle) * a;
            var outAngle = angle1 - Mathf.PI * 0.5f;
            var outX = x1 + Mathf.Cos(outAngle) * a;
            var outY = y1 - Mathf.Sin(outAngle) * a;

            if (i == 0)
            {
                svg.Move(x0, y0);
            }
            
            svg.Curve(inX, inY, outX, outY, x1, y1);
        }
    }
    
    // http://shspage.com/aijs/

    private void Metaball(SVGData svg, Vector2 c1, float r1, Vector2 c2, float r2, float v)
    {
        if (r1 == 0f || r2 == 0f)
        {
            return;
        }
  
        var pi2 = Mathf.PI / 2f;

        var d = (c2 - c1).magnitude;

        var u1 = 0f;
        var u2 = 0f;
        if (d <= Mathf.Abs(r1 - r2))
        {
            return;
        }
        else if (d < r1 + r2)
        {
            // case circles are overlapping
            u1 = Mathf.Acos((r1 * r1 + d * d - r2 * r2) / (2 * r1 * d));
            u2 = Mathf.Acos((r2 * r2 + d * d - r1 * r1) / (2 * r2 * d));
        }

        var t1 = Mathf.Atan2(c2.y - c1.y, c2.x - c1.x);
        var t2 = Mathf.Acos((r1 - r2) / d);
  
        var t1a = t1 + u1 + (t2 - u1) * v;
        var t1b = t1 - u1 - (t2 - u1) * v;
        var t2a = t1 + Mathf.PI - u2 - (Mathf.PI - u2 - t2) * v;
        var t2b = t1 - Mathf.PI + u2 + (Mathf.PI - u2 - t2) * v;
  
        var p1a = PointOnCircle(c1, t1a, r1);
        var p1b = PointOnCircle(c1, t1b, r1);
        var p2a = PointOnCircle(c2, t2a, r2);
        var p2b = PointOnCircle(c2, t2b, r2);

        // define handle length by the distance between both ends of the curve to draw
        var handle_len_rate = 2;
        var d2 = Mathf.Min(v * handle_len_rate, (p2a - p1a).magnitude / (r1 + r2));
        d2 *= Mathf.Min(1, d * 2 / (r1 + r2)); // case circles are overlapping
        r1 *= d2;
        r2 *= d2;
        
        svg.Move(p1a);
        svg.Curve(PointOnCircle(p1a, t1a - pi2, r1), PointOnCircle(p2a, t2a + pi2, r2), p2a);
        svg.Line(p2b);
        svg.Curve(PointOnCircle(p2b, t2b - pi2, r2), PointOnCircle(p1b, t1b + pi2, r1), p1b);
        svg.Line(p1a);
    }

    private Vector2 PointOnCircle(Vector2 c, float angle, float r)
    {
        return c + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * r;
    }
}