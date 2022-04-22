using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterHost : MonoBehaviour
{
    public ActionPlanner actionPlanner;
    [SerializeField] [Min(0)] private float actionPlannerFrequency = 1f;
    public SteeringHost steering;
    [SerializeField] [Min(0)] private float steeringFrequency = 0.05f;

    private void Start()
    {
        if (actionPlanner != null) StartCoroutine(Worker(actionPlanner, actionPlannerFrequency));
        if (steering      != null) StartCoroutine(Worker(steering     , steeringFrequency     ));
    }

    private IEnumerator Worker(IAILayer layer, float frequency)
    {
        yield return new WaitForSeconds(Random.value * frequency);
        
        while (true)
        {
            layer.PeriodicUpdate();
            yield return new WaitForSeconds(frequency);
        }
    }
}
