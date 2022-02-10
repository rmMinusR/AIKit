using System.Collections;
using System.Collections.Generic;
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
            Rect texRect = EditorGUILayout.GetControlRect(false, PREVIEW_SIZE);
            RefreshPreview(texRect);
            EditorGUI.DrawPreviewTexture(texRect, Preview, mat: null, scaleMode: ScaleMode.ScaleToFit);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    ~IContextProviderEditor()
    {
        Preview.Release(); //TODO is this proper procedure?
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
            RenderTexture.active = Preview;
            //GL.LoadPixelMatrix();

            //Set up for rendering
            Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));
            GL.Clear(true, true, Color.black);
            mat.SetPass(0);

            //Draw agent needle
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.cyan);
            const int needleRadius = 80;
            GL.Vertex3( 0                , -     needleRadius, 0);
            GL.Vertex3( 0.7f*needleRadius,  0.7f*needleRadius, 0);
            GL.Vertex3( 0                ,  0.3f*needleRadius, 0);
            GL.Vertex3(-0.7f*needleRadius,  0.7f*needleRadius, 0);
            GL.Vertex3( 0                , -     needleRadius, 0);
            GL.End();

            //Draw center circle
            GL.Begin(GL.LINE_STRIP);
            int nEntries = contextMap.entries.Length;
            const int centerRadius = 100;
            const float QUARTER_TURN = Mathf.PI / 2;
            for (int i = 0; i < nEntries+1; ++i)
            {
                GL.Color(Color.cyan);
                GL.Vertex3(Mathf.Cos(Mathf.PI*2*i/nEntries - QUARTER_TURN) * centerRadius, Mathf.Sin(Mathf.PI*2*i/nEntries - QUARTER_TURN) * centerRadius, 0);
            }
            GL.End();

            //Draw context map entries
            //context.RefreshContextMapValues();
            //TODO

            //GL.Flush();
        }
        GUI.EndClip();

        GL.PopMatrix();
        
        RenderTexture.active = prevDrawTarget;
    }
}
