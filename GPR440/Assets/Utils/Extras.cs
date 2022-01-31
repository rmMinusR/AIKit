using System.Collections;
using UnityEngine;

public static class Ext
{
    public const float TWO_PI = Mathf.PI * 2;

    private static float PosMod(float x, float div) => x - Mathf.FloorToInt(x/div)*div;

    public static float PositiveWrap(float angle, float FULL_CIRCLE = TWO_PI) => PosMod(angle, FULL_CIRCLE);
    public static float HalfWrap(float angle, float FULL_CIRCLE = TWO_PI) => PosMod(angle+FULL_CIRCLE/2, FULL_CIRCLE)-FULL_CIRCLE/2;

    public static float AngleDiffSigned(float from, float to, float FULL_CIRCLE = TWO_PI)
    {
        from = PositiveWrap(from, FULL_CIRCLE: FULL_CIRCLE);
        to = PositiveWrap(to, FULL_CIRCLE: FULL_CIRCLE);

        float diff = to - from;

        return HalfWrap(diff, FULL_CIRCLE: FULL_CIRCLE);
    }

    public static float AngleDiffUnsigned(float from, float to) => Mathf.Abs(AngleDiffSigned(from, to));
}