using System.Collections;
using UnityEngine;

public class RandomWalkContext : IContextProvider
{
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;

    void Start()
    {
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());
    }

    public override void RefreshContextMapValues()
    {
        for (int i = 0; i < ContextMap.entries.Length; ++i)
        {
            float directionNoise = wanderDirectionNoiseGenerator.GetNoise(ContextMap.entries[i].direction.x, Time.time, ContextMap.entries[i].direction.z) * wanderWeight;
            ContextMap.entries[i].value += Mathf.Abs(directionNoise);
        }
    }
}