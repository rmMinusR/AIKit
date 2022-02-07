using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FlockNeighborhood : MonoBehaviour
{
    public struct Record
    {
        public CharacterHost neighbor;
        public float distanceSq;
        public float targetAngleRadians;
        public float headingInVisionRadians;
    }

    [NonSerialized] public List<Record> neighborhood = new List<Record>();

    [SerializeField] [Range(0, 180)] [Tooltip("In degrees")] private float fovAngle = 45;
    [SerializeField] [Min(0)] private float fovDistance = 4;

    private CharacterHost host;
    private void Start()
    {
        host = GetComponent<CharacterHost>();
        Debug.Assert(host != null);
    }

    private void Update() => UpdateNeighborhood();

    private void UpdateNeighborhood()
    {
        neighborhood.Clear();

        float sqNeighborhoodDist = fovDistance * fovDistance;

        foreach (CharacterHost i in FindObjectsOfType<CharacterHost>())
        {
            Record data = new Record();
            data.neighbor = i;
            Vector3 diff = i.transform.position - transform.position;
            data.distanceSq = diff.sqrMagnitude;
            data.targetAngleRadians = Mathf.Atan2(diff.z, diff.x);
            data.headingInVisionRadians = Ext.AngleDiffUnsigned(data.targetAngleRadians, host.Heading);

            if (data.distanceSq < sqNeighborhoodDist && data.headingInVisionRadians < fovAngle / 2) neighborhood.Add(data);
        }
    }
}
