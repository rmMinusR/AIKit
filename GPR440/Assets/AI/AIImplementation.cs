using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class AIImplementation : ControlProviderContextMap
{
    [Space]
    [SerializeField] [Min(0)] private float currentHeadingWeight = 1;

    [Header("Wander")]
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;

    private void Awake()
    {
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());
    }

    public override ControlData GetControlCommand(CharacterHost context)
    {
        ControlData data = base.GetControlCommand(context);

        //data.steering += directionNoise;

        return data;
    }

    protected override void RefreshContextMapValues(CharacterHost context)
    {
        base.RefreshContextMapValues(context);
        for (int i = 0; i < contextMap.Length; ++i) _RefreshContextMapValue(ref contextMap[i], context);
    }


    [Header("Obstacle avoidance")]
    [SerializeField] [Min(0)] private float obstacleWeight = 1;
    [SerializeField] [Min(0)] private float maxProbeDistance;

    [Header("Dynamic avoidance")]
    [SerializeField] [Min(0)] private float dynamicAvoidWeight = 1;
    [SerializeField] [Min(0)] private float dynamicAvoidRange = 1;

    private void _RefreshContextMapValue(ref ContextMapEntry entry, CharacterHost context)
    {
        entry.value = 0;

        //Prefer current heading
        entry.value += (1-Ext.HalfWrap(context.Heading, entry.sourceAngle)/Mathf.PI)*currentHeadingWeight;

        //Avoidance: Whisker raycast
        {
            RaycastHit[] hits = Physics.RaycastAll(new Ray { origin = context.transform.position, direction = entry.direction }, maxProbeDistance);
            RaycastHit closestHit = new RaycastHit { distance = maxProbeDistance };
            //Select closest, filtered excluding self
            foreach(RaycastHit h in hits) if(h.distance < closestHit.distance && h.collider.gameObject != gameObject && h.collider.gameObject.GetComponent<Obstacle>() != null) closestHit = h;
            float pressure = 1-closestHit.distance/maxProbeDistance;
            entry.value -= pressure * obstacleWeight;
        }

        //Wander
        float directionNoise = wanderDirectionNoiseGenerator.GetNoise(entry.direction.x, Time.time, entry.direction.z) * wanderWeight;
        entry.value += Mathf.Abs(directionNoise);

        //Spacing
        HashSet<Obstacle> nearby = new HashSet<Obstacle>(FindObjectsOfType<Obstacle>().Where(o => o.type != Obstacle.Type.Static));
        nearby.Remove(context.GetComponent<Obstacle>());
        foreach(Obstacle o in nearby)
        {
            Vector3 diff = o.transform.position - context.transform.position;
            float ang = Mathf.Atan2(diff.z, diff.x);
            float dist = diff.magnitude;

            float pressure = 1 - Mathf.Clamp01(dist / dynamicAvoidRange);
            pressure *= 1 - Ext.AngleDiffUnsigned(context.Heading, ang)/Mathf.PI;

            entry.value -= pressure * dynamicAvoidWeight;
        }
    }
}
