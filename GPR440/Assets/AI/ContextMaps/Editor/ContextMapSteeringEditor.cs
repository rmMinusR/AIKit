using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ContextMapSteering))]
public sealed class ContextMapSteeringEditor : ContextMapEditorHelper
{
    private static bool showingPreview = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if(showingPreview = EditorGUILayout.BeginFoldoutHeaderGroup(showingPreview, "Preview"))
        {
            Rect drawRect = EditorGUILayout.GetControlRect(false, PREVIEW_SIZE);
            //Rect drawRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, PREVIEW_SIZE, PREVIEW_SIZE);
            if(Event.current.type == EventType.Repaint) RefreshPreview(drawRect);
            EditorGUI.DrawPreviewTexture(drawRect, preview, mat: null, scaleMode: ScaleMode.ScaleToFit);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void RefreshPreview(Rect renderPos)
    {
        ContextMapSteering contextMap = target as ContextMapSteering;

        try
        {
            contextMap.RefreshContextMapValues();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        RenderPreview(preview, lineDrawMat, contextMap.entries, contextMap.GetComponent<CharacterHost>());
    }
}