using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using UnityEngine;

namespace MOM
{
    public static class IEnchantableExtension
    {
        public static EnchantmentInstance AddEnchantment(this IEnchantable obj, Enchantment e, object owner, int countdown = -1, string parameters = null, int dispelCost = 0)
        {
            if (owner != null && !(owner is Entity))
            {
                Debug.LogError("Non-null owner have to be Entity and is: " + owner);
            }
            bool inBattle = false;
            if (obj is Battle || obj is BattleUnit || obj is BattlePlayer)
            {
                inBattle = true;
            }
            EnchantmentInstance enchantmentInstance = null;
            enchantmentInstance = ((!IEnchantableExtension.IsEnchantmentAlreadyOnTarget(obj, e, owner, countdown, parameters, dispelCost)) ? obj.GetEnchantmentManager().Add(e, owner as Entity, countdown, parameters, inBattle, dispelCost) : obj.GetEnchantments().Find((EnchantmentInstance o) => o.source == e && o.owner == owner));
            if (e.applicationScript != null)
            {
                ScriptLibrary.Call(e.applicationScript.script, obj, e, enchantmentInstance);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            if (obj is Location location && location.GetUnits() != null)
            {
                foreach (Reference<Unit> unit in location.GetUnits())
                {
                    unit.Get().attributes.SetDirty();
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), enchantmentInstance);
            return enchantmentInstance;
        }

        public static EnchantmentInstance AddEnchantment(this IEnchantable obj, EnchantmentInstance ei)
        {
            bool inBattle = false;
            if (obj is Battle || obj is BattleUnit)
            {
                inBattle = true;
            }
            obj.GetEnchantmentManager().Add2(ei, inBattle);
            if (ei.source.Get().applicationScript != null)
            {
                ScriptLibrary.Call(ei.source.Get().applicationScript.script, obj, ei.source.Get(), ei);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            if (obj is Location location && location.GetUnits() != null)
            {
                foreach (Reference<Unit> unit in location.GetUnits())
                {
                    unit.Get().attributes.SetDirty();
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), ei);
            return ei;
        }

        public static void RemoveEnchantment(this IEnchantable obj, Enchantment e)
        {
            EnchantmentInstance enchantmentInstance = obj.GetEnchantmentManager().Remove(e);
            if (e.removalScript != null)
            {
                ScriptLibrary.Call(e.removalScript.script, obj, e, enchantmentInstance);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            if (obj is Location location && location.GetUnits() != null)
            {
                foreach (Reference<Unit> unit in location.GetUnits())
                {
                    unit.Get().attributes.SetDirty();
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), enchantmentInstance);
        }

        public static void RemoveEnchantment(this IEnchantable obj, EnchantmentInstance e)
        {
            EnchantmentInstance enchantmentInstance = obj.GetEnchantmentManager().Remove(e);
            if (e.source.Get().removalScript != null)
            {
                ScriptLibrary.Call(e.source.Get().removalScript.script, obj, e.source.Get(), enchantmentInstance);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            if (obj is Location location && location.GetUnits() != null)
            {
                foreach (Reference<Unit> unit in location.GetUnits())
                {
                    unit.Get().attributes.SetDirty();
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), enchantmentInstance);
        }

        public static bool IsEnchantmentAlreadyOnTarget(IEnchantable obj, Enchantment e, object owner, int countdown, string parameters, int dispelCost)
        {
            foreach (EnchantmentInstance enchantment in obj.GetEnchantments())
            {
                if (enchantment.source == e && enchantment.owner == owner)
                {
                    if (enchantment.countDown > -1 && countdown > -1)
                    {
                        enchantment.countDown += countdown;
                    }
                    if (enchantment.dispelCost > 0 || dispelCost > 0)
                    {
                        enchantment.dispelCost = Math.Max(enchantment.dispelCost, dispelCost);
                    }
                    if (!string.IsNullOrEmpty(enchantment.parameters) || !string.IsNullOrEmpty(parameters))
                    {
                        enchantment.parameters = IEnchantableExtension.CombineStringParameters(enchantment.parameters, parameters);
                    }
                    return true;
                }
            }
            return false;
        }

        public static void TriggerScripts(this IEnchantable obj, EEnchantmentType eType, object data = null, IEnchantable customTarget = null)
        {
            obj.GetEnchantmentManager().TriggerScripts(eType, data, customTarget);
        }

        public static List<EnchantmentInstance> GetEnchantments(this IEnchantable obj)
        {
            return obj.GetEnchantmentManager().GetEnchantments();
        }

        public static HashSet<EnchantmentInstance> GetRemoteEnchantments(this IEnchantable obj, IEnchantable target)
        {
            return obj.GetEnchantmentManager().GetRemoteEnchantments(target);
        }

        public static void ProcessIntigerScripts(this IEnchantable obj, EEnchantmentType eType, ref int value)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref value);
        }

        public static void ProcessIntigerScripts(this IEnchantable obj, EEnchantmentType eType, ref int income, ref int upkeep)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref income, ref upkeep);
        }

        public static void ProcessIntigerScripts(this IEnchantable obj, EEnchantmentType eType, ref int income, StatDetails details)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref income, details);
        }

        public static void ProcessFloatScripts(this IEnchantable obj, EEnchantmentType eType, ref float value)
        {
            obj.GetEnchantmentManager().ProcessFloatScripts(eType, ref value);
        }

        public static void ProcessFIntScripts(this IEnchantable obj, EEnchantmentType eType, ref FInt value)
        {
            obj.GetEnchantmentManager().ProcessFIntScripts(eType, ref value);
        }

        public static void CountdownUpdate(this IEnchantable obj)
        {
            obj.GetEnchantmentManager().CountedownUpdate(obj);
        }

        public static void BattleCountdownUpdate(this IEnchantable obj, bool isAttackerTurn)
        {
            obj.GetEnchantmentManager().BattleCountdownUpdate(obj, isAttackerTurn);
        }

        public static EnchantmentManager CopyEnchantmentManager(this IEnchantable obj, IEnchantable newOwner)
        {
            return obj.GetEnchantmentManager().CopyEnchantmentManager(newOwner);
        }

        public static void CopyEnchantmentManagerFrom(this IEnchantable obj, IEnchantable source, bool useRequirementScripts = false)
        {
            obj.GetEnchantmentManager().CopyEnchantmentManagerFrom(source, useRequirementScripts);
        }

        public static bool IsIteratingEnchantments(this IEnchantable obj)
        {
            return obj.GetEnchantmentManager().localActiveIterators > 0;
        }

        public static void EnsureEnchantments(this IEnchantable obj)
        {
            obj.GetEnchantmentManager().EnsureEnchantments();
        }

        public static string CombineStringParameters(string param, string param2)
        {
            try
            {
                string[] array = param.Split(';'); //, StringSplitOptions.None);
                float num = 100f;
                string text = null;
                string text2 = null;
                for (int i = 0; i < array.Length; i++)
                {
                    int num2 = array[i].IndexOf("%");
                    if (num2 > -1)
                    {
                        array[i].Substring(0, num2).Replace(" ", "");
                    }
                    else if (text == null)
                    {
                        text = array[i].Replace(" ", "");
                    }
                    else
                    {
                        text2 = array[i].Replace(" ", "");
                    }
                }
                string[] array2 = param2.Split(';'); //, StringSplitOptions.None);
                float num3 = 100f;
                string text3 = null;
                string text4 = null;
                for (int j = 0; j < array2.Length; j++)
                {
                    int num4 = array2[j].IndexOf("%");
                    if (num4 > -1)
                    {
                        array2[j].Substring(0, num4).Replace(" ", "");
                    }
                    else if (text3 == null)
                    {
                        text3 = array2[j].Replace(" ", "");
                    }
                    else
                    {
                        text4 = array2[j].Replace(" ", "");
                    }
                }
                float num5 = ((text != null) ? Convert.ToSingle(text, Globals.GetCultureInfo()) : 0f);
                float num6 = ((text2 != null) ? Convert.ToSingle(text2, Globals.GetCultureInfo()) : num5);
                float num7 = ((text3 != null) ? Convert.ToSingle(text3, Globals.GetCultureInfo()) : 0f);
                float num8 = ((text4 != null) ? Convert.ToSingle(text4, Globals.GetCultureInfo()) : num7);
                float num9 = 1f;
                if (num != 100f || num3 != 100f)
                {
                    num9 = Math.Max(num, num3);
                }
                return num9 + "%;" + (num5 + num7) + ";" + (num6 + num8);
            }
            catch
            {
                Debug.LogError("UTIL_StringParameterProcessor error");
                return null;
            }
        }
    }
}
