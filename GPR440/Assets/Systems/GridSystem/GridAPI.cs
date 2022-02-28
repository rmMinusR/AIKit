using System.Text;
using UnityEngine;

namespace Grid
{
    /// <summary>
    /// The grid uses three coordinate systems:
    ///  - World space is what Unity uses, and is floats
    ///  - Cell coordinates are the X and Z index of a cell. This is different than casting
    ///    to int. It might or might not correspond to world space, depending on CELL_SIZE.
    ///  - Chunk coordinates work like cell coordinates, but CHUNK_SIZE times bigger.
    /// </summary>
    public static class GridAPI
    {
        #region Coordinate handling

        internal const int CHUNK_SIZE = 16;
        internal const float CELL_SIZE = 1;
        
        /// <summary>
        /// Convert from world space to cell coordinates
        /// </summary>
        public static Vector2Int WorldToCellCoord(float worldX, float worldZ)
        {
            //Cell roots are on the negative side. Always floor instead of casting to int!
            return new Vector2Int(
                Mathf.FloorToInt(worldX / CELL_SIZE),
                Mathf.FloorToInt(worldZ / CELL_SIZE)
            );
        }

        /// <summary>
        /// Gets a world space position from a cell coordinate.
        /// Can select either the cell's center or root position.
        /// 
        /// Can be used with both Vectors and floats:
        /// 
        /// float x, z;
        /// CellCoordToWorld(myCellCoord, out x, out z);
        /// Vector3 myPos = Vector3.zero;
        /// CellCoordToWorld(myCellCoord, out myPos.x, out myPos.z);
        /// </summary>
        public static void CellCoordToWorld(Vector2Int cellCoord, out float worldX, out float worldZ, bool center = true)
        {
            Vector2 @out = new Vector2(cellCoord.x, cellCoord.y);
            if(center) @out += Vector2.one/2; //Apply centering
            @out *= CELL_SIZE; //Convert to world coordinates
            worldX = @out.x;
            worldZ = @out.y;
        }

        /// <summary>
        /// Snaps a world position to the root of the cell it occupies.
        /// </summary>
        public static Vector3 WorldToCellRoot(Vector3 worldPos)
        {
            Vector2Int cellCoord = WorldToCellCoord(worldPos.x, worldPos.z);
            CellCoordToWorld(cellCoord, out worldPos.x, out worldPos.z, center: false);
            return worldPos;
        }

        /// <summary>
        /// Snaps a world position to the center of the cell it occupies.
        /// </summary>
        public static Vector3 WorldToCellCenter(Vector3 worldPos)
        {
            Vector2Int cellCoord = WorldToCellCoord(worldPos.x, worldPos.z);
            CellCoordToWorld(cellCoord, out worldPos.x, out worldPos.z, center: true);
            return worldPos;
        }

        /// <summary>
        /// Gets the coordinate of the chunk that owns a given cell
        /// </summary>
        internal static Vector2Int CellToChunkCoord(Vector2Int cellCoord)
        {
            return new Vector2Int(
                Mathf.FloorToInt(cellCoord.x / (float)CHUNK_SIZE),
                Mathf.FloorToInt(cellCoord.y / (float)CHUNK_SIZE)
            );
        }

        #endregion

        #region Flag handling

        /// <summary>
        /// Signal that the cell at the position no longer
        /// has valid data, and needs to be refreshed.
        /// </summary>
        public static void MarkDirty(Vector2Int gridCell)
        {
            //Mark world
            WorldGrid.INSTANCE.MarkDirtySweep();

            //Mark chunk
            WorldGridChunk chunk = WorldGrid.INSTANCE.GetOwningChunk(gridCell, true).Value;
            chunk.MarkDirty();
            WorldGrid.INSTANCE.SetChunk(chunk);

            //Mark cell
            WorldGridCellData cell = WorldGrid.INSTANCE.GetCell(gridCell);
            cell.isDirty = true;
            WorldGrid.INSTANCE.SetCell(gridCell, cell);
        }

        /// <summary>
        /// Check a cell's flags to see if it is buildable, demolishable, or pathfindable.
        /// If matchAll is false, it will attempt to match any of the given flags. If matchAll
        /// is true, it will attempt to match all.
        /// </summary>
        public static bool HasFlag(Vector2Int cellCoord, CellFlags flag, bool matchAll = true)
        {
            CellFlags status = WorldGrid.INSTANCE.GetCell(cellCoord).GetFlags() & flag;
            if(matchAll) return status == flag;
            else return status != CellFlags.None;
        }

        #endregion

        /// <summary>
        /// Get a string representing the state of the cell at the given coordinate
        /// </summary>
        public static string GetCellDebugInfo(Vector2Int cellCoord)
        {
            const string format = "GridCell global@{0} chunk@{1}*{2}+{3} with flags {4}, {5}";
            //0 = global grid coordinate
            //1 = coord of owning chunk
            //2 = chunk size
            //3 = coord within owning chunk
            //4 = flags
            //5 = dirty/not-dirty/not-initialized
            WorldGridCellData cell = WorldGrid.INSTANCE.GetCell(cellCoord);
            return string.Format(format,
                    cell.coordGlobal,
                    CellToChunkCoord(cell.coordGlobal),
                    CHUNK_SIZE,
                    cell.coordWithinChunk,
                    cell.GetFlags(),
                    cell.isDirty
                );
        }
    }
}
