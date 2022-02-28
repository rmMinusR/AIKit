using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This file is for random helper functions
/// that didn't make sense to put elsewhere.
/// </summary>

public static class DataStructExt
{
    /// <summary>
    /// Get a value if its key exists; create and return a new one if it doesn't.
    /// </summary>
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
    {
        if (!dict.TryGetValue(key, out TValue val))
        {
            val = new TValue();
            dict.Add(key, val);
        }
        return val;
    }
}

public static class InputExt
{
    /// <summary>
    /// "Button" controls are technically floats. Weird.
    /// This function should help us keep the values somewhat standard.
    /// </summary>
    public static bool ReadAsButton(this InputAction control, float threshold = 0.9f) => control.ReadValue<float>() > threshold;
}

public static class CollectionExt
{
    /// <summary>
    /// Where is the given value?
    /// </summary>
    public static int IndexOf<T>(this IEnumerable<T> coll, T searchFor) where T : class
    {
        int ind = 0;
        foreach(T i in coll)
        {
            if(searchFor == i) return ind;
            else ++ind;
        }
        return -1;
    }

    /// <summary>
    /// Find the best-fit based on the given function. Higher values win.
    /// </summary>
    public static TData MaxBy<TData>(this IEnumerable<TData> coll, Func<TData, float> evaluator)
    {
        TData bestData = default;
        float bestVal = float.MinValue;
        bool hasVal = false;

        foreach(TData i in coll)
        {
            float iVal = evaluator(i);
            if (!hasVal || iVal > bestVal)
            {
                bestData = i;
                bestVal = iVal;
                hasVal = true;
            }
        }

        if (!hasVal) throw new InvalidOperationException("No values!");

        return bestData;
    }
}

/// <summary>
/// Helper class for all doing math with angles. If
/// unspecified, angles default to radians, although
/// this often can be changed.
/// </summary>
public static class AngleMath
{
    public const float TWO_PI = Mathf.PI * 2;

    /// <summary>
    /// Works like the % operator, but only returns positive values.
    /// </summary>
    private static float PosMod(float x, float div) => x - Mathf.FloorToInt(x/div)*div;

    /// <summary>
    /// Ensure the given angle is between 0 and MAX_RANGE (default 2pi)
    /// </summary>
    public static float PositiveWrap(float angle, float MAX_RANGE = TWO_PI) => PosMod(angle, MAX_RANGE);

    /// <summary>
    /// Ensure the given angle is a positive or negative half-circle. (default pi)
    /// </summary>
    public static float HalfWrap(float angle, float MAX_RANGE = TWO_PI) => PosMod(angle+MAX_RANGE/2, MAX_RANGE)-MAX_RANGE/2;

    /// <summary>
    /// Find the signed difference between two angles.
    /// </summary>
    public static float AngleDiffSigned(float from, float to, float FULL_CIRCLE = TWO_PI)
    {
        from = PositiveWrap(from, MAX_RANGE: FULL_CIRCLE);
        to = PositiveWrap(to, MAX_RANGE: FULL_CIRCLE);

        float diff = to - from;

        return HalfWrap(diff, MAX_RANGE: FULL_CIRCLE);
    }

    /// <summary>
    /// Find the unsigned difference between two angles.
    /// </summary>
    public static float AngleDiffUnsigned(float from, float to) => Mathf.Abs(AngleDiffSigned(from, to));

    /// <summary>
    /// Why doesn't Mathf have this?
    /// </summary>
    public static float Secant(float angleRadians) => 1 / Mathf.Cos(angleRadians);
}