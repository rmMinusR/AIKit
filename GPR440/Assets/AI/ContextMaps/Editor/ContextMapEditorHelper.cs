using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class ContextMapEditorHelper : Editor
{
    protected const int PREVIEW_SIZE = 200;

    protected Material lineDrawMat;
    protected RenderTexture preview;

    protected static float scale = 1;

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

    public override bool RequiresConstantRepaint() => Application.isPlaying;

    protected static void RenderPreview(RenderTexture renderTarget, Material lineDrawMat, ContextMapSteering.Entry[] entries, CharacterHost host)
    {
        //CommandBuffer gpu = new CommandBuffer();
        //gpu.ClearRenderTarget()

        float mostExtremeVal = Mathf.Max(entries.Select(i => Mathf.Abs(i.value)).ToArray());
        scale = Mathf.Max(scale, mostExtremeVal);
        float baseScale = Mathf.Pow(10, Mathf.CeilToInt(Mathf.Log10(scale)));
        float RenormalizeValueAndAbs(float val) => Mathf.Abs(val)/scale;

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

            int nEntries = entries.Length;
            const int centerRadius = 30;
            const int entryRadius = 65;
            void DrawCircle(float valAmt, Color col)
            {
                GL.Begin(GL.LINE_STRIP);
                GL.Color(col);
                for (int i = 0; i < nEntries+1; ++i)
                {
                    VertexRadial(entries[i % nEntries].sourceAngle, centerRadius + RenormalizeValueAndAbs(valAmt) * entryRadius);
                }
                GL.End();
            }

            //Draw center circle
            DrawCircle(0, Color.white);
            
            //Draw scale circles
            for(float i = 0.1f; i < 1.0f; i += 0.1f) DrawCircle(i*baseScale, Color.gray);
            DrawCircle(baseScale, Color.white);
            
            //Draw context map entries
            GL.Begin(GL.LINES);
            for (int i = 0; i < nEntries; ++i)
            {
                ContextMapSteering.Entry entry = entries[i];
                GL.Color(entry.value<0 ? Color.red : Color.green);
                VertexRadial(entry.sourceAngle, centerRadius);
                VertexRadial(entry.sourceAngle, centerRadius + RenormalizeValueAndAbs(entry.value) * entryRadius);
            }
            GL.End();
            
            //GL.Flush();
        }
        
        //Restore state
        GL.PopMatrix();
        RenderTexture.active = prevDrawTarget;
    }
}
