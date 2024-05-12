using Cga.LinearAlgebra;
using GlmNet;

namespace Cga.Graphics;

public struct Face
{
    public int[] VertIndices;
    
    public int[] NormIndices;
    
    public int[] TextIndices;

    public mat3 TbnMatrix;

    public void CalculateTbn(Mesh mesh)
    {
        vec4 v1 = mesh.Vertices[VertIndices[0] - 1];
        vec4 v2 = mesh.Vertices[VertIndices[1] - 1];
        vec4 v3 = mesh.Vertices[VertIndices[2] - 1];

        vec3 t1 = mesh.Textures[TextIndices[0] - 1];
        vec3 t2 = mesh.Textures[TextIndices[1] - 1];
        vec3 t3 = mesh.Textures[TextIndices[2] - 1];

        vec3 dt12 = t2 - t1;
        vec3 dt13 = t3 - t1;

        float u12 = dt12.x;
        float u13 = dt13.x;

        float v12 = dt12.y;
        float v13 = dt13.y;
            
        vec3 e12 = new vec3(v2 - v1);
        vec3 e13 = new vec3(v3 - v1);

        vec3 norm = glm.normalize(glm.cross(e12, e13));
        
        vec3 tang = glm.normalize((e13*v12 - e12*v13)/(u13*v12 - u12*v13));

        vec3 bitang = glm.cross(norm, tang);

        TbnMatrix = new mat3(
            tang,
            bitang,
            norm
        );
    }
}