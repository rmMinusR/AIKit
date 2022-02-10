using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlockNeighborhood))]
public sealed class FlockCohesionContext : IContextProvider
{
    private FlockNeighborhood neighborhood;

    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.Linear(1, 0, 0, 1);

    protected override void Start()
    {
        base.Start();
        neighborhood = GetComponent<FlockNeighborhood>();
        Debug.Assert(neighborhood != null);
    }

    public override void RefreshContextMapValues()
    {
        foreach (FlockNeighborhood.Record record in neighborhood.neighborhood)
        {
            for(int i = 0; i < contextMap.entries.Length; ++i)
            {
                float angleToTarget = Ext.AngleDiffUnsigned(contextMap.entries[i].sourceAngle, record.targetAngleRadians);
                float val = shapingFunction.Evaluate(angleToTarget);
                val *= falloffCurve.Evaluate(record.distance / neighborhood.fovDistance);
                contextMap.entries[i].value += val;
            }
        }
    }
}
