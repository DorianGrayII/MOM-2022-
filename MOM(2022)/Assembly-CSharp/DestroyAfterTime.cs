using System;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float time = 2f;

    private void Start()
    {
        Destroy(base.gameObject, this.time);
    }
}

