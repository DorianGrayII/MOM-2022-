namespace UnityEngine.UI.Extensions
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

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
        private UnityEngine.UI.Extensions.ResolutionMode m_improveResolution;
        [SerializeField]
        protected float m_Resolution;
        [SerializeField]
        private bool m_useNativeSize;

        protected UIPrimitiveBase()
        {
            base.useLegacyMeshGeneration = false;
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        protected virtual void GeneratedUVs()
        {
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int i = 0; i <= 1; i++)
            {
                float num2 = border[i] + border[i + 2];
                if ((rect.size[i] < num2) && (num2 != 0f))
                {
                    Vector2 size = rect.size;
                    float num3 = size[i] / num2;
                    ref Vector4 vectorRef = ref border;
                    int num4 = i;
                    vectorRef[num4] *= num3;
                    vectorRef = ref border;
                    num4 = i + 2;
                    vectorRef[num4] *= num3;
                }
            }
            return border;
        }

        protected List<Vector2> IncreaseResolution(List<Vector2> input)
        {
            float num2;
            this.outputList.Clear();
            UnityEngine.UI.Extensions.ResolutionMode improveResolution = this.ImproveResolution;
            if (improveResolution == UnityEngine.UI.Extensions.ResolutionMode.PerSegment)
            {
                int num9 = 0;
                while (num9 < (input.Count - 1))
                {
                    Vector2 item = input[num9];
                    this.outputList.Add(item);
                    Vector2 b = input[num9 + 1];
                    this.ResolutionToNativeSize(Vector2.Distance(item, b));
                    num2 = 1f / this.m_Resolution;
                    float num10 = 1f;
                    while (true)
                    {
                        if (num10 >= this.m_Resolution)
                        {
                            this.outputList.Add(b);
                            num9++;
                            break;
                        }
                        this.outputList.Add(Vector2.Lerp(item, b, num2 * num10));
                        num10++;
                    }
                }
            }
            else if (improveResolution == UnityEngine.UI.Extensions.ResolutionMode.PerLine)
            {
                float distance = 0f;
                num2 = 0f;
                int num4 = 0;
                while (true)
                {
                    if (num4 >= (input.Count - 1))
                    {
                        this.ResolutionToNativeSize(distance);
                        num2 = distance / this.m_Resolution;
                        int num3 = 0;
                        int num5 = 0;
                        while (num5 < (input.Count - 1))
                        {
                            Vector2 item = input[num5];
                            this.outputList.Add(item);
                            Vector2 b = input[num5 + 1];
                            float num6 = Vector2.Distance(item, b) / num2;
                            float num7 = 1f / num6;
                            int num8 = 0;
                            while (true)
                            {
                                if (num8 >= num6)
                                {
                                    this.outputList.Add(b);
                                    num5++;
                                    break;
                                }
                                this.outputList.Add(Vector2.Lerp(item, b, num8 * num7));
                                num3++;
                                num8++;
                            }
                        }
                        break;
                    }
                    distance += Vector2.Distance(input[num4], input[num4 + 1]);
                    num4++;
                }
            }
            return this.outputList;
        }

        protected Vector2[] IncreaseResolution(Vector2[] input)
        {
            return this.IncreaseResolution(new List<Vector2>(input)).ToArray();
        }

        public virtual unsafe bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 vector;
            if (this.m_EventAlphaThreshold >= 1f)
            {
                return true;
            }
            Sprite overrideSprite = this.overrideSprite;
            if (overrideSprite == null)
            {
                return true;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            float* singlePtr1 = &vector.x;
            singlePtr1[0] += base.rectTransform.pivot.x * pixelAdjustedRect.width;
            float* singlePtr2 = &vector.y;
            singlePtr2[0] += base.rectTransform.pivot.y * pixelAdjustedRect.height;
            vector = this.MapCoordinate(vector, pixelAdjustedRect);
            Rect textureRect = overrideSprite.textureRect;
            Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
            float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / ((float) overrideSprite.texture.width);
            float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / ((float) overrideSprite.texture.height);
            try
            {
                return (overrideSprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold);
            }
            catch (UnityException exception)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + exception.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect rect1 = this.sprite.rect;
            return new Vector2(local.x * rect.width, local.y * rect.height);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.SetAllDirty();
        }

        protected virtual void ResolutionToNativeSize(float distance)
        {
        }

        protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
        {
            UIVertex[] vertexArray = new UIVertex[4];
            for (int i = 0; i < vertices.Length; i++)
            {
                UIVertex simpleVert = UIVertex.simpleVert;
                simpleVert.color = this.color;
                simpleVert.position = (Vector3) vertices[i];
                simpleVert.uv0 = uvs[i];
                vertexArray[i] = simpleVert;
            }
            return vertexArray;
        }

        public Sprite sprite
        {
            get
            {
                return this.m_Sprite;
            }
            set
            {
                if (UnityEngine.UI.Extensions.SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
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
                if (UnityEngine.UI.Extensions.SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
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
                return ((this.m_OverrideSprite != null) ? this.m_OverrideSprite : this.sprite);
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

        public UnityEngine.UI.Extensions.ResolutionMode ImproveResolution
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
                if (s_ETC1DefaultUI == null)
                {
                    s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
                }
                return s_ETC1DefaultUI;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return ((this.activeSprite != null) ? this.activeSprite.texture : (((this.material == null) || (this.material.mainTexture == null)) ? Graphic.s_WhiteTexture : this.material.mainTexture));
            }
        }

        public bool hasBorder
        {
            get
            {
                return ((this.activeSprite != null) && (this.activeSprite.border.sqrMagnitude > 0f));
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float pixelsPerUnit = 100f;
                if (this.activeSprite)
                {
                    pixelsPerUnit = this.activeSprite.pixelsPerUnit;
                }
                float referencePixelsPerUnit = 100f;
                if (base.canvas)
                {
                    referencePixelsPerUnit = base.canvas.referencePixelsPerUnit;
                }
                return (pixelsPerUnit / referencePixelsPerUnit);
            }
        }

        public override Material material
        {
            get
            {
                return ((base.m_Material == null) ? ((!this.activeSprite || (this.activeSprite.associatedAlphaSplitTexture == null)) ? this.defaultMaterial : defaultETC1GraphicMaterial) : base.m_Material);
            }
            set
            {
                base.material = value;
            }
        }

        public virtual float minWidth
        {
            get
            {
                return 0f;
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                return ((this.overrideSprite != null) ? (this.overrideSprite.rect.size.x / this.pixelsPerUnit) : 0f);
            }
        }

        public virtual float flexibleWidth
        {
            get
            {
                return -1f;
            }
        }

        public virtual float minHeight
        {
            get
            {
                return 0f;
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                return ((this.overrideSprite != null) ? (this.overrideSprite.rect.size.y / this.pixelsPerUnit) : 0f);
            }
        }

        public virtual float flexibleHeight
        {
            get
            {
                return -1f;
            }
        }

        public virtual int layoutPriority
        {
            get
            {
                return 0;
            }
        }
    }
}

