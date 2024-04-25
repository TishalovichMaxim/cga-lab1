using System.Numerics;
using GlmNet;

namespace Cga.Drawing;

public struct Color
{
    public static Color Red = new Color(byte.MaxValue, 0, 0);
    
    public static Color Green = new Color(0, byte.MaxValue, 0);
    
    public static Color Blue = new Color(0, 0, byte.MaxValue);
    
    public byte R;
    
    public byte G;
    
    public byte B;

    public byte A;

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    public Color(Vector3 color)
    {
        this.R = (byte)color.X;
        this.G = (byte)color.Y;
        this.B = (byte)color.Z;
        this.A = 255;
    }

    public Color(vec3 color)
    {
        this.R = (byte)color.x;
        this.G = (byte)color.y;
        this.B = (byte)color.z;
        this.A = 255;
    }
    
    public Vector3 ToVector3()
    {
        return new Vector3(R, G, B);
    }
}