using System;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{
    public Renderer targetRenderer;
    public Material[] materials;

    public void ChangeMaterial()
    {
        this.targetRenderer.sharedMaterial = this.materials[UnityEngine.Random.Range(0, this.materials.Length)];
    }

    public void Start()
    {
        this.ChangeMaterial();
    }
}

