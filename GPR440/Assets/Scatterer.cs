using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Scatterer : MonoBehaviour
{
    [TestButton("Scatter", "_Scatter")]
    [TestButton("Clean up", "_Cleanup")]
    [SerializeField] [Min(0)] private int count;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject targetZone;
    [SerializeField] private Transform targetParent;
    [SerializeField] private Vector3 posOffset;

    [Space]
    [SerializeField] private List<GameObject> instances;

    public Vector3 FindRandomValidSpawnpoint()
    {
        Bounds bounds = targetZone.GetComponent<Collider>().bounds;
        for(int i = 0; i < 256; ++i)
        {
            Vector3 origPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 100,
                Random.Range(bounds.min.z, bounds.max.z)
            );
            if (Physics.Raycast(new Ray(origPos, Vector3.down), out RaycastHit hit) && hit.collider.GetComponent<Obstacle>() == null)
            {
                return hit.point;
            }
        }
        throw new System.Exception("Failed to find a valid spawn location");
    }

    private GameObject InstantiatePrefab()
    {
#if UNITY_EDITOR
        return (GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
#else
        return Instantiate(prefab);
#endif
    }

    public GameObject Drop()
    {
        Vector3 target = FindRandomValidSpawnpoint() + posOffset;
        GameObject obj = InstantiatePrefab();
        prefab.transform.position = target;
        obj.transform.parent = targetParent;
        instances.Add(obj);
        return obj;
    } 

    private void _Scatter()
    {
        for(int i = instances.Count; i < count; ++i) Drop();
    }

    private void _Cleanup()
    {
        foreach (GameObject o in instances) DestroyImmediate(o);
        instances.Clear();
    }
}
