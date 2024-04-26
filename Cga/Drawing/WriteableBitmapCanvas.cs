using System.Windows;
using System.Windows.Media.Imaging;
using Cga.Graphics;
using Cga.LinearAlgebra;
using GlmNet;

namespace Cga.Drawing;

delegate void DrawHorizontalLine(int a);

public class WriteableBitmapCanvas
{
    private static readonly float Delta = 0.001f;
    
    private static readonly int BytesPerPixel = 4;

    private readonly WriteableBitmap _writeableBitmap;

    private readonly byte[] _pixelsData;

    private readonly float[] _zBuffer;

    private int Round(float num) => (int)(num + 0.5);

    public int Width => _writeableBitmap.PixelWidth;

    public int Height => _writeableBitmap.PixelHeight;


    public WriteableBitmapCanvas(WriteableBitmap writeableBitmap)
    {
        _writeableBitmap = writeableBitmap;
        int nPixels = _writeableBitmap.PixelWidth * _writeableBitmap.PixelHeight;
        int bytesLen = nPixels * BytesPerPixel;
        _pixelsData = new byte[bytesLen];
        _zBuffer = new float[nPixels];
        //Array.Fill(_zBuffer, 1.1f);
    }

    public void DrawPixel(Color color, byte[] pixelsData, int stride, (int x, int y, float z) pos)
    {
        int pixelNumber = pos.y * _writeableBitmap.PixelWidth + pos.x;

        if (pos.z > _zBuffer[pixelNumber])
        {
            return;
        }

        _zBuffer[pixelNumber] = pos.z;

        DrawPixel(color, pixelsData, stride, (pos.x, pos.y));
    }

    public void Clear()
    {
        Array.Clear(_pixelsData, 0, _pixelsData.Length);
    }

    public void Clear(Color color)
    {

        Array.Fill(_pixelsData, (byte)0);
        //for (int i = 0; i < _writeableBitmap.Height; i++)
        //{
        //    for (int j = 0; j < _writeableBitmap.Width; j++)
        //    {
        //        DrawPixel(color, _pixelsData, Width, (j, i, -1.0f));
        //    }
        //}

        Array.Fill(_zBuffer, 1.1f);
    }

    public void Swap()
    {
        Int32Rect dirtyRect = new Int32Rect(
            0, 0, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight);

        _writeableBitmap.WritePixels(
            dirtyRect,
            _pixelsData,
            _writeableBitmap.PixelWidth * BytesPerPixel,
            0
            );
    }

    public void DrawLine(Color color, byte[] pixelData, int stride, (int, int, float) from, (int, int, float) to)
    {
        int dx = to.Item1 - from.Item1;
        int dy = to.Item2 - from.Item2;

        int maxDelta = Math.Max(Math.Abs(dx), Math.Abs(dy));
        if (maxDelta == 0)
        {
            return;
        }

        float stepX = ((float)dx) / maxDelta;
        float stepY = ((float)dy) / maxDelta;
        float stepZ = (to.Item3 - from.Item3) / maxDelta;

        for (int i = 0; i <= maxDelta; i++)
        {
            int x = Round(from.Item1 + stepX * i);
            int y = Round(from.Item2 + stepY * i);
            float z = from.Item3 + stepZ * i;
            DrawPixel(
                color,
                pixelData,
                stride,
                (x, y, z)
                );
        }
    }

    public void DrawLine(Color color, vec3 from, vec3 to)
    {
        DrawLine(
            color,
            _pixelsData,
            Width,
            (from.x.NearInt(), from.y.NearInt(), from.z),
            (to.x.NearInt(), to.y.NearInt(), to.z)
            );
    }

    public void DrawPixel(Color color, byte[] pixelsData, int stride, (int x, int y) pos)
    {
        int offset = (pos.y * stride + pos.x) * BytesPerPixel;

        pixelsData[offset] = color.B;
        pixelsData[offset + 1] = color.G;
        pixelsData[offset + 2] = color.R;
        pixelsData[offset + 3] = color.A;
    }
    
    public vec3 GetTextureValue(vec3 textureCoords, vec3[,] diffuseMap)
    {
        float floatRow = (diffuseMap.GetLength(0) - 1) * textureCoords.y;
        float floatCol = (diffuseMap.GetLength(1) - 1) * textureCoords.x;

        int row = (int)floatRow;
        int col = (int)floatCol;

        int nextRow = Math.Min((diffuseMap.GetLength(0) - 1), row + 1);
        int nextCol = Math.Min((diffuseMap.GetLength(1) - 1), col + 1);

        vec3 top = Vec.Lerp(diffuseMap[nextRow, col], diffuseMap[nextRow, nextCol], floatCol - col);
        vec3 bottom = Vec.Lerp(diffuseMap[row, col], diffuseMap[row, nextCol], floatCol - col);

        return Vec.Lerp(bottom, top, floatRow - row);
    }
        
    private void ScanLineCommon(
        Mesh mesh,
        int y,
        int left,
        float zLeft,
        vec3 textureCoordsLeft,
        vec4 worldLeft,
        int right,
        float zRight,
        vec3 textureCoordsRight,
        vec4 worldRight,
        vec3 ambientColor,
        LightCoeffs lightCoeffs,
        vec4 lightPos,
        vec4 eye,
        mat4 model,
        float wLeft,
        float wRight
        )
    {
        vec3 lightColor = new vec3(230, 230, 230);
        
        if (left > right)
        {
            (left, right) = (right, left);
            (zLeft, zRight) = (zRight, zLeft);
            (worldLeft, worldRight) = (worldRight, worldLeft);
            (textureCoordsLeft, textureCoordsRight) = (textureCoordsRight, textureCoordsLeft);
            (wLeft, wRight) = (wRight, wLeft);
        }

        for (int x = left; x < right; x++)
        {
            float t = ((float)(x - left)) / (right - left);

            float z = float.Lerp(zLeft, zRight, t);
            vec3 textureCoords = LerpTextureCoords(
                textureCoordsLeft,
                wLeft,
                textureCoordsRight,
                wRight,
                t
            );

            vec3 normal = glm.normalize(new vec3(model * new vec4(GetTextureValue(textureCoords, mesh.NormalsMap), 1.0f)));
            
            vec3 kd = GetTextureValue(textureCoords, mesh.DiffuseMap);
            
            vec3 ks = GetTextureValue(textureCoords, mesh.SpecularMap);
            
            vec4 world = Vec.Lerp(worldLeft, worldRight, t);

            vec3 light = new vec3(glm.normalize(lightPos - world));
            vec3 diffuseColor = kd * MathF.Max(0.0f, glm.dot(normal, light)) * lightColor;

            vec3 reflected = light - 2 * glm.dot(light, normal) * normal;
            vec3 view = new vec3(glm.normalize(eye - world));

            vec3 specularColor = ks * MathF.Pow(glm.dot(reflected, view), lightCoeffs.shiny) * lightColor;

            //vec3 resColor = diffuseColor;
            //vec3 resColor = specularColor;
            vec3 resColor = diffuseColor + specularColor;
            
            DrawPixel(new Color(resColor), _pixelsData, Width, (x, y, z));
        }
    }

    public vec3 LerpTextureCoords(vec3 from, float zFrom, vec3 to, float zTo, float t)
    {
        return ((1 - t) * from / zFrom + t * to / zTo) / ((1 - t)*(1/zFrom) + t/zTo);
    }
    
    public void ScanLine(
        Mesh mesh,
        Vertex vertex1,
        Vertex vertex2,
        Vertex vertex3,
        vec3 ambientColor,
        LightCoeffs lightCoeffs,
        vec4 lightSourcePos,
        vec4 eye,
        mat4 model
        )
    {
        if (vertex1.ScreenPos.y > vertex2.ScreenPos.y)
        {
            (vertex1, vertex2) = (vertex2, vertex1);
        }

        if (vertex1.ScreenPos.y > vertex3.ScreenPos.y)
        {
            (vertex1, vertex3) = (vertex3, vertex1);
        }

        if (vertex2.ScreenPos.y > vertex3.ScreenPos.y)
        {
            (vertex2, vertex3) = (vertex3, vertex2);
        }

        vec4 p1 = vertex1.ScreenPos;
        vec4 p2 = vertex2.ScreenPos;
        vec4 p3 = vertex3.ScreenPos;

        int p1x = (int)p1.x;
        int p1y = (int)p1.y;

        int p2x = (int)p2.x;
        int p2y = (int)p2.y;

        int p3x = (int)p3.x;
        int p3y = (int)p3.y;

        for (int y = p1y; y < p2y; y++)
        {
            float tLeft = ((float)(y - p1y)) / (p2y - p1y);
            float tRight = ((float)(y - p1y)) / (p3y - p1y);

            int left = (int)float.Lerp(p1x, p2x, tLeft);
            float zLeft = float.Lerp(p1.z, p2.z, tLeft);
            vec4 worldLeft = Vec.Lerp(vertex1.WorldPos, vertex2.WorldPos, tLeft);
            vec3 textureCoordsLeft = LerpTextureCoords(
                vertex1.TextureCoords,
                vertex1.ScreenPos.w,
                vertex2.TextureCoords,
                vertex2.ScreenPos.w,
                tLeft
            );

            int right = (int)float.Lerp(p1x, p3x, tRight);
            float zRight = float.Lerp(p1.z, p3.z, tRight);
            vec4 worldRight = Vec.Lerp(vertex1.WorldPos, vertex3.WorldPos, tRight);
            vec3 textureCoordsRight = LerpTextureCoords(
                vertex1.TextureCoords,
                vertex1.ScreenPos.w,
                vertex3.TextureCoords,
                vertex3.ScreenPos.w,
                tRight
            );

            float wLeft = float.Lerp(vertex1.ScreenPos.w, vertex2.ScreenPos.w, tLeft);
            float wRight = float.Lerp(vertex1.ScreenPos.w, vertex3.ScreenPos.w, tRight);
            
            ScanLineCommon(
                mesh,
                y,
                left,
                zLeft,
                textureCoordsLeft,
                worldLeft,
                right,
                zRight,
                textureCoordsRight,
                worldRight,
                ambientColor,
                lightCoeffs,
                lightSourcePos,
                eye,
                model,
                wLeft,
                wRight
            );
        }

        for (int y = p2y; y < p3y; y++)
        {
            float tLeft = ((float)(y - p2y)) / (p3y - p2y);
            float tRight = ((float)(y - p1y)) / (p3y - p1y);

            int left = (int)float.Lerp(p2x, p3x, tLeft);
            float zLeft = float.Lerp(p2.z, p3.z, tLeft);
            vec4 worldLeft = Vec.Lerp(vertex2.WorldPos, vertex3.WorldPos, tLeft);
            vec3 textureCoordsLeft = LerpTextureCoords(
                vertex2.TextureCoords,
                vertex2.ScreenPos.w,
                vertex3.TextureCoords,
                vertex3.ScreenPos.w,
                tLeft
            );

            int right = (int)float.Lerp(p1x, p3x, tRight);
            float zRight = float.Lerp(p1.z, p3.z, tRight);
            vec4 worldRight = Vec.Lerp(vertex1.WorldPos, vertex3.WorldPos, tRight);
            vec3 textureCoordsRight = LerpTextureCoords(
                vertex1.TextureCoords,
                vertex1.ScreenPos.w,
                vertex3.TextureCoords,
                vertex3.ScreenPos.w,
                tRight
            );

            float wLeft = float.Lerp(vertex2.ScreenPos.w, vertex3.ScreenPos.w, tLeft);
            float wRight = float.Lerp(vertex1.ScreenPos.w, vertex3.ScreenPos.w, tRight);
            
            ScanLineCommon(
                mesh,
                y,
                left,
                zLeft,
                textureCoordsLeft,
                worldLeft,
                right,
                zRight,
                textureCoordsRight,
                worldRight,
                ambientColor,
                lightCoeffs,
                lightSourcePos,
                eye,
                model,
                wLeft,
                wRight
            );
        }
    }
}