using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ContextMapSteering))]
public sealed class AIImplementation : MonoBehaviour
{
    [Space]
    [SerializeField] [Min(0)] private float currentHeadingWeight = 1;
    [SerializeField] private AnimationCurve currentHeadingShapeFunc = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Wander")]
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;

    private ContextMapSteering contextMap;

    private void Awake()
    {
        contextMap = GetComponent<ContextMapSteering>();
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());
    }

    private void Update() => RefreshContextMapValues();

    private void RefreshContextMapValues()
    {
        for (int i = 0; i < contextMap.entries.Length; ++i) _RefreshContextMapValue(ref contextMap.entries[i], contextMap.host);
    }

    [Header("Obstacle avoidance")]
    [SerializeField] [Min(0)] private float obstacleWeight = 1;
    [SerializeField] [Min(0)] private float maxProbeDistance;

    [Header("Dynamic avoidance")]
    [SerializeField] [Min(0)] private float dynamicAvoidWeight = 1;
    [SerializeField] [Min(0)] private float dynamicAvoidRange = 1;
    [SerializeField] private AnimationCurve dynamicAvoidShapeFunc = AnimationCurve.Linear(0, 1, 1, 0);

    private void _RefreshContextMapValue(ref ContextMapSteering.Entry entry, CharacterHost context)
    {
        entry.value = 0;

        //Prefer current heading
        float angleToHeading = Ext.AngleDiffUnsigned(context.Heading, entry.sourceAngle);
        entry.value += currentHeadingShapeFunc.Evaluate(angleToHeading/Mathf.PI)*currentHeadingWeight;

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
        HashSet<Obstacle> nearby = new HashSet<Obstacle>(Obstacle.OBSTACLES.Where(o => o.type != Obstacle.Type.Static && o.gameObject != this.gameObject));
        foreach(Obstacle o in nearby)
        {
            Vector3 diff = o.transform.position - context.transform.position;
            float ang = Mathf.Atan2(diff.z, diff.x);
            float dist = diff.magnitude;

            float pressure = 1 - Mathf.Clamp01(dist / dynamicAvoidRange);
            pressure *= Mathf.Clamp01(dynamicAvoidShapeFunc.Evaluate(Mathf.Clamp01(Ext.AngleDiffUnsigned(entry.sourceAngle, ang)/Mathf.PI)));

            entry.value -= pressure * dynamicAvoidWeight;
        }
    }
}
