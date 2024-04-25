using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GlmNet;

namespace Cga.Graphics;

public struct Vertex
{
    public vec4 ScreenPos;

    public vec4 WorldPos;

    public vec3 Normal;

    public Color Color;

    public vec3 TextureCoords;

    public Vertex(vec4 screenPos, vec4 worldPos, vec3 normal, Color color)
    {
        ScreenPos = screenPos;
        WorldPos = worldPos;
        Normal = normal;
        Color = color;
    }

    public Vertex(vec4 screenPos, vec4 worldPos, vec3 textureCoords)
    {
        ScreenPos = screenPos;
        WorldPos = worldPos;
        TextureCoords = textureCoords;
    }
}
