using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures
{
    public class DesaturationRenderPass : ScriptableRenderPass
    {
        private readonly DesaturationSettings _mSettings;
        private readonly Material _mFullscreenMaterial;
        private readonly Material _mOverrideMaterial;

        private readonly FilteringSettings _mFilteringSettings;
        private readonly List<ShaderTagId> _mShaderTagIds = new();

        private RendererList _mRendererList;

        /// <summary>
        /// Used as a render target for drawing objects.
        /// </summary>
        private RTHandle _mFilterTextureHandle;

        /// <summary>
        /// Used for the fullscreen blit.
        /// </summary>
        private RTHandle _mTemporaryColorTextureHandle;

        private static readonly int SaturationId = Shader.PropertyToID("_Saturation");

        public DesaturationRenderPass(DesaturationSettings settings)
        {
            _mSettings = settings;
            _mFullscreenMaterial = new Material(settings.FullscreenShader);

            if (settings.OverrideShader != null)
                _mOverrideMaterial = new Material(settings.OverrideShader);

            // Make sure we use layers in our filtering settings.
            var renderLayer = (uint)1 << settings.RenderLayerMask;
            _mFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, settings.LayerMask, renderLayer);

            // Use default shader tags.
            _mShaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));
            _mShaderTagIds.Add(new ShaderTagId("UniversalForward"));
            _mShaderTagIds.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        private void UpdateSettings()
        {
            if (_mFullscreenMaterial == null) return;

            _mFullscreenMaterial.SetFloat(SaturationId, _mSettings.Saturation);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var cameraTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            cameraTextureDescriptor.colorFormat = _mSettings.RenderTextureFormat;
            cameraTextureDescriptor.depthBufferBits = (int)DepthBits.None;

            RenderingUtils.ReAllocateHandleIfNeeded(ref _mFilterTextureHandle, cameraTextureDescriptor,
                name: "_FilterTexture");

            RenderingUtils.ReAllocateHandleIfNeeded(ref _mTemporaryColorTextureHandle, cameraTextureDescriptor,
                name: "_TemporaryColor");

            var cameraDepthTextureHandle = renderingData.cameraData.renderer.cameraDepthTargetHandle;

            ConfigureTarget(_mFilterTextureHandle, cameraDepthTextureHandle);
            ConfigureClear(ClearFlag.Color, new Color(0, 0, 0, 0));
        }

        private void InitRendererLists(ref RenderingData renderingData, ScriptableRenderContext context)
        {
            var sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

            var drawingSettings = CreateDrawingSettings(_mShaderTagIds, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = _mOverrideMaterial;
            drawingSettings.overrideMaterialPassIndex = 0;

            var param = new RendererListParams(renderingData.cullResults, drawingSettings, _mFilteringSettings);
            _mRendererList = context.CreateRendererList(ref param);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Make sure we have a valid material
            if (_mFullscreenMaterial == null)
                return;

            var cmd = CommandBufferPool.Get();
            var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

            UpdateSettings();

            using (new ProfilingScope(cmd, new ProfilingSampler("DesaturationPass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Initialize and draw all renderers.
                InitRendererLists(ref renderingData, context);
                cmd.DrawRendererList(_mRendererList);

                // Pass our filter texture to shaders as a global texture reference.
                // Obtain this in a shader graph as a Texture2D with exposed un-ticked
                // and reference _FilterTexture.
                cmd.SetGlobalTexture(Shader.PropertyToID(_mFilterTextureHandle.name),
                    _mFilterTextureHandle);

                // For some reasons these rt are null for a frame when selecting in scene view.
                if (cameraTargetHandle.rt != null && _mTemporaryColorTextureHandle.rt != null)
                {
                    Blitter.BlitCameraTexture(cmd, cameraTargetHandle, _mTemporaryColorTextureHandle,
                        _mFullscreenMaterial,
                        0);
                    Blitter.BlitCameraTexture(cmd, _mTemporaryColorTextureHandle, cameraTargetHandle);
                }
            }

            // Execute and release command buffer
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <summary>
        /// Releases all used resources. Called by the feature.
        /// </summary>
        public void Dispose()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(_mFullscreenMaterial);
                Object.Destroy(_mOverrideMaterial);
            }
            else
            {
                Object.DestroyImmediate(_mFullscreenMaterial);
                Object.DestroyImmediate(_mOverrideMaterial);
            }
#else
            Object.Destroy(m_FullscreenMaterial);
            Object.Destroy(m_OverrideMaterial);
#endif

            _mFilterTextureHandle?.Release();
            _mTemporaryColorTextureHandle?.Release();
        }
    }
}