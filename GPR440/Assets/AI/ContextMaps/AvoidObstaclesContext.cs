using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstaclesContext : IContextProvider
{
    [SerializeField] [Min(0)] private float avoidWeight = 1;
    [SerializeField] [Min(0)] private float avoidRange = 1;
    [SerializeField] private AnimationCurve avoidShapeFunc = AnimationCurve.Linear(0, 1, 1, 0);

    private struct ClosePoint
    {
        public Vector3 point;
        public float angle;
        public float distance;

        public float basePressure;
    }

    public override void RefreshContextMapValues()
    {
        //Compute nearby obstacles
        List<ClosePoint> closePoints = new List<ClosePoint>();
        foreach (Obstacle o in Obstacle.OBSTACLES)
        {
            if (o.gameObject == this.gameObject) continue; //Don't count self

            ClosePoint data;
            data.point = o.mainCollider.ClosestPoint(this.transform.position);
            Vector3 diff = data.point - this.transform.position;
            data.angle = Mathf.Atan2(diff.z, diff.x);
            data.distance = diff.magnitude;

            data.basePressure = 1 - Mathf.Clamp01(data.distance / avoidRange);

            closePoints.Add(data);
        }

        for (int i = 0; i < contextMap.entries.Length; ++i)
        {
            foreach(ClosePoint data in closePoints)
            {
                float pressure = data.basePressure * Mathf.Clamp01(avoidShapeFunc.Evaluate(Mathf.Clamp01(Ext.AngleDiffUnsigned(contextMap.entries[i].sourceAngle, data.angle) / Mathf.PI)));
                contextMap.entries[i].value -= pressure * avoidWeight;
            }
        }
    }
}