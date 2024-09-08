namespace UnityEngine.PostProcessing
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class MaterialFactory : IDisposable
    {
        private Dictionary<string, Material> m_Materials = new Dictionary<string, Material>();

        public void Dispose()
        {
            foreach (KeyValuePair<string, Material> pair in this.m_Materials)
            {
                GraphicsUtils.Destroy(pair.Value);
            }
            this.m_Materials.Clear();
        }

        public Material Get(string shaderName)
        {
            Material material;
            if (!this.m_Materials.TryGetValue(shaderName, out material))
            {
                Shader shader1 = Shader.Find(shaderName);
                if (Shader.Find(shaderName) == null)
                {
                    throw new ArgumentException(string.Format("Shader not found ({0})", shaderName));
                }
                Material material1 = new Material(Shader.Find(shaderName));
                material1.name = string.Format("PostFX - {0}", shaderName.Substring(shaderName.LastIndexOf("/") + 1));
                material1.hideFlags = HideFlags.DontSave;
                material = material1;
                this.m_Materials.Add(shaderName, material);
            }
            return material;
        }
    }
}

