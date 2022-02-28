using Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class GridCellContentsQuery : Query
    {
        public readonly Vector2Int chunkCoord;
        public readonly Vector2Int coordWithinChunk;
        public readonly Vector2Int coordGlobal;

        public CellFlags flags;
        public List<GameObject> structures;

        public GridCellContentsQuery(Vector2Int worldCoord)
        {
            coordGlobal = worldCoord;
            chunkCoord = GridAPI.CellToChunkCoord(coordGlobal);
            coordWithinChunk = coordGlobal - chunkCoord * GridAPI.CHUNK_SIZE;

            structures = null;
            flags = CellFlags.Default;
        }

        public void AddStructure(GameObject gameObject)
        {
            if (structures == null) structures = new List<GameObject>();
            structures.Add(gameObject);
        }
    }
}
