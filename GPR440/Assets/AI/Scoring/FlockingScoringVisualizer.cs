using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlockingScoringVisualizer : MonoBehaviour
{
    [SerializeField] private FlockingScoringSystem source;
    [SerializeField] private Text target;

    private void Update()
    {
        target.text =   (int)source.Score + "\n"
                    + source. StaticCollisionCount+"S" + "/"
                    + source.DynamicCollisionCount+"D";
    }
}
