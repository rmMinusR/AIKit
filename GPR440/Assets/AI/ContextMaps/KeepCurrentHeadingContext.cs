using System.Collections;
using UnityEngine;

public sealed class KeepCurrentHeadingContext : IContextProvider
{
    [SerializeField] [Min(0)] private float currentHeadingWeight = 1;
    [SerializeField] private AnimationCurve currentHeadingShapeFunc = AnimationCurve.Linear(0, 1, 1, 0);

    protected override void RefreshContextMapValues()
    {
        for (int i = 0; i < entries.Length; ++i)
        {
            float angleToHeading = Ext.AngleDiffUnsigned(ContextMap.Host.Heading, entries[i].sourceAngle);
            entries[i].value += currentHeadingShapeFunc.Evaluate(angleToHeading / Mathf.PI) * currentHeadingWeight;
        }
    }
}