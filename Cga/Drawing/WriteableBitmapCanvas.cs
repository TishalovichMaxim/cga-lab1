using System.Windows;
using System.Windows.Media.Imaging;
using Cga.LinearAlgebra;

namespace Cga.Drawing;

public class WriteableBitmapCanvas
{
    private static readonly int BytesPerPixel = 4;
    
    private readonly WriteableBitmap _writeableBitmap;

    private readonly byte[] _pixelsData;

    private int Round(float num) => (int)(num + 0.5);

    public int Width => _writeableBitmap.PixelWidth;
    
    public int Height => _writeableBitmap.PixelHeight;
    
    public WriteableBitmapCanvas(WriteableBitmap writeableBitmap)
    {
        _writeableBitmap = writeableBitmap;
        int bytesLen = _writeableBitmap.PixelWidth
                       * _writeableBitmap.PixelHeight * BytesPerPixel;
        _pixelsData = new byte[bytesLen];
    }
    
    public void DrawPixel(Color color, (int x, int y) pos)
    {
        int offset = (pos.y * _writeableBitmap.PixelWidth + pos.x) * BytesPerPixel;

        _pixelsData[offset] = color.B;
        _pixelsData[offset + 1] = color.G;
        _pixelsData[offset + 2] = color.R;
        _pixelsData[offset + 3] = color.A;
    }

    public void Clear()
    {
        Array.Clear(_pixelsData, 0, _pixelsData.Length);
    }

    public void Clear(Color color)
    {
        for (int i = 0; i < _writeableBitmap.Height; i++)
        {
            for (int j = 0; j < _writeableBitmap.Width; j++)
            {
                DrawPixel(color, (j, i));
            }
        }
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

    public void DrawLine(Color color, (int, int) from, (int, int) to)
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

        for (int i = 0; i < maxDelta; i++)
        {
            int x = Round(from.Item1 + stepX*i);
            int y = Round(from.Item2 + stepY*i);
            DrawPixel(
                color,
                (x, y)
                );
        }
    }

    public void DrawLine(Color color, (float, float) from, (float, float) to)
    {
        DrawLine(
            color,
            (from.Item1.NearInt(), from.Item2.NearInt()),
            (to.Item1.NearInt(), to.Item2.NearInt())
            );
    }
}