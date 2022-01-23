using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class CharacterHost : MonoBehaviour
{
    [Header("State")]
    [SerializeReference] private IControlProvider controller;
    
    [Header("Movement settings")]
    [Min  (0)   ] [SerializeField] private float moveSpeed = 1f;
    [Range(0, 1)] [SerializeField] private float moveControlRatio = 0.95f;

    //Component references
    [InspectorReadOnly(editMode = AccessMode.ReadWrite, playMode = AccessMode.ReadOnly)] [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //Ensure we have valid dependencies
        Debug.Assert(controller != null);
        Debug.Assert(rb         != null);
    }


    [NonSerialized] public ControlData lastControlInput;
    public Vector3 Movement3D => new Vector3(lastControlInput.movement.x, 0, lastControlInput.movement.y);
    private void Update()
    {
        //Fetch control data
        lastControlInput = controller!=null ? controller.GetControlCommand(this) : default;

        //Update movement with control data
        _TickMovement(); //FIXME coupling?
    }
    
    private void _TickMovement()
    {
        //Pull velocity from rigidbody
        Vector3 velocity = rb.velocity;

        //Tick velocity
        Vector3 targetVelocity = Movement3D * moveSpeed;
        velocity = Vector3.Lerp(targetVelocity, velocity, Mathf.Pow(1-moveControlRatio, Time.deltaTime));

        //Apply velocity to rigidbody
        velocity.y = rb.velocity.y; //Don't affect Y axis
        rb.velocity = velocity;
    }
}