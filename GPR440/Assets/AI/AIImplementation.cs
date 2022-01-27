using System.Collections;
using UnityEngine;

public sealed class AIImplementation : ControlProviderContextMap
{
    [Header("Wander")]
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] [Min(0)] private float wanderDirectionNoiseAmplitude = 1;
    [SerializeField] [Min(0)] private float wanderSpeedNoiseAmplitude = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator = new FastNoiseLite(0x46851); //Arbitrary seed
    [SerializeField] private FastNoiseLite wanderSpeedNoiseGenerator     = new FastNoiseLite(0x36891); //Arbitrary seed
    [SerializeField] private float wanderDirection;
    [SerializeField] private float wanderSpeed;

    protected override void _RefreshContextMapValues()
    {
        //Update wander direction
        wanderDirection += wanderDirectionNoiseGenerator.GetNoise(0, Time.time) * wanderDirectionNoiseAmplitude * Time.deltaTime;
        wanderSpeed     += wanderSpeedNoiseGenerator    .GetNoise(0, Time.time) * wanderSpeedNoiseAmplitude     * Time.deltaTime;
        
        wanderDirection %= Mathf.PI*2;
        wanderSpeed = Mathf.Clamp(wanderSpeed, 0.3f, 1);

        for (int i = 0; i < contextMap.Length; ++i) _RefreshContextMapValue(ref contextMap[i]);
    }


    [Header("Obstacle avoidance")]
    [SerializeField] [Min(0)] private float obstacleWeight = 1;
    [SerializeField] [Min(0)] private float maxProbeDistance;

    private void _RefreshContextMapValue(ref ContextMapEntry entry)
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
        float angleDiff = Mathf.Abs(wanderDirection-entry.sourceAngle)*Mathf.Rad2Deg;
        if(angleDiff > 180) angleDiff = 360-angleDiff;
        Debug.Log(angleDiff);
        entry.value += wanderWeight * wanderSpeed * (1-angleDiff/180);
    }
}
