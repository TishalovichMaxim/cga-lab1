using System.Numerics;
using System.Windows.Controls;
using Cga.Drawing;
using GlmNet;

namespace Cga.Graphics;

public static class MeshDrawer {

    private static readonly LightCoeffs coeffs = new LightCoeffs(0.0f, 0.0f, 0.0f, 0.0f);

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

        r = Vector3.Normalize(r);//i think that i can delete it.

        return coeff * MathF.Pow(Vector3.Dot(r, view), shinyCoeff) * color;
    }

    public static void Draw(
        this Mesh mesh,
        WriteableBitmapCanvas canvas,
        mat4 resMat,
        Color color,
        vec3 backLight,
        vec3 view,
        mat4 model,
        Vector3 lightCoeffs
    ) {
        List<vec4> vec4List = new List<vec4>((IEnumerable<vec4>)mesh.Vertices);
        for (int index = 0; index < vec4List.Count; ++index)
        {
            vec4List[index] = resMat * vec4List[index];
            vec4List[index] /= vec4List[index].w;
        }

        mat3 normalsResMat = new mat3(
                new vec3(model[0]),
                new vec3(model[1]),
                new vec3(model[2])
            );

        List<vec3> normals = new List<vec3>((IEnumerable<vec3>)mesh.Normals);
        for (int index = 0; index < normals.Count; ++index)
        {
            normals[index] = normalsResMat * normals[index];
        }

        vec3 baseColor = new vec3(color.R, color.G, color.B);

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

            float intensity1 = float.Max(glm.dot(normal1, backLight), 0.0f);
            float intensity2 = float.Max(glm.dot(normal2, backLight), 0.0f);
            float intensity3 = float.Max(glm.dot(normal3, backLight), 0.0f);

            vec3 resCol = ((intensity1 + intensity2 + intensity3) / 3) * baseColor;

            canvas.DirtyScanLine(
                new Vector3(vertex1.x, vertex1.y, vertex1.z),
                new Vector3(vertex2.x, vertex2.y, vertex2.z),
                new Vector3(vertex3.x, vertex3.y, vertex3.z),
                new Color((byte)resCol.x, (byte)resCol.y, (byte)resCol.z)
                );
        }
    }
}