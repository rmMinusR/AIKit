using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class AvoidObstaclesContext : IContextProvider
{
    [SerializeField] [Min(0)] private float avoidRange = 1;
    [SerializeField] private AnimationCurve falloffCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [SerializeField] private bool normalizeValues;

    private struct ClosePoint
    {
        public Vector3 point;
        public float angle;
        public float distance;

        public float basePressure;
    }

    protected override void RefreshContextMapValues()
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

            data.basePressure = falloffCurve.Evaluate(Mathf.Clamp01(data.distance / avoidRange));

            closePoints.Add(data);
        }

        for (int i = 0; i < entries.Length; ++i)
        {
            foreach(ClosePoint data in closePoints)
            {
                entries[i].value += data.basePressure * shapingFunction.Evaluate(Mathf.Clamp(Ext.AngleDiffUnsigned(entries[i].sourceAngle, data.angle), 0, Mathf.PI));
            }
        }

        if (normalizeValues)
        {
            float normalizer = closePoints.Count;
            normalizer = (normalizer!=0) ? 1/normalizer : 1;
            for (int i = 0; i < entries.Length; ++i) entries[i].value *= normalizer;
        }
    }
}