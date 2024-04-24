using System.Numerics;
using Cga.Drawing;
using GlmNet;

namespace Cga.Graphics;

public static class MeshDrawer {

    public static readonly LightCoeffs coeffs = new LightCoeffs(0.15f, 0.4f, 0.2f, 4.0f);

    private static Vector3 GetAmbitentColor(float coeff, Vector3 color)
    {
        return coeff * color;
    }

    //normal - normalized
    //light - normalized
    private static Vector3 GetDiffuseColor(float coeff, Vector3 normal, Vector3 light, Vector3 color)
    {
        return coeff * MathF.Max(0.0f, Vector3.Dot(normal, -light)) * color;
    }

    //normal - normalized
    //light - normalized
    private static Vector3 GetSpecularColor(float coeff, float shinyCoeff, Vector3 light, Vector3 normal, Vector3 view, Vector3 color)
    {
        Vector3 r = light + 2 * Vector3.Dot(light, normal) * normal;

        r = Vector3.Normalize(r);//I think that I can delete it.

        return coeff * MathF.Pow(Vector3.Dot(r, view), shinyCoeff) * color;
    }

    public static void Draw(
        this Mesh mesh,
        WriteableBitmapCanvas canvas,
        Matrix4x4 resMat,
        Vector3 view,
        Matrix4x4 model,
        LightCoeffs lightCoeffs,
        Vector3 lightPos,
        Vector3 color,
        Vector3 eye
    ) {
        Vector3 ambientColor = lightCoeffs.ka * color;

        List<vec4> vec4List = mesh.Vertices.;
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
            vec4List[index] /= vec4List[index].w;
        }

        List<vec3> normals = new List<vec3>((IEnumerable<vec3>)mesh.Normals);
        for (int index = 0; index < normals.Count; ++index)
        {
            normals[index] = model3 * normals[index];
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

            vec3 normal1 = normals[face.NormIndices[0] - 1];
            vec3 normal2 = normals[face.NormIndices[1] - 1];
            vec3 normal3 = normals[face.NormIndices[2] - 1];

            vec4 world1 = worldVertices[face.VertIndices[0] - 1];
            vec4 world2 = worldVertices[face.VertIndices[1] - 1];
            vec4 world3 = worldVertices[face.VertIndices[2] - 1];

            Vertex vert1 = new Vertex(
                new Vector3(vertex1.x, vertex1.y, vertex1.z),
                new Vector3(world1.x, world1.y, world1.z),
                new Vector3(normal1.x, normal1.y, normal1.z)
                );

            Vertex vert2 = new Vertex(
                new Vector3(vertex2.x, vertex2.y, vertex2.z),
                new Vector3(world2.x, world2.y, world2.z),
                new Vector3(normal2.x, normal2.y, normal2.z)
                );

            Vertex vert3 = new Vertex(
                new Vector3(vertex3.x, vertex3.y, vertex3.z),
                new Vector3(world3.x, world3.y, world3.z),
                new Vector3(normal3.x, normal3.y, normal3.z)
                );

            canvas.ScanLine(
                vert1,
                vert2,
                vert3,
                ambientColor,
                color,
                lightCoeffs,
                lightPos,
                eye
                );
        }
    }
}