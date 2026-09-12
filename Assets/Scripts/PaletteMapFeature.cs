using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class PaletteMapFeature : ScriptableRendererFeature
{
    class PaletteMapPassData
    {
        public TextureHandle source;
        public TextureHandle destination;
        public Material material;
    }

    class Pass : ScriptableRenderPass
    {
        Material mat;

        public Pass(Material m)
        {
            mat = m;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var resources = frameData.Get<UniversalResourceData>();

            // Get the camera color buffer using the Render Graph-aware resource data.
            var colorHandle = resources.activeColorTexture;

            // Create a new texture for the output based on the source texture descriptor.
            var output = renderGraph.CreateTexture(colorHandle, "PaletteMapOutput");

            var passData = renderGraph.AddRenderPass<PaletteMapPassData>(
                "PaletteMap",
                out var data,
                "PaletteMap",
                0);

            data.source = colorHandle;
            data.destination = output;
            data.material = mat;

            passData.SetRenderFunc((PaletteMapPassData p, RenderGraphContext ctx) =>
            {
                Blitter.BlitTexture(ctx.cmd, p.source, p.destination, p.material, 0);
            });

            // In Render Graph, the camera target is managed by the render graph itself,
            // so we do not mutate renderer.cameraColorTargetHandle here.
        }
    }

    public Shader shader;
    Material mat;
    Pass pass;

    public override void Create()
    {
        mat = CoreUtils.CreateEngineMaterial(shader);
        pass = new Pass(mat);

        // RenderGraph only runs AfterRendering
        pass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        renderer.EnqueuePass(pass);
    }
}
