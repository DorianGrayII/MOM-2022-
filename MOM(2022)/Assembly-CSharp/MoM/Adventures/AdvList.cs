using System.Collections.Generic;
using System.Reflection;
using MHUtils;
using UnityEngine;

namespace MOM.Adventures
{
    public class AdvList
    {
        public List<object> list;

        public LogicEntry.LEntry listType;

        public bool makePublic;

        public int Count()
        {
            if (this.list == null)
            {
                return 0;
            }
            return this.list.Count;
        }

        public T Get<T>(int index) where T : class
        {
            if (this.list != null && this.list.Count > index)
            {
                return this.list[index] as T;
            }
            return null;
        }

        public static AdvList MakeList(List<object> l, AdvLogic advLogic, AdventureData advData, Dictionary<string, AdvList> tempList)
        {
            AdvList advList = new AdvList();
            advList.list = l;
            if (advLogic is LogicEntry)
            {
                advList.listType = (advLogic as LogicEntry).logicType;
            }
            else if (advLogic is LogicProcessing)
            {
                LogicProcessing logicProcessing = advLogic as LogicProcessing;
                ScriptRetType scriptRetType = ScriptLibrary.Get(logicProcessing.GetScriptName())?.GetCustomAttribute<ScriptRetType>();
                if (scriptRetType != null)
                {
                    advList.listType = scriptRetType.type;
                }
                else if (!string.IsNullOrEmpty(logicProcessing.listA))
                {
                    AdvList listByName = advData.GetListByName(logicProcessing.listA, tempList);
                    advList.listType = listByName.listType;
                }
                else
                {
                    Debug.LogError("No speciffied list in LogicProcessing");
                }
            }
            return advList;
        }

        public AdvList Clone()
        {
            return new AdvList
            {
                list = this.list,
                listType = this.listType,
                makePublic = this.makePublic
            };
        }
    }
}
