using System.Collections;
using UnityEngine;

public class RandomWalkContext : IContextProvider
{
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;

    protected override void Start()
    {
        base.Start();
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());
    }

    public override void RefreshContextMapValues()
    {
        for (int i = 0; i < contextMap.entries.Length; ++i)
        {
            float directionNoise = wanderDirectionNoiseGenerator.GetNoise(contextMap.entries[i].direction.x, Time.time, contextMap.entries[i].direction.z) * wanderWeight;
            contextMap.entries[i].value += Mathf.Abs(directionNoise);
        }
    }
}