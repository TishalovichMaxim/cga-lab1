using GlmNet;

namespace Cga.LinearAlgebra;

public static class Vec
{
    public static vec4 Lerp(vec4 from, vec4 to, float t)
    { 
        return (from * (1.0f - t)) + (to * t);
    }
    
    public static vec3 Lerp(vec3 from, vec3 to, float t)
    { 
        return (from * (1.0f - t)) + (to * t);
    }
}