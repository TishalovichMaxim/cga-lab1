using System.Drawing;
using System.IO;
using System.Windows.Media;
using Cga.Graphics;
using GlmNet;

namespace Cga.Graphics;

public class ObjParser
{
    private readonly Dictionary<string, Action<string[]>> _handlers;
    
    private readonly List<vec4> _vertices = new List<vec4>();
    
    private readonly List<vec3> _normals = new List<vec3>();
    
    private readonly List<Face> _faces = new List<Face>();
    
    private readonly List<vec3> _textures = new List<vec3>();

    private TexturesLoader _texturesLoader = new();

    public ObjParser()
    {
        this._handlers = new Dictionary<string, Action<string[]>>();
        this._handlers["v"] = new Action<string[]>(this.VHandler);
        this._handlers["vt"] = new Action<string[]>(this.VtHandler);
        this._handlers["vn"] = new Action<string[]>(this.VnHandler);
        this._handlers["f"] = new Action<string[]>(this.FHandler);
    }

    public Mesh Parse(string fileName, string normalsMapPath, string diffuseMapPath, string specularMapPath)
    {
        foreach (string readAllLine in File.ReadAllLines(fileName))
        {
            string[] strArray = readAllLine.Split();
            if (strArray.Length != 0 && this._handlers.ContainsKey(strArray[0]))
                this._handlers[strArray[0]](strArray);
        }
        return new Mesh()
        {
            Faces = this._faces,
            Normals = this._normals,
            Vertices = this._vertices,
            Textures = this._textures,

            NormalsMap = _texturesLoader.LoadNormalsMap(normalsMapPath),
            DiffuseMap = _texturesLoader.LoadMap(diffuseMapPath),
            SpecularMap = _texturesLoader.LoadMap(specularMapPath),
        };
    }

    private void VHandler(string[] parts)
    {
        float w = 1f;
        if (parts.Length == 5)
            w = float.Parse(parts[4]);
        vec4 vec4 = new vec4(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), w);
        vec4 /= w;
        this._vertices.Add(vec4);
    }

    private void VtHandler(string[] parts)
    {
        float x = float.Parse(parts[1]);
        float y = 0.0f;
        float z = 0.0f;
        if (parts.Length >= 3)
        {
            y = float.Parse(parts[2]);
            if (parts.Length == 4)
                z = float.Parse(parts[3]);
        }
        this._textures.Add(new vec3(x, y, z));
    }

    private void VnHandler(string[] parts)
    {
        this._normals.Add(new vec3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
    }

    private void FHandler(string[] parts)
    {
        int nVert = parts.Length - 1;
        int[] numArray1 = new int[nVert];
        int[] numArray2 = new int[nVert];
        int[] numArray3 = new int[nVert];

        for (int index = 1; index < parts.Length; ++index)
        {
            string[] strArray = parts[index].Split("/", StringSplitOptions.None);
            int num1 = int.Parse(strArray[0]);
            int num2 = 0;
            int num3 = 0;

            if (strArray.Length == 2)
            {
                num2 = int.Parse(strArray[1]);
            }

            else if (strArray.Length == 3)
            {
                if (!strArray[1].Equals(""))
                {
                    num2 = int.Parse(strArray[1]);
                }
                num3 = int.Parse(strArray[2]);
            }

            numArray1[index - 1] = num1;
            numArray3[index - 1] = num2;
            numArray2[index - 1] = num3;
        }

        for (int i = 0; i < numArray1.Length - 2; ++i)
        {
            _faces.Add(new Face()
            {
                VertIndices = [numArray1[0], numArray1[i + 1], numArray1[i + 2]],
                NormIndices = [numArray2[0], numArray2[i + 1], numArray2[i + 2]],
                TextIndices = [numArray3[0], numArray3[i + 1], numArray3[i + 2]]
            });
        }
    }
}