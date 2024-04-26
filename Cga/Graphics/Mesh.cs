using GlmNet;

namespace Cga.Graphics;

public struct Mesh
{
    public List<vec4> Vertices;
    
    public List<Face> Faces;
    
    public List<vec3> Normals;
    
    public List<vec3> Textures;

    public vec3[,] NormalsMap;

    public vec3[,] DiffuseMap;

    public vec3[,] SpecularMap;
}