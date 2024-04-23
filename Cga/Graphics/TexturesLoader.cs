using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Cga.Graphics;

public class TexturesLoader
{
    public Color[,] LoadDiffuseMap(string path)
    {
        using Bitmap bitmap = new Bitmap(path);

        Color[,] data = new Color[bitmap.Height, bitmap.Width];

        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                data[bitmap.Height - 1 - i, j] = bitmap.GetPixel(j, i);
            }
        }

        return data;
    }

    public Vector3[,] LoadNormalsMap(string path)
    {
        using Bitmap bitmap = new Bitmap(path);

        Vector3[,] data = new Vector3[bitmap.Height, bitmap.Width];

        for (int i = 0; i < bitmap.Height; i++)
        {
            for (int j = 0; j < bitmap.Width; j++)
            {
                Color c = bitmap.GetPixel(j, i);
                data[bitmap.Height - 1 - i, j] = new Vector3(
                    (c.R/255.0f)*2 - 1,
                    (c.G/255.0f)*2 - 1,
                    (c.B/255.0f)*2 - 1
                );
            }
        }

        return data;
    }
}
