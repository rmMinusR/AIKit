using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class ScoringSystem : MonoBehaviour
{
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int collisionCount;
    public int CollisionCount => collisionCount;
    
    [Space]
    [SerializeField] private float markCooldown;
    [InspectorReadOnly] [SerializeField] private float nextTimeMarkable;

    private void Start()
    {
        collisionCount = 0;
        nextTimeMarkable = 0;
    }

    private void _TryMark()
    {
        if(nextTimeMarkable < Time.time)
        {
            nextTimeMarkable = Time.time + markCooldown;
            ++collisionCount;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<Obstacle>() != null) _TryMark();
    }
}
