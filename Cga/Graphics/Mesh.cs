using System.Drawing;
using System.Numerics;
using GlmNet;

namespace Cga.Graphics;

public struct Mesh
{
    public List<vec4> Vertices;
    
    public List<Face> Faces;
    
    public List<vec3> Normals;
    
    public List<vec3> Textures;

    public Vector3[,] NormalsMap;

    public Color[,] DiffuseMap;
}