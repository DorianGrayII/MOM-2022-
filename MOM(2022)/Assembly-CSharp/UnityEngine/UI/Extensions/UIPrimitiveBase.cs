using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
    {
        protected static Material s_ETC1DefaultUI;

        private List<Vector2> outputList = new List<Vector2>();

        [SerializeField]
        private Sprite m_Sprite;

        [NonSerialized]
        private Sprite m_OverrideSprite;

        internal float m_EventAlphaThreshold = 1f;

        [SerializeField]
        private ResolutionMode m_improveResolution;

        [SerializeField]
        protected float m_Resolution;

        [SerializeField]
        private bool m_useNativeSize;

        public Sprite sprite
        {
            get
            {
                return this.m_Sprite;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref this.m_Sprite, value))
                {
                    this.GeneratedUVs();
                }
                this.SetAllDirty();
            }
        }

        public Sprite overrideSprite
        {
            get
            {
                return this.activeSprite;
            }
            set
            {
                if (SetPropertyUtility.SetClass(ref this.m_OverrideSprite, value))
                {
                    this.GeneratedUVs();
                }
                this.SetAllDirty();
            }
        }

        protected Sprite activeSprite
        {
            get
            {
                if (!(this.m_OverrideSprite != null))
                {
                    return this.sprite;
                }
                return this.m_OverrideSprite;
            }
        }

        public float eventAlphaThreshold
        {
            get
            {
                return this.m_EventAlphaThreshold;
            }
            set
            {
                this.m_EventAlphaThreshold = value;
            }
        }

        public ResolutionMode ImproveResolution
        {
            get
            {
                return this.m_improveResolution;
            }
            set
            {
                this.m_improveResolution = value;
                this.SetAllDirty();
            }
        }

        public float Resolution
        {
            get
            {
                return this.m_Resolution;
            }
            set
            {
                this.m_Resolution = value;
                this.SetAllDirty();
            }
        }

        public bool UseNativeSize
        {
            get
            {
                return this.m_useNativeSize;
            }
            set
            {
                this.m_useNativeSize = value;
                this.SetAllDirty();
            }
        }

        public static Material defaultETC1GraphicMaterial
        {
            get
            {
                if (UIPrimitiveBase.s_ETC1DefaultUI == null)
                {
                    UIPrimitiveBase.s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
                }
                return UIPrimitiveBase.s_ETC1DefaultUI;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (this.activeSprite == null)
                {
                    if (this.material != null && this.material.mainTexture != null)
                    {
                        return this.material.mainTexture;
                    }
                    return Graphic.s_WhiteTexture;
                }
                return this.activeSprite.texture;
            }
        }

        public bool hasBorder
        {
            get
            {
                if (this.activeSprite != null)
                {
                    return this.activeSprite.border.sqrMagnitude > 0f;
                }
                return false;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float num = 100f;
                if ((bool)this.activeSprite)
                {
                    num = this.activeSprite.pixelsPerUnit;
                }
                float num2 = 100f;
                if ((bool)base.canvas)
                {
                    num2 = base.canvas.referencePixelsPerUnit;
                }
                return num / num2;
            }
        }

        public override Material material
        {
            get
            {
                if (base.m_Material != null)
                {
                    return base.m_Material;
                }
                if ((bool)this.activeSprite && this.activeSprite.associatedAlphaSplitTexture != null)
                {
                    return UIPrimitiveBase.defaultETC1GraphicMaterial;
                }
                return this.defaultMaterial;
            }
            set
            {
                base.material = value;
            }
        }

        public virtual float minWidth => 0f;

        public virtual float preferredWidth
        {
            get
            {
                if (this.overrideSprite == null)
                {
                    return 0f;
                }
                return this.overrideSprite.rect.size.x / this.pixelsPerUnit;
            }
        }

        public virtual float flexibleWidth => -1f;

        public virtual float minHeight => 0f;

        public virtual float preferredHeight
        {
            get
            {
                if (this.overrideSprite == null)
                {
                    return 0f;
                }
                return this.overrideSprite.rect.size.y / this.pixelsPerUnit;
            }
        }

        public virtual float flexibleHeight => -1f;

        public virtual int layoutPriority => 0;

        protected UIPrimitiveBase()
        {
            base.useLegacyMeshGeneration = false;
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] array = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                UIVertex simpleVert = UIVertex.simpleVert;
                simpleVert.color = this.color;
                simpleVert.position = vertices[i];
                simpleVert.uv0 = uvs[i];
                array[i] = simpleVert;
            }
            return array;
        }

        protected Vector2[] IncreaseResolution(Vector2[] input)
        {
            return this.IncreaseResolution(new List<Vector2>(input)).ToArray();
        }

        protected List<Vector2> IncreaseResolution(List<Vector2> input)
        {
            this.outputList.Clear();
            switch (this.ImproveResolution)
            {
            case ResolutionMode.PerLine:
            {
                float num3 = 0f;
                float num = 0f;
                for (int j = 0; j < input.Count - 1; j++)
                {
                    num3 += Vector2.Distance(input[j], input[j + 1]);
                }
                this.ResolutionToNativeSize(num3);
                num = num3 / this.m_Resolution;
                int num4 = 0;
                for (int k = 0; k < input.Count - 1; k++)
                {
                    Vector2 vector3 = input[k];
                    this.outputList.Add(vector3);
                    Vector2 vector4 = input[k + 1];
                    float num5 = Vector2.Distance(vector3, vector4) / num;
                    float num6 = 1f / num5;
                    for (int l = 0; (float)l < num5; l++)
                    {
                        this.outputList.Add(Vector2.Lerp(vector3, vector4, (float)l * num6));
                        num4++;
                    }
                    this.outputList.Add(vector4);
                }
                break;
            }
            case ResolutionMode.PerSegment:
            {
                for (int i = 0; i < input.Count - 1; i++)
                {
                    Vector2 vector = input[i];
                    this.outputList.Add(vector);
                    Vector2 vector2 = input[i + 1];
                    this.ResolutionToNativeSize(Vector2.Distance(vector, vector2));
                    float num = 1f / this.m_Resolution;
                    for (float num2 = 1f; num2 < this.m_Resolution; num2 += 1f)
                    {
                        this.outputList.Add(Vector2.Lerp(vector, vector2, num * num2));
                    }
                    this.outputList.Add(vector2);
                }
                break;
            }
            }
            return this.outputList;
        }

        protected virtual void GeneratedUVs()
        {
        }

        protected virtual void ResolutionToNativeSize(float distance)
        {
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (this.m_EventAlphaThreshold >= 1f)
            {
                return true;
            }
            Sprite sprite = this.overrideSprite;
            if (sprite == null)
            {
                return true;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out var localPoint);
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            localPoint.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
            localPoint.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
            localPoint = this.MapCoordinate(localPoint, pixelAdjustedRect);
            Rect textureRect = sprite.textureRect;
            Vector2 vector = new Vector2(localPoint.x / textureRect.width, localPoint.y / textureRect.height);
            float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector.x) / (float)sprite.texture.width;
            float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector.y) / (float)sprite.texture.height;
            try
            {
                return sprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold;
            }
            catch (UnityException ex)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            _ = this.sprite.rect;
            return new Vector2(local.x * rect.width, local.y * rect.height);
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int i = 0; i <= 1; i++)
            {
                float num = border[i] + border[i + 2];
                if (rect.size[i] < num && num != 0f)
                {
                    float num2 = rect.size[i] / num;
                    border[i] *= num2;
                    border[i + 2] *= num2;
                }
            }
            return border;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetAllDirty();
        }
    }
}
