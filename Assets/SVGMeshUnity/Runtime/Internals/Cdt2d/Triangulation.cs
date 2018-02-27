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

        public void BuildTriangles(MeshData data)
        {
            //Handle trivial case
            if((!Interior && !Exterior) || data.Vertices.Count == 0)
            {
                return;
            }

            //Construct initial triangulation
            MonotoneTriangulation.WorkBufferPool = WorkBufferPool;
            MonotoneTriangulation.BuildTriangles(data);

            //If delaunay refinement needed, then improve quality by edge flipping
            if (Delaunay || Interior != Exterior || Infinity)
            {
                /**
                //Index all of the cells to support fast neighborhood queries
                var triangulation = makeIndex(points.length, canonicalizeEdges(edges))
                for (var i = 0; i < cells.length; ++i)
                {
                    var f = cells[i]
                    triangulation.addTriangle(f[0], f[1], f[2])
                }

                //Run edge flipping
                if (delaunay)
                {
                    delaunayFlip(points, triangulation)
                }

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
                {
                    return triangulation.cells()
                }
                /**/

            }
        }

    }
}