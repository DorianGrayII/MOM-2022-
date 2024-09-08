namespace UnityEngine.UI.Extensions
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode, AddComponentMenu("UI/Effects/Extensions/SoftMaskScript")]
    public class SoftMaskScript : MonoBehaviour
    {
        public Material mat;
        private Canvas canvas;
        [Tooltip("The area that is to be used as the container.")]
        public RectTransform MaskArea;
        private RectTransform myRect;
        [Tooltip("A Rect Transform that can be used to scale and move the mask - Does not apply to Text UI Components being masked")]
        public RectTransform maskScalingRect;
        [Tooltip("Texture to be used to do the soft alpha")]
        public Texture AlphaMask;
        [Tooltip("At what point to apply the alpha min range 0-1"), Range(0f, 1f)]
        public float CutOff;
        [Tooltip("Implement a hard blend based on the Cutoff")]
        public bool HardBlend;
        [Tooltip("Flip the masks alpha value")]
        public bool FlipAlphaMask;
        private Vector3[] worldCorners;
        private Vector2 AlphaUV;
        private Vector2 min;
        private Vector2 max = Vector2.one;
        private Vector2 p;
        private Vector2 siz;
        private Rect maskRect;
        private Rect contentRect;
        private Vector2 centre;
        private bool isText;

        private void GetCanvas()
        {
            Transform t = base.transform;
            int num = 100;
            for (int i = 0; (this.canvas == null) && (i < num); i++)
            {
                this.canvas = t.gameObject.GetComponent<Canvas>();
                if (this.canvas == null)
                {
                    t = this.GetParentTranform(t);
                }
            }
        }

        private Transform GetParentTranform(Transform t)
        {
            return t.parent;
        }

        private void SetMask()
        {
            this.maskRect = this.MaskArea.rect;
            this.contentRect = this.myRect.rect;
            if (this.isText)
            {
                this.maskScalingRect = null;
                if ((this.canvas.renderMode == RenderMode.ScreenSpaceOverlay) && Application.isPlaying)
                {
                    this.p = this.canvas.transform.InverseTransformPoint(this.MaskArea.transform.position);
                    this.siz = new Vector2(this.maskRect.width, this.maskRect.height);
                }
                else
                {
                    this.worldCorners = new Vector3[4];
                    this.MaskArea.GetWorldCorners(this.worldCorners);
                    this.siz = this.worldCorners[2] - this.worldCorners[0];
                    this.p = this.MaskArea.transform.position;
                }
                this.min = this.p - (new Vector2(this.siz.x, this.siz.y) * 0.5f);
                this.max = this.p + (new Vector2(this.siz.x, this.siz.y) * 0.5f);
            }
            else
            {
                if (this.maskScalingRect != null)
                {
                    this.maskRect = this.maskScalingRect.rect;
                }
                this.centre = this.myRect.transform.InverseTransformPoint(this.MaskArea.transform.position);
                if (this.maskScalingRect != null)
                {
                    this.centre = this.myRect.transform.InverseTransformPoint(this.maskScalingRect.transform.position);
                }
                this.AlphaUV = new Vector2(this.maskRect.width / this.contentRect.width, this.maskRect.height / this.contentRect.height);
                this.min = this.centre;
                this.max = this.min;
                this.siz = new Vector2(this.maskRect.width, this.maskRect.height) * 0.5f;
                this.min -= this.siz;
                this.max += this.siz;
                this.min = new Vector2(this.min.x / this.contentRect.width, this.min.y / this.contentRect.height) + new Vector2(0.5f, 0.5f);
                this.max = new Vector2(this.max.x / this.contentRect.width, this.max.y / this.contentRect.height) + new Vector2(0.5f, 0.5f);
            }
            this.mat.SetFloat("_HardBlend", this.HardBlend ? ((float) 1) : ((float) 0));
            this.mat.SetVector("_Min", this.min);
            this.mat.SetVector("_Max", this.max);
            this.mat.SetTexture("_AlphaMask", this.AlphaMask);
            this.mat.SetInt("_FlipAlphaMask", this.FlipAlphaMask ? 1 : 0);
            if (!this.isText)
            {
                this.mat.SetVector("_AlphaUV", this.AlphaUV);
            }
            this.mat.SetFloat("_CutOff", this.CutOff);
        }

        private void Start()
        {
            this.myRect = base.GetComponent<RectTransform>();
            if (!this.MaskArea)
            {
                this.MaskArea = this.myRect;
            }
            if (base.GetComponent<Graphic>() != null)
            {
                base.GetComponent<Graphic>().material = this.mat;
            }
            if (base.GetComponent<Text>())
            {
                this.isText = true;
                base.GetComponent<Text>().material = this.mat;
                this.GetCanvas();
                if (base.transform.parent.GetComponent<Mask>() == null)
                {
                    base.transform.parent.gameObject.AddComponent<Mask>();
                }
                base.transform.parent.GetComponent<Mask>().enabled = false;
            }
        }

        private void Update()
        {
            this.SetMask();
        }
    }
}

