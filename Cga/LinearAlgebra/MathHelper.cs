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
}