using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IContextProvider), true)]
public class IContextMapProviderEditor : ContextMapEditorHelper
{
    private static HashSet<Type> openPreviewWindows = new HashSet<Type>();
    protected bool showingPreview
    {
        get => openPreviewWindows.Contains(target.GetType());
        set
        {
            if(value) openPreviewWindows.Add(target.GetType());
            else openPreviewWindows.Remove(target.GetType());
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (showingPreview = EditorGUILayout.BeginFoldoutHeaderGroup(showingPreview, "Preview"))
        {
            Rect drawRect = EditorGUILayout.GetControlRect(false, PREVIEW_SIZE);
            //Rect drawRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE, PREVIEW_SIZE, PREVIEW_SIZE);
            if (Event.current.type == EventType.Repaint) RefreshPreview(drawRect);
            EditorGUI.DrawPreviewTexture(drawRect, preview, mat: null, scaleMode: ScaleMode.ScaleToFit);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void RefreshPreview(Rect renderPos)
    {
        IContextProvider contextMap = target as IContextProvider;

        try
        {
            contextMap.RefreshAndCopyContextMapValues();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        RenderPreview(preview, lineDrawMat, contextMap.entries, contextMap.GetComponent<CharacterHost>());
    }
}