namespace Cga.LinearAlgebra;

using GlmNet;

public class MatrixManager
{
    public mat4 CreateMatrix(
        float a00,
        float a01,
        float a02,
        float a03,
        float a10,
        float a11,
        float a12,
        float a13,
        float a20,
        float a21,
        float a22,
        float a23,
        float a30,
        float a31,
        float a32,
        float a33)
    {
        return new mat4(new vec4(a00, a10, a20, a30), new vec4(a01, a11, a21, a31), new vec4(a02, a12, a22, a32), new vec4(a03, a13, a23, a33));
    }

    public mat4 GetScaleMatrix(float x, float y, float z)
    {
        return this.CreateMatrix(x, 0.0f, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 0.0f, z, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetTranslate(float x, float y, float z)
    {
        return this.CreateMatrix(1f, 0.0f, 0.0f, x, 0.0f, 1f, 0.0f, y, 0.0f, 0.0f, 1f, z, 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetRotateX(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a21 = MathF.Sin(radians);
        return this.CreateMatrix(1f, 0.0f, 0.0f, 0.0f, 0.0f, num, -a21, 0.0f, 0.0f, a21, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetRotateY(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a20 = MathF.Sin(radians);
        return this.CreateMatrix(num, 0.0f, -a20, 0.0f, 0.0f, 1f, 0.0f, 0.0f, a20, 0.0f, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetRotateZ(float degreesAngle)
    {
        float radians = degreesAngle.ToRadians();
        float num = MathF.Cos(radians);
        float a10 = MathF.Sin(radians);
        return this.CreateMatrix(num, -a10, 0.0f, 0.0f, a10, num, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetViewMatrix(vec3 up, vec3 eye, vec3 target)
    {
        vec3 vec3 = glm.normalize(eye - target);
        vec3 x1 = glm.normalize(glm.cross(up, vec3));
        vec3 x2 = up;
        return this.CreateMatrix(x1.x, x1.y, x1.z, -glm.dot(x1, eye), x2.x, x2.y, x2.z, -glm.dot(x2, eye), vec3.x, vec3.y, vec3.z, -glm.dot(vec3, eye), 0.0f, 0.0f, 0.0f, 1f);
    }

    public mat4 GetProjectionMatrix(float zNear, float zFar, float aspect, float fov)
    {
        float num = MathF.Tan(fov.ToRadians() / 2f);
        return this.CreateMatrix((float) (1.0 / ((double) aspect * (double) num)), 0.0f, 0.0f, 0.0f, 0.0f, 1f / num, 0.0f, 0.0f, 0.0f, 0.0f, zFar / (zNear - zFar), (float) ((double) zNear * (double) zFar / ((double) zNear - (double) zFar)), 0.0f, 0.0f, -1f, 0.0f);
    }

    public mat4 GetViewportMatrix(float width, float height, float xMin, float yMin)
    {
        return this.CreateMatrix(width / 2f, 0.0f, 0.0f, xMin + width / 2f, 0.0f, (float) (-(double) height / 2.0), 0.0f, yMin + height / 2f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
    }
}
