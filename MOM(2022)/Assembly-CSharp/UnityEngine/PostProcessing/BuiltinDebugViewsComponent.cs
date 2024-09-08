using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    public sealed class BuiltinDebugViewsComponent : PostProcessingComponentCommandBuffer<BuiltinDebugViewsModel>
    {
        private static class Uniforms
        {
            internal static readonly int _DepthScale = Shader.PropertyToID("_DepthScale");

            internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");

            internal static readonly int _Opacity = Shader.PropertyToID("_Opacity");

            internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");

            internal static readonly int _TempRT2 = Shader.PropertyToID("_TempRT2");

            internal static readonly int _Amplitude = Shader.PropertyToID("_Amplitude");

            internal static readonly int _Scale = Shader.PropertyToID("_Scale");
        }

        private enum Pass
        {
            Depth = 0,
            Normals = 1,
            MovecOpacity = 2,
            MovecImaging = 3,
            MovecArrows = 4
        }

        private class ArrowArray
        {
            public Mesh mesh { get; private set; }

            public int columnCount { get; private set; }

            public int rowCount { get; private set; }

            public void BuildMesh(int columns, int rows)
            {
                Vector3[] array = new Vector3[6]
                {
                    new Vector3(0f, 0f, 0f),
                    new Vector3(0f, 1f, 0f),
                    new Vector3(0f, 1f, 0f),
                    new Vector3(-1f, 1f, 0f),
                    new Vector3(0f, 1f, 0f),
                    new Vector3(1f, 1f, 0f)
                };
                int num = 6 * columns * rows;
                List<Vector3> list = new List<Vector3>(num);
                List<Vector2> list2 = new List<Vector2>(num);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Vector2 item = new Vector2((0.5f + (float)j) / (float)columns, (0.5f + (float)i) / (float)rows);
                        for (int k = 0; k < 6; k++)
                        {
                            list.Add(array[k]);
                            list2.Add(item);
                        }
                    }
                }
                int[] array2 = new int[num];
                for (int l = 0; l < num; l++)
                {
                    array2[l] = l;
                }
                this.mesh = new Mesh
                {
                    hideFlags = HideFlags.DontSave
                };
                this.mesh.SetVertices(list);
                this.mesh.SetUVs(0, list2);
                this.mesh.SetIndices(array2, MeshTopology.Lines, 0);
                this.mesh.UploadMeshData(markNoLongerReadable: true);
                this.columnCount = columns;
                this.rowCount = rows;
            }

            public void Release()
            {
                GraphicsUtils.Destroy(this.mesh);
                this.mesh = null;
            }
        }

        private const string k_ShaderString = "Hidden/Post FX/Builtin Debug Views";

        private ArrowArray m_Arrows;

        public override bool active
        {
            get
            {
                if (!base.model.IsModeActive(BuiltinDebugViewsModel.Mode.Depth) && !base.model.IsModeActive(BuiltinDebugViewsModel.Mode.Normals))
                {
                    return base.model.IsModeActive(BuiltinDebugViewsModel.Mode.MotionVectors);
                }
                return true;
            }
        }

        public override DepthTextureMode GetCameraFlags()
        {
            BuiltinDebugViewsModel.Mode mode = base.model.settings.mode;
            DepthTextureMode depthTextureMode = DepthTextureMode.None;
            switch (mode)
            {
            case BuiltinDebugViewsModel.Mode.Normals:
                depthTextureMode |= DepthTextureMode.DepthNormals;
                break;
            case BuiltinDebugViewsModel.Mode.MotionVectors:
                depthTextureMode |= DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
                break;
            case BuiltinDebugViewsModel.Mode.Depth:
                depthTextureMode |= DepthTextureMode.Depth;
                break;
            }
            return depthTextureMode;
        }

        public override CameraEvent GetCameraEvent()
        {
            if (base.model.settings.mode != BuiltinDebugViewsModel.Mode.MotionVectors)
            {
                return CameraEvent.BeforeImageEffectsOpaque;
            }
            return CameraEvent.BeforeImageEffects;
        }

        public override string GetName()
        {
            return "Builtin Debug Views";
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
            BuiltinDebugViewsModel.Settings settings = base.model.settings;
            Material material = base.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
            material.shaderKeywords = null;
            if (base.context.isGBufferAvailable)
            {
                material.EnableKeyword("SOURCE_GBUFFER");
            }
            switch (settings.mode)
            {
            case BuiltinDebugViewsModel.Mode.Depth:
                this.DepthPass(cb);
                break;
            case BuiltinDebugViewsModel.Mode.Normals:
                this.DepthNormalsPass(cb);
                break;
            case BuiltinDebugViewsModel.Mode.MotionVectors:
                this.MotionVectorsPass(cb);
                break;
            }
            base.context.Interrupt();
        }

        private void DepthPass(CommandBuffer cb)
        {
            Material mat = base.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
            BuiltinDebugViewsModel.DepthSettings depth = base.model.settings.depth;
            cb.SetGlobalFloat(Uniforms._DepthScale, 1f / depth.scale);
            cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 0);
        }

        private void DepthNormalsPass(CommandBuffer cb)
        {
            Material mat = base.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
            cb.Blit(null, BuiltinRenderTextureType.CameraTarget, mat, 1);
        }

        private void MotionVectorsPass(CommandBuffer cb)
        {
            Material material = base.context.materialFactory.Get("Hidden/Post FX/Builtin Debug Views");
            BuiltinDebugViewsModel.MotionVectorsSettings motionVectors = base.model.settings.motionVectors;
            int num = Uniforms._TempRT;
            cb.GetTemporaryRT(num, base.context.width, base.context.height, 0, FilterMode.Bilinear);
            cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.sourceOpacity);
            cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
            cb.Blit(BuiltinRenderTextureType.CameraTarget, num, material, 2);
            if (motionVectors.motionImageOpacity > 0f && motionVectors.motionImageAmplitude > 0f)
            {
                int tempRT = Uniforms._TempRT2;
                cb.GetTemporaryRT(tempRT, base.context.width, base.context.height, 0, FilterMode.Bilinear);
                cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.motionImageOpacity);
                cb.SetGlobalFloat(Uniforms._Amplitude, motionVectors.motionImageAmplitude);
                cb.SetGlobalTexture(Uniforms._MainTex, num);
                cb.Blit(num, tempRT, material, 3);
                cb.ReleaseTemporaryRT(num);
                num = tempRT;
            }
            if (motionVectors.motionVectorsOpacity > 0f && motionVectors.motionVectorsAmplitude > 0f)
            {
                this.PrepareArrows();
                float num2 = 1f / (float)motionVectors.motionVectorsResolution;
                float x = num2 * (float)base.context.height / (float)base.context.width;
                cb.SetGlobalVector(Uniforms._Scale, new Vector2(x, num2));
                cb.SetGlobalFloat(Uniforms._Opacity, motionVectors.motionVectorsOpacity);
                cb.SetGlobalFloat(Uniforms._Amplitude, motionVectors.motionVectorsAmplitude);
                cb.DrawMesh(this.m_Arrows.mesh, Matrix4x4.identity, material, 0, 4);
            }
            cb.SetGlobalTexture(Uniforms._MainTex, num);
            cb.Blit(num, BuiltinRenderTextureType.CameraTarget);
            cb.ReleaseTemporaryRT(num);
        }

        private void PrepareArrows()
        {
            int motionVectorsResolution = base.model.settings.motionVectors.motionVectorsResolution;
            int num = motionVectorsResolution * Screen.width / Screen.height;
            if (this.m_Arrows == null)
            {
                this.m_Arrows = new ArrowArray();
            }
            if (this.m_Arrows.columnCount != num || this.m_Arrows.rowCount != motionVectorsResolution)
            {
                this.m_Arrows.Release();
                this.m_Arrows.BuildMesh(num, motionVectorsResolution);
            }
        }

        public override void OnDisable()
        {
            if (this.m_Arrows != null)
            {
                this.m_Arrows.Release();
            }
            this.m_Arrows = null;
        }
    }
}
