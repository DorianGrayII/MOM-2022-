namespace WorldCode
{
    using System;
    using UnityEngine;

    public class ChunkMesh : MonoBehaviour
    {
        private void OnDestroy()
        {
            MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
            if (component != null)
            {
                Destroy(component.material);
            }
            MeshFilter filter = base.gameObject.GetComponent<MeshFilter>();
            if ((filter != null) && (filter.mesh != null))
            {
                Destroy(filter.mesh);
            }
            MeshCollider collider = base.gameObject.GetComponent<MeshCollider>();
            if ((collider != null) && (collider.material != null))
            {
                Destroy(collider.material);
            }
            if ((collider != null) && (collider.sharedMesh != null))
            {
                Destroy(collider.sharedMesh);
            }
        }
    }
}

