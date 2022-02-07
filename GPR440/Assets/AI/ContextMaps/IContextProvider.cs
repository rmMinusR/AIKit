using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ContextMapSteering))]
public abstract class IContextProvider : MonoBehaviour
{
    [NonSerialized] public ContextMapSteering contextMap;

    [SerializeReference] [SubclassSelector] protected IShapingFunction shapingFunction;

    protected virtual void Start()
    {
        contextMap = GetComponent<ContextMapSteering>();
    }

    public abstract void RefreshContextMapValues();
}