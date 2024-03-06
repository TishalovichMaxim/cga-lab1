using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cga.LinearAlgebra;
using GlmNet;

namespace Cga.Drawing;

public class WriteableBitmapCanvas
{
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
            _writeableBitmap.PixelWidth*BytesPerPixel,
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
            int x = Round(from.Item1 + stepX*i);
            int y = Round(from.Item2 + stepY*i);
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

    public void DrawTriangle(Color color, vec3 p1, vec3 p2, vec3 p3)
    {
        (int x, int y) pi1 = (p1.x.NearInt(), p1.y.NearInt());
        (int x, int y) pi2 = (p2.x.NearInt(), p2.y.NearInt());
        (int x, int y) pi3 = (p3.x.NearInt(), p3.y.NearInt());

        int left = Math.Min(
            Math.Min(pi1.x, pi2.x),
            pi3.x
            );
        
        int right = Math.Max(
            Math.Max(pi1.x, pi2.x),
            pi3.x
            );
        
        int top = Math.Min(
            Math.Min(pi1.y, pi2.y),
            pi3.y
            );
        
        int bottom = Math.Max(
            Math.Max(pi1.y, pi2.y),
            pi3.y
            );

        byte[] tempPixelsData = new byte[(right - left + 1)*(bottom - left + 1)*BytesPerPixel];
        
        DrawLine(color,
            tempPixelsData,
            right - left + 1,
            (pi1.x - left, pi1.y - top, p1.z),
            (pi2.x - left, pi2.y - top, p2.z)
            );
        
        DrawLine(color,
            tempPixelsData,
            right - left + 1,
            (pi2.x - left, pi2.y - top, p2.z),
            (pi3.x - left, pi3.y - top, p3.z)
            );
    }

    private Func<int, float> getPosFunc(int p1x, int p1y, int p2x, int p2y)
    {
        return (int y) =>
        {
            return (p1x + (p2x - p1x) * ((float)(y - p1y)) / (p2y - p1y));
        };
    }

    public void DrawTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
    {
        if (p2.Y < p1.Y)
        {
            (p1, p2) = (p2, p1);
        }

        if (p2.Y < p3.Y)
        {
            (p2, p3) = (p3, p2);
        }

        if (p1.Y > p3.Y)
        {
            (p1, p3) = (p3, p1);
        }

        int p1x = (int)p1.X;
        int p1y = (int)p1.Y;

        int p2x = (int)p2.X;
        int p2y = (int)p2.Y;

        int p3x = (int)p3.X;
        int p3y = (int)p3.Y;

        Func<int, float> longFunc = getPosFunc(p1x, p1y, p2x, p2y);
        Func<int, float> firstFunc = getPosFunc(p1x, p1y, p3x, p3y);
        Func<int, float> secondFunc = getPosFunc(p3x, p3y, p2x, p2y);

        Func<int, float> leftFunc = firstFunc;
        Func<int, float> rightFunc = longFunc;

        if (p2x < p3x)
        {
            leftFunc = longFunc;
            rightFunc = firstFunc;
        }

        if (p1y == p3y)
        {
            int left = p1x;
            int right = p1x;

            if (p1x > p3x)
            {
                left = p3x;
            }
            else
            {
                right = p1x;
            }

            for (int j = left; j <= right; j++)
            {
                DrawPixel(color, _pixelsData, Width, (j, p1y, 0.01f));//change 0.01f here
            }
        }
        else
        {
            for (int i = p1y; i <= p3y; i++)
            {
                float val12 = rightFunc(i);
                float val13 = leftFunc(i);

                int right = val12.NearInt();
                int left = val13.NearInt();

                for (int j = left; j <= right; j++)
                {
                    DrawPixel(color, _pixelsData, Width, (j, i, 0.01f));//change 0.01f here
                }
            }
        }

        if (p2y == p3y)
        {
            int left = p2x;
            int right = p2x;

            if (p2x > p3x)
            {
                left = p3x;
            }
            else
            {
                right = p1x;
            }

            for (int j = left; j <= right; j++)
            {
                DrawPixel(color, _pixelsData, Width, (j, p2y, 0.01f));//change 0.01f here
            }
        }
        else
        {

            if (p2x < p3x)
            {
                rightFunc = secondFunc;
            } else
            {
                leftFunc = secondFunc;
            } 

            for (int i = p3y + 1; i <= p2y; i++)
            {
                float val12 = rightFunc(i);
                float val13 = leftFunc(i);

                int right = val12.NearInt();
                int left = val13.NearInt();

                for (int j = left; j <= right; j++)
                {
                    DrawPixel(color, _pixelsData, Width, (j, i, 0.01f));//change 0.01f here
                }
            }
        }
    }
}