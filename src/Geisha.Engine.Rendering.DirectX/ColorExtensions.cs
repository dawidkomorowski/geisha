﻿using Geisha.Engine.Core.Math;
using SharpDX.Mathematics.Interop;

namespace Geisha.Engine.Rendering.DirectX
{
    internal static class ColorExtensions
    {
        public static RawColor4 ToRawColor4(this Color color)
        {
            return new RawColor4((float) color.DoubleR, (float) color.DoubleG, (float) color.DoubleB, (float) color.DoubleA);
        }
    }
}