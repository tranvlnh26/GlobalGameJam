using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures.Blur
{
    [Serializable]
    public class BlurSettings
    {
        [Range(0, 0.4f)]
        public float HorizontalBlur;

        [Range(0, 0.4f)]
        public float VerticalBlur;
    }

    public class BlurRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        private BlurSettings BlurSettings;

        [SerializeField]
        private Shader Shader;

        private Material _mMaterial;

        private BlurRenderPass _mBlurRenderPass;

        public override void Create()
        {
            if (Shader == null)
                return;

            _mMaterial = new Material(Shader);
            _mBlurRenderPass = new BlurRenderPass(_mMaterial, BlurSettings)
            {
                renderPassEvent = RenderPassEvent.AfterRenderingSkybox
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
                renderer.EnqueuePass(_mBlurRenderPass);
        }

        protected override void Dispose(bool disposing)
        {
            _mBlurRenderPass.Dispose();

            if (_mMaterial == null)
                return;

#if UNITY_EDITOR
            if (Application.isPlaying)
                Destroy(_mMaterial);
            else
                DestroyImmediate(_mMaterial);
#else
            Destroy(_mMaterial);
#endif
        }
    }
}