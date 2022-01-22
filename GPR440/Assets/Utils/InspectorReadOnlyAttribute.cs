using UnityEngine;

/*
 * 
 * Makes a property read-only in inspector
 * Taken from https://answers.unity.com/questions/489942/how-to-make-a-readonly-property-in-inspector.html
 * 
 */

public class InspectorReadOnlyAttribute : PropertyAttribute
{
    public AccessMode editMode;
    public AccessMode playMode;

    public InspectorReadOnlyAttribute(AccessMode editMode = AccessMode.ReadOnly, AccessMode playMode = AccessMode.ReadOnly)
    {
        this.editMode = editMode;
        this.playMode = playMode;
    }
}

public enum AccessMode
{
    ReadOnly  = 0b01,
    ReadWrite = 0b11
}