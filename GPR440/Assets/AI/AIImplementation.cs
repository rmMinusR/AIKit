using System.Collections;
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

    private void _RefreshContextMapValue(ref ContextMapEntry entry, CharacterHost context)
    {
        entry.value = 0;

        //Prefer current heading
        entry.value += (1-Mathf.Abs(context.SteerTowards(entry.sourceAngle)/Mathf.PI))*currentHeadingWeight;
        
        //Avoidance: Whisker raycast
        RaycastHit[] hits = Physics.RaycastAll(new Ray { origin = transform.position, direction = entry.direction }, maxProbeDistance);
        RaycastHit closestHit = new RaycastHit { distance = maxProbeDistance };
        //Select closest, filtered excluding self
        foreach(RaycastHit h in hits) if(h.distance < closestHit.distance && h.collider.gameObject != gameObject && h.collider.gameObject.GetComponent<Obstacle>() != null) closestHit = h;
        float pressure = 1-closestHit.distance/maxProbeDistance;
        entry.value += -pressure * obstacleWeight;

        //Wander
        float directionNoise = wanderDirectionNoiseGenerator.GetNoise(entry.direction.x, Time.time, entry.direction.z) * wanderWeight;
        entry.value += Mathf.Abs(directionNoise);
    }
}
