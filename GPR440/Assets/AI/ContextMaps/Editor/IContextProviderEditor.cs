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
    private Material lineDrawMat;

    private void OnEnable()
    {
        preview = new RenderTexture(PREVIEW_SIZE, PREVIEW_SIZE, 0);
        if (!preview.IsCreated()) preview.Create();

        lineDrawMat = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineDrawMat.hideFlags = HideFlags.HideAndDontSave;
    }

    private void OnDisable()
    {
        if(preview != null) preview.Release();
        preview = null;

        DestroyImmediate(lineDrawMat);
        lineDrawMat = null;
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

    public static void RenderPreview(RenderTexture renderTarget, Material lineDrawMat, ContextMapSteering.Entry[] entries)
    {
        //CommandBuffer gpu = new CommandBuffer();
        //gpu.ClearRenderTarget()

        float highestVal = Mathf.Max(entries.Select(i => i.value).ToArray());
        float  lowestVal = Mathf.Min(entries.Select(i => i.value).ToArray());
        float RenormalizeValueAndAbs(float val)
        {
            val = (val<0) ? val/lowestVal : val/highestVal;
            if (float.IsNaN(val)) val = 0;
            return val;
        }

        //Store state
        RenderTexture prevDrawTarget = RenderTexture.active;
        GL.PushMatrix();

        //Draw preview
        {
            RenderTexture.active = renderTarget;
            GL.LoadOrtho();

            //Set up for rendering
            GL.Clear(true, true, Color.black);
            lineDrawMat.SetPass(0);

            void VertexXY(float x, float y) => GL.Vertex3(x/renderTarget.width, 1 - y/renderTarget.height, 0);
            const float QUARTER_TURN = Mathf.PI / 2;
            void VertexRadial(float ang, float r) => VertexXY(Mathf.Cos(ang - QUARTER_TURN)*r + renderTarget.width/2, Mathf.Sin(ang - QUARTER_TURN)*r + renderTarget.height/2);

            //Draw agent needle
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.white);
            const int needleRadius = 20;
            VertexXY(renderTarget.width/2 +  0                , renderTarget.height/2 + -     needleRadius);
            VertexXY(renderTarget.width/2 +  0.7f*needleRadius, renderTarget.height/2 +  0.7f*needleRadius);
            VertexXY(renderTarget.width/2 +  0                , renderTarget.height/2 +  0.3f*needleRadius);
            VertexXY(renderTarget.width/2 + -0.7f*needleRadius, renderTarget.height/2 +  0.7f*needleRadius);
            VertexXY(renderTarget.width/2 +  0                , renderTarget.height/2 + -     needleRadius);
            GL.End();

            //Draw center circle
            GL.Begin(GL.LINE_STRIP);
            int nEntries = entries.Length;
            const int centerRadius = 30;
            GL.Color(Color.white);
            for (int i = 0; i < nEntries+1; ++i)
            {
                VertexRadial(entries[i%nEntries].sourceAngle, centerRadius);
            }
            GL.End();

            //Draw context map entries
            GL.Begin(GL.LINES);
            const int entryRadius = 50;
            for (int i = 0; i < nEntries; ++i)
            {
                ContextMapSteering.Entry entry = entries[i];
                GL.Color(entry.value<0 ? Color.red : Color.green);
                VertexRadial(entry.sourceAngle, centerRadius);
                VertexRadial(entry.sourceAngle, centerRadius+RenormalizeValueAndAbs(entry.value)*entryRadius);
            }
            GL.End();
            
            //GL.Flush();
        }
        
        //Restore state
        GL.PopMatrix();
        RenderTexture.active = prevDrawTarget;
    }

    private void RefreshPreview(Rect renderPos)
    {
        IContextProvider context = target as IContextProvider;
        ContextMapSteering contextMap = context.GetComponent<ContextMapSteering>();

        try
        {
            context.RefreshContextMapValues();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        RenderPreview(preview, lineDrawMat, contextMap.entries);
    }
}
