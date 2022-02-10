using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class FlockNeighborhood : MonoBehaviour
{
    [Serializable]
    public struct Record
    {
        public CharacterHost neighbor;
        public float distance;
        public float targetAngleRadians;
        public float headingInVisionRadians;
    }

    [InspectorReadOnly] public List<Record> neighborhood = new List<Record>();

    [SerializeField] [Range(0, 180)] [Tooltip("In degrees")] private float fovAngle = 45;
    [Min(0)] public float fovDistance = 4;

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

        foreach (CharacterHost i in FindObjectsOfType<CharacterHost>())
        {
            //Don't count self as a neighbor
            if (i.gameObject == this.gameObject) continue;

            Record data = new Record();
            data.neighbor = i;
            Vector3 diff = i.transform.position - transform.position;
            data.distance = diff.magnitude;
            data.targetAngleRadians = Mathf.Atan2(diff.z, diff.x);
            data.headingInVisionRadians = Ext.AngleDiffUnsigned(data.targetAngleRadians, host.Heading);

            if (data.distance < fovDistance && data.headingInVisionRadians < fovAngle / 2) neighborhood.Add(data);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach (Record r in neighborhood)
        {
            Gizmos.DrawLine(transform.position, r.neighbor.transform.position);
            Vector3 midpoint = (transform.position + r.neighbor.transform.position) / 2;
            Vector3 dir = r.neighbor.transform.position - transform.position;
            dir = dir.normalized * 0.2f;
            Vector3 sidestepDir = new Vector3(dir.z, dir.y, -dir.x);
            Gizmos.DrawLine(midpoint + dir, midpoint + sidestepDir);
        }
    }
}
