using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ContextMapSteering))]
public abstract class IContextProvider : MonoBehaviour
{
    protected ContextMapSteering contextMap;

    protected virtual void Start()
    {
        contextMap = GetComponent<ContextMapSteering>();
    }

    public abstract void RefreshContextMapValues();
}