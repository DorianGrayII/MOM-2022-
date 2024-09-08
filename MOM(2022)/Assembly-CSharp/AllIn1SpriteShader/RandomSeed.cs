using UnityEngine;
using UnityEngine.UI;

namespace AllIn1SpriteShader
{
    public class RandomSeed : MonoBehaviour
    {
        private void Start()
        {
            Renderer component = base.GetComponent<Renderer>();
            if (component != null)
            {
                if (component.material != null)
                {
                    component.material.SetFloat("_RandomSeed", Random.Range(0f, 1000f));
                }
                else
                {
                    Debug.LogError("Missing Renderer or Material: " + base.gameObject.name);
                }
                return;
            }
            Image component2 = base.GetComponent<Image>();
            if (component2 != null)
            {
                if (component2.material != null)
                {
                    component2.material.SetFloat("_RandomSeed", Random.Range(0f, 1000f));
                }
                else
                {
                    Debug.LogError("Missing Material on UI Image: " + base.gameObject.name);
                }
            }
            else
            {
                Debug.LogError("Missing Renderer or UI Image on: " + base.gameObject.name);
            }
        }
    }
}
