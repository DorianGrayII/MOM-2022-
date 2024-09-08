using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 2f;

    private void Start()
    {
        Object.Destroy(base.gameObject, this.time);
    }
}
