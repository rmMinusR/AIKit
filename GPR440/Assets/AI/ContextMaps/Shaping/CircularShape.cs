using System;
using UnityEngine;

[Serializable]
public class CircularShape : IShapingFunction
{
    [SerializeField] [Range(-1, 1)] private float bias = 0;
    [SerializeField] [Range(-1, 1)] private float min = -1;
    [SerializeField] [Range(-1, 1)] private float max = 1;

    public override float Evaluate(float angleRadians)
    {
        float val = Mathf.Cos(angleRadians) + bias;
        return Mathf.Clamp(val, min, max);
    }
}
