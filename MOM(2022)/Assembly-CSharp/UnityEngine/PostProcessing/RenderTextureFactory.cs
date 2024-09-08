namespace UnityEngine.PostProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public sealed class RenderTextureFactory : IDisposable
    {
        private HashSet<RenderTexture> m_TemporaryRTs = new HashSet<RenderTexture>();

        public void Dispose()
        {
            this.ReleaseAll();
        }

        public RenderTexture Get(RenderTexture baseRenderTexture)
        {
            return this.Get(baseRenderTexture.width, baseRenderTexture.height, baseRenderTexture.depth, baseRenderTexture.format, baseRenderTexture.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear, baseRenderTexture.filterMode, baseRenderTexture.wrapMode, "FactoryTempTexture");
        }

        public RenderTexture Get(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite rw, FilterMode filterMode, TextureWrapMode wrapMode, string name)
        {
            RenderTexture item = RenderTexture.GetTemporary(width, height, depthBuffer, format, rw);
            item.filterMode = filterMode;
            item.wrapMode = wrapMode;
            item.name = name;
            this.m_TemporaryRTs.Add(item);
            return item;
        }

        public void Release(RenderTexture rt)
        {
            if (rt != null)
            {
                if (!this.m_TemporaryRTs.Contains(rt))
                {
                    throw new ArgumentException(string.Format("Attempting to remove a RenderTexture that was not allocated: {0}", rt));
                }
                this.m_TemporaryRTs.Remove(rt);
                RenderTexture.ReleaseTemporary(rt);
            }
        }

        public void ReleaseAll()
        {
            HashSet<RenderTexture>.Enumerator enumerator = this.m_TemporaryRTs.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RenderTexture.ReleaseTemporary(enumerator.Current);
            }
            this.m_TemporaryRTs.Clear();
        }
    }
}

