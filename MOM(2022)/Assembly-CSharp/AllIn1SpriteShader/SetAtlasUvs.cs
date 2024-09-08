namespace AllIn1SpriteShader
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class SetAtlasUvs : MonoBehaviour
    {
        [SerializeField]
        private bool updateEveryFrame;
        private Renderer render;
        private SpriteRenderer spriteRender;
        private Image uiImage;
        private bool isUI;

        public unsafe void GetAndSetUVs()
        {
            if (this.GetRendererReferencesIfNeeded())
            {
                if (!this.isUI)
                {
                    Rect textureRect = this.spriteRender.sprite.textureRect;
                    Rect* rectPtr1 = &textureRect;
                    rectPtr1.x /= (float) this.spriteRender.sprite.texture.width;
                    Rect* rectPtr2 = &textureRect;
                    rectPtr2.width /= (float) this.spriteRender.sprite.texture.width;
                    Rect* rectPtr3 = &textureRect;
                    rectPtr3.y /= (float) this.spriteRender.sprite.texture.height;
                    Rect* rectPtr4 = &textureRect;
                    rectPtr4.height /= (float) this.spriteRender.sprite.texture.height;
                    this.render.sharedMaterial.SetFloat("_MinXUV", textureRect.xMin);
                    this.render.sharedMaterial.SetFloat("_MaxXUV", textureRect.xMax);
                    this.render.sharedMaterial.SetFloat("_MinYUV", textureRect.yMin);
                    this.render.sharedMaterial.SetFloat("_MaxYUV", textureRect.yMax);
                }
                else
                {
                    Rect textureRect = this.uiImage.sprite.textureRect;
                    Rect* rectPtr5 = &textureRect;
                    rectPtr5.x /= (float) this.uiImage.sprite.texture.width;
                    Rect* rectPtr6 = &textureRect;
                    rectPtr6.width /= (float) this.uiImage.sprite.texture.width;
                    Rect* rectPtr7 = &textureRect;
                    rectPtr7.y /= (float) this.uiImage.sprite.texture.height;
                    Rect* rectPtr8 = &textureRect;
                    rectPtr8.height /= (float) this.uiImage.sprite.texture.height;
                    this.uiImage.material.SetFloat("_MinXUV", textureRect.xMin);
                    this.uiImage.material.SetFloat("_MaxXUV", textureRect.xMax);
                    this.uiImage.material.SetFloat("_MinYUV", textureRect.yMin);
                    this.uiImage.material.SetFloat("_MaxYUV", textureRect.yMax);
                }
            }
        }

        private bool GetRendererReferencesIfNeeded()
        {
            if (this.spriteRender == null)
            {
                this.spriteRender = base.GetComponent<SpriteRenderer>();
            }
            if (this.spriteRender != null)
            {
                if (this.spriteRender.sprite == null)
                {
                    DestroyImmediate(this);
                    return false;
                }
                if (this.render == null)
                {
                    this.render = base.GetComponent<Renderer>();
                }
                this.isUI = false;
            }
            else
            {
                if (this.uiImage == null)
                {
                    this.uiImage = base.GetComponent<Image>();
                    if (this.uiImage == null)
                    {
                        DestroyImmediate(this);
                        return false;
                    }
                }
                if (this.render == null)
                {
                    this.render = base.GetComponent<Renderer>();
                }
                this.isUI = true;
            }
            if ((this.spriteRender != null) || (this.uiImage != null))
            {
                return true;
            }
            DestroyImmediate(this);
            return false;
        }

        private void OnWillRenderObject()
        {
            if (this.updateEveryFrame)
            {
                this.GetAndSetUVs();
            }
        }

        private void Reset()
        {
            this.Setup();
        }

        public void ResetAtlasUvs()
        {
            if (this.GetRendererReferencesIfNeeded())
            {
                if (!this.isUI)
                {
                    this.render.sharedMaterial.SetFloat("_MinXUV", 0f);
                    this.render.sharedMaterial.SetFloat("_MaxXUV", 1f);
                    this.render.sharedMaterial.SetFloat("_MinYUV", 0f);
                    this.render.sharedMaterial.SetFloat("_MaxYUV", 1f);
                }
                else
                {
                    this.uiImage.material.SetFloat("_MinXUV", 0f);
                    this.uiImage.material.SetFloat("_MaxXUV", 1f);
                    this.uiImage.material.SetFloat("_MinYUV", 0f);
                    this.uiImage.material.SetFloat("_MaxYUV", 1f);
                }
            }
        }

        private void Setup()
        {
            if (this.GetRendererReferencesIfNeeded())
            {
                this.GetAndSetUVs();
            }
            if (!this.updateEveryFrame && (Application.isPlaying && (this != null)))
            {
                base.enabled = false;
            }
        }

        private void Start()
        {
            this.Setup();
        }

        public void UpdateEveryFrame(bool everyFrame)
        {
            this.updateEveryFrame = everyFrame;
        }
    }
}

