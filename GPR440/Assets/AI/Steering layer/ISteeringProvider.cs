using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISteeringProvider : MonoBehaviour
{
    public abstract ControlData GetControlCommand();
}


/// <summary>
/// Messaging data structure for AI/etc to tell their body how to move
/// </summary>
[Serializable]
public struct ControlData
{
    public float steering;
    public float targetSpeed;
    //public Vector2 movement;
}