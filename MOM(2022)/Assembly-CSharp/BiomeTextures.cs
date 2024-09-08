using UnityEngine;

public class BiomeTextures : MonoBehaviour
{
    public enum SeasonPack
    {
        Normal = 0,
        Autumn = 1,
        Winter = 2
    }

    public const string path = "Assets/Game/Terrain/_BTextures.prefab";

    public Texture2DArray[] diffuse;

    public Texture2DArray[] normal;

    public static BiomeTextures instance;

    private void Start()
    {
        BiomeTextures.instance = this;
    }
}
