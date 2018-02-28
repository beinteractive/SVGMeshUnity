namespace SVGMeshUnity.Internals.Cdt2d
{
    public class Triangulation
    {
        // https://github.com/mikolalysenko/cdt2d
        
        public bool Delaunay = true;
        public bool Interior = true;
        public bool Exterior = true;
        public bool Infinity = false;

        public WorkBufferPool WorkBufferPool;

        private MonotoneTriangulation MonotoneTriangulation = new MonotoneTriangulation();
        private DelaunayRefine DelaunayRefine = new DelaunayRefine();

        public void BuildTriangles(MeshData data)
        {
            //Handle trivial case
            if ((!Interior && !Exterior) || data.Vertices.Count == 0)
            {
                return;
            }

            //Construct initial triangulation
            MonotoneTriangulation.WorkBufferPool = WorkBufferPool;
            MonotoneTriangulation.BuildTriangles(data);

            //If delaunay refinement needed, then improve quality by edge flipping
            if (Delaunay || Interior != Exterior || Infinity)
            {
                //Index all of the cells to support fast neighborhood queries
                var triangles = new Triangles(data);

                //Run edge flipping
                if (Delaunay)
                {
                    DelaunayRefine.WorkBufferPool = WorkBufferPool;
                    DelaunayRefine.RefineTriangles(triangles);
                }

                /**
                //Filter points
                if (!exterior)
                {
                    return filterTriangulation(triangulation, -1)
                }
                else if (!interior)
                {
                    return filterTriangulation(triangulation, 1, infinity)
                }
                else if (infinity)
                {
                    return filterTriangulation(triangulation, 0, infinity)
                }
                else
                /**/
                {
                    triangles.Fill(data.Triangles);
                }

            }
        }

    }
}