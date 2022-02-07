using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlockNeighborhood))]
public sealed class FlockAlignContext : IContextProvider
{
    private FlockNeighborhood neighborhood;

    protected override void Start()
    {
        base.Start();
        neighborhood = GetComponent<FlockNeighborhood>();
        Debug.Assert(neighborhood != null);
    }
    
    public override void RefreshContextMapValues()
    {
        throw new System.NotImplementedException();
    }
}
