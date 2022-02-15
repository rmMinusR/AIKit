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

    [SerializeField] [Range(0, 360)] [Tooltip("In degrees")] private float fovAngle = 45;
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

        float allowedMaxAngleRadians = fovAngle/2 * Mathf.Deg2Rad;

        foreach (CharacterHost i in FindObjectsOfType<CharacterHost>())
        {
            //Don't count self as a neighbor
            if (i.gameObject == this.gameObject) continue;

            Record data = new Record();
            data.neighbor = i;
            Vector3 diff = i.transform.position - transform.position;
            data.distance = diff.magnitude;
            data.targetAngleRadians = Mathf.Atan2(diff.z, diff.x);
            data.headingInVisionRadians = Ext.AngleDiffUnsigned(host.Heading, data.targetAngleRadians);

            if (data.distance < fovDistance && data.headingInVisionRadians < allowedMaxAngleRadians) neighborhood.Add(data);
        }
    }

    private void OnDrawGizmos()
    {
        if (host == null) return;

        //Draw neighbors
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
        
        //Draw vision cone
        float allowedMaxAngleRadians = fovAngle / 2 * Mathf.Deg2Rad;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(host.Heading + allowedMaxAngleRadians), 0, Mathf.Sin(host.Heading + allowedMaxAngleRadians))*fovDistance);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(host.Heading - allowedMaxAngleRadians), 0, Mathf.Sin(host.Heading - allowedMaxAngleRadians))*fovDistance);
        const float fovStep = 0.2f;
        for (float angle = -allowedMaxAngleRadians; angle < allowedMaxAngleRadians; angle += fovStep)
        {
            Gizmos.DrawLine(
                transform.position + new Vector3(Mathf.Cos(host.Heading + angle          ), 0, Mathf.Sin(host.Heading + angle          )) * fovDistance,
                transform.position + new Vector3(Mathf.Cos(host.Heading + angle + fovStep), 0, Mathf.Sin(host.Heading + angle + fovStep)) * fovDistance
            );
        }
    }
}
