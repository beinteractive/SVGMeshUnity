using System;
using System.Collections.Generic;
using System.Linq;
using SVGMeshUnity;
using UnityEngine;
using Random = UnityEngine.Random;

public class Cut : MonoBehaviour
{
    class Rect : MonoBehaviour
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;
        public Vector2 D;

        public Vector4 R;
        
        public Vector2 Velocity;

        public Func<Action<Rect>, Rect> NewRect;

        private SVGData SVG;
        private SVGMesh Mesh;

        void Start()
        {
            SVG = new SVGData();
            Mesh = GetComponent<SVGMesh>();
            Render();
        }

        void Update()
        {
            Update(Time.deltaTime);
            Render();
        }

        private void Update(float dt)
        {
            R = Vector4.Lerp(Vector4.one * 0.6f, R, Mathf.Exp(-0.6f * dt));

            A += Velocity;
            B += Velocity;
            C += Velocity;
            D += Velocity;

            Velocity = Vector2.Lerp(Vector2.zero, Velocity, Mathf.Exp(-2f * dt));
        }

        private void Render()
        {
            SVG.Clear();
            Draw(SVG);
            Mesh.Fill(SVG);
        }

        private void Draw(SVGData svg)
        {
            DrawR(svg, A, D, B, R.x, true);
            DrawR(svg, B, A, C, R.y);
            DrawR(svg, C, B, D, R.z);
            DrawR(svg, D, C, A, R.w);
        }
        
        private void DrawR(SVGData svg, Vector2 p, Vector2 pPrev, Vector2 pNext, float r, bool first = false)
        {
            var angle0 = Mathf.Atan2(p.y - pNext.y, p.x - pNext.x);
            var angle1 = Mathf.Atan2(p.y - pPrev.y, p.x - pPrev.x);
            
            if (angle0 > angle1)
            {
                angle0 = angle0 - Mathf.PI * 2f;
            }

            var angleBetween = Mathf.Lerp(angle0, angle1, 0.5f);

            var cx = p.x - Mathf.Cos(angleBetween) * r * Mathf.Sqrt(2f);
            var cy = p.y - Mathf.Sin(angleBetween) * r * Mathf.Sqrt(2f);

            var x0 = cx + Mathf.Cos(angle0) * r;
            var y0 = cy + Mathf.Sin(angle0) * r;
            var x1 = cx + Mathf.Cos(angle1) * r;
            var y1 = cy + Mathf.Sin(angle1) * r;

            var a = r * (4f / 3f) * Mathf.Tan((angle1 - angle0) / 4f);
            var inAngle = angle0 + Mathf.PI * 0.5f;
            var inX = x0 + Mathf.Cos(inAngle) * a;
            var inY = y0 + Mathf.Sin(inAngle) * a;
            var outAngle = angle1 - Mathf.PI * 0.5f;
            var outX = x1 + Mathf.Cos(outAngle) * a;
            var outY = y1 + Mathf.Sin(outAngle) * a;

            if (first)
            {
                svg.Move(x0, y0);
            }
            else
            {
                svg.Line(x0, y0);
            }
        
            svg.Curve(inX, inY, outX, outY, x1, y1);
        }

        public void Split(Vector2 p1, Vector2 p2, List<Rect> rects)
        {
            var ab = GetIntersection(p1, p2, A, B);
            var bc = GetIntersection(p1, p2, B, C);
            var cd = GetIntersection(p1, p2, C, D);
            var da = GetIntersection(p1, p2, D, A);

            var vel = Random.Range(0.006f, 0.007f);
            var vel2 = vel * Random.Range(0.2f, 0.3f);
            
            if (ab != null && cd != null)
            {
                rects.Add(NewRect(r =>
                {
                    r.A = A;
                    r.B = ab.Value;
                    r.C = cd.Value;
                    r.D = D;
                    r.R = new Vector4(R.x, 0f, 0f, R.w);
                    r.Velocity = Velocity + new Vector2(vel, vel2);
                }));
                rects.Add(NewRect(r =>
                {
                    r.A = ab.Value;
                    r.B = B;
                    r.C = C;
                    r.D = cd.Value;
                    r.R = new Vector4(0f, R.y, R.z, 0f);
                    r.Velocity = Velocity - new Vector2(vel, vel2);
                }));
                Destroy(gameObject);
            }

            if (bc != null && da != null)
            {
                rects.Remove(this);
                rects.Add(NewRect(r =>
                {
                    r.A = A;
                    r.B = B;
                    r.C = bc.Value;
                    r.D = da.Value;
                    r.R = new Vector4(R.x, R.y, 0f, 0f);
                    r.Velocity = Velocity + new Vector2(vel2, vel);
                }));
                rects.Add(NewRect(r =>
                {
                    r.A = da.Value;
                    r.B = bc.Value;
                    r.C = C;
                    r.D = D;
                    r.R = new Vector4(0f, 0f, R.z, R.w);
                    r.Velocity = Velocity - new Vector2(vel2, vel);
                }));
                Destroy(gameObject);
            }
        }

        private Vector2? GetIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);
            if (d == 0f)
            {
                return null;
            }

            var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
            var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;
            if (u < 0.0f || u > 1.0f)
            {
                return null;
            }
            if (v < 0.0f || v > 1.0f)
            {
                return null;
            }

            return Vector2.Lerp(p1, p2, u);
        }
    }

    [SerializeField] private GameObject Prefab;
    
    private List<Rect> Rects = new List<Rect>();
    private Vector2 DragStart;
    private bool IsDragging;

    void Start()
    {
        Rects.Add(NewRect(r =>
        {
            r.A = new Vector2(3f, 3f);
            r.B = new Vector2(-3f, 3f);
            r.C = new Vector2(-3f, -3f);
            r.D = new Vector2(3f, -3f);
        }));
    }

    void Update()
    {
        Rects.RemoveAll(r => r == null);
        
        if (IsDragging)
        {
            var p = GetMousePoint();
            
            if (Input.GetMouseButton(0))
            {
                
            }
            else
            {
                IsDragging = false;
                
                foreach (var r in Rects.ToList())
                {
                    r.Split(DragStart, p, Rects);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                DragStart = GetMousePoint();
                IsDragging = true;
            }
        }
    }

    private Vector2 GetMousePoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var enter = 0f;
        if (new Plane(Vector3.forward, Vector3.zero).Raycast(ray, out enter))
        {
            return Vector2.Scale(ray.GetPoint(enter), new Vector2(1f, -1f));
        }
        return Vector2.zero;
    }

    private Rect NewRect(Action<Rect> f)
    {
        var g = Instantiate(Prefab, transform);
        var r = g.AddComponent<Rect>();
        r.NewRect = NewRect;
        f(r);
        g.SetActive(true);
        return r;
    }
}