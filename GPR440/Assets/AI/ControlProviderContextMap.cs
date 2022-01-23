using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main 8way
//https://www.youtube.com/watch?v=6BrZryMz-ac

//Other stuff
//https://www.youtube.com/watch?v=I5UWsjT4udI&list=PL4QJmtZWf50kvAZap4Xd0JhVEgo9lxdZL&index=2
//http://www.gameaipro.com/GameAIPro2/GameAIPro2_Chapter18_Context_Steering_Behavior-Driven_Steering_at_the_Macro_Scale.pdf

public class ControlProviderContextMap : IControlProviderAI
{
    [Serializable]
    protected struct ContextMapEntry
    {
        public Vector2 direction; //Should be normalized
        public float value;
    }

    [InspectorReadOnly] [SerializeField] protected ContextMapEntry[] contextMap = new ContextMapEntry[1];

    private void Start()
    {
        Debug.Assert(contextMap.Length > 0);

        //Build context map angles
        float angleStep = 360f/contextMap.Length;
        for (int i = 0; i < contextMap.Length; ++i)
        {
            float angle = i*angleStep;
            contextMap[i].direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    private void OnDrawGizmos()
    {
        foreach(ContextMapEntry i in contextMap)
        {
            Vector3 rootPos = transform.position + (Vector3)i.direction;
            Gizmos.color = i.value > 0 ? Color.green : Color.red;
            Gizmos.DrawLine(rootPos, rootPos + (Vector3)i.direction * Mathf.Abs(i.value));
        }
    }

    public override ControlData GetControlCommand(CharacterHost context)
    {
        //throw new System.NotImplementedException();
        return new ControlData { movement = Vector2.right * ((Time.time%3 > 1.5)?1:-1) };
    }
}
