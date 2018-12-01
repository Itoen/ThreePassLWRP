using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;

namespace ThreePassLWRP
{
    public abstract class ThreePassLWRPForward2ndPass : ScriptableRenderPass
    {
        private RenderTargetHandle colorAttachmentHandle { get; set; }
        private RenderTargetHandle depthAttachmentHandle { get; set; }
        private RenderTextureDescriptor descriptor { get; set; }
        protected ClearFlag clearFlag { get; set; }
        protected Color clearColor { get; set; }

        const string k_SwitchRTs = "Switch RT";

        Material m_ErrorMaterial;

        List<ShaderPassName> m_LegacyShaderPassNames;
        protected RendererConfiguration rendererConfiguration;
        protected bool dynamicBatching;

        protected ThreePassLWRPForward2ndPass (LightweightForwardRenderer renderer) : base(renderer)
        {

            m_ErrorMaterial = renderer.GetMaterial(MaterialHandles.Error);

            m_LegacyShaderPassNames = new List<ShaderPassName>();
            m_LegacyShaderPassNames.Add(new ShaderPassName("Always"));
            m_LegacyShaderPassNames.Add(new ShaderPassName("ForwardAdd"));

            RegisterShaderPassName("LightweightForward2nd");
        }

        public void Setup (
            RenderTextureDescriptor baseDescriptor,
            RenderTargetHandle colorAttachmentHandle,
            RenderTargetHandle depthAttachmentHandle,
            ClearFlag clearFlag,
            Color clearColor,
            RendererConfiguration configuration,
            bool dynamicbatching)
        {
            this.colorAttachmentHandle = colorAttachmentHandle;
            this.depthAttachmentHandle = depthAttachmentHandle;
            this.clearColor = clearColor;
            this.clearFlag = clearFlag;
            descriptor = baseDescriptor;
            this.rendererConfiguration = configuration;
            this.dynamicBatching = dynamicbatching;
        }

        protected void SetRenderTarget (CommandBuffer cmd, RenderBufferLoadAction loadOp, RenderBufferStoreAction storeOp, ClearFlag clearFlag, Color clearColor)
        {
            if (colorAttachmentHandle != RenderTargetHandle.CameraTarget)
            {
                if (depthAttachmentHandle != RenderTargetHandle.CameraTarget)
                    SetRenderTarget(
                        cmd,
                        colorAttachmentHandle.Identifier(),
                        loadOp,
                        storeOp,
                        depthAttachmentHandle.Identifier(),
                        loadOp,
                        storeOp,
                        clearFlag,
                        clearColor,
                        descriptor.dimension);
                else
                    SetRenderTarget(cmd, colorAttachmentHandle.Identifier(), loadOp, storeOp, clearFlag, clearColor, descriptor.dimension);
            }
            else
            {
                SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget, loadOp, storeOp, clearFlag, clearColor, descriptor.dimension);
            }
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR")]
        protected void RenderObjectsWithError (ref ScriptableRenderContext context, ref CullResults cullResults, Camera camera, FilterRenderersSettings filterSettings, SortFlags sortFlags)
        {
            if (m_ErrorMaterial != null)
            {
                DrawRendererSettings errorSettings = new DrawRendererSettings(camera, m_LegacyShaderPassNames[0]);
                for (int i = 1; i < m_LegacyShaderPassNames.Count; ++i)
                    errorSettings.SetShaderPassName(i, m_LegacyShaderPassNames[i]);

                errorSettings.sorting.flags = sortFlags;
                errorSettings.rendererConfiguration = RendererConfiguration.None;
                errorSettings.SetOverrideMaterial(m_ErrorMaterial, 0);
                context.DrawRenderers(cullResults.visibleRenderers, ref errorSettings, filterSettings);
            }
        }
    }
}
