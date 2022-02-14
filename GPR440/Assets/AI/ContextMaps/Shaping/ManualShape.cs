using System;
using UnityEngine;

[Serializable]
public sealed class ManualShape : IShapingFunction
{
    [SerializeField] [Tooltip("Uses DEGREES as units")] private AnimationCurve shape = AnimationCurve.Linear(0, 0, 180, 1);

    public override float Evaluate(float angleRadians) => Mathf.Clamp(shape.Evaluate(angleRadians*Mathf.Rad2Deg), -1, 1);
}
