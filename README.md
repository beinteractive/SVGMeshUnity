# SVGMeshUnity

<img width="498" alt="twtitter-wire" src="https://user-images.githubusercontent.com/1482297/36832554-49228084-1d6f-11e8-9049-7a5e9bdc7b1b.png">

Generates mesh from SVG path (like the following) in realtime for Unity.

```
M17.316,6.246c0.008,0.162,0.011,0.326,0.011,0.488c0,4.99-3.797,10.742-10.74,10.742c-2.133,0-4.116-0.625-5.787-1.697
c0.296,0.035,0.596,0.053,0.9,0.053c1.77,0,3.397-0.604,4.688-1.615c-1.651-0.031-3.046-1.121-3.526-2.621
c0.23,0.043,0.467,0.066,0.71,0.066c0.345,0,0.679-0.045,0.995-0.131c-1.727-0.348-3.028-1.873-3.028-3.703c0-0.016,0-0.031,0-0.047
c0.509,0.283,1.092,0.453,1.71,0.473c-1.013-0.678-1.68-1.832-1.68-3.143c0-0.691,0.186-1.34,0.512-1.898
C3.942,5.498,6.725,7,9.862,7.158C9.798,6.881,9.765,6.594,9.765,6.297c0-2.084,1.689-3.773,3.774-3.773
c1.086,0,2.067,0.457,2.756,1.191c0.859-0.17,1.667-0.484,2.397-0.916c-0.282,0.881-0.881,1.621-1.66,2.088
c0.764-0.092,1.49-0.293,2.168-0.594C18.694,5.051,18.054,5.715,17.316,6.246z
```

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
