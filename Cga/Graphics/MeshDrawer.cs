using Cga.Drawing;
using GlmNet;

namespace Cga.Graphics;

public static class MeshDrawer {
    public static void Draw(this Mesh mesh, WriteableBitmapCanvas canvas, mat4 resMat, Color color)
    {
        List<vec4> vec4List = new List<vec4>((IEnumerable<vec4>)mesh.Vertices);
        for (int index = 0; index < vec4List.Count; ++index)
        {
            vec4List[index] = resMat * vec4List[index];
            vec4List[index] /= vec4List[index].w;
        }

        foreach (Face face in mesh.Faces)
        {
            vec4 vec4_1 = vec4List[face.VertIndices[0] - 1];
            vec4 vec4_2 = vec4List[face.VertIndices[1] - 1];
            vec4 vec4_3 = vec4List[face.VertIndices[2] - 1];
            
            canvas.DrawLine(color, new vec3(vec4_1), new vec3(vec4_2));
            canvas.DrawLine(color, new vec3(vec4_2), new vec3(vec4_3));
            canvas.DrawLine(color, new vec3(vec4_3), new vec3(vec4_1));
        }
    }
}