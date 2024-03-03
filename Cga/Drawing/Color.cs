namespace Cga.Drawing;

public struct Color
{
    public static Color Red = new Color(byte.MaxValue,  0,  0);
    
    public static Color Green = new Color(0, byte.MaxValue,  0);
    
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
}