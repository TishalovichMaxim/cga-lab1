using System.Numerics;

namespace Cga.LinearAlgebra;

using GlmNet;

public class MatrixManager
{
    public Matrix4x4 GetScaleMatrix(float x, float y, float z)
    {
        return new Matrix4x4(x, 0.0f, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 0.0f, z, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public Matrix4x4 GetTranslate(float x, float y, float z)
    {
        return new Matrix4x4(1f, 0.0f, 0.0f, x, 0.0f, 1f, 0.0f, y, 0.0f, 0.0f, 1f, z, 0.0f, 0.0f, 0.0f, 1f);
    }

    public Matrix4x4 GetRotateX(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a21 = MathF.Sin(radians);
        return new Matrix4x4(1f, 0.0f, 0.0f, 0.0f, 0.0f, num, -a21, 0.0f, 0.0f, a21, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public Matrix4x4 GetRotateY(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a20 = MathF.Sin(radians);
        return new Matrix4x4(num, 0.0f, -a20, 0.0f, 0.0f, 1f, 0.0f, 0.0f, a20, 0.0f, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public Matrix4x4 GetRotateZ(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a10 = MathF.Sin(radians);
        return new Matrix4x4(num, -a10, 0.0f, 0.0f, a10, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public Matrix4x4 GetViewMatrix(vec3 up, vec3 eye, vec3 target)
    {
        vec3 vec3 = glm.normalize(eye - target);
        vec3 x1 = glm.normalize(glm.cross(up, vec3));
        vec3 x2 = glm.normalize(glm.cross(vec3, x1));

        return new Matrix4x4(
            x1.x, x1.y, x1.z, -glm.dot(x1, eye),
            x2.x, x2.y, x2.z, -glm.dot(x2, eye),
            vec3.x, vec3.y, vec3.z, -glm.dot(vec3, eye),
            0.0f, 0.0f, 0.0f, 1f
            );
    }

    public Matrix4x4 GetProjectionMatrix(float zNear, float zFar, float aspect, float fov)
    {
        float num = MathF.Tan(fov.ToRadians() / 2f);
        return new Matrix4x4((float) (1.0 / ((double) aspect * (double) num)), 0.0f, 0.0f, 0.0f, 0.0f, 1f / num, 0.0f, 0.0f, 0.0f, 0.0f, zFar / (zNear - zFar), (float) ((double) zNear * (double) zFar / ((double) zNear - (double) zFar)), 0.0f, 0.0f, -1f, 0.0f);
    }

    public Matrix4x4 GetViewportMatrix(float width, float height, float xMin, float yMin)
    {
        return new Matrix4x4(width / 2f, 0.0f, 0.0f, xMin + width / 2f, 0.0f, (float) (-(double) height / 2.0), 0.0f, yMin + height / 2f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }
}
