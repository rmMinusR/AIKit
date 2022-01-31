using System.Collections;
using UnityEngine;

public sealed class AIImplementation : ControlProviderContextMap
{
    [Header("Wander")]
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] [Min(0)] private float wanderDirectionNoiseAmplitude = 1;
    [SerializeField] [Min(0)] private float wanderSpeedNoiseAmplitude = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;
    [SerializeField] private FastNoiseLite wanderSpeedNoiseGenerator;

    private void Awake()
    {
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());//Random.Range(0, 1<<12));
        wanderSpeedNoiseGenerator    .SetSeed(GetInstanceID()^0x2894);//Random.Range(0, 1<<12));
    }

    protected override void _RefreshContextMapValues(CharacterHost context)
    {
        for (int i = 0; i < contextMap.Length; ++i) _RefreshContextMapValue(ref contextMap[i], context);
    }


    [Header("Obstacle avoidance")]
    [SerializeField] [Min(0)] private float obstacleWeight = 1;
    [SerializeField] [Min(0)] private float maxProbeDistance;

    private void _RefreshContextMapValue(ref ContextMapEntry entry, CharacterHost context)
    {
        entry.value = 0;
        
        //Avoidance: Whisker raycast
        RaycastHit[] hits = Physics.RaycastAll(new Ray { origin = transform.position, direction = entry.direction }, maxProbeDistance);
        RaycastHit closestHit = new RaycastHit { distance = maxProbeDistance };
        //Select closest, filtered excluding self
        foreach(RaycastHit h in hits) if(h.distance < closestHit.distance && h.collider.gameObject != gameObject && h.collider.gameObject.GetComponent<Obstacle>() != null) closestHit = h;
        float pressure = 1-closestHit.distance/maxProbeDistance;
        entry.value += -pressure * obstacleWeight;

        //Wander
        float directionNoise = wanderDirectionNoiseGenerator.GetNoise(0, Time.time) * wanderDirectionNoiseAmplitude * Time.deltaTime;
        float f = context.SteerTowards(entry.sourceAngle+directionNoise);
        entry.value += wanderWeight * (1-f/Mathf.PI);
    }
}
