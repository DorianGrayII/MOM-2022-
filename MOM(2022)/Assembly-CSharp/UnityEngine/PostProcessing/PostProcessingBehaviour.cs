using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    [ImageEffectAllowedInSceneView]
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
    public class PostProcessingBehaviour : MonoBehaviour
    {
        public PostProcessingProfile profile;

        public Func<Vector2, Matrix4x4> jitteredMatrixFunc;

        private Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;

        private List<PostProcessingComponentBase> m_Components;

        private Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;

        private MaterialFactory m_MaterialFactory;

        private RenderTextureFactory m_RenderTextureFactory;

        private PostProcessingContext m_Context;

        private Camera m_Camera;

        private PostProcessingProfile m_PreviousProfile;

        private bool m_RenderingInSceneView;

        private BuiltinDebugViewsComponent m_DebugViews;

        private AmbientOcclusionComponent m_AmbientOcclusion;

        private ScreenSpaceReflectionComponent m_ScreenSpaceReflection;

        private FogComponent m_FogComponent;

        private MotionBlurComponent m_MotionBlur;

        private TaaComponent m_Taa;

        private EyeAdaptationComponent m_EyeAdaptation;

        private DepthOfFieldComponent m_DepthOfField;

        private BloomComponent m_Bloom;

        private ChromaticAberrationComponent m_ChromaticAberration;

        private ColorGradingComponent m_ColorGrading;

        private UserLutComponent m_UserLut;

        private GrainComponent m_Grain;

        private VignetteComponent m_Vignette;

        private DitheringComponent m_Dithering;

        private FxaaComponent m_Fxaa;

        private List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();

        private List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();

        private void OnEnable()
        {
            this.m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
            this.m_MaterialFactory = new MaterialFactory();
            this.m_RenderTextureFactory = new RenderTextureFactory();
            this.m_Context = new PostProcessingContext();
            this.m_Components = new List<PostProcessingComponentBase>();
            this.m_DebugViews = this.AddComponent(new BuiltinDebugViewsComponent());
            this.m_AmbientOcclusion = this.AddComponent(new AmbientOcclusionComponent());
            this.m_ScreenSpaceReflection = this.AddComponent(new ScreenSpaceReflectionComponent());
            this.m_FogComponent = this.AddComponent(new FogComponent());
            this.m_MotionBlur = this.AddComponent(new MotionBlurComponent());
            this.m_Taa = this.AddComponent(new TaaComponent());
            this.m_EyeAdaptation = this.AddComponent(new EyeAdaptationComponent());
            this.m_DepthOfField = this.AddComponent(new DepthOfFieldComponent());
            this.m_Bloom = this.AddComponent(new BloomComponent());
            this.m_ChromaticAberration = this.AddComponent(new ChromaticAberrationComponent());
            this.m_ColorGrading = this.AddComponent(new ColorGradingComponent());
            this.m_UserLut = this.AddComponent(new UserLutComponent());
            this.m_Grain = this.AddComponent(new GrainComponent());
            this.m_Vignette = this.AddComponent(new VignetteComponent());
            this.m_Dithering = this.AddComponent(new DitheringComponent());
            this.m_Fxaa = this.AddComponent(new FxaaComponent());
            this.m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();
            foreach (PostProcessingComponentBase component in this.m_Components)
            {
                this.m_ComponentStates.Add(component, value: false);
            }
            base.useGUILayout = false;
        }

        private void OnPreCull()
        {
            this.m_Camera = base.GetComponent<Camera>();
            if (this.profile == null || this.m_Camera == null)
            {
                return;
            }
            PostProcessingContext postProcessingContext = this.m_Context.Reset();
            postProcessingContext.profile = this.profile;
            postProcessingContext.renderTextureFactory = this.m_RenderTextureFactory;
            postProcessingContext.materialFactory = this.m_MaterialFactory;
            postProcessingContext.camera = this.m_Camera;
            this.m_DebugViews.Init(postProcessingContext, this.profile.debugViews);
            this.m_AmbientOcclusion.Init(postProcessingContext, this.profile.ambientOcclusion);
            this.m_ScreenSpaceReflection.Init(postProcessingContext, this.profile.screenSpaceReflection);
            this.m_FogComponent.Init(postProcessingContext, this.profile.fog);
            this.m_MotionBlur.Init(postProcessingContext, this.profile.motionBlur);
            this.m_Taa.Init(postProcessingContext, this.profile.antialiasing);
            this.m_EyeAdaptation.Init(postProcessingContext, this.profile.eyeAdaptation);
            this.m_DepthOfField.Init(postProcessingContext, this.profile.depthOfField);
            this.m_Bloom.Init(postProcessingContext, this.profile.bloom);
            this.m_ChromaticAberration.Init(postProcessingContext, this.profile.chromaticAberration);
            this.m_ColorGrading.Init(postProcessingContext, this.profile.colorGrading);
            this.m_UserLut.Init(postProcessingContext, this.profile.userLut);
            this.m_Grain.Init(postProcessingContext, this.profile.grain);
            this.m_Vignette.Init(postProcessingContext, this.profile.vignette);
            this.m_Dithering.Init(postProcessingContext, this.profile.dithering);
            this.m_Fxaa.Init(postProcessingContext, this.profile.antialiasing);
            if (this.m_PreviousProfile != this.profile)
            {
                this.DisableComponents();
                this.m_PreviousProfile = this.profile;
            }
            this.CheckObservers();
            DepthTextureMode depthTextureMode = postProcessingContext.camera.depthTextureMode;
            foreach (PostProcessingComponentBase component in this.m_Components)
            {
                if (component.active)
                {
                    depthTextureMode |= component.GetCameraFlags();
                }
            }
            postProcessingContext.camera.depthTextureMode = depthTextureMode;
            if (!this.m_RenderingInSceneView && this.m_Taa.active && !this.profile.debugViews.willInterrupt)
            {
                this.m_Taa.SetProjectionMatrix(this.jitteredMatrixFunc);
            }
        }

        private void OnPreRender()
        {
            if (!(this.profile == null))
            {
                this.TryExecuteCommandBuffer(this.m_DebugViews);
                this.TryExecuteCommandBuffer(this.m_AmbientOcclusion);
                this.TryExecuteCommandBuffer(this.m_ScreenSpaceReflection);
                this.TryExecuteCommandBuffer(this.m_FogComponent);
                if (!this.m_RenderingInSceneView)
                {
                    this.TryExecuteCommandBuffer(this.m_MotionBlur);
                }
            }
        }

        private void OnPostRender()
        {
            if (!(this.profile == null) && !(this.m_Camera == null) && !this.m_RenderingInSceneView && this.m_Taa.active && !this.profile.debugViews.willInterrupt)
            {
                this.m_Context.camera.ResetProjectionMatrix();
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.profile == null || this.m_Camera == null)
            {
                Graphics.Blit(source, destination);
                return;
            }
            bool flag = false;
            bool active = this.m_Fxaa.active;
            bool flag2 = this.m_Taa.active && !this.m_RenderingInSceneView;
            bool num = this.m_DepthOfField.active && !this.m_RenderingInSceneView;
            Material material = this.m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
            material.shaderKeywords = null;
            RenderTexture renderTexture = source;
            if (flag2)
            {
                RenderTexture renderTexture2 = this.m_RenderTextureFactory.Get(renderTexture);
                this.m_Taa.Render(renderTexture, renderTexture2);
                renderTexture = renderTexture2;
            }
            Texture texture = GraphicsUtils.whiteTexture;
            if (this.m_EyeAdaptation.active)
            {
                flag = true;
                texture = this.m_EyeAdaptation.Prepare(renderTexture, material);
            }
            material.SetTexture("_AutoExposure", texture);
            if (num)
            {
                flag = true;
                this.m_DepthOfField.Prepare(renderTexture, material, flag2, this.m_Taa.jitterVector, this.m_Taa.model.settings.taaSettings.motionBlending);
            }
            if (this.m_Bloom.active)
            {
                flag = true;
                this.m_Bloom.Prepare(renderTexture, material, texture);
            }
            flag |= this.TryPrepareUberImageEffect(this.m_ChromaticAberration, material);
            flag |= this.TryPrepareUberImageEffect(this.m_ColorGrading, material);
            flag |= this.TryPrepareUberImageEffect(this.m_Vignette, material);
            flag |= this.TryPrepareUberImageEffect(this.m_UserLut, material);
            Material material2 = (active ? this.m_MaterialFactory.Get("Hidden/Post FX/FXAA") : null);
            if (active)
            {
                material2.shaderKeywords = null;
                this.TryPrepareUberImageEffect(this.m_Grain, material2);
                this.TryPrepareUberImageEffect(this.m_Dithering, material2);
                if (flag)
                {
                    RenderTexture renderTexture3 = this.m_RenderTextureFactory.Get(renderTexture);
                    Graphics.Blit(renderTexture, renderTexture3, material, 0);
                    renderTexture = renderTexture3;
                }
                this.m_Fxaa.Render(renderTexture, destination);
            }
            else
            {
                flag |= this.TryPrepareUberImageEffect(this.m_Grain, material);
                flag |= this.TryPrepareUberImageEffect(this.m_Dithering, material);
                if (flag)
                {
                    if (!GraphicsUtils.isLinearColorSpace)
                    {
                        material.EnableKeyword("UNITY_COLORSPACE_GAMMA");
                    }
                    Graphics.Blit(renderTexture, destination, material, 0);
                }
            }
            if (!flag && !active)
            {
                Graphics.Blit(renderTexture, destination);
            }
            this.m_RenderTextureFactory.ReleaseAll();
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.Repaint && !(this.profile == null) && !(this.m_Camera == null))
            {
                if (this.m_EyeAdaptation.active && this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
                {
                    this.m_EyeAdaptation.OnGUI();
                }
                else if (this.m_ColorGrading.active && this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.LogLut))
                {
                    this.m_ColorGrading.OnGUI();
                }
                else if (this.m_UserLut.active && this.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.UserLut))
                {
                    this.m_UserLut.OnGUI();
                }
            }
        }

        private void OnDisable()
        {
            foreach (KeyValuePair<CameraEvent, CommandBuffer> value in this.m_CommandBuffers.Values)
            {
                this.m_Camera.RemoveCommandBuffer(value.Key, value.Value);
                value.Value.Dispose();
            }
            this.m_CommandBuffers.Clear();
            if (this.profile != null)
            {
                this.DisableComponents();
            }
            this.m_Components.Clear();
            this.m_MaterialFactory.Dispose();
            this.m_RenderTextureFactory.Dispose();
            GraphicsUtils.Dispose();
        }

        public void ResetTemporalEffects()
        {
            this.m_Taa.ResetHistory();
            this.m_MotionBlur.ResetHistory();
            this.m_EyeAdaptation.ResetHistory();
        }

        private void CheckObservers()
        {
            foreach (KeyValuePair<PostProcessingComponentBase, bool> componentState in this.m_ComponentStates)
            {
                PostProcessingComponentBase key = componentState.Key;
                bool flag = key.GetModel().enabled;
                if (flag != componentState.Value)
                {
                    if (flag)
                    {
                        this.m_ComponentsToEnable.Add(key);
                    }
                    else
                    {
                        this.m_ComponentsToDisable.Add(key);
                    }
                }
            }
            for (int i = 0; i < this.m_ComponentsToDisable.Count; i++)
            {
                PostProcessingComponentBase postProcessingComponentBase = this.m_ComponentsToDisable[i];
                this.m_ComponentStates[postProcessingComponentBase] = false;
                postProcessingComponentBase.OnDisable();
            }
            for (int j = 0; j < this.m_ComponentsToEnable.Count; j++)
            {
                PostProcessingComponentBase postProcessingComponentBase2 = this.m_ComponentsToEnable[j];
                this.m_ComponentStates[postProcessingComponentBase2] = true;
                postProcessingComponentBase2.OnEnable();
            }
            this.m_ComponentsToDisable.Clear();
            this.m_ComponentsToEnable.Clear();
        }

        private void DisableComponents()
        {
            foreach (PostProcessingComponentBase component in this.m_Components)
            {
                PostProcessingModel model = component.GetModel();
                if (model != null && model.enabled)
                {
                    component.OnDisable();
                }
            }
        }

        private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            CommandBuffer value = new CommandBuffer
            {
                name = name
            };
            KeyValuePair<CameraEvent, CommandBuffer> value2 = new KeyValuePair<CameraEvent, CommandBuffer>(evt, value);
            this.m_CommandBuffers.Add(typeof(T), value2);
            this.m_Camera.AddCommandBuffer(evt, value2.Value);
            return value2.Value;
        }

        private void RemoveCommandBuffer<T>() where T : PostProcessingModel
        {
            Type typeFromHandle = typeof(T);
            if (this.m_CommandBuffers.TryGetValue(typeFromHandle, out var value))
            {
                this.m_Camera.RemoveCommandBuffer(value.Key, value.Value);
                this.m_CommandBuffers.Remove(typeFromHandle);
                value.Value.Dispose();
            }
        }

        private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : PostProcessingModel
        {
            if (!this.m_CommandBuffers.TryGetValue(typeof(T), out var value))
            {
                return this.AddCommandBuffer<T>(evt, name);
            }
            if (value.Key != evt)
            {
                this.RemoveCommandBuffer<T>();
                return this.AddCommandBuffer<T>(evt, name);
            }
            return value.Value;
        }

        private void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component) where T : PostProcessingModel
        {
            if (component.active)
            {
                CommandBuffer commandBuffer = this.GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
                commandBuffer.Clear();
                component.PopulateCommandBuffer(commandBuffer);
            }
            else
            {
                this.RemoveCommandBuffer<T>();
            }
        }

        private bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material) where T : PostProcessingModel
        {
            if (!component.active)
            {
                return false;
            }
            component.Prepare(material);
            return true;
        }

        private T AddComponent<T>(T component) where T : PostProcessingComponentBase
        {
            this.m_Components.Add(component);
            return component;
        }
    }
}
