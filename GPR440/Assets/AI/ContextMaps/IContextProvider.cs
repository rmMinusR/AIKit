using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ContextMapSteering))]
public abstract class IContextProvider : MonoBehaviour
{
    [NonSerialized] public ContextMapSteering __contextMap;
    public ContextMapSteering ContextMap => (__contextMap!=null) ? __contextMap : (__contextMap = GetComponent<ContextMapSteering>());

    [SerializeReference] [SubclassSelector] protected IShapingFunction shapingFunction;

    public abstract void RefreshContextMapValues();
}