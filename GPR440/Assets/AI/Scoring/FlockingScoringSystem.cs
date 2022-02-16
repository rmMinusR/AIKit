using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class FlockingScoringSystem : ScoringSystem
{
    //Implicit [Header("Scoring")] from IScoringSystem
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int staticCollisionCount;
    public int StaticCollisionCount => staticCollisionCount;
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int dynamicCollisionCount;
    public int DynamicCollisionCount => dynamicCollisionCount;

    [Space]
    [SerializeField] private float collisionPenalty = 10;
    [SerializeField] private AnimationCurve neighborCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Collision counting")]
    [SerializeField] private float markCooldown;
    [InspectorReadOnly] [SerializeField] private float nextTimeMarkable;

    private FlockNeighborhood neighborhood;

    protected override void OnEnable()
    {
        base.OnEnable();

        neighborhood = GetComponent<FlockNeighborhood>();
    }

    private void _TryMark(Obstacle.Type whichType)
    {
        if(nextTimeMarkable < Time.time)
        {
            nextTimeMarkable = Time.time + markCooldown;
            Score -= collisionPenalty;
            if(whichType == Obstacle.Type.Static) ++staticCollisionCount;
            if(whichType == Obstacle.Type.Dynamic) ++dynamicCollisionCount;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Obstacle o = collision.gameObject.GetComponent<Obstacle>();
        if (o != null) _TryMark(o.type);
    }

    protected override void Update()
    {
        base.Update();

        foreach(FlockNeighborhood.Record i in neighborhood.neighborhood)
        {
            Score += Time.deltaTime * neighborCurve.Evaluate(i.distance/neighborhood.fovDistance);
        }
    }

    public override void ResetScoring()
    {
        base.ResetScoring();

        staticCollisionCount = 0;
        dynamicCollisionCount = 0;
        nextTimeMarkable = 0;
    }
}
