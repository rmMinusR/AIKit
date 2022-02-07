using System.Collections;
using UnityEngine;

public class KeepCurrentHeadingContext : IContextProvider
{
    [SerializeField] [Min(0)] private float currentHeadingWeight = 1;
    [SerializeField] private AnimationCurve currentHeadingShapeFunc = AnimationCurve.Linear(0, 1, 1, 0);

    public override void RefreshContextMapValues()
    {
        for (int i = 0; i < contextMap.entries.Length; ++i)
        {
            float angleToHeading = Ext.AngleDiffUnsigned(contextMap.host.Heading, contextMap.entries[i].sourceAngle);
            contextMap.entries[i].value += currentHeadingShapeFunc.Evaluate(angleToHeading / Mathf.PI) * currentHeadingWeight;
        }
    }
}