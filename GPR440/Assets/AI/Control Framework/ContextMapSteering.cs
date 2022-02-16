using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main 8way
//https://www.youtube.com/watch?v=6BrZryMz-ac

//Other stuff
//https://www.youtube.com/watch?v=I5UWsjT4udI&list=PL4QJmtZWf50kvAZap4Xd0JhVEgo9lxdZL&index=2
//http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter18_Context_Steering_Behavior-Driven_Steering_at_the_Macro_Scale.pdf

public sealed class ContextMapSteering : ISteeringProviderAI
{
    [Serializable]
    public struct Entry
    {
        [InspectorReadOnly] public float sourceAngle; //In radians
        [InspectorReadOnly] public Vector3 direction; //Must be normalized
        public float value;
    }

    private void BuildEntries()
    {
        if (__entries.Length == lastKnownEntryCount) return;

        Debug.Assert(__entries.Length > 0);

        //Build context map angles
        float angleStep = Mathf.PI * 2f / __entries.Length;
        for (int i = 0; i < __entries.Length; ++i)
        {
            __entries[i].sourceAngle = i * angleStep;
            __entries[i].direction = new Vector3(Mathf.Cos(__entries[i].sourceAngle), 0, Mathf.Sin(__entries[i].sourceAngle));
        }

        lastKnownEntryCount = __entries.Length;
    }

    [HideInInspector] [SerializeField] private int lastKnownEntryCount = -1;
    [SerializeField] private Entry[] __entries = new Entry[1];
    public Entry[] entries
    {
        get
        {
            BuildEntries();
            return __entries;
        }
    }

    private CharacterHost __host;
    public CharacterHost Host => __host!=null ? __host : (__host=GetComponent<CharacterHost>());

    private void Start()
    {
        BuildEntries();
    }

#if UNITY_EDITOR
    [SerializeField] private bool enableGizmos = false;
#endif

    private void OnDrawGizmos()
    {
        if (!enableGizmos) return;

        _FindHighestAndLowest(recalc: false);

        foreach (Entry i in __entries)
        {
            Vector3 rootPos = transform.position + i.direction;
            Vector3 endPos = rootPos + i.direction * Mathf.Abs(i.value);

            float renormalizedVal = RenormalizeValue(i.value);

            Gizmos.color = renormalizedVal < 0 ? Color.Lerp(Color.yellow, Color.red, -renormalizedVal) : Color.Lerp(Color.yellow, Color.green, renormalizedVal);
            Gizmos.DrawLine(rootPos, endPos);
            Gizmos.DrawSphere(endPos, 0.2f);
            //Gizmos.DrawSphere(rootPos, 0.2f);

#if UNITY_EDITOR
            //UnityEditor.Handles.Label(transform.position+i.direction*0.75f, i.value.ToString());
#endif
        }
    }

    public override ControlData GetControlCommand()
    {
        //Update values
        RefreshContextMapValues();

        //Find average value
        float avgValue = 0;
        for(int i = 0; i < __entries.Length; ++i) avgValue += __entries[i].value;
        avgValue /= __entries.Length;

        //Find ID with highest associated value
        int bestChoiceID = 0;
        for(int i = 1; i < __entries.Length; ++i) if(__entries[i].value > __entries[bestChoiceID].value) bestChoiceID = i;

        //TODO lerp angle based on gradient

        return new ControlData {
            targetSpeed = RenormalizeValue(GetSmoothedValueAt(Host.Heading)),
            steering = Ext.AngleDiffSigned(Host.Heading, __entries[bestChoiceID].sourceAngle)
        };
    }

    public float GetSmoothedValueAt(float angleRadians)
    {
        angleRadians = Ext.PositiveWrap(angleRadians);

        float radiansPerMapEntry = Mathf.PI*2/__entries.Length;
        float index = angleRadians/radiansPerMapEntry;
        int lowerIndex = (int)index;
        int upperIndex = (lowerIndex + 1) % __entries.Length;
        float lerpAmt = index%1;

        return Mathf.Lerp(__entries[lowerIndex].value, __entries[upperIndex].value, lerpAmt);
    }

    private float? highestVal = null;
    private float? lowestVal  = null;
    private void _FindHighestAndLowest(bool recalc = false)
    {
        if(recalc)
        {
            highestVal = null;
            lowestVal  = null;
        }

        foreach (Entry i in __entries)
        {
            if (!highestVal.HasValue || i.value > highestVal.Value) highestVal = i.value;
            if (!lowestVal .HasValue || i.value < lowestVal .Value) lowestVal  = i.value;
        }
    }

    public float RenormalizeValue(float val)
    {
        _FindHighestAndLowest(recalc: false);

        float renormalizedVal = val < 0 ? val/-lowestVal.Value : val/highestVal.Value;
        if (float.IsNaN(renormalizedVal)) renormalizedVal = 0;

        return renormalizedVal;
    }

    public void RefreshContextMapValues()
    {
        //Reset values
        for (int i = 0; i < __entries.Length; ++i) __entries[i].value = 0;
        highestVal = null;
        lowestVal  = null;

        //Poll values
        foreach (IContextProvider i in GetComponents<IContextProvider>()) if(i.isActiveAndEnabled) i.RefreshAndCopyContextMapValues();

        //Recalc bounds
        _FindHighestAndLowest(recalc: true);
    }
}
