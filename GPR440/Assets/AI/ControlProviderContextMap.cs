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

    private void OnDrawGizmos()
    {
        foreach(ContextMapEntry i in contextMap)
        {
            Vector3 rootPos = transform.position + i.direction;
            Gizmos.color = i.value < 0 ? Color.Lerp(Color.yellow, Color.red, -i.value) : Color.Lerp(Color.yellow, Color.green, i.value);
            Gizmos.DrawSphere(rootPos, 0.2f);
            Gizmos.DrawLine(rootPos, rootPos + i.direction * Mathf.Abs(i.value));
        }
    }

    public override ControlData GetControlCommand(CharacterHost context)
    {
        _RefreshContextMapValues(context);

        //Find average value
        float avgValue = 0;
        for(int i = 0; i < contextMap.Length; ++i) avgValue += contextMap[i].value;
        avgValue /= contextMap.Length;

        //Find ID with highest associated value
        int bestChoiceID = 0;
        for(int i = 1; i < contextMap.Length; ++i) if(contextMap[i].value > contextMap[bestChoiceID].value) bestChoiceID = i;

        //TODO lerp angle based on gradient

        //FIXME later
        float speed = contextMap[bestChoiceID].value - avgValue;
        speed *= 2; //TODO use standard deviation instead
        speed = Mathf.Clamp01(speed);

        return new ControlData {
            targetSpeed = speed,
            steering = context.SteerTowards(contextMap[bestChoiceID].sourceAngle)
        };
    }

    protected abstract void _RefreshContextMapValues(CharacterHost context);
}
