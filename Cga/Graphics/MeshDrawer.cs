using System.Numerics;
using System.Windows.Controls;
using Cga.Drawing;
using GlmNet;

namespace Cga.Graphics;

public static class MeshDrawer {
    public static void Draw(this Mesh mesh, WriteableBitmapCanvas canvas, mat4 resMat, Color color, vec3 backLight, mat4 model)
    {
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
            vec4 vec4_1 = vec4List[face.VertIndices[0] - 1];
            vec4 vec4_2 = vec4List[face.VertIndices[1] - 1];
            vec4 vec4_3 = vec4List[face.VertIndices[2] - 1];

            vec3 normal1 = normals[face.NormIndices[0] - 1];
            vec3 normal2 = normals[face.NormIndices[1] - 1];
            vec3 normal3 = normals[face.NormIndices[2] - 1];

            float intensity1 = glm.dot(normal1, backLight);
            float intensity2 = glm.dot(normal2, backLight);
            float intensity3 = glm.dot(normal3, backLight);

            vec3 resCol = ((intensity1 + intensity2 + intensity3) / 3) * baseColor;

            canvas.DirtyScanLine(
                new Vector3(vec4_1.x, vec4_1.y, vec4_1.z),
                new Vector3(vec4_2.x, vec4_2.y, vec4_2.z),
                new Vector3(vec4_3.x, vec4_3.y, vec4_3.z),
                new Color((byte)resCol.x, (byte)resCol.y, (byte)resCol.z)
                );
        }
    }
}