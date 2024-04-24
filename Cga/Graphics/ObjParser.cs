using System.IO;
using System.Numerics;

namespace Cga.Graphics;

public class ObjParser
{
    private readonly Dictionary<string, Action<string[]>> _handlers;
    
    private readonly List<Vector4> _vertices = new List<Vector4>();
    
    private readonly List<Vector3> _normals = new List<Vector3>();
    
    private readonly List<Face> _faces = new List<Face>();
    
    private readonly List<Vector3> _textures = new List<Vector3>();

    private readonly TexturesLoader _texturesLoader = new();

    public ObjParser()
    {
        _handlers = new Dictionary<string, Action<string[]>>();
        _handlers["v"] = VHandler;
        _handlers["vt"] = VtHandler;
        _handlers["vn"] = VnHandler;
        _handlers["f"] = FHandler;
    }

    public Mesh Parse(string fileName, string normalsMapPath, string diffuseMapPath)
    {
        foreach (string readAllLine in File.ReadAllLines(fileName))
        {
            string[] strArray = readAllLine.Split();
            if (strArray.Length != 0 && this._handlers.ContainsKey(strArray[0]))
                this._handlers[strArray[0]](strArray);
        }
        return new Mesh()
        {
            Faces = _faces,
            Normals = _normals,
            Vertices = _vertices,
            Textures = _textures,

            NormalsMap = _texturesLoader.LoadNormalsMap(normalsMapPath),
            DiffuseMap = _texturesLoader.LoadDiffuseMap(diffuseMapPath),
        };
    }

    private void VHandler(string[] parts)
    {
        float w = 1f;
        if (parts.Length == 5)
            w = float.Parse(parts[4]);
        Vector4 vec = new Vector4(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]), w);
        vec /= w;
        this._vertices.Add(vec);
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
        this._textures.Add(new Vector3(x, y, z));
    }

    private void VnHandler(string[] parts)
    {
        this._normals.Add(new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
    }

    private void FHandler(string[] parts)
    {
        int nVert = parts.Length - 1;
        int[] numArray1 = new int[nVert];
        int[] numArray2 = new int[nVert];
        int[] numArray3 = new int[nVert];

        for (int index = 1; index < parts.Length; ++index)
        {
            string[] strArray = parts[index].Split("/");
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