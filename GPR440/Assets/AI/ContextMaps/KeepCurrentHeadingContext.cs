using System.Collections;
using UnityEngine;

public sealed class KeepCurrentHeadingContext : IContextProvider
{
    protected override void RefreshContextMapValues()
    {
        for (int i = 0; i < entries.Length; ++i)
        {
            float angleToHeading = Ext.AngleDiffUnsigned(ContextMap.Host.Heading, entries[i].sourceAngle);
            entries[i].value += shapingFunction.Evaluate(angleToHeading);
        }
    }
}