using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Grid
{

    internal sealed class WorldGrid : MonoBehaviour, ISerializationCallbackReceiver
    {
        #region Singleton

        private static WorldGrid __instance;
        public static WorldGrid INSTANCE {
            get {
                //If we've already captured an instance, return it
                if(__instance != null) return __instance;

                //Fallback 1: try to find uncaptured instance
                __instance = FindObjectOfType<WorldGrid>();
                if(__instance != null) return __instance;

                //Fallback 2: create instance and capture it
                GameObject go = new GameObject("WorldGrid");
                __instance = go.AddComponent<WorldGrid>();
                return __instance;
            }
        }

        private void Awake()
        {
            Debug.Assert(__instance == null || __instance == this);
            __instance = this;

            if (contentsDirty) MarkDirtySweep();
        }

        #endregion

        [NonSerialized] public Dictionary<Vector2Int, WorldGridChunk> chunkRecord = new Dictionary<Vector2Int, WorldGridChunk>(); //Key is CHUNK coord not CELL coord

        #region Cell I/O
        
        public WorldGridChunk? GetOwningChunk(Vector2Int cellCoord, bool create)
        {
            Vector2Int chunkCoord = GridAPI.CellToChunkCoord(cellCoord);
            WorldGridChunk chunk;
            //Try to fetch chunk if it exists
            if(chunkRecord.TryGetValue(chunkCoord, out chunk)) return chunk;
            //It doesn't exist, create a new one if policy specifies
            else if(create) { chunk = new WorldGridChunk(chunkCoord); chunkRecord.Add(chunkCoord, chunk); return chunk;}
            //It doesn't exist and we can't create one, return null
            else return null;
        }

        public void SetChunk(WorldGridChunk chunk) => SetChunk(chunk.ChunkCoord, chunk);
        public void SetChunk(Vector2Int chunkCoord, WorldGridChunk chunk) => chunkRecord[chunkCoord] = chunk;

        public WorldGridCellData GetCell(Vector2Int cellCoord) => GetOwningChunk(cellCoord, false)?.GetCell(cellCoord) //Try to get it if the chunk exists
                                                                ?? new WorldGridCellData(new Vector2Int(), cellCoord); //Otherwise make dummy that would have the same value
        public void SetCell(Vector2Int cellCoord, WorldGridCellData cellData)
        {
            WorldGridChunk chunk = GetOwningChunk(cellCoord, true).Value;
            chunk.SetCell(cellCoord, cellData);
            SetChunk(chunk);
        }

        #endregion Cell I/O

        #region Dirty refresher

        [SerializeField] [HideInInspector] private bool contentsDirty = true;
        private IEnumerator dirtyRefreshWorker;

        public void MarkDirtySweep()
        {
            if (dirtyRefreshWorker == null)
            {
                contentsDirty = true;
                StartCoroutine(dirtyRefreshWorker = _DirtyRefreshWorker());
            }
        }

        private void OnDestroy()
        {
            if (dirtyRefreshWorker != null)
            {
                StopCoroutine(dirtyRefreshWorker);
                dirtyRefreshWorker = null;
            }

            __instance = null;
        }

        private IEnumerator _DirtyRefreshWorker()
        {
            yield return new WaitForEndOfFrame();

            Debug.Log("Refreshing "+chunkRecord.Count+" chunks");

            foreach (Vector2Int coord in chunkRecord.Keys.ToArray())
            {
                WorldGridChunk chunk = chunkRecord[coord];
                chunk.RefreshIfDirty();
                chunkRecord[coord] = chunk;
            }

            contentsDirty = false;
            dirtyRefreshWorker = null;
            yield break;
        }

        #endregion

        #region Serialization fix

        [SerializeField] private List<WorldGridChunk> __chunkSerializationHelper = new List<WorldGridChunk>();

        public void OnAfterDeserialize()
        {
            foreach(WorldGridChunk chunk in __chunkSerializationHelper) chunkRecord.Add(chunk.ChunkCoord, chunk);
            __chunkSerializationHelper.Clear();
        }

        public void OnBeforeSerialize()
        {
            foreach(WorldGridChunk chunk in chunkRecord.Values) __chunkSerializationHelper.Add(chunk);
            chunkRecord.Clear();
        }

        #endregion Serialization fix

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //if (!debugGizmos) return;

            void drawRect(float x1, float z1, float x2, float z2)
            {
                UnityEditor.Handles.DrawPolyLine(
                    new Vector3(x1, 0, z1),
                    new Vector3(x1, 0, z2),
                    new Vector3(x2, 0, z2),
                    new Vector3(x2, 0, z1),
                    new Vector3(x1, 0, z1)
                );
            }

            foreach(WorldGridChunk c in chunkRecord.Values)
            {
                //Draw chunk outline
                GridAPI.CellCoordToWorld(c.RootCellCoord                                  , out float chunkMinX, out float chunkMinZ, center: false);
                GridAPI.CellCoordToWorld(c.RootCellCoord+Vector2Int.one*GridAPI.CHUNK_SIZE, out float chunkMaxX, out float chunkMaxZ, center: false);

                UnityEditor.Handles.color = Color.blue;
                drawRect(chunkMinX+0.05f, chunkMinZ+0.05f, chunkMaxX-0.05f, chunkMaxZ-0.05f);

                for(Vector2Int relPos = Vector2Int.zero; relPos.y < GridAPI.CHUNK_SIZE; ++relPos.y)
                {
                    for(relPos.x = 0; relPos.x < GridAPI.CHUNK_SIZE; ++relPos.x)
                    {
                        //Draw cell outline
                        Vector2Int accessCellCoord = c.RootCellCoord + relPos;
                        GridAPI.CellCoordToWorld(accessCellCoord               , out float minX   , out float minZ, center: false);
                        GridAPI.CellCoordToWorld(accessCellCoord+Vector2Int.one, out float maxX   , out float maxZ, center: false);

                        UnityEditor.Handles.color = Color.green;
                        drawRect(minX+0.1f, minZ+0.1f, maxX-0.1f, maxZ-0.1f);

                        //Draw text info
                        GridAPI.CellCoordToWorld(accessCellCoord, out float centerX, out float centerZ, center: true);
                        WorldGridCellData cell = c.GetCell(accessCellCoord);
                        UnityEditor.Handles.Label(
                                new Vector3(centerX, 0, centerZ),
                                (cell.isDirty ? "Dirty" : "Not dirty")+"\n"+
                                "Flags="+cell.GetFlags()+"\n"+
                                "Contains "+cell.Contents.Count
                            );
                    }
                }
            }
        }
#endif
    }

}