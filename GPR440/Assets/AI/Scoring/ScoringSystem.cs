using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class ScoringSystem : MonoBehaviour
{
    [Header("Scoring")]
    [InspectorReadOnly(editMode = AccessMode.ReadOnly, playMode = AccessMode.ReadWrite)] [SerializeField] private float score;
    public float Score { get; protected set; }
    [SerializeField] protected float passiveGain = 0.2f;

    protected virtual void OnEnable()
    {
        ResetScoring();
    }

    protected virtual void Update()
    {
        Score += Time.deltaTime * passiveGain;
    }

    public virtual void ResetScoring()
    {
        Score = 0;
    }

    //For sorting
    public class Comparer : IComparer<ScoringSystem>
    {
        public int Compare(ScoringSystem x, ScoringSystem y)
        {
            return x.Score.CompareTo(y.Score);
        }
    }
}
