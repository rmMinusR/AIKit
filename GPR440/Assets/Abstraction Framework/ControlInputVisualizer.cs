using System.Collections;
using UnityEngine;

public sealed class ControlInputVisualizer : MonoBehaviour
{
    [Header("Component dependencies")]
    [SerializeField] private CharacterHost host;
    [SerializeField] private LineRenderer lastInputVisualizer;

    [Header("Scaling")]
    [SerializeField] [Min(0)] private float baseOffset = 1;
    [SerializeField] [Min(0)] private float movementScale = 1;

    void Start()
    {
        Debug.Assert(host                != null);
        Debug.Assert(lastInputVisualizer != null);
    }

    void Update()
    {
        Vector3 movement = host.Movement3D;
        Vector3 @base = movement.normalized * baseOffset;

        lastInputVisualizer.SetPosition(0, @base);
        lastInputVisualizer.SetPosition(1, @base + movement*movementScale);
    }
}
