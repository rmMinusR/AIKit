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
    public float weight = 1;
    [SerializeField] private bool clampValues = true;

    protected virtual string GetDisplayName() => GetType().Name;

    protected abstract void RefreshContextMapValues();

    private void RefreshContextMapValuesSafe()
    {
        //Reset internal buffer
        for (int i = 0; i < entries.Length; ++i) entries[i].value = 0;

        //Poll for new values
        RefreshContextMapValues();

        //Clamp internal buffer
        if (clampValues) for (int i = 0; i < entries.Length; ++i) entries[i].value = Mathf.Clamp(entries[i].value, -1, 1);
    }

    public void RefreshAndCopyContextMapValues()
    {
        RefreshContextMapValuesSafe();

        //Copy to steering
        for (int i = 0; i < entries.Length; ++i) ContextMap.entries[i].value += entries[i].value * weight;
    }


    //Literally only here so we can enable and disable
    private void Update() { }
}