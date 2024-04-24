using System.Drawing;
using System.Numerics;
using GlmNet;

namespace Cga.Graphics;

public struct Mesh
{
    public List<Vector4> Vertices;
    
    public List<Face> Faces;
    
    public List<Vector3> Normals;
    
    public List<Vector3> Textures;

    public Vector3[,] NormalsMap;

    public Color[,] DiffuseMap;
}
