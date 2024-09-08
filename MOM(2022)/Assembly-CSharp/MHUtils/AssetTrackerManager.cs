using System;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

namespace MHUtils
{
    public class AssetTrackerManager
    {
        private AssetTracker first;

        private AssetTracker last;

        private int totalItemCount;

        public object GetAsset(AssetTracker at)
        {
            at.lastUseTime = Time.time;
            if (this.first == null)
            {
                this.first = at;
            }
            if (at.nextAsset != null)
            {
                at.nextAsset.prevAsset = at.prevAsset;
            }
            if (at.prevAsset != null)
            {
                at.prevAsset.nextAsset = at.nextAsset;
            }
            at.nextAsset = null;
            at.prevAsset = null;
            if (this.last != null && this.last != at)
            {
                this.last.nextAsset = at;
                at.prevAsset = this.last.prevAsset;
            }
            this.last = at;
            if (!at.loaded)
            {
                if (at.assetSource != null)
                {
                    at.instance = at.assetSource.LoadAsset(at.assetOriginalName);
                }
                else if (at.assetPath.EndsWith(".jpg") || at.assetPath.EndsWith(".png"))
                {
                    Texture2D tex = (Texture2D)(at.instance = new Texture2D(2, 2));
                    try
                    {
                        byte[] data = File.ReadAllBytes(at.assetPath);
                        tex.LoadImage(data);
                    }
                    catch (FileNotFoundException ex)
                    {
                        Debug.LogWarning("File not found: " + at.assetOriginalName + ", path: " + at.assetPath + ", assetBundle: " + at.assetSource?.ToString() + "\n" + ex);
                    }
                    catch (IOException ex2)
                    {
                        Debug.LogWarning("IO error: " + at.assetOriginalName + ", path: " + at.assetPath + ", assetBundle: " + at.assetSource?.ToString() + "\n" + ex2);
                    }
                    catch (Exception ex3)
                    {
                        Debug.LogWarning("Unexpected error: " + at.assetOriginalName + ", path: " + at.assetPath + ", assetBundle: " + at.assetSource?.ToString() + "\n" + ex3);
                    }
                }
                else if (at.assetPath.EndsWith(".wav") || at.assetPath.EndsWith(".ogg"))
                {
                    Debug.LogWarning("Do not attempt to access audio files this way. Use Audio Library instead.");
                    return null;
                }
                at.loaded = true;
                this.totalItemCount++;
            }
            return at.instance;
        }

        public void FreeAssets()
        {
            if (this.first == null || Time.time - this.first.lastUseTime < 20f)
            {
                return;
            }
            long totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
            long totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
            int num = 1073741824;
            Debug.Log("Reserved:" + ((double)totalReservedMemoryLong / (double)num).ToString("N3") + "(free:" + ((double)totalUnusedReservedMemoryLong / (double)num).ToString("N3") + ") item count " + this.totalItemCount);
            if (totalReservedMemoryLong > 7 * num && totalUnusedReservedMemoryLong < num)
            {
                float num2 = Time.time - 30f;
                int num3 = this.totalItemCount / 2;
                while (this.first != null && this.totalItemCount > 100 && this.totalItemCount > num3 && this.first.lastUseTime < num2 && this.TryToFree(this.first))
                {
                }
            }
        }

        private bool TryToFree(AssetTracker asset)
        {
            if (this.first == null)
            {
                return false;
            }
            bool flag = false;
            if (asset == this.first)
            {
                if (asset.nextAsset != null)
                {
                    this.first = asset.nextAsset;
                    asset.nextAsset = null;
                }
                else
                {
                    this.first = null;
                }
                flag = true;
            }
            if (asset == this.last)
            {
                if (asset.prevAsset != null)
                {
                    this.last = asset.prevAsset;
                    asset.prevAsset = null;
                }
                else
                {
                    this.last = null;
                }
                flag = true;
            }
            if (asset.nextAsset != null && asset.prevAsset != null)
            {
                asset.nextAsset.prevAsset = asset.prevAsset;
                asset.prevAsset.nextAsset = asset.nextAsset;
                asset.nextAsset = null;
                asset.prevAsset = null;
                flag = true;
            }
            if (flag)
            {
                this.totalItemCount--;
                asset.loaded = false;
                if (asset.assetSource == null && asset.instance != null)
                {
                    global::UnityEngine.Object.Destroy(asset.instance);
                }
                asset.instance = null;
                return true;
            }
            return false;
        }
    }
}
