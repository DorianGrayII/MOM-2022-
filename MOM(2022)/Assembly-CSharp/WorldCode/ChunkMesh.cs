using UnityEngine;

namespace WorldCode
{
    public class ChunkMesh : MonoBehaviour
    {
        private void OnDestroy()
        {
            MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
            if (component != null)
            {
                Object.Destroy(component.material);
            }
            MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
            if (component2 != null && component2.mesh != null)
            {
                Object.Destroy(component2.mesh);
            }
            MeshCollider component3 = base.gameObject.GetComponent<MeshCollider>();
            if (component3 != null && component3.material != null)
            {
                Object.Destroy(component3.material);
            }
            if (component3 != null && component3.sharedMesh != null)
            {
                Object.Destroy(component3.sharedMesh);
            }
        }
    }
}
