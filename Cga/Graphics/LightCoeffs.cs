using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cga.Graphics;

internal struct LightCoeffs
{
    float ka;
    float kd;
    float ks;
    float shiny;

    public LightCoeffs(float ka, float kd, float ks, float shiny)
    {
        this.ka = ka;
        this.kd = kd;
        this.ks = ks;
        this.shiny = shiny;
    }
}
