using System;

namespace Grid
{
    [Flags]
    public enum CellFlags
    {
        None = 0,
        Default = Pathfindable | Buildable,

        Pathfindable = (1 << 0),
        Buildable    = (1 << 1),
        Demolishable = (1 << 2), //TODO redundant with interaction system
        
        //TODO implement later?
        //Damageable = (1 << 3),
        //Repairable = (1 << 4)
    }
}
