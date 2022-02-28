using Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{

    [Serializable]
    internal struct WorldGridCellData
    {
        [SerializeField] private CellFlags flags;
        [SerializeField] internal Vector2Int coordWithinChunk;
        [SerializeField] internal Vector2Int coordGlobal;

        //Should this be here?
        //TODO run performance/memory tests
        [SerializeField] private List<GameObject> contents; //For now, null is equivalent to empty list. See Contents getter.

        public bool isDirty;

        public CellFlags GetFlags() => flags;
        public void SetFlag(CellFlags flag, bool val)
        {
            if(val) flags |=  flag;
            else    flags &= ~flag;
        }

        public IReadOnlyList<GameObject> Contents => contents ?? new List<GameObject>();

        public WorldGridCellData(Vector2Int coordWithinChunk, Vector2Int coordGlobal)
        {
            flags = CellFlags.Default;
            this.coordGlobal = coordGlobal;
            this.coordWithinChunk = coordWithinChunk;
            isDirty = true;
            contents = null;
        }

        public void Refresh(bool force = false)
        {
            if(isDirty || force)
            {
                isDirty = false;

                GridCellContentsQuery query = new GridCellContentsQuery(coordGlobal);
                EventAPI.Dispatch(query);

                this.flags = query.flags;
                this.contents = query.structures; //FIXME rename?
            }
        }
    }

}