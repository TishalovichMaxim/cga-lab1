using GlmNet;

namespace Cga.LinearAlgebra;

internal static class MathHelper
{
    public static float ToRadians(this float angleInDegrees)
    {
        return MathF.PI * angleInDegrees / 180.0f;
    }

    public static int NearInt(this float number)
    {
        return (int)(number + 0.5f);
    }

    public static void add(this mat3 m1, mat3 m2)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                m1[i, j] += m2[i, j];
            }
        }
    }
    
    public static void div(this mat3 m, float v)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                m[i, j] /= v;
            }
        }
    }
}