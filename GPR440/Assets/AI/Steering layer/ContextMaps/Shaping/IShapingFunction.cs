using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class IShapingFunction
{
    /// <summary>
    /// Evaluate this shaping function at the given angle
    /// </summary>
    /// <param name="angleRadians">Angle to evaluate, in radians</param>
    /// <returns>Value -1 to 1</returns>
    public abstract float Evaluate(float angleRadians);
}