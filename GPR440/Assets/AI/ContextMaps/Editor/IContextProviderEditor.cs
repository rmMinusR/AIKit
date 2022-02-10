using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//[CustomEditor(typeof(IContextProvider))]
[CustomEditor(typeof(AvoidObstaclesContext))]
//[CustomEditor(typeof(KeepCurrentHeadingContext))]
//[CustomEditor(typeof(RandomWalkContext))]
public class IContextProviderEditor : Editor
{
    private const int PREVIEW_SIZE = 200;

    private RenderTexture preview;
    private bool showingPreview = false;

    private void OnEnable()
    {
        preview = new RenderTexture(PREVIEW_SIZE, PREVIEW_SIZE, 0);
        if (!preview.IsCreated()) preview.Create();
    }

    private void OnDisable()
    {
        preview.Release();
        preview = null;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

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
        IContextProvider context = target as IContextProvider;
        ContextMapSteering contextMap = context.GetComponent<ContextMapSteering>();

        //CommandBuffer gpu = new CommandBuffer();
        //gpu.ClearRenderTarget()

        //Store state
        RenderTexture prevDrawTarget = RenderTexture.active;
        GL.PushMatrix();

        //Draw preview
        GUI.BeginClip(renderPos);
        {
            RenderTexture.active = preview;
            GL.LoadOrtho();

            //Set up for rendering
            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            mat.hideFlags = HideFlags.HideAndDontSave;
            GL.Clear(true, true, Color.black);
            mat.SetPass(0);

            void Vertex(float x, float y) => GL.Vertex3(x/PREVIEW_SIZE, 1 - y/PREVIEW_SIZE, 0);

            //Draw agent needle
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.white);
            const int needleRadius = 20;
            Vertex(PREVIEW_SIZE/2 +  0                , PREVIEW_SIZE/2 + -     needleRadius);
            Vertex(PREVIEW_SIZE/2 +  0.7f*needleRadius, PREVIEW_SIZE/2 +  0.7f*needleRadius);
            Vertex(PREVIEW_SIZE/2 +  0                , PREVIEW_SIZE/2 +  0.3f*needleRadius);
            Vertex(PREVIEW_SIZE/2 + -0.7f*needleRadius, PREVIEW_SIZE/2 +  0.7f*needleRadius);
            Vertex(PREVIEW_SIZE/2 +  0                , PREVIEW_SIZE/2 + -     needleRadius);
            GL.End();

            //Draw center circle
            GL.Begin(GL.LINE_STRIP);
            int nEntries = contextMap.entries.Length;
            const int centerRadius = 30;
            const float QUARTER_TURN = Mathf.PI / 2;
            GL.Color(Color.white);
            for (int i = 0; i < nEntries+1; ++i)
            {
                Vector2 pos = new Vector2(Mathf.Cos(Mathf.PI*2*i/nEntries - QUARTER_TURN), Mathf.Sin(Mathf.PI*2*i/nEntries - QUARTER_TURN));
                pos *= centerRadius; //Scale up
                pos += Vector2.one * PREVIEW_SIZE/2;
                Vertex(pos.x, pos.y);
            }
            GL.End();

            //Draw context map entries
            //context.RefreshContextMapValues();
            //TODO

            //GL.Flush();
        }
        GUI.EndClip();
        
        //Restore state
        GL.PopMatrix();
        RenderTexture.active = prevDrawTarget;
    }
}
