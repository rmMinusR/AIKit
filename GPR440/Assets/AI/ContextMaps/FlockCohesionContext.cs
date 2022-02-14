using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlockNeighborhood))]
public sealed class FlockCohesionContext : IContextProvider
{
    private FlockNeighborhood __neighborhood;
    private FlockNeighborhood Neighborhood => (__neighborhood!=null) ? __neighborhood : (__neighborhood=GetComponent<FlockNeighborhood>());

    [SerializeField] private bool normalizeValues;

    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.Linear(1, 0, 0, 1);

    protected override void RefreshContextMapValues()
    {
        foreach (FlockNeighborhood.Record record in Neighborhood.neighborhood)
        {
            for(int i = 0; i < entries.Length; ++i)
            {
                float angleToTarget = Ext.AngleDiffUnsigned(entries[i].sourceAngle, record.targetAngleRadians);
                float val = shapingFunction.Evaluate(angleToTarget);
                val *= falloffCurve.Evaluate(record.distance / Neighborhood.fovDistance);
                entries[i].value += val;
            }
        }

        if (normalizeValues)
        {
            float normalizer = Neighborhood.neighborhood.Count;
            normalizer = (normalizer!=0) ? 1/normalizer : 1;
            for (int i = 0; i < entries.Length; ++i) entries[i].value *= normalizer;
        }
    }
}
