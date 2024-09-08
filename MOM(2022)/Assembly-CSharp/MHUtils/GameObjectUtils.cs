namespace MHUtils
{
    using DBUtils;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [Extension]
    public static class GameObjectUtils
    {
        public static void CanvasDrawLayer(GameObject root, int layer)
        {
            bool activeSelf = root.activeSelf;
            if (!activeSelf)
            {
                root.SetActive(true);
            }
            Canvas orAddComponent = GetOrAddComponent<Canvas>(root);
            orAddComponent.overrideSorting = true;
            orAddComponent.sortingOrder = layer;
            GetOrAddComponent<GraphicRaycaster>(root);
            GetOrAddComponent<CanvasGroup>(root);
            if (!activeSelf)
            {
                root.SetActive(false);
            }
        }

        public static void ChangeLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform transform in root)
            {
                if (transform != null)
                {
                    ChangeLayer(transform, layer);
                }
            }
        }

        public static List<GameObject> FindAllByName(GameObject root, string name, bool onlyDirectChildren)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (Transform transform in GetComponentsInChildren<Transform>(root.transform, onlyDirectChildren))
            {
                if (transform && (transform.name == name))
                {
                    list.Add(transform.gameObject);
                }
            }
            return list;
        }

        public static GameObject FindByName(string name, bool includeInactive)
        {
            foreach (UnityEngine.Object obj2 in UnityEngine.Object.FindObjectsOfType<GameObject>())
            {
                if ((obj2 is GameObject) && (obj2.name == name))
                {
                    return (obj2 as GameObject);
                }
            }
            return null;
        }

        public static GameObject FindByName(GameObject root, string name, bool onlyDirectChildren)
        {
            foreach (Transform transform in GetComponentsInChildren<Transform>(root.transform, onlyDirectChildren))
            {
                if ((transform != null) && (transform.name == name))
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        public static T FindByNameGetComponent<T>(GameObject root, string name, bool onlyDirectChildren) where T: Component
        {
            if (onlyDirectChildren)
            {
                T[] componentsInChildren = GetComponentsInChildren<T>(root.transform, onlyDirectChildren);
                if (componentsInChildren.Length != 0)
                {
                    return componentsInChildren[0];
                }
                return default(T);
            }
            GameObject obj2 = FindByName(root, name, false);
            if (obj2 != null)
            {
                return obj2.GetComponent<T>();
            }
            return default(T);
        }

        public static T FindByNameGetComponentInChildren<T>(GameObject root, string name) where T: Component
        {
            return FindByNameGetComponentInChildren<T>(root, name, false, false);
        }

        public static T FindByNameGetComponentInChildren<T>(GameObject root, string name, bool ignoreRoot, bool onlyDirectChildren) where T: Component
        {
            GameObject obj2 = FindByName(root, name, false);
            if (obj2 != null)
            {
                if (!ignoreRoot)
                {
                    T[] componentsInChildren = GetComponentsInChildren<T>(obj2.transform, onlyDirectChildren);
                    if (componentsInChildren.Length != 0)
                    {
                        return componentsInChildren[0];
                    }
                    return default(T);
                }
                foreach (T local in GetComponentsInChildren<T>(obj2.transform, onlyDirectChildren))
                {
                    if (local.gameObject != obj2)
                    {
                        return local;
                    }
                }
            }
            return default(T);
        }

        public static T FindByNameGetComponentReport<T>(GameObject root, string name, bool onlyDirectChildren) where T: Component
        {
            T local = FindByNameGetComponent<T>(root, name, onlyDirectChildren);
            if (local == null)
            {
                string[] textArray1 = new string[] { "Expected to find ", name, " at ", root.ToString(), " of type ", typeof(T).ToString(), " but none was found" };
                Debug.LogError(string.Concat(textArray1));
            }
            return local;
        }

        public static GameObject FindByNameParent(GameObject root, string name)
        {
            if (root != null)
            {
                for (Transform transform = root.transform; transform != null; transform = transform.parent)
                {
                    if (transform.name == name)
                    {
                        return transform.gameObject;
                    }
                }
            }
            return null;
        }

        public static T FindByType<T>(GameObject root)
        {
            T[] componentsInChildren = root.GetComponentsInChildren<T>(true);
            if (componentsInChildren.Length == 1)
            {
                return componentsInChildren[0];
            }
            Debug.LogError("[ERROR]It was expected to find single component but it found :" + componentsInChildren.Length.ToString());
            return default(T);
        }

        public static T FindInParents<T>(Transform source, bool ignoreSource) where T: Component
        {
            for (Transform transform = ignoreSource ? source.parent : source; transform != null; transform = transform.parent)
            {
                T component = transform.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
            return default(T);
        }

        public static GameObject FindParentAtDepth(GameObject root, string name, int depthAgainstNamed)
        {
            if (root != null)
            {
                Transform item = root.transform;
                List<Transform> list = new List<Transform>();
                while (item != null)
                {
                    list.Add(item);
                    if (item.name == name)
                    {
                        int num = (list.Count - 1) - depthAgainstNamed;
                        return ((num >= 0) ? list[num].gameObject : null);
                    }
                    item = item.parent;
                }
            }
            return null;
        }

        private static T[] GetComponentsInChildren<T>(Transform transform, bool onlyDirectChildren)
        {
            if (!onlyDirectChildren)
            {
                return transform.GetComponentsInChildren<T>(true);
            }
            List<T> list = new List<T>();
            using (IEnumerator enumerator = transform.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    T component = ((Transform) enumerator.Current).GetComponent<T>();
                    if (component != null)
                    {
                        list.Add(component);
                    }
                }
            }
            return list.ToArray();
        }

        [Extension]
        public static T GetOrAddComponent<T>(GameObject target) where T: Component
        {
            T component = target.GetComponent<T>();
            if (!component)
            {
                component = target.AddComponent<T>();
            }
            return component;
        }

        public static T GetUIComponentAtPosition<T>(Vector2 position, bool allowPassThrough) where T: Component
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current) {
                position = position
            };
            EventSystem.current.RaycastAll(eventData, raycastResults);
            for (int i = 0; (i < raycastResults.Count) && (allowPassThrough || (i <= 0)); i++)
            {
                if (raycastResults[i].gameObject != null)
                {
                    Transform parent = raycastResults[i].gameObject.transform;
                    T component = default(T);
                    while (parent != null)
                    {
                        component = parent.gameObject.GetComponent<T>();
                        if (component != null)
                        {
                            return component;
                        }
                        parent = parent.parent;
                    }
                }
            }
            return default(T);
        }

        public static GameObject Instantiate(GameObject source, Transform parent)
        {
            GameObject local1 = UnityEngine.Object.Instantiate<GameObject>(source, parent);
            local1.transform.localPosition = Vector3.zero;
            local1.transform.localScale = Vector3.one;
            return local1;
        }

        public static GameObject InstantiateWithLocalization(GameObject source, Transform parent)
        {
            GameObject obj2 = Instantiate(source, parent);
            if (DataBase.IsInitialized())
            {
                TextMeshProUGUI[] componentsInChildren = obj2.GetComponentsInChildren<TextMeshProUGUI>();
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    TextMeshProUGUI ougui1 = componentsInChildren[i];
                    ougui1.text = Localization.Get(ougui1.text, true, Array.Empty<object>());
                }
            }
            return obj2;
        }

        public static bool IsParentOf(Transform parent, Transform child)
        {
            for (Transform transform = child; transform.parent != null; transform = transform.parent)
            {
                if (transform.parent == parent)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsParentOrEqualOf(Transform parent, Transform child)
        {
            return ((parent == child) || IsParentOf(parent, child));
        }

        public static void PrintHierarchy(Transform transform)
        {
            PrintHierarchy(transform, "");
        }

        public static void PrintHierarchy(Transform transform, string header)
        {
            StringBuilder message = new StringBuilder();
            Transform parent = transform;
            if (header.Length > 0)
            {
                message.Append(header);
            }
            message.Append("Printing structure for object: ");
            while (parent != null)
            {
                message.Append("->[" + parent.name + "]");
                parent = parent.parent;
            }
            Debug.LogWarning(message);
        }

        public static void RemoveChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(root.GetChild(i).gameObject);
            }
        }
    }
}

