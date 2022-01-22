using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class ScoringSystem : MonoBehaviour
{
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private int collisionCount;
    public int CollisionCount => collisionCount;
    
    [Space]
    [SerializeField] private float markCooldown;
    [InspectorReadOnly] [SerializeField] private float lastTimeMarked;

    private void Start()
    {
        collisionCount = 0;
        lastTimeMarked = -markCooldown;
    }

    private void _TryMark()
    {
        if(lastTimeMarked + markCooldown < Time.time)
        {
            lastTimeMarked = Time.time + markCooldown;
            ++collisionCount;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _TryMark(); //TODO constrain with component tag?
    }
}
