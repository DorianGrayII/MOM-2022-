using UnityEngine;

namespace MHUtils
{
    public class AssetTracker
    {
        public Object instance;

        public bool loaded;

        public float lastUseTime;

        public AssetBundle assetSource;

        public string assetPath;

        public string assetOriginalName;

        public string assetCallName;

        public AssetTracker nextAsset;

        public AssetTracker prevAsset;

        public AssetTracker(string assetName, string orgName, AssetBundle ab, string assetPath)
        {
            this.assetPath = assetPath;
            this.assetCallName = assetName;
            this.assetOriginalName = orgName;
            this.assetSource = ab;
        }
    }
}
