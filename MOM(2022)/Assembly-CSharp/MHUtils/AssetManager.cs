using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBDef;
using UnityEngine;

namespace MHUtils
{
    public class AssetManager : MonoBehaviour
    {
        private static AssetManager instance;

        private HashSet<AssetBundle> bundles = new HashSet<AssetBundle>();

        private Dictionary<string, AssetTracker> managedAssets = new Dictionary<string, AssetTracker>();

        private AssetTrackerManager assetTrackerManager = new AssetTrackerManager();

        public Material foregroundMaterial;

        public Material arcanusTerrainMaterial;

        public Material myrrorTerrainMaterial;

        public Material arcanusFOWMaterial;

        public Material myrrorFOWMaterial;

        private void Start()
        {
            AssetManager.instance = this;
        }

        public static AssetManager Get()
        {
            if (AssetManager.instance == null)
            {
                Debug.LogError("Asset manager no-existing, is it runtime?");
            }
            return AssetManager.instance;
        }

        private void OnDestroy()
        {
            this.managedAssets?.Clear();
            this.assetTrackerManager = null;
        }

        public void Initialize()
        {
            List<string> list = new List<string>(Directory.GetFiles(Path.Combine(Application.streamingAssetsPath, "AssetBundles")));
            list.AddRange(this.LoadModAssetBundles());
            HashSet<string> hashSet = new HashSet<string>();
            foreach (string item in list)
            {
                string text = Path.GetFileNameWithoutExtension(item);

                string[] array = text.Split('.'); //, StringSplitOptions.None);
                if (array.Length > 1)
                {
                    text = array[0];
                }
                if (hashSet.Contains(text) || !text.StartsWith("ab_"))
                {
                    continue;
                }
                hashSet.Add(text);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(item);
                this.bundles.Add(assetBundle);
                bool flag = assetBundle.name.StartsWith("ab_3d");
                new StringBuilder().AppendLine("Assets in " + text);
                string[] allAssetNames = assetBundle.GetAllAssetNames();
                foreach (string text2 in allAssetNames)
                {
                    int num = text2.LastIndexOf('/') + 1;
                    int num2 = text2.LastIndexOf('.');
                    if (num2 <= num)
                    {
                        Debug.LogError("[ERROR]Name is incorrect: " + text2);
                    }
                    string text3 = text2.Substring(num, num2 - num).ToLowerInvariant();
                    string orgName = text3;
                    if (flag && !text3.EndsWith("_3d"))
                    {
                        text3 += "_3d";
                    }
                    AssetTracker value = new AssetTracker(text3, orgName, assetBundle, null);
                    this.managedAssets[text3] = value;
                }
            }
            int num3 = 1;
            bool flag2 = false;
            if (PlayerPrefs.HasKey("UseDLC"))
            {
                num3 = PlayerPrefs.GetInt("UseDLC");
                flag2 = true;
            }
            foreach (DLCManager.DLCs value2 in Enum.GetValues(typeof(DLCManager.DLCs)))
            {
                string text4 = value2.ToString().ToLowerInvariant();
                bool flag3 = false;
                foreach (AssetBundle bundle in this.bundles)
                {
                    if (bundle.name == "ab_" + text4)
                    {
                        flag3 = true;
                        break;
                    }
                }
                bool flag4 = ((uint)num3 & (uint)value2) != 0;
                if (!flag3 && flag4)
                {
                    num3 &= (int)(~value2);
                    flag2 = true;
                }
                else if (flag3)
                {
                    DLCManager.ownedDLC.Add(value2);
                }
            }
            if (flag2)
            {
                PlayerPrefs.SetInt("UseDLC", num3);
                PlayerPrefs.Save();
            }
            this.LoadModExternalImages();
        }

        private List<string> LoadModAssetBundles()
        {
            List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
            List<string> list = new List<string>();
            foreach (ModOrder item in activeValidMods)
            {
                string path = item.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("Path for mod " + item.name + " is missing!");
                    continue;
                }
                string path2 = Path.Combine(path, "Assetbundles");
                if (!Directory.Exists(path2))
                {
                    continue;
                }
                string[] files = Directory.GetFiles(path2);
                foreach (string text in files)
                {
                    if (!Path.HasExtension(text))
                    {
                        list.Add(text);
                    }
                }
            }
            return list;
        }

        private List<string> LoadModExternalImages()
        {
            List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
            List<string> result = new List<string>();
            foreach (ModOrder item in activeValidMods)
            {
                string path = item.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("Path for mod " + item.name + " is missing!");
                    continue;
                }
                string path2 = Path.Combine(path, "Images");
                if (!Directory.Exists(path2))
                {
                    continue;
                }
                string[] files = Directory.GetFiles(path2);
                foreach (string text in files)
                {
                    if (text.EndsWith(".jpg") || text.EndsWith(".png"))
                    {
                        string text2 = Path.GetFileNameWithoutExtension(text);
                        if (text2 != null)
                        {
                            text2 = text2.ToLowerInvariant();
                        }
                        this.managedAssets[text2] = new AssetTracker(text2, text2, null, text);
                    }
                }
            }
            return result;
        }

        public static void ClearUnused()
        {
            AssetManager.Get().assetTrackerManager?.FreeAssets();
        }

        public static T Get<T>(string assetName, bool passModValidation = true) where T : class
        {
            if (assetName == null)
            {
                return null;
            }
            assetName = assetName.ToLowerInvariant();
            if (typeof(T) == typeof(GameObject) && !assetName.EndsWith("_3d"))
            {
                assetName += "_3d";
            }
            AssetManager assetManager = AssetManager.Get();
            if (assetManager.assetTrackerManager == null)
            {
                assetManager.assetTrackerManager = new AssetTrackerManager();
            }
            if (AssetManager.Get().managedAssets != null && AssetManager.Get().managedAssets.ContainsKey(assetName))
            {
                AssetTracker at = AssetManager.Get().managedAssets[assetName];
                return AssetManager.Get().assetTrackerManager.GetAsset(at) as T;
            }
            if (assetName.EndsWith(".jpg") || assetName.EndsWith(".png"))
            {
                if (assetName.IndexOf(":") > 0)
                {
                    string[] array = assetName.Split(':'); //, StringSplitOptions.None);
                    if (array.Length == 2)
                    {
                        List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
                        foreach (KeyValuePair<string, ModSettings> v in ModManager.Get().GetModList())
                        {
                            if (!(v.Value.prefix == array[0]) || (passModValidation && activeValidMods.FindIndex((ModOrder o) => o.name == v.Value.name) == -1))
                            {
                                continue;
                            }
                            string text = array[1];
                            string[] files = Directory.GetFiles(v.Key, "*", SearchOption.AllDirectories);
                            foreach (string text2 in files)
                            {
                                string fileName = Path.GetFileName(text2);
                                if (text == fileName.ToLowerInvariant())
                                {
                                    AssetTracker assetTracker = new AssetTracker(assetName, assetName, null, text2);
                                    AssetManager.Get().managedAssets[assetName] = assetTracker;
                                    return AssetManager.Get().assetTrackerManager.GetAsset(assetTracker) as T;
                                }
                            }
                        }
                    }
                }
                Debug.LogError("Asset not found: " + assetName);
            }
            return null;
        }

        public static string GetAssetPath<T>(string assetName)
        {
            if (assetName == null)
            {
                return null;
            }
            assetName = assetName.ToLowerInvariant();
            if (typeof(T) == typeof(GameObject) && !assetName.EndsWith("_3d"))
            {
                assetName += "_3d";
            }
            if (AssetManager.Get().managedAssets != null && AssetManager.Get().managedAssets.ContainsKey(assetName))
            {
                return AssetManager.Get().managedAssets[assetName].assetPath;
            }
            if ((assetName.EndsWith(".jpg") || assetName.EndsWith(".png") || assetName.EndsWith(".wav") || assetName.EndsWith(".ogg")) && assetName.IndexOf(":") > 0)
            {
                string[] array = assetName.Split(':'); //, StringSplitOptions.None);
                if (array.Length == 2)
                {
                    List<ModOrder> activeValidMods = ModManager.Get().GetActiveValidMods();
                    foreach (KeyValuePair<string, ModSettings> v in ModManager.Get().GetModList())
                    {
                        if (!(v.Value.prefix == array[0]) || activeValidMods.FindIndex((ModOrder o) => o.name == v.Value.name) == -1)
                        {
                            continue;
                        }
                        string text = array[1];
                        string[] files = Directory.GetFiles(v.Key, "*", SearchOption.AllDirectories);
                        foreach (string text2 in files)
                        {
                            string fileName = Path.GetFileName(text2);
                            if (text == fileName.ToLowerInvariant())
                            {
                                AssetTracker assetTracker = new AssetTracker(assetName, assetName, null, text2);
                                AssetManager.Get().managedAssets[assetName] = assetTracker;
                                return assetTracker.assetPath;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static Texture2D GetTexture(Subrace u)
        {
            return AssetManager.Get<Texture2D>(u.GetDescriptionInfo().graphic);
        }

        public void SetForegroundTexture(Texture2D t2d)
        {
            Material material = new Material(this.foregroundMaterial.shader);
            material.CopyPropertiesFromMaterial(this.foregroundMaterial);
            material.mainTexture = t2d;
            this.foregroundMaterial = material;
            MemoryManager.RegisterPermanent(material);
        }

        public AssetTracker GetAsset(string name)
        {
            if (this.managedAssets.ContainsKey(name))
            {
                return this.managedAssets[name];
            }
            Debug.LogError("Asset not found: " + name);
            return null;
        }
    }
}
