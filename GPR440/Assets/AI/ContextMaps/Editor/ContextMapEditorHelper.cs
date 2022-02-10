using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

//[CustomEditor(typeof(IContextProvider))]
//[CustomEditor(typeof(AvoidObstaclesContext))]
//[CustomEditor(typeof(KeepCurrentHeadingContext))]
//[CustomEditor(typeof(RandomWalkContext))]
public abstract class ContextMapEditorHelper : Editor
{
    protected const int PREVIEW_SIZE = 200;

    protected Material lineDrawMat;
    protected RenderTexture preview;

    protected virtual void OnEnable()
    {
        preview = new RenderTexture(PREVIEW_SIZE, PREVIEW_SIZE, 0);
        if (!preview.IsCreated()) preview.Create();

        lineDrawMat = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineDrawMat.hideFlags = HideFlags.HideAndDontSave;
    }

    protected virtual void OnDisable()
    {
        if(preview != null) preview.Release();
        preview = null;

        DestroyImmediate(lineDrawMat);
        lineDrawMat = null;
    }

    protected static void RenderPreview(RenderTexture renderTarget, Material lineDrawMat, ContextMapSteering.Entry[] entries, CharacterHost host)
    {
        //CommandBuffer gpu = new CommandBuffer();
        //gpu.ClearRenderTarget()

        float mostExtremeVal = Mathf.Max(entries.Select(i => Mathf.Abs(i.value)).ToArray());
        float RenormalizeValueAndAbs(float val) => mostExtremeVal!=0 ? Mathf.Abs(val)/mostExtremeVal : 0;

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
            void VertexRadial(float ang, float r) => VertexXY(Mathf.Cos(-ang)*r + renderTarget.width/2, Mathf.Sin(-ang)*r + renderTarget.height/2);

            //Draw agent needle
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.white);
            const int needleRadius = 20;
            VertexRadial(host.Heading +                0,      needleRadius);
            VertexRadial(host.Heading + Mathf.PI * 0.75f,      needleRadius);
            VertexRadial(host.Heading + Mathf.PI        , 0.2f*needleRadius);
            VertexRadial(host.Heading + Mathf.PI * 1.25f,      needleRadius);
            VertexRadial(host.Heading +                0,      needleRadius);
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
}
