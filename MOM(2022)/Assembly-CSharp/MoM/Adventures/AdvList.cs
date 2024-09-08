namespace MOM.Adventures
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class AdvList
    {
        public List<object> list;
        public LogicEntry.LEntry listType;
        public bool makePublic;

        public AdvList Clone()
        {
            AdvList list1 = new AdvList();
            list1.list = this.list;
            list1.listType = this.listType;
            list1.makePublic = this.makePublic;
            return list1;
        }

        public int Count()
        {
            return ((this.list != null) ? this.list.Count : 0);
        }

        public T Get<T>(int index) where T: class
        {
            if ((this.list != null) && (this.list.Count > index))
            {
                return (this.list[index] as T);
            }
            return default(T);
        }

        public static AdvList MakeList(List<object> l, AdvLogic advLogic, AdventureData advData, Dictionary<string, AdvList> tempList)
        {
            AdvList list = new AdvList {
                list = l
            };
            if (advLogic is LogicEntry)
            {
                list.listType = (advLogic as LogicEntry).logicType;
            }
            else if (advLogic is LogicProcessing)
            {
                ScriptRetType customAttribute;
                LogicProcessing processing = advLogic as LogicProcessing;
                MethodInfo element = ScriptLibrary.Get(processing.GetScriptName());
                if (element != null)
                {
                    customAttribute = CustomAttributeExtensions.GetCustomAttribute<ScriptRetType>(element);
                }
                else
                {
                    MethodInfo local1 = element;
                    customAttribute = null;
                }
                ScriptRetType type = customAttribute;
                if (type != null)
                {
                    list.listType = type.type;
                }
                else if (!string.IsNullOrEmpty(processing.listA))
                {
                    list.listType = advData.GetListByName(processing.listA, tempList).listType;
                }
                else
                {
                    Debug.LogError("No speciffied list in LogicProcessing");
                }
            }
            return list;
        }
    }
}

