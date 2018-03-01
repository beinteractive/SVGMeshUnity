# SVGMeshUnity

<img width="757" alt="typo" src="https://user-images.githubusercontent.com/1482297/36834202-765a8abe-1d75-11e8-8be9-369d5978ed71.png">

Generates mesh from [SVG path](https://github.com/beinteractive/SVGMeshUnity/blob/de472c2f716329e2991f75fcab9bd7253cc8ff12/Assets/Samples/Scripts/Simple.cs#L8-L14) in realtime for Unity.

This is a port of https://github.com/mattdesl/svg-mesh-3d

## Install

Copy `Assets/SVGMeshUnity` directory to your Assets directory.

Add a `SVGMesh` component to a GameObject that has `MeshFilter` and `MeshRenderer`.

<img width="321" alt="inspector" src="https://user-images.githubusercontent.com/1482297/36832680-d7e41b0c-1d6f-11e8-93f6-98b146e924dc.png">

## Usage

### Create mesh from SVG path

<img width="239" alt="twitter" src="https://user-images.githubusercontent.com/1482297/36832746-20568ff0-1d70-11e8-8cc1-7e8d0f2156b4.png"> <img width="252" alt="twtitter-wire" src="https://user-images.githubusercontent.com/1482297/36832554-49228084-1d6f-11e8-9049-7a5e9bdc7b1b.png">

```C#
var mesh = GetComponent<SVGMesh>();
var svg = new SVGData();
svg.Path("M17.316,6.246c0.008,0.162,0.011,... and so on");
mesh.Fill(svg);
```

Simply create an instance of `SVGData` and set SVG path data string by calling `SVGData.Path()`.
Then call `Mesh.Fill()`, a mesh will be generated.

### Create mesh from code

Instead of use SVG path data, you can directly make path data in your code.

![pentagon](https://user-images.githubusercontent.com/1482297/36834678-f834db56-1d76-11e8-8845-56547b673004.gif)
![square](https://user-images.githubusercontent.com/1482297/36834680-f882d6bc-1d76-11e8-9330-05bec5611a6e.gif)
![square-wire](https://user-images.githubusercontent.com/1482297/36834679-f85b554c-1d76-11e8-9f84-75953f338ea7.gif)

```C#
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
```

Use `SVGData.Move`, `SVGData.Line`, `SVGData.Curve`, ... and so on. Create realtime path as you like.

### Options

The following options are provided in SVGMesh component.

- `Delaunay` (default `false`)
  - whether to use Delaunay triangulation
  - Delaunay triangulation is slower, but looks better
- `Scale` (default `1`)
  - a positive number, the scale at which to [approximate the curves](https://github.com/mattdesl/adaptive-bezier-curve) from the SVG paths
  - higher number leads to smoother corners, but slower triangulation
- `Interior` if set, only return interior faces. See note. (Default `true`)
- `Exterior` if set, only return exterior faces. See note. (Default `false`)
- `Infinity` if set, then the triangulation is augmented with a [point at infinity](https://en.wikipedia.org/wiki/Point_at_infinity) represented by the index `-1`.  (Default `false`)

## License

MIT
