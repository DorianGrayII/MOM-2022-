namespace MOM
{
    using DBDef;
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Extension]
    public static class IEnchantableExtension
    {
        [Extension]
        public static EnchantmentInstance AddEnchantment(IEnchantable obj, EnchantmentInstance ei)
        {
            bool inBattle = false;
            if ((obj is Battle) || (obj is BattleUnit))
            {
                inBattle = true;
            }
            obj.GetEnchantmentManager().Add2(ei, inBattle);
            if (ei.source.Get().applicationScript != null)
            {
                object[] parameters = new object[] { obj, ei.source.Get(), ei };
                ScriptLibrary.Call(ei.source.Get().applicationScript.script, parameters);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            MOM.Location location = obj as MOM.Location;
            if ((location != null) && (location.GetUnits() != null))
            {
                using (List<Reference<MOM.Unit>>.Enumerator enumerator = location.GetUnits().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Get().attributes.SetDirty();
                    }
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), ei);
            return ei;
        }

        [Extension]
        public static EnchantmentInstance AddEnchantment(IEnchantable obj, Enchantment e, object owner, int countdown, string parameters, int dispelCost)
        {
            if ((owner != null) && !(owner is Entity))
            {
                string text1;
                if (owner != null)
                {
                    text1 = owner.ToString();
                }
                else
                {
                    object local1 = owner;
                    text1 = null;
                }
                Debug.LogError("Non-null owner have to be Entity and is: " + text1);
            }
            bool inBattle = false;
            if ((obj is Battle) || ((obj is BattleUnit) || (obj is BattlePlayer)))
            {
                inBattle = true;
            }
            EnchantmentInstance args = null;
            args = !IsEnchantmentAlreadyOnTarget(obj, e, owner, countdown, parameters, dispelCost) ? obj.GetEnchantmentManager().Add(e, owner as Entity, countdown, parameters, inBattle, dispelCost) : GetEnchantments(obj).Find(o => (o.source == e) && (o.owner == owner));
            if (e.applicationScript != null)
            {
                object[] objArray1 = new object[] { obj, e, args };
                ScriptLibrary.Call(e.applicationScript.script, objArray1);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            MOM.Location location = obj as MOM.Location;
            if ((location != null) && (location.GetUnits() != null))
            {
                using (List<Reference<MOM.Unit>>.Enumerator enumerator = location.GetUnits().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Get().attributes.SetDirty();
                    }
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), args);
            return args;
        }

        [Extension]
        public static void BattleCountdownUpdate(IEnchantable obj, bool isAttackerTurn)
        {
            obj.GetEnchantmentManager().BattleCountdownUpdate(obj, isAttackerTurn);
        }

        public static string CombineStringParameters(string param, string param2)
        {
            string str5;
            try
            {
                string[] strArray = param.Split(';', StringSplitOptions.None);
                float num = 100f;
                string str = null;
                string str2 = null;
                int index = 0;
                while (true)
                {
                    if (index >= strArray.Length)
                    {
                        string[] strArray2 = param2.Split(';', StringSplitOptions.None);
                        float num2 = 100f;
                        string str3 = null;
                        string str4 = null;
                        int num10 = 0;
                        while (true)
                        {
                            if (num10 >= strArray2.Length)
                            {
                                float num3 = (str != null) ? Convert.ToSingle(str, Globals.GetCultureInfo()) : 0f;
                                float num4 = (str2 != null) ? Convert.ToSingle(str2, Globals.GetCultureInfo()) : num3;
                                float num5 = (str3 != null) ? Convert.ToSingle(str3, Globals.GetCultureInfo()) : 0f;
                                float num6 = (str4 != null) ? Convert.ToSingle(str4, Globals.GetCultureInfo()) : num5;
                                float num7 = 1f;
                                if ((num != 100f) || (num2 != 100f))
                                {
                                    num7 = Math.Max(num, num2);
                                }
                                string[] textArray1 = new string[] { num7.ToString(), "%;", (num3 + num5).ToString(), ";", (num4 + num6).ToString() };
                                str5 = string.Concat(textArray1);
                                break;
                            }
                            int num11 = strArray2[num10].IndexOf("%");
                            if (num11 > -1)
                            {
                                strArray2[num10].Substring(0, num11).Replace(" ", "");
                            }
                            else if (str3 == null)
                            {
                                str3 = strArray2[num10].Replace(" ", "");
                            }
                            else
                            {
                                str4 = strArray2[num10].Replace(" ", "");
                            }
                            num10++;
                        }
                        break;
                    }
                    int length = strArray[index].IndexOf("%");
                    if (length > -1)
                    {
                        strArray[index].Substring(0, length).Replace(" ", "");
                    }
                    else if (str == null)
                    {
                        str = strArray[index].Replace(" ", "");
                    }
                    else
                    {
                        str2 = strArray[index].Replace(" ", "");
                    }
                    index++;
                }
            }
            catch
            {
                Debug.LogError("UTIL_StringParameterProcessor error");
                str5 = null;
            }
            return str5;
        }

        [Extension]
        public static EnchantmentManager CopyEnchantmentManager(IEnchantable obj, IEnchantable newOwner)
        {
            return obj.GetEnchantmentManager().CopyEnchantmentManager(newOwner);
        }

        [Extension]
        public static void CopyEnchantmentManagerFrom(IEnchantable obj, IEnchantable source, bool useRequirementScripts)
        {
            obj.GetEnchantmentManager().CopyEnchantmentManagerFrom(source, useRequirementScripts);
        }

        [Extension]
        public static void CountdownUpdate(IEnchantable obj)
        {
            obj.GetEnchantmentManager().CountedownUpdate(obj);
        }

        [Extension]
        public static void EnsureEnchantments(IEnchantable obj)
        {
            obj.GetEnchantmentManager().EnsureEnchantments();
        }

        [Extension]
        public static List<EnchantmentInstance> GetEnchantments(IEnchantable obj)
        {
            return obj.GetEnchantmentManager().GetEnchantments();
        }

        [Extension]
        public static HashSet<EnchantmentInstance> GetRemoteEnchantments(IEnchantable obj, IEnchantable target)
        {
            return obj.GetEnchantmentManager().GetRemoteEnchantments(target);
        }

        public static bool IsEnchantmentAlreadyOnTarget(IEnchantable obj, Enchantment e, object owner, int countdown, string parameters, int dispelCost)
        {
            bool flag;
            using (List<EnchantmentInstance>.Enumerator enumerator = GetEnchantments(obj).GetEnumerator())
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        EnchantmentInstance current = enumerator.Current;
                        if ((current.source != e) || (current.owner != owner))
                        {
                            continue;
                        }
                        if ((current.countDown > -1) && (countdown > -1))
                        {
                            current.countDown += countdown;
                        }
                        if ((current.dispelCost > 0) || (dispelCost > 0))
                        {
                            current.dispelCost = Math.Max(current.dispelCost, dispelCost);
                        }
                        if (!string.IsNullOrEmpty(current.parameters) || !string.IsNullOrEmpty(parameters))
                        {
                            current.parameters = CombineStringParameters(current.parameters, parameters);
                        }
                        flag = true;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                }
            }
            return flag;
        }

        [Extension]
        public static bool IsIteratingEnchantments(IEnchantable obj)
        {
            return (obj.GetEnchantmentManager().localActiveIterators > 0);
        }

        [Extension]
        public static void ProcessFIntScripts(IEnchantable obj, EEnchantmentType eType, ref FInt value)
        {
            obj.GetEnchantmentManager().ProcessFIntScripts(eType, ref value);
        }

        [Extension]
        public static void ProcessFloatScripts(IEnchantable obj, EEnchantmentType eType, ref float value)
        {
            obj.GetEnchantmentManager().ProcessFloatScripts(eType, ref value);
        }

        [Extension]
        public static void ProcessIntigerScripts(IEnchantable obj, EEnchantmentType eType, ref int value)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref value);
        }

        [Extension]
        public static void ProcessIntigerScripts(IEnchantable obj, EEnchantmentType eType, ref int income, ref int upkeep)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref income, ref upkeep);
        }

        [Extension]
        public static void ProcessIntigerScripts(IEnchantable obj, EEnchantmentType eType, ref int income, StatDetails details)
        {
            obj.GetEnchantmentManager().ProcessIntigerScripts(eType, ref income, details);
        }

        [Extension]
        public static void RemoveEnchantment(IEnchantable obj, Enchantment e)
        {
            EnchantmentInstance args = obj.GetEnchantmentManager().Remove(e);
            if (e.removalScript != null)
            {
                object[] parameters = new object[] { obj, e, args };
                ScriptLibrary.Call(e.removalScript.script, parameters);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            MOM.Location location = obj as MOM.Location;
            if ((location != null) && (location.GetUnits() != null))
            {
                using (List<Reference<MOM.Unit>>.Enumerator enumerator = location.GetUnits().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Get().attributes.SetDirty();
                    }
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), args);
        }

        [Extension]
        public static void RemoveEnchantment(IEnchantable obj, EnchantmentInstance e)
        {
            EnchantmentInstance args = obj.GetEnchantmentManager().Remove(e);
            if (e.source.Get().removalScript != null)
            {
                object[] parameters = new object[] { obj, e.source.Get(), args };
                ScriptLibrary.Call(e.source.Get().removalScript.script, parameters);
            }
            if (obj is IAttributable)
            {
                (obj as IAttributable).GetAttributes().SetDirty();
            }
            MOM.Location location = obj as MOM.Location;
            if ((location != null) && (location.GetUnits() != null))
            {
                using (List<Reference<MOM.Unit>>.Enumerator enumerator = location.GetUnits().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Get().attributes.SetDirty();
                    }
                }
            }
            MHEventSystem.TriggerEvent<EnchantmentManager>(obj.GetEnchantmentManager(), args);
        }

        [Extension]
        public static void TriggerScripts(IEnchantable obj, EEnchantmentType eType, object data, IEnchantable customTarget)
        {
            obj.GetEnchantmentManager().TriggerScripts(eType, data, customTarget);
        }
    }
}

