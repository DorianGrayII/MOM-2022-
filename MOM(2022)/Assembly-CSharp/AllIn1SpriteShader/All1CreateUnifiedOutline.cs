namespace AllIn1SpriteShader
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class All1CreateUnifiedOutline : MonoBehaviour
    {
        [SerializeField]
        private Material outlineMaterial;
        [SerializeField]
        private Transform outlineParentTransform;
        [Space, Header("Only needed if Sprite (ignored if UI)"), SerializeField]
        private int duplicateOrderInLayer = -100;
        [SerializeField]
        private string duplicateSortingLayer = "Default";
        [Space, Header("This operation will delete the component"), SerializeField]
        private bool createUnifiedOutline;

        private void CreateOutlineSpriteDuplicate(GameObject target)
        {
            bool flag = false;
            SpriteRenderer component = target.GetComponent<SpriteRenderer>();
            Image image = target.GetComponent<Image>();
            if (component != null)
            {
                flag = false;
            }
            else if (image != null)
            {
                flag = true;
            }
            else if ((component == null) && ((image == null) && !base.transform.Equals(this.outlineParentTransform)))
            {
                return;
            }
            GameObject obj2 = new GameObject {
                name = target.name + "Outline",
                transform = { 
                    position = target.transform.position,
                    rotation = target.transform.rotation,
                    localScale = target.transform.lossyScale,
                    parent = (this.outlineParentTransform != null) ? this.outlineParentTransform : target.transform
                }
            };
            if (flag)
            {
                Image local2 = obj2.AddComponent<Image>();
                local2.sprite = image.sprite;
                local2.material = this.outlineMaterial;
            }
            else
            {
                SpriteRenderer local1 = obj2.AddComponent<SpriteRenderer>();
                local1.sprite = component.sprite;
                local1.sortingOrder = this.duplicateOrderInLayer;
                local1.sortingLayerName = this.duplicateSortingLayer;
                local1.material = this.outlineMaterial;
                local1.flipX = component.flipX;
                local1.flipY = component.flipY;
            }
        }

        private void GetAllChildren(Transform parent, ref List<Transform> transforms)
        {
            foreach (Transform transform in parent)
            {
                transforms.Add(transform);
                this.GetAllChildren(transform, ref transforms);
            }
        }

        private void MissingMaterial()
        {
        }

        private void Update()
        {
            if (this.createUnifiedOutline)
            {
                if (this.outlineMaterial == null)
                {
                    this.createUnifiedOutline = false;
                    this.MissingMaterial();
                }
                else
                {
                    List<Transform> transforms = new List<Transform>();
                    this.GetAllChildren(base.transform, ref transforms);
                    foreach (Transform transform in transforms)
                    {
                        this.CreateOutlineSpriteDuplicate(transform.gameObject);
                    }
                    this.CreateOutlineSpriteDuplicate(base.gameObject);
                    DestroyImmediate(this);
                }
            }
        }
    }
}

