using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ContextMapSteering))]
public abstract class IContextProvider : MonoBehaviour
{
    private ContextMapSteering __contextMap;
    protected ContextMapSteering ContextMap => (__contextMap!=null) ? __contextMap : (__contextMap = GetComponent<ContextMapSteering>());

    private ContextMapSteering.Entry[] __entries = null;
    public ContextMapSteering.Entry[] entries
    {
        get
        {
            if (__entries != null && __entries.Length == ContextMap.entries.Length) return __entries;

            __entries = new ContextMapSteering.Entry[ContextMap.entries.Length];
            for(int i = 0; i < __entries.Length; ++i)
            {
                __entries[i].direction   = ContextMap.entries[i].direction;
                __entries[i].sourceAngle = ContextMap.entries[i].sourceAngle;
            }

            return __entries;
        }
    }

    [SerializeReference] [SubclassSelector] protected IShapingFunction shapingFunction;

    protected abstract void RefreshContextMapValues();

    public void RefreshAndCopyContextMapValues()
    {
        RefreshContextMapValues();
        for(int i = 0; i < entries.Length; ++i)
        {
            ContextMap.entries[i].value += entries[i].value;
        }
    }
}