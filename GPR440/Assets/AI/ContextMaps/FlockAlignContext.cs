using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlockNeighborhood))]
public sealed class FlockAlignContext : IContextProvider
{
    private FlockNeighborhood __neighborhood;
    private FlockNeighborhood Neighborhood => (__neighborhood != null) ? __neighborhood : (__neighborhood = GetComponent<FlockNeighborhood>());


    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.Linear(1, 0, 0, 1);
    
    public override void RefreshContextMapValues()
    {
        foreach (FlockNeighborhood.Record record in Neighborhood.neighborhood)
        {
            for(int i = 0; i < ContextMap.entries.Length; ++i)
            {
                float angleDiff = Ext.AngleDiffUnsigned(ContextMap.entries[i].sourceAngle, record.neighbor.Heading);
                float val = shapingFunction.Evaluate(angleDiff);
                val *= falloffCurve.Evaluate(record.distance / Neighborhood.fovDistance);
                ContextMap.entries[i].value += val;
            }
        }
    }
}
