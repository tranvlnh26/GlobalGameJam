using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures.Blur
{
    public class BlurRenderPass : ScriptableRenderPass
    {
        private readonly BlurSettings _mDefaultSettings;
        private readonly Material _mMaterial;
        private RenderTextureDescriptor _mBlurTextureDescriptor;

        private RTHandle _mBlurTextureHandle;

        private static readonly int HorizontalBlurId = Shader.PropertyToID("_HorizontalBlur");
        private static readonly int VerticalBlurId = Shader.PropertyToID("_VerticalBlur");

        public BlurRenderPass(Material material, BlurSettings defaultSettings)
        {
            _mDefaultSettings = defaultSettings;
            _mMaterial = material;

            _mBlurTextureDescriptor =
                new RenderTextureDescriptor(Screen.width, Screen.height, RenderTextureFormat.Default, 0);
        }

        private void UpdateBlurSettings()
        {
            if (_mMaterial == null) return;

            _mMaterial.SetFloat(HorizontalBlurId, _mDefaultSettings.HorizontalBlur);
            _mMaterial.SetFloat(VerticalBlurId, _mDefaultSettings.VerticalBlur);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            // Set the blur texture size to the same as the camera target size.
            _mBlurTextureDescriptor.width = cameraTextureDescriptor.width;
            _mBlurTextureDescriptor.height = cameraTextureDescriptor.height;

            // Check if the descriptor has changed, and reallocate the RTHandle if necessary.
            RenderingUtils.ReAllocateHandleIfNeeded(ref _mBlurTextureHandle, _mBlurTextureDescriptor, name: "_BlurTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Get a command buffer from the pool.
            var cmd = CommandBufferPool.Get();
            var cameraTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;

            UpdateBlurSettings();

            using (new ProfilingScope(cmd, new ProfilingSampler("BlurRenderPass")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                // Blit from the camera target to the temporary render texture using the first pass.
                Blit(cmd, cameraTargetHandle, _mBlurTextureHandle, _mMaterial);
                // Blit from the temporary render texture to the camera target using the second pass.
                Blit(cmd, _mBlurTextureHandle, cameraTargetHandle, _mMaterial, 1);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
                Object.Destroy(_mMaterial);
            else
                Object.DestroyImmediate(_mMaterial);
#else
            Object.Destroy(m_Material);
#endif

            _mBlurTextureHandle?.Release();
        }
    }
}