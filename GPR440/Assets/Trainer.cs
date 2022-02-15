using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Scatterer))]
public sealed class Trainer : MonoBehaviour
{
    [SerializeField] [InspectorReadOnly] private int count = 0;
    [SerializeField] [Min(0)] private float duration = 60;
    [SerializeField] [Min(0)] private float mutationRate = 0.1f;
    [SerializeField] [Range(0, 1)] private float mutationRateDecay = 0.9f;
    [SerializeField] [Range(0, 1)] private float cullRate = 0.5f;

    private void Start()
    {
        StartCoroutine(TrainingCoroutine());
    }

    IEnumerator TrainingCoroutine()
    {
        Scatterer scatterer = GetComponent<Scatterer>();

        while(isActiveAndEnabled)
        {
            List<ScoringSystem> agents = new List<ScoringSystem>(FindObjectsOfType<ScoringSystem>());
            int targetCount = scatterer.targetCount;

            Debug.Log(targetCount+" agents active");

            //Order population
            agents.Sort(new ScoringSystem.Comparer());

            //Kill worse half
            int nToRemove = (int)( targetCount * cullRate );
            Debug.Log("Culling "+nToRemove);
            if (agents.Count > 0)
            {
                for (int i = 0; i < nToRemove; ++i) scatterer.KillAgent(agents[i].gameObject);
                agents.RemoveRange(0, nToRemove);
            }

            //Scatter existing agents
            foreach (ScoringSystem i in agents) i.transform.position = scatterer.FindRandomValidSpawnpoint();

            System.Random rand = new System.Random();

            //Use scatter to replace them
            for(int i = agents.Count; i < targetCount; ++i)
            {
                GameObject srcObj = agents.Count>0 ? agents[i % agents.Count].gameObject : scatterer.prefab;
                GameObject dstObj = scatterer.SpawnAgent();
                foreach (IContextProvider src in srcObj.GetComponents<IContextProvider>())
                {
                    IContextProvider dst = (IContextProvider) dstObj.GetComponent(src.GetType());
                    dst.weight = src.weight;
                    float mutation = (float)(rand.NextDouble() * 2 - 1) * mutationRate;
                    mutation = mutation>0 ? 1+mutation : 1/(1+Mathf.Abs(mutation));
                    dst.weight *= mutation;
                }
            }

            //Reset ALL scores
            foreach (ScoringSystem i in FindObjectsOfType<ScoringSystem>()) i.ResetScoring();
            Debug.Log("Cycle #"+count+" complete");

            mutationRate *= mutationRateDecay;

            count++;

            yield return new WaitForSeconds(duration);
        }

        yield break;
    }

    [Serializable]
    public struct ScoreRecord
    {
        public ScoringSystem obj;
        public float score;
    }

    [Space(20)]
    [TestButton("Find best", "_FindAndDisplayBest")]
    [SerializeField] private List<ScoreRecord> best = new List<ScoreRecord>();
    void _FindAndDisplayBest()
    {
        List<ScoringSystem> agents = new List<ScoringSystem>(FindObjectsOfType<ScoringSystem>());
        agents.Sort(new ScoringSystem.Comparer());
        agents.Reverse();

        best.Clear();
        best.AddRange(agents.Select(x => new ScoreRecord { obj = x, score = x.score }));
    }
}
