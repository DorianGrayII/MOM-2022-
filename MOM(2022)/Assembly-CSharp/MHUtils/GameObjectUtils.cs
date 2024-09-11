using System.Collections.Generic;
using System.Text;
using DBUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MHUtils
{
    public static class GameObjectUtils
    {
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            T val = target.GetComponent<T>();
            if (!val)
            {
                val = target.AddComponent<T>();
            }
            return val;
        }

        public static GameObject FindByName(string name, bool includeInactive = false)
        {
            Object[] array = Object.FindObjectsOfType<GameObject>();

            // array = array;
            foreach (Object @object in array)
            {
                if (@object is GameObject && @object.name == name)
                {
                    return @object as GameObject;
                }
            }
            return null;
        }

        public static T FindByType<T>(GameObject root)
        {
            T[] componentsInChildren = root.GetComponentsInChildren<T>(includeInactive: true);
            if (componentsInChildren.Length == 1)
            {
                return componentsInChildren[0];
            }
            Debug.LogError("[ERROR]It was expected to find single component but it found :" + componentsInChildren.Length);
            return default(T);
        }

        public static GameObject FindByName(GameObject root, string name, bool onlyDirectChildren = false)
        {
            Transform[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<Transform>(root.transform, onlyDirectChildren);
            foreach (Transform transform in componentsInChildren)
            {
                if ((object)transform != null && transform.name == name)
                {
                    return transform.gameObject;
                }
            }
            return null;
        }

        public static GameObject FindByNameParent(GameObject root, string name)
        {
            if (root == null)
            {
                return null;
            }
            Transform transform = root.transform;
            while (transform != null)
            {
                if (transform.name == name)
                {
                    return transform.gameObject;
                }
                transform = transform.parent;
            }
            return null;
        }

        public static GameObject FindParentAtDepth(GameObject root, string name, int depthAgainstNamed = 0)
        {
            if (root == null)
            {
                return null;
            }
            Transform transform = root.transform;
            List<Transform> list = new List<Transform>();
            while (true)
            {
                if (transform == null)
                {
                    return null;
                }
                list.Add(transform);
                if (transform.name == name)
                {
                    break;
                }
                transform = transform.parent;
            }
            int num = list.Count - 1 - depthAgainstNamed;
            if (num < 0)
            {
                return null;
            }
            return list[num].gameObject;
        }

        public static List<GameObject> FindAllByName(GameObject root, string name, bool onlyDirectChildren = false)
        {
            Transform[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<Transform>(root.transform, onlyDirectChildren);
            List<GameObject> list = new List<GameObject>();
            Transform[] array = componentsInChildren;
            foreach (Transform transform in array)
            {
                if ((bool)transform && transform.name == name)
                {
                    list.Add(transform.gameObject);
                }
            }
            return list;
        }

        public static T FindByNameGetComponentReport<T>(GameObject root, string name, bool onlyDirectChildren = false) where T : Component
        {
            T val = GameObjectUtils.FindByNameGetComponent<T>(root, name, onlyDirectChildren);
            if (val == null)
            {
                Debug.LogError("Expected to find " + name + " at " + root.ToString() + " of type " + typeof(T).ToString() + " but none was found");
            }
            return val;
        }

        public static T FindByNameGetComponent<T>(GameObject root, string name, bool onlyDirectChildren = false) where T : Component
        {
            if (onlyDirectChildren)
            {
                T[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<T>(root.transform, onlyDirectChildren);
                if (componentsInChildren.Length != 0)
                {
                    return componentsInChildren[0];
                }
                return null;
            }
            GameObject gameObject = GameObjectUtils.FindByName(root, name, onlyDirectChildren = false);
            if (gameObject != null)
            {
                return gameObject.GetComponent<T>();
            }
            return null;
        }

        public static T FindByNameGetComponentInChildren<T>(GameObject root, string name) where T : Component
        {
            return GameObjectUtils.FindByNameGetComponentInChildren<T>(root, name, ignoreRoot: false);
        }

        public static T FindByNameGetComponentInChildren<T>(GameObject root, string name, bool ignoreRoot, bool onlyDirectChildren = false) where T : Component
        {
            GameObject gameObject = GameObjectUtils.FindByName(root, name);
            if (gameObject != null)
            {
                if (ignoreRoot)
                {
                    T[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<T>(gameObject.transform, onlyDirectChildren);
                    foreach (T val in componentsInChildren)
                    {
                        if (val.gameObject != gameObject)
                        {
                            return val;
                        }
                    }
                    return null;
                }
                T[] componentsInChildren2 = GameObjectUtils.GetComponentsInChildren<T>(gameObject.transform, onlyDirectChildren);
                if (componentsInChildren2.Length != 0)
                {
                    return componentsInChildren2[0];
                }
                return null;
            }
            return null;
        }

        public static T FindInParents<T>(Transform source, bool ignoreSource) where T : Component
        {
            Transform transform = (ignoreSource ? source.parent : source);
            while (transform != null)
            {
                T component = transform.gameObject.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                transform = transform.parent;
            }
            return null;
        }

        public static bool IsParentOrEqualOf(Transform parent, Transform child)
        {
            if (!(parent == child))
            {
                return GameObjectUtils.IsParentOf(parent, child);
            }
            return true;
        }

        public static bool IsParentOf(Transform parent, Transform child)
        {
            Transform transform = child;
            while (transform.parent != null)
            {
                if (transform.parent == parent)
                {
                    return true;
                }
                transform = transform.parent;
            }
            return false;
        }

        public static void PrintHierarchy(Transform transform)
        {
            GameObjectUtils.PrintHierarchy(transform, "");
        }

        public static void PrintHierarchy(Transform transform, string header)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Transform transform2 = transform;
            if (header.Length > 0)
            {
                stringBuilder.Append(header);
            }
            stringBuilder.Append("Printing structure for object: ");
            while (transform2 != null)
            {
                stringBuilder.Append("->[" + transform2.name + "]");
                transform2 = transform2.parent;
            }
            Debug.LogWarning(stringBuilder);
        }

        public static void RemoveChildren(Transform root)
        {
            for (int num = root.childCount - 1; num >= 0; num--)
            {
                Object.Destroy(root.GetChild(num).gameObject);
            }
        }

        public static GameObject InstantiateWithLocalization(GameObject source, Transform parent)
        {
            GameObject gameObject = GameObjectUtils.Instantiate(source, parent);
            if (!DataBase.IsInitialized())
            {
                return gameObject;
            }
            TextMeshProUGUI[] componentsInChildren = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI obj in componentsInChildren)
            {
                obj.text = Localization.Get(obj.text, true);
            }
            return gameObject;
        }

        public static GameObject Instantiate(GameObject source, Transform parent)
        {
            GameObject gameObject = Object.Instantiate(source, parent);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            return gameObject;
        }

        private static T[] GetComponentsInChildren<T>(Transform transform, bool onlyDirectChildren = false)
        {
            if (onlyDirectChildren)
            {
                List<T> list = new List<T>();
                foreach (Transform item in transform)
                {
                    T component = item.GetComponent<T>();
                    if (component != null)
                    {
                        list.Add(component);
                    }
                }
                return list.ToArray();
            }
            return transform.GetComponentsInChildren<T>(includeInactive: true);
        }

        public static void ChangeLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform item in root)
            {
                if (!(item == null))
                {
                    GameObjectUtils.ChangeLayer(item, layer);
                }
            }
        }

        public static void CanvasDrawLayer(GameObject root, int layer)
        {
            bool activeSelf = root.activeSelf;
            if (!activeSelf)
            {
                root.SetActive(value: true);
            }
            Canvas orAddComponent = root.GetOrAddComponent<Canvas>();
            orAddComponent.overrideSorting = true;
            orAddComponent.sortingOrder = layer;
            root.GetOrAddComponent<GraphicRaycaster>();
            root.GetOrAddComponent<CanvasGroup>();
            if (!activeSelf)
            {
                root.SetActive(value: false);
            }
        }

        public static T GetUIComponentAtPosition<T>(Vector2 position, bool allowPassThrough = true) where T : Component
        {
            List<RaycastResult> list = new List<RaycastResult>();
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = position;
            EventSystem.current.RaycastAll(pointerEventData, list);
            for (int i = 0; i < list.Count && (allowPassThrough || i <= 0); i++)
            {
                if (!(list[i].gameObject != null))
                {
                    continue;
                }
                Transform transform = list[i].gameObject.transform;
                T val = null;
                while (transform != null)
                {
                    val = transform.gameObject.GetComponent<T>();
                    if (val != null)
                    {
                        return val;
                    }
                    transform = transform.parent;
                }
            }
            return null;
        }
    }
}
