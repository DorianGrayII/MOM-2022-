using UnityEngine;
using UnityEngine.UI;

namespace AllIn1SpriteShader
{
    [ExecuteInEditMode]
    public class SetAtlasUvs : MonoBehaviour
    {
        [SerializeField]
        private bool updateEveryFrame;

        private Renderer render;

        private SpriteRenderer spriteRender;

        private Image uiImage;

        private bool isUI;

        private void Start()
        {
            this.Setup();
        }

        private void Reset()
        {
            this.Setup();
        }

        private void Setup()
        {
            if (this.GetRendererReferencesIfNeeded())
            {
                this.GetAndSetUVs();
            }
            if (!this.updateEveryFrame && Application.isPlaying && this != null)
            {
                base.enabled = false;
            }
        }

        private void OnWillRenderObject()
        {
            if (this.updateEveryFrame)
            {
                this.GetAndSetUVs();
            }
        }

        public void GetAndSetUVs()
        {
            if (this.GetRendererReferencesIfNeeded())
            {
                if (!this.isUI)
                {
                    Rect textureRect = this.spriteRender.sprite.textureRect;
                    textureRect.x /= this.spriteRender.sprite.texture.width;
                    textureRect.width /= this.spriteRender.sprite.texture.width;
                    textureRect.y /= this.spriteRender.sprite.texture.height;
                    textureRect.height /= this.spriteRender.sprite.texture.height;
                    this.render.sharedMaterial.SetFloat("_MinXUV", textureRect.xMin);
                    this.render.sharedMaterial.SetFloat("_MaxXUV", textureRect.xMax);
                    this.render.sharedMaterial.SetFloat("_MinYUV", textureRect.yMin);
                    this.render.sharedMaterial.SetFloat("_MaxYUV", textureRect.yMax);
                }
                else
                {
                    Rect textureRect2 = this.uiImage.sprite.textureRect;
                    textureRect2.x /= this.uiImage.sprite.texture.width;
                    textureRect2.width /= this.uiImage.sprite.texture.width;
                    textureRect2.y /= this.uiImage.sprite.texture.height;
                    textureRect2.height /= this.uiImage.sprite.texture.height;
                    this.uiImage.material.SetFloat("_MinXUV", textureRect2.xMin);
                    this.uiImage.material.SetFloat("_MaxXUV", textureRect2.xMax);
                    this.uiImage.material.SetFloat("_MinYUV", textureRect2.yMin);
                    this.uiImage.material.SetFloat("_MaxYUV", textureRect2.yMax);
                }
            }
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

        public void UpdateEveryFrame(bool everyFrame)
        {
            this.updateEveryFrame = everyFrame;
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
                    Object.DestroyImmediate(this);
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
                    if (!(this.uiImage != null))
                    {
                        Object.DestroyImmediate(this);
                        return false;
                    }
                }
                if (this.render == null)
                {
                    this.render = base.GetComponent<Renderer>();
                }
                this.isUI = true;
            }
            if (this.spriteRender == null && this.uiImage == null)
            {
                Object.DestroyImmediate(this);
                return false;
            }
            return true;
        }
    }
}
