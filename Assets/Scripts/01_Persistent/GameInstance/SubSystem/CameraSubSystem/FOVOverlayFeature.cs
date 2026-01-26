using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
 
public class FOVOverlayFeature : ScriptableRendererFeature
{
    [Serializable]
    public class FOVOverlaySettings
    {
        public Texture fovTexture;      // RT_FOV
        public Material blendMaterial;  // FOV_Darken 머티리얼
    }

    [SerializeField] FOVOverlaySettings settings;

    FOVOverlayPass m_ScriptablePass;

    public override void Create()
    { 
        m_ScriptablePass = new FOVOverlayPass(settings);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }     
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blendMaterial != null && settings.fovTexture != null)
            renderer.EnqueuePass(m_ScriptablePass);
    }
      
    class FOVOverlayPass : ScriptableRenderPass
    {
        readonly FOVOverlaySettings settings;

        Mesh fullscreenMesh;

        public FOVOverlayPass(FOVOverlaySettings settings)
        {
            this.settings = settings;
            fullscreenMesh = GenerateFullscreenMesh();
        }

        Mesh GenerateFullscreenMesh()
        {
            Mesh mesh = new Mesh { name = "Fullscreen Triangle" };

            mesh.vertices = new Vector3[]
            {
                new Vector3(-1f, -1f, 0f),
                new Vector3(-1f,  3f, 0f),
                new Vector3( 3f, -1f, 0f)
            };
            mesh.uv = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(0, 2),
                new Vector2(2, 0),
            };
            mesh.triangles = new[] { 0, 1, 2 };
            mesh.UploadMeshData(false);
            return mesh;
        }
         
        private class PassData
        {
            public TextureHandle colorTarget;
            public Texture fovTexture;
            public Material mat;
            public Mesh fsMesh;
        }

        static void ExecutePass(PassData data, RasterGraphContext ctx)
        {
            data.mat.SetTexture("_FOVTex", data.fovTexture);
            
            // 랜더 패스 2번 그려주기 시야 안과 밖

            // Out 
            ctx.cmd.DrawMesh(data.fsMesh, Matrix4x4.identity, data.mat, 0, 0);

            // Inside
            ctx.cmd.DrawMesh(data.fsMesh, Matrix4x4.identity, data.mat, 0, 1);
        }   
         
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            if (cameraData.cameraType != CameraType.Game)
                return;
             
            var resourceData = frameData.Get<UniversalResourceData>();
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("FOV Overlay Pass", out var passData))
            {
                passData.colorTarget = resourceData.activeColorTexture;
                passData.fovTexture = settings.fovTexture;
                passData.mat = settings.blendMaterial;
                passData.fsMesh = fullscreenMesh;
                 
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data, ctx));
            }
        }
    }
}


