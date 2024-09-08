namespace AllIn1SpriteShader
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class RandomSeed : MonoBehaviour
    {
        private void Start()
        {
            Renderer component = base.GetComponent<Renderer>();
            if (component != null)
            {
                if (component.material != null)
                {
                    component.material.SetFloat("_RandomSeed", UnityEngine.Random.Range((float) 0f, (float) 1000f));
                }
                else
                {
                    Debug.LogError("Missing Renderer or Material: " + base.gameObject.name);
                }
            }
            else
            {
                Image image = base.GetComponent<Image>();
                if (image == null)
                {
                    Debug.LogError("Missing Renderer or UI Image on: " + base.gameObject.name);
                }
                else if (image.material != null)
                {
                    image.material.SetFloat("_RandomSeed", UnityEngine.Random.Range((float) 0f, (float) 1000f));
                }
                else
                {
                    Debug.LogError("Missing Material on UI Image: " + base.gameObject.name);
                }
            }
        }
    }
}

