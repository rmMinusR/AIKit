using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(IContextProvider))]
[CustomEditor(typeof(AvoidObstaclesContext))]
//[CustomEditor(typeof(KeepCurrentHeadingContext))]
//[CustomEditor(typeof(RandomWalkContext))]
public class IContextProviderEditor : Editor
{
    private const int PREVIEW_SIZE = 200;

    private RenderTexture __preview;
    private RenderTexture Preview
    {
        get
        {
            if (__preview != null && __preview.IsCreated()) return __preview;

            __preview = new RenderTexture(PREVIEW_SIZE, PREVIEW_SIZE, 0);
            if(!__preview.IsCreated()) __preview.Create();
            return __preview;
        }
    }
    private bool showingPreview = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        if(showingPreview = EditorGUILayout.BeginFoldoutHeaderGroup(showingPreview, "Preview"))
        {
            RefreshPreview();
            Rect texRect = EditorGUILayout.GetControlRect(false, PREVIEW_SIZE);
            EditorGUI.DrawPreviewTexture(texRect, Preview, mat: null, scaleMode: ScaleMode.ScaleToFit);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    ~IContextProviderEditor()
    {
        Preview.Release(); //TODO is this proper procedure?
    }

    private void RefreshPreview()
    {
        ContextMapSteering contextMap = (target as IContextProvider).contextMap;

    }
}
