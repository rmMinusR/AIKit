using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Empty pseudotag class for objects that should be avoided, and should mark score down when collided with
/// </summary>
[RequireComponent(typeof(Collider))]
public class Obstacle : MonoBehaviour
{
    public enum Type
    {
        Static,
        Kinematic,
        Dynamic
    }

    public Type type;

    [InspectorReadOnly] public Collider mainCollider;

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
    }

    public static List<Obstacle> OBSTACLES = new List<Obstacle>();
    private void OnEnable() => OBSTACLES.Add(this);
    private void OnDisable() => OBSTACLES.Remove(this);
}