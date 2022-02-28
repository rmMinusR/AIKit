using System;
using System.Collections;
using UnityEngine;

namespace Grid
{

    [Serializable]
    internal struct WorldGridChunk
    {
        [TestButton("Reset Cells", "_FillCellsWithDefault")]
        [SerializeField] private WorldGridCellData[] data;
        [SerializeField] [HideInInspector] private bool contentsDirty;

        private int IndexOfRel(Vector2Int relCoord) => relCoord.x + relCoord.y * GridAPI.CHUNK_SIZE;
        private int IndexOfAbs(Vector2Int cellCoord) => IndexOfRel(cellCoord-RootCellCoord); //0 <= X,Y < CHUNK_SIZE

        public WorldGridCellData GetCell(Vector2Int absPos) => data[IndexOfAbs(absPos)];
        public void SetCell(Vector2Int absPos, WorldGridCellData newCellData) => data[IndexOfAbs(absPos)] = newCellData;

        [SerializeField] private Vector2Int _chunkCoord;
        public Vector2Int ChunkCoord { get => _chunkCoord; private set => _chunkCoord = value; }

        [SerializeField] private Vector2Int _rootCellCoord;
        public Vector2Int RootCellCoord { get => _rootCellCoord; private set => _rootCellCoord = value; }
    
        public WorldGridChunk(Vector2Int chunkCoord)
        {
            _chunkCoord = chunkCoord;
            _rootCellCoord = chunkCoord * GridAPI.CHUNK_SIZE;

            contentsDirty = true;

            data = null;
            _FillCellsWithDefault();
        }

        public override string ToString()
        {
            return "WorldGridChunk<"+GridAPI.CHUNK_SIZE+">@["+ChunkCoord.x+","+ChunkCoord.y+"]";
        }

        private void _FillCellsWithDefault()
        {
            data = new WorldGridCellData[GridAPI.CHUNK_SIZE * GridAPI.CHUNK_SIZE];
            for (int i = 0; i < data.Length; ++i)
            {
                Vector2Int coordWithinChunk = new Vector2Int(i%GridAPI.CHUNK_SIZE, i/GridAPI.CHUNK_SIZE);
                data[i] = new WorldGridCellData(coordWithinChunk, coordWithinChunk + RootCellCoord);
            }
        }

        internal void RefreshIfDirty()
        {
            if (contentsDirty)
            {
                for(int i = 0; i < data.Length; ++i) data[i].Refresh(force: false);
                contentsDirty = false;
            }
        }

        internal void MarkDirty()
        {
            contentsDirty = true;
        }
    }

}