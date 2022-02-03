using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main 8way
//https://www.youtube.com/watch?v=6BrZryMz-ac

//Other stuff
//https://www.youtube.com/watch?v=I5UWsjT4udI&list=PL4QJmtZWf50kvAZap4Xd0JhVEgo9lxdZL&index=2
//http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter18_Context_Steering_Behavior-Driven_Steering_at_the_Macro_Scale.pdf

public abstract class ControlProviderContextMap : IControlProviderAI
{
    [Serializable]
    protected struct ContextMapEntry
    {
        public float sourceAngle; //In radians
        public Vector3 direction; //Must be normalized
        public float value;
    }

    [InspectorReadOnly] [SerializeField] protected ContextMapEntry[] contextMap = new ContextMapEntry[1];

    private void Start()
    {
        Debug.Assert(contextMap.Length > 0);

        //Build context map angles
        float angleStep = Mathf.PI*2f/contextMap.Length;
        for (int i = 0; i < contextMap.Length; ++i)
        {
            contextMap[i].sourceAngle = i*angleStep;
            contextMap[i].direction = new Vector3(Mathf.Cos(contextMap[i].sourceAngle), 0, Mathf.Sin(contextMap[i].sourceAngle));
        }
    }

    protected virtual void OnDrawGizmos()
    {
        _FindHighestAndLowest(recalc: false);

        foreach (ContextMapEntry i in contextMap)
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

    public override ControlData GetControlCommand(CharacterHost context)
    {
        RefreshContextMapValues(context);

        //Find average value
        float avgValue = 0;
        for(int i = 0; i < contextMap.Length; ++i) avgValue += contextMap[i].value;
        avgValue /= contextMap.Length;

        //Find ID with highest associated value
        int bestChoiceID = 0;
        for(int i = 1; i < contextMap.Length; ++i) if(contextMap[i].value > contextMap[bestChoiceID].value) bestChoiceID = i;

        //TODO lerp angle based on gradient

        return new ControlData {
            targetSpeed = RenormalizeValue(GetSmoothedValueAt(context.Heading)),
            steering = Ext.AngleDiffSigned(context.Heading, contextMap[bestChoiceID].sourceAngle)
        };
    }

    public virtual float GetSmoothedValueAt(float angleRadians)
    {
        angleRadians = Ext.PositiveWrap(angleRadians);

        float radiansPerMapEntry = Mathf.PI*2/contextMap.Length;
        float index = angleRadians/radiansPerMapEntry;
        int lowerIndex = (int)index;
        int upperIndex = (lowerIndex + 1) % contextMap.Length;
        float lerpAmt = index%1;

        return Mathf.Lerp(contextMap[lowerIndex].value, contextMap[upperIndex].value, lerpAmt);
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

        foreach (ContextMapEntry i in contextMap)
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

    protected virtual void RefreshContextMapValues(CharacterHost context)
    {
        highestVal = null;
        lowestVal  = null;
    }
}
