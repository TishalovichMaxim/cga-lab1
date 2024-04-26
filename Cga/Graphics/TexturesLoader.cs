using System.Drawing;
using GlmNet;

namespace Cga.Graphics;

public class TexturesLoader
{
    public vec3[,] LoadMap(string path)
    {
        using Bitmap bitmap = new Bitmap(path);

        vec3[,] data = new vec3[bitmap.Height, bitmap.Width];

        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color pixel = bitmap.GetPixel(j, i);
                data[bitmap.Height - 1 - i, j] = (new vec3(pixel.R, pixel.G, pixel.B))/255.0f;
            }
        }

        return data;
    }

    public vec3[,] LoadNormalsMap(string path)
    {
        using Bitmap bitmap = new Bitmap(path);

        vec3[,] data = new vec3[bitmap.Height, bitmap.Width];

        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color c = bitmap.GetPixel(j, i);
                data[bitmap.Height - 1 - i, j] = new vec3(
                    (c.R/255.0f)*2 - 1,
                    (c.G/255.0f)*2 - 1,
                    (c.B/255.0f)*2 - 1
                );
            }
        }

        return data;
    }
}
