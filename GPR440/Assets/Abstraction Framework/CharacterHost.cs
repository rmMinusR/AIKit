using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class CharacterHost : MonoBehaviour
{
    [Header("State")]
    [SerializeReference] private IControlProvider controller;
    
    [Header("Movement settings")]
    [Min  (0)   ] [SerializeField] private float moveSpeed = 1f;
    [Range(0, 1)] [SerializeField] private float moveControlRatio = 0.95f;

    //Component references
    [InspectorReadOnly(editMode = AccessMode.ReadWrite, playMode = AccessMode.ReadOnly)] [SerializeField] private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Ensure we have valid dependencies
        Debug.Assert(controller != null);
        Debug.Assert(rb         != null);
    }


    [NonSerialized] private ControlData lastControlInput;
    private void Update()
    {
        //Fetch control data
        lastControlInput = controller!=null ? controller.GetControlCommand(this) : default;

        //Update movement with control data
        _TickMovement(lastControlInput);
    }
    
    private void _TickMovement(ControlData controlInput)
    {
        //Pull velocity from rigidbody
        Vector2 velocity = rb.velocity;

        //Tick velocity
        Vector2 targetVelocity = controlInput.movement * moveSpeed;
        velocity = Vector2.Lerp(targetVelocity, velocity, Mathf.Pow(1-moveControlRatio, Time.deltaTime));
        
        //Apply velocity to rigidbody
        rb.velocity = velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)lastControlInput.movement);
    }
}