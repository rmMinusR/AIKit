using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class CharacterHost : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] [Min  (0)   ] private float moveSpeed = 1f;
    [SerializeField] [Range(0, 1)] private float moveGripRatio = 0.95f;
    [SerializeField] [Range(0, 1)] private float moveControlAccelRate = 0.5f;
    [SerializeField] [Min  (0)   ] private float moveControlSteerRate = 0.5f;

    [Header("State")]
    [SerializeReference] private ISteeringProvider controller;
    [SerializeField] private float heading;
    public float Heading => heading;
    [SerializeField] [Range(0, 1)] private float speed;
    public float Speed => speed;

    //Component references
    [InspectorReadOnly(editMode = AccessMode.ReadWrite, playMode = AccessMode.ReadOnly)] [SerializeField] private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        heading = UnityEngine.Random.Range(0, Mathf.PI * 2);

        //Ensure we have valid dependencies
        Debug.Assert(controller != null);
        Debug.Assert(rb         != null);
    }

    [NonSerialized] public ControlData lastControlInput;
    public Vector3 Movement3D => new Vector3(Mathf.Cos(heading), 0, Mathf.Sin(heading))*speed;

    private void Update()
    {
        //Fetch control data
        lastControlInput = controller!=null ? controller.GetControlCommand() : default;

        //Update movement with control data
        _TickMovement(); //FIXME coupling?
    }
    
    private void _TickMovement()
    {
        //Update heading and sanity check
        heading += lastControlInput.steering * moveControlSteerRate * Time.deltaTime;
        heading -= Mathf.FloorToInt(heading / Mathf.PI/2) * Mathf.PI*2;

        //Update speed and sanity check
        speed = Mathf.Lerp(lastControlInput.targetSpeed, speed, Mathf.Pow(1-moveControlAccelRate, Time.deltaTime));
        speed = Mathf.Clamp01(speed);

        //Pull velocity from rigidbody
        Vector3 velocity = rb.velocity;

        //Tick velocity
        Vector3 targetVelocity = Movement3D * moveSpeed;
        velocity = Vector3.Lerp(targetVelocity, velocity, Mathf.Pow(1-moveGripRatio, Time.deltaTime));

        //Apply velocity to rigidbody
        velocity.y = rb.velocity.y; //Don't affect Y axis
        rb.velocity = velocity;
    }
}