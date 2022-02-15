using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Scatterer))]
public sealed class Trainer : MonoBehaviour
{
    [SerializeField] [InspectorReadOnly] private int count = 0;
    [SerializeField] [Min(0)] private float duration = 60;
    [SerializeField] [Min(0)] private float variation = 0.1f;
    [SerializeField] [Min(0)] private float variationDecay = 0.9f;

    private void Start()
    {
        StartCoroutine(TrainingCoroutine());
    }

    IEnumerator TrainingCoroutine()
    {
        while(isActiveAndEnabled)
        {
            List<ScoringSystem> agents = new List<ScoringSystem>(FindObjectsOfType<ScoringSystem>());
            int targetCount = agents.Count;

            Debug.Log(targetCount+" agents active");

            //Order population
            agents.Sort(new ScoringSystem.Comparer());

            //Kill worse half
            int nToRemove = targetCount / 2;
            Debug.Log("Culling "+nToRemove);
            for (int i = 0; i < nToRemove; ++i) Destroy(agents[i].gameObject);
            agents.RemoveRange(0, nToRemove);

            //Use scatter to replace them
            for(int i = agents.Count; i < targetCount; ++i)
            {
                GameObject obj = GetComponent<Scatterer>().Drop();
                foreach (IContextProvider c in agents[i % agents.Count].GetComponents<IContextProvider>())
                {
                    (obj.GetComponent(c.GetType()) as IContextProvider).weight = c.weight * (1 + Random.Range(-1, 1) * variation);
                }
            }

            //Reset ALL scores
            foreach (ScoringSystem i in FindObjectsOfType<ScoringSystem>()) i.score = 0;
            Debug.Log("Cycle complete");

            variation *= variationDecay;

            count++;

            yield return new WaitForSeconds(duration);
        }

        yield break;
    }
}
