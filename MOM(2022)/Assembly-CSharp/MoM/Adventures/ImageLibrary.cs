using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MHUtils;
using UnityEngine;

namespace MOM.Adventures
{
    public class ImageLibrary : MonoBehaviour
    {
        public static ImageLibrary instance;

        private string PATH = Path.Combine(MHApplication.EXTERNAL_ASSETS, "AdventureGraphics");

        private Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();

        private Dictionary<string, Texture2D> additionalImages = new Dictionary<string, Texture2D>();

        public static void Destroy()
        {
            if (ImageLibrary.instance != null)
            {
                global::UnityEngine.Object.Destroy(ImageLibrary.instance);
            }
        }

        public static ImageLibrary Get()
        {
            if (ImageLibrary.instance == null)
            {
                ImageLibrary.instance = new GameObject("ImageLibrary").AddComponent<ImageLibrary>();
            }
            return ImageLibrary.instance;
        }

        public static Texture2D RequestSingleImage(string name)
        {
            return ImageLibrary.Get()._RequestSingleImage(name);
        }

        public static IEnumerator InitializeFullLibrary()
        {
            yield return ImageLibrary.Get()._InitializeFullLibrary();
        }

        public static Dictionary<string, Texture2D> GetImageDictionary()
        {
            return ImageLibrary.Get().images;
        }

        public static Texture2D RequestAdditionalImage(string name)
        {
            if (ImageLibrary.Get() == null)
            {
                return null;
            }
            if (name != null)
            {
                name = name.ToLowerInvariant();
            }
            if (name == null || ImageLibrary.Get().additionalImages == null || !ImageLibrary.Get().additionalImages.ContainsKey(name))
            {
                return null;
            }
            return ImageLibrary.Get().additionalImages[name];
        }

        public Texture2D _RequestSingleImage(string name)
        {
            Texture2D result = null;
            if (string.IsNullOrEmpty(name))
            {
                return result;
            }
            if (!this.images.ContainsKey(name))
            {
                result = this.PrepareTexture(name);
                Tuple<string, byte[]> tuple = new Tuple<string, byte[]>(null, null);
                try
                {
                    string[] files = Directory.GetFiles(this.PATH);
                    foreach (string path in files)
                    {
                        if (Path.GetFileNameWithoutExtension(path) == name)
                        {
                            byte[] item = File.ReadAllBytes(path);
                            tuple = new Tuple<string, byte[]>(name, item);
                            break;
                        }
                    }
                }
                catch (Exception message)
                {
                    Debug.LogWarning(message);
                }
                if (tuple.Item1 != null)
                {
                    Texture2D texture2D = this.PrepareTexture(tuple.Item1);
                    texture2D.LoadImage(tuple.Item2);
                    texture2D.name = tuple.Item1;
                    result = texture2D;
                }
                return result;
            }
            return this.PrepareTexture(name);
        }

        public Texture2D PrepareTexture(string name)
        {
            if (!this.images.ContainsKey(name))
            {
                Texture2D value = new Texture2D(2, 2);
                this.images[name] = value;
            }
            return this.images[name];
        }

        public Texture2D PrepareAdditionalImage(string name)
        {
            if (name != null)
            {
                name = name.ToLowerInvariant();
            }
            if (!this.additionalImages.ContainsKey(name))
            {
                Texture2D value = new Texture2D(2, 2);
                this.additionalImages[name] = value;
            }
            return this.additionalImages[name];
        }

        public IEnumerator _InitializeFullLibrary()
        {
            string[] files = Directory.GetFiles(this.PATH);
            List<string> list = new List<string>(files.Length);
            foreach (string text in files)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
                if (!this.images.ContainsKey(fileNameWithoutExtension))
                {
                    this.PrepareTexture(fileNameWithoutExtension);
                    list.Add(text);
                }
            }
            CallbackRet action = delegate(object o)
            {
                Tuple<string, byte[]> result = new Tuple<string, byte[]>(null, null);
                try
                {
                    string path = o as string;
                    result = new Tuple<string, byte[]>(item2: File.ReadAllBytes(path), item1: Path.GetFileNameWithoutExtension(path));
                }
                catch (Exception message)
                {
                    Debug.LogWarning(message);
                }
                return result;
            };
            if (list.Count > 0)
            {
                yield return MHNonThread.CreateMulti(action, list, LoadingLibrary);
            }
        }

        private void LoadingLibrary(object obj)
        {
            object[] array = obj as object[];
            for (int i = 0; i < array.Length; i++)
            {
                Tuple<string, byte[]> tuple = (Tuple<string, byte[]>)array[i];
                try
                {
                    Texture2D texture2D = this.PrepareTexture(tuple.Item1);
                    texture2D.LoadImage(tuple.Item2);
                    texture2D.name = tuple.Item1;
                }
                catch (Exception message)
                {
                    Debug.LogWarning(message);
                }
            }
        }
    }
}
