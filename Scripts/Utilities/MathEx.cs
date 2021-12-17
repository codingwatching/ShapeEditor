﻿#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    public static class MathEx
    {
        public static Rect RectXYXY(float x1, float y1, float x2, float y2)
        {
            var mx1 = math.min(x1, x2);
            var my1 = math.min(y1, y2);
            var mx2 = math.max(x1, x2);
            var my2 = math.max(y1, y2);

            return new Rect(mx1, my1, mx2 - mx1, my2 - my1);
        }

        public static Rect RectXYXY(float2 a, float2 b)
        {
            var mx1 = math.min(a.x, b.x);
            var my1 = math.min(a.y, b.y);
            var mx2 = math.max(a.x, b.x);
            var my2 = math.max(a.y, b.y);
            return new Rect(mx1, my1, mx2 - mx1, my2 - my1);
        }
    }
}

#endif