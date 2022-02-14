using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringVisualizer : MonoBehaviour
{
    [SerializeField] private ScoringSystem source;
    [SerializeField] private Text target;

    private void Update()
    {
        target.text =   (int)source.score + "\n"
                    + source. StaticCollisionCount+"S" + "/"
                    + source.DynamicCollisionCount+"D";
    }
}
