using System.Numerics;
using Cga.Drawing;
using GlmNet;

namespace Cga.Graphics;

public static class MeshDrawer
{
    private static readonly float delta = 0.00001f;

    public static readonly LightCoeffs coeffs = new LightCoeffs(0.15f, 0.4f, 0.3f, 8.0f);

    public static void Draw(
        this Mesh mesh,
        WriteableBitmapCanvas canvas,
        mat4 resMat,
        vec3 view,
        mat4 model,
        LightCoeffs lightCoeffs,
        vec4 lightPos,
        vec3 color,
        vec4 eye
    ) {
        vec3 ambientColor = lightCoeffs.ka * color;

        List<vec4> vec4List = new List<vec4>(mesh.Vertices);
        List<vec4> worldVertices = new List<vec4>();

        mat3 model3 = new mat3(
                new vec3(model[0]),
                new vec3(model[1]),
                new vec3(model[2])
            );

        for (int index = 0; index < vec4List.Count; ++index)
        {
            vec4 worldVertex = model * vec4List[index];
            worldVertex /= worldVertex.w;
            
            worldVertices.Add(worldVertex);

            vec4List[index] = resMat * vec4List[index];
            
            float w = vec4List[index].w;
            vec4List[index] /= w;
            vec4List[index] = new vec4(
                vec4List[index].x,
                vec4List[index].y,
                vec4List[index].z,
                w
            ); 
        }

        foreach (Face face in mesh.Faces)
        {
            vec4 vertex1 = vec4List[face.VertIndices[0] - 1];
            vec4 vertex2 = vec4List[face.VertIndices[1] - 1];
            vec4 vertex3 = vec4List[face.VertIndices[2] - 1];

            vec3 faceNormal = glm.cross(new vec3(vertex2 - vertex1), new vec3(vertex3 - vertex2));

            if (glm.dot(faceNormal, view) < 0)
            {
                continue;
            }

            vec4 world1 = worldVertices[face.VertIndices[0] - 1];
            vec4 world2 = worldVertices[face.VertIndices[1] - 1];
            vec4 world3 = worldVertices[face.VertIndices[2] - 1];

            Vertex vert1 = new Vertex(
                vertex1,
                world1,
                new vec3(mesh.Textures[face.TextIndices[0] - 1][0], mesh.Textures[face.TextIndices[0] - 1][1], 0.0f)
                );

            Vertex vert2 = new Vertex(
                vertex2,
                world2,
                new vec3(mesh.Textures[face.TextIndices[1] - 1][0], mesh.Textures[face.TextIndices[1] - 1][1], 0.0f)
                );

            Vertex vert3 = new Vertex(
                vertex3,
                world3,
                new vec3(mesh.Textures[face.TextIndices[2] - 1][0], mesh.Textures[face.TextIndices[2] - 1][1], 0.0f)
                );

            canvas.ScanLine(
                mesh,
                vert1,
                vert2,
                vert3,
                ambientColor,
                lightCoeffs,
                lightPos,
                eye,
                model
                );
        }
    }
}