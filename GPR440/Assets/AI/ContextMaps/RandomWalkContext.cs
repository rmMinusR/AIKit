using System.Collections;
using UnityEngine;

public sealed class RandomWalkContext : IContextProvider
{
    [SerializeField] [Min(0)] private float wanderWeight = 1;
    [SerializeField] private FastNoiseLite wanderDirectionNoiseGenerator;

    void Start()
    {
        wanderDirectionNoiseGenerator.SetSeed(GetInstanceID());
    }

    protected override void RefreshContextMapValues()
    {
        for (int i = 0; i < entries.Length; ++i)
        {
            float directionNoise = wanderDirectionNoiseGenerator.GetNoise(entries[i].direction.x, Time.time, entries[i].direction.z) * wanderWeight;
            entries[i].value += Mathf.Abs(directionNoise);
        }
    }
}