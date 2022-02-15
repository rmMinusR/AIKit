using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class ScoringSystem : MonoBehaviour
{
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int staticCollisionCount;
    public int StaticCollisionCount => staticCollisionCount;
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int dynamicCollisionCount;
    public int DynamicCollisionCount => dynamicCollisionCount;

    [Header("Scoring")]
    public float score;
    [SerializeField] private float collisionPenalty = 10;
    [SerializeField] private AnimationCurve neighborCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float passiveGain = 0.2f;

    [Space]
    [SerializeField] private float markCooldown;
    [InspectorReadOnly] [SerializeField] private float nextTimeMarkable;

    private FlockNeighborhood neighborhood;

    private void Start()
    {
        staticCollisionCount = 0;
        dynamicCollisionCount = 0;
        nextTimeMarkable = 0;

        neighborhood = GetComponent<FlockNeighborhood>();
    }

    private void _TryMark(Obstacle.Type whichType)
    {
        if(nextTimeMarkable < Time.time)
        {
            nextTimeMarkable = Time.time + markCooldown;
            score -= collisionPenalty;
            if(whichType == Obstacle.Type.Static) ++staticCollisionCount;
            if(whichType == Obstacle.Type.Dynamic) ++dynamicCollisionCount;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Obstacle o = collision.gameObject.GetComponent<Obstacle>();
        if (o != null) _TryMark(o.type);
    }

    private void Update()
    {
        score += Time.deltaTime * passiveGain;

        if (neighborhood != null)
        {
            foreach(FlockNeighborhood.Record i in neighborhood.neighborhood)
            {
                score += Time.deltaTime * neighborCurve.Evaluate(i.distance/neighborhood.fovDistance);
            }
        }
    }

    public void ResetScoring()
    {
        score = 0;
        nextTimeMarkable = -markCooldown;
        staticCollisionCount = 0;
        dynamicCollisionCount = 0;
    }

    public class Comparer : IComparer<ScoringSystem>
    {
        public int Compare(ScoringSystem x, ScoringSystem y)
        {
            return x.score.CompareTo(y.score);
        }
    }
}
