using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Cga.Graphics;

public struct Vertex
{
    public Vector3 ScreenPos;

    public Vector3 WorldPos;

    public Vector3 Normal;

    public Color Color;

    public Vector3 TextureCoords;

    public Vertex(Vector3 screenPos, Vector3 worldPos, Vector3 normal, Color color)
    {
        ScreenPos = screenPos;
        WorldPos = worldPos;
        Normal = normal;
        Color = color;
    }

    public Vertex(Vector3 screenPos, Vector3 worldPos, Vector3 textureCoords)
    {
        ScreenPos = screenPos;
        WorldPos = worldPos;
        TextureCoords = textureCoords;
    }
}
