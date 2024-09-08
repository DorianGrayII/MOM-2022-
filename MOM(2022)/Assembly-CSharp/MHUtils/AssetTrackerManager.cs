namespace MHUtils
{
    using System;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Profiling;

    public class AssetTrackerManager
    {
        private AssetTracker first;
        private AssetTracker last;
        private int totalItemCount;

        public void FreeAssets()
        {
            if ((this.first != null) && ((Time.time - this.first.lastUseTime) >= 20f))
            {
                long totalReservedMemoryLong = Profiler.GetTotalReservedMemoryLong();
                long totalUnusedReservedMemoryLong = Profiler.GetTotalUnusedReservedMemoryLong();
                int num3 = 0x40000000;
                string[] textArray1 = new string[] { "Reserved:", (((double) totalReservedMemoryLong) / ((double) num3)).ToString("N3"), "(free:", (((double) totalUnusedReservedMemoryLong) / ((double) num3)).ToString("N3"), ") item count ", this.totalItemCount.ToString() };
                Debug.Log(string.Concat(textArray1));
                if ((totalReservedMemoryLong > (7 * num3)) && (totalUnusedReservedMemoryLong < num3))
                {
                    float num5 = Time.time - 30f;
                    int num6 = this.totalItemCount / 2;
                    while ((this.first != null) && ((this.totalItemCount > 100) && ((this.totalItemCount > num6) && ((this.first.lastUseTime < num5) && this.TryToFree(this.first)))))
                    {
                    }
                }
            }
        }

        public object GetAsset(AssetTracker at)
        {
            at.lastUseTime = Time.time;
            if (this.first == null)
                this.first = at;

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
            if ((this.last != null) && !ReferenceEquals(this.last, at))
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
                else if (!at.assetPath.EndsWith(".jpg") && !at.assetPath.EndsWith(".png"))
                {
                    if (at.assetPath.EndsWith(".wav") || at.assetPath.EndsWith(".ogg"))
                    {
                        Debug.LogWarning("Do not attempt to access audio files this way. Use Audio Library instead.");
                        return null;
                    }
                }
                else
                {
                    Texture2D tex = new Texture2D(2, 2);
                    at.instance = tex;
                    try
                    {
                        ImageConversion.LoadImage(tex, File.ReadAllBytes(at.assetPath));
                    }
                    catch (FileNotFoundException exception)
                    {
                        string text1;
                        string text2;
                        string[] textArray1 = new string[8];
                        textArray1[0] = "File not found: ";
                        textArray1[1] = at.assetOriginalName;
                        textArray1[2] = ", path: ";
                        textArray1[3] = at.assetPath;
                        textArray1[4] = ", assetBundle: ";
                        string[] textArray4 = textArray1;
                        if (at.assetSource != null)
                        {
                            text1 = at.assetSource.ToString();
                        }
                        else
                        {
                            AssetBundle assetSource = at.assetSource;
                            text1 = null;
                        }
                        textArray1[5] = text1;
                        string[] local2 = textArray1;
                        local2[6] = "\n";
                        string[] textArray5 = local2;
                        if (exception != null)
                        {
                            text2 = exception.ToString();
                        }
                        else
                        {
                            FileNotFoundException local3 = exception;
                            text2 = null;
                        }
                        local2[7] = text2;
                        Debug.LogWarning(string.Concat(local2));
                    }
                    catch (IOException exception2)
                    {
                        string text3;
                        string text4;
                        string[] textArray2 = new string[8];
                        textArray2[0] = "IO error: ";
                        textArray2[1] = at.assetOriginalName;
                        textArray2[2] = ", path: ";
                        textArray2[3] = at.assetPath;
                        textArray2[4] = ", assetBundle: ";
                        string[] textArray6 = textArray2;
                        if (at.assetSource != null)
                        {
                            text3 = at.assetSource.ToString();
                        }
                        else
                        {
                            AssetBundle assetSource = at.assetSource;
                            text3 = null;
                        }
                        textArray2[5] = text3;
                        string[] local5 = textArray2;
                        local5[6] = "\n";
                        string[] textArray7 = local5;
                        if (exception2 != null)
                        {
                            text4 = exception2.ToString();
                        }
                        else
                        {
                            IOException local6 = exception2;
                            text4 = null;
                        }
                        local5[7] = text4;
                        Debug.LogWarning(string.Concat(local5));
                    }
                    catch (Exception exception3)
                    {
                        string text5;
                        string text6;
                        string[] textArray3 = new string[8];
                        textArray3[0] = "Unexpected error: ";
                        textArray3[1] = at.assetOriginalName;
                        textArray3[2] = ", path: ";
                        textArray3[3] = at.assetPath;
                        textArray3[4] = ", assetBundle: ";
                        string[] textArray8 = textArray3;
                        if (at.assetSource != null)
                        {
                            text5 = at.assetSource.ToString();
                        }
                        else
                        {
                            AssetBundle assetSource = at.assetSource;
                            text5 = null;
                        }
                        textArray3[5] = text5;
                        string[] local8 = textArray3;
                        local8[6] = "\n";
                        string[] textArray9 = local8;
                        if (exception3 != null)
                        {
                            text6 = exception3.ToString();
                        }
                        else
                        {
                            Exception local9 = exception3;
                            text6 = null;
                        }
                        local8[7] = text6;
                        Debug.LogWarning(string.Concat(local8));
                    }
                }
                at.loaded = true;
                this.totalItemCount++;
            }
            return at.instance;
        }

        private bool TryToFree(AssetTracker asset)
        {
            if (this.first == null)
            {
                return false;
            }
            bool flag = false;
            if (ReferenceEquals(asset, this.first))
            {
                if (asset.nextAsset == null)
                {
                    this.first = null;
                }
                else
                {
                    this.first = asset.nextAsset;
                    asset.nextAsset = null;
                }
                flag = true;
            }
            if (ReferenceEquals(asset, this.last))
            {
                if (asset.prevAsset == null)
                {
                    this.last = null;
                }
                else
                {
                    this.last = asset.prevAsset;
                    asset.prevAsset = null;
                }
                flag = true;
            }
            if ((asset.nextAsset != null) && (asset.prevAsset != null))
            {
                asset.nextAsset.prevAsset = asset.prevAsset;
                asset.prevAsset.nextAsset = asset.nextAsset;
                asset.nextAsset = null;
                asset.prevAsset = null;
                flag = true;
            }
            if (!flag)
            {
                return false;
            }
            this.totalItemCount--;
            asset.loaded = false;
            if ((asset.assetSource == null) && (asset.instance != null))
            {
                UnityEngine.Object.Destroy(asset.instance);
            }
            asset.instance = null;
            return true;
        }
    }
}

