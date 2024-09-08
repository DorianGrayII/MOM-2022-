using System;
using System.Collections.Generic;
using UnityEngine;

namespace MOM
{
    [Serializable]
    public class GameObjectEnabler<T> where T : Enum
    {
        [Serializable]
        public class Items
        {
            public T itemType;

            public GameObject gameObject;
        }

        public List<Items> items = new List<Items>();

        public void Set(T itemType)
        {
            foreach (Items item in this.items)
            {
                _ = item.itemType;
                if ((bool)item.gameObject)
                {
                    item.gameObject.SetActive(itemType.CompareTo(item.itemType) == 0);
                }
            }
        }

        public void Clear()
        {
            foreach (Items item in this.items)
            {
                if ((bool)item.gameObject)
                {
                    item.gameObject.SetActive(value: false);
                }
            }
        }

        public GameObjectEnabler()
        {
            T[] array = (T[])Enum.GetValues(typeof(T));
            foreach (T itemType in array)
            {
                this.items.Add(new Items
                {
                    itemType = itemType
                });
            }
        }
    }
}
