using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlProviderContextMap : IControlProviderAI
{
    protected struct ContextMapEntry
    {
        public Vector2 direction; //Should be normalized
        public float value;
    }

    [SerializeField] protected ContextMapEntry[] contextMap = new ContextMapEntry[1];

    private void Start()
    {
        Debug.Assert(contextMap.Length > 0);

        //Build context map angles
        for(int i = 0; i < contextMap.Length; ++i)
        {
            float angle = ((float)i) * 360/contextMap.Length;
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
