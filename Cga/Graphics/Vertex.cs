using System;
using System.Collections.Generic;
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

    public Vertex(Vector3 screenPos, Vector3 worldPos, Vector3 normal)
    {
        ScreenPos = screenPos;
        WorldPos = worldPos;
        Normal = normal;
    }
}
