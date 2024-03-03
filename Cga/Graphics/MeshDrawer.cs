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
            
            canvas.DrawLine(color, (vec4_1.x, vec4_1.y), (vec4_2.x, vec4_2.y));
            canvas.DrawLine(color, (vec4_3.x, vec4_3.y), (vec4_2.x, vec4_2.y));
            canvas.DrawLine(color, (vec4_1.x, vec4_1.y), (vec4_3.x, vec4_3.y));
        }
    }
}