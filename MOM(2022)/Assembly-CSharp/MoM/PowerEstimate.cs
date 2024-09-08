using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.NeuralNetwork.PowerEstimation2;
using ProtoBuf;
using UnityEngine;
using WorldCode;

namespace MOM
{
    [ProtoContract]
    public class PowerEstimate
    {
        private enum SpellTests
        {
            eNone = 0,
            eOnce = 1,
            eEachTime = 2,
            MAX = 3
        }

        private static SpellTests spellTests = SpellTests.eNone;

        public static List<BattleUnit> powerOrder;

        private static List<ITask> pool = new List<ITask>();

        private static PowerEstimate instance;

        [ProtoMember(1)]
        public List<Multitype<BattleUnit, int>> battleResults;

        public static List<Multitype<BattleUnit, int>> powerValues;

        public static void LoadData()
        {
            PowerEstimate.powerOrder = new List<BattleUnit>();
            List<Subrace> list = new List<Subrace>();
            list.AddRange(DataBase.GetType<global::DBDef.Unit>());
            list.AddRange(DataBase.GetType<Hero>());
            foreach (Subrace item in list)
            {
                Unit unit = Unit.CreateFrom(item, simulation: true);
                PowerEstimate.powerOrder.Add(BattleUnit.Create(unit, abstractMode: true));
                unit.Destroy();
            }
            PowerEstimate.powerOrder.Sort(delegate(BattleUnit A, BattleUnit B)
            {
                float fakePower = A.GetFakePower();
                float fakePower2 = B.GetFakePower();
                return fakePower.CompareTo(fakePower2);
            });
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < PowerEstimate.powerOrder.Count; i++)
            {
                BattleFigure baseFigure = PowerEstimate.powerOrder[i].GetBaseFigure();
                stringBuilder.AppendLine(PowerEstimate.powerOrder[i].dbSource.dbName + " A:" + baseFigure.attack + " HP:" + baseFigure.maxHitPoints + " Def:" + baseFigure.defence + " Fig:" + PowerEstimate.powerOrder[i].GetMaxFigureCount());
            }
        }

        public static PowerEstimate Get()
        {
            if (PowerEstimate.instance == null)
            {
                string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "powerTrainingData.bin");
                if (!File.Exists(path))
                {
                    return null;
                }
                try
                {
                    byte[] array = File.ReadAllBytes(path);
                    using (MemoryStream memoryStream = new MemoryStream(array))
                    {
                        memoryStream.Write(array, 0, array.Length);
                        memoryStream.Position = 0L;
                        PowerEstimate.instance = Serializer.Deserialize<PowerEstimate>(memoryStream);
                    }
                }
                catch (Exception message)
                {
                    Debug.LogWarning(message);
                    return null;
                }
            }
            return PowerEstimate.instance;
        }

        private static int GetSingleUnitValue(List<BattleUnit> challengedBase, BattleUnit unit)
        {
            float num = 3 * (challengedBase.Count - 1) + 1;
            float num2 = (float)(int)new BattleTask
            {
                random = new MHRandom(global::UnityEngine.Random.Range(int.MinValue, int.MaxValue)),
                defenderSources = challengedBase,
                attacker = unit
            }.Execute() / num;
            int num3 = Mathf.RoundToInt(4500f * num2);
            if (num3 < 1000)
            {
                return PowerEstimate.Scaling(0, 1000, 0, 200, num3);
            }
            if (num3 < 2000)
            {
                return PowerEstimate.Scaling(1000, 2000, 200, 300, num3);
            }
            if (num3 < 3000)
            {
                return PowerEstimate.Scaling(2000, 3000, 300, 400, num3);
            }
            if (num3 < 3250)
            {
                return PowerEstimate.Scaling(3000, 3250, 400, 600, num3);
            }
            if (num3 < 4000)
            {
                return PowerEstimate.Scaling(3250, 4000, 600, 2000, num3);
            }
            return PowerEstimate.Scaling(4000, 4500, 2000, 4500, num3);
        }

        public static IEnumerator SortByFight(List<BattleUnit> baseUnits, List<BattleUnit> units, string fileName, ModOrder m)
        {
            List<Multitype<BattleUnit, int>> battleResults = new List<Multitype<BattleUnit, int>>();
            bool changed = false;
            MHTimer t = MHTimer.StartNew();
            PowerEstimate p = null;
            for (int i = 0; i < units.Count; i++)
            {
                if (i > 0)
                {
                    yield return null;
                }
                BattleUnit unit = units[i];
                if (unit.dbSource.Get().strategicValueOverride > 0)
                {
                    battleResults.Add(new Multitype<BattleUnit, int>(unit, unit.dbSource.Get().strategicValueOverride));
                    continue;
                }
                if (p == null)
                {
                    p = PowerEstimate.LoadPowerEstimate(m);
                    if (p == null)
                    {
                        p = new PowerEstimate();
                    }
                }
                if (p.battleResults != null)
                {
                    Multitype<BattleUnit, int> multitype = p.battleResults.Find((Multitype<BattleUnit, int> x) => x.t0.dbSource.Get() == unit.dbSource.Get());
                    if (multitype != null)
                    {
                        battleResults.Add(multitype);
                        continue;
                    }
                }
                int singleUnitValue = PowerEstimate.GetSingleUnitValue(baseUnits, unit);
                battleResults.Add(new Multitype<BattleUnit, int>(unit, singleUnitValue));
                changed = true;
                Debug.Log(i + " Power sim: " + unit.dbSource?.ToString() + " " + t.GetTime());
            }
            p.battleResults = battleResults;
            if (PowerEstimate.instance == null)
            {
                PowerEstimate.instance = p;
            }
            if (changed)
            {
                p.SaveToDrive(fileName, m);
                p.ExportToDrive(fileName, m);
            }
            if (PowerEstimate.powerValues == null)
            {
                PowerEstimate.powerValues = new List<Multitype<BattleUnit, int>>();
            }
            foreach (Multitype<BattleUnit, int> v in battleResults)
            {
                int num = PowerEstimate.powerValues.FindIndex((Multitype<BattleUnit, int> o) => o.t0.GetDBName() == v.t0.GetDBName());
                if (num >= 0)
                {
                    PowerEstimate.powerValues[num] = v;
                }
                else
                {
                    PowerEstimate.powerValues.Add(v);
                }
            }
        }

        private static PowerEstimate LoadPowerEstimate(ModOrder m)
        {
            if (m == null)
            {
                string path = Path.Combine(MHApplication.EXTERNAL_ASSETS, "powerTrainingData.bin");
                if (File.Exists(path))
                {
                    return PowerEstimate.LoadPowerEstimate(path);
                }
                return null;
            }
            string path2 = Path.Combine(MHApplication.EXTERNAL_ASSETS, m.name + ".bin");
            if (File.Exists(path2))
            {
                return PowerEstimate.LoadPowerEstimate(path2);
            }
            path2 = m.GetPath();
            string[] files = Directory.GetFiles(path2, m.name + ".bin", SearchOption.AllDirectories);
            if (files.Length == 0)
            {
                return null;
            }
            return PowerEstimate.LoadPowerEstimate(files[0]);
        }

        private static PowerEstimate LoadPowerEstimate(string path)
        {
            try
            {
                byte[] array = File.ReadAllBytes(path);
                using (MemoryStream memoryStream = new MemoryStream(array))
                {
                    memoryStream.Write(array, 0, array.Length);
                    memoryStream.Position = 0L;
                    return Serializer.Deserialize<PowerEstimate>(memoryStream);
                }
            }
            catch (Exception message)
            {
                Debug.LogWarning(message);
            }
            return null;
        }

        private static int Scaling(int oldRangeMin, int oldRangeMax, int newRangeMin, int newRangeMax, int value)
        {
            int num = oldRangeMax - oldRangeMin;
            float num2 = (float)(value - oldRangeMin) / (float)num;
            int num3 = newRangeMax - newRangeMin;
            return newRangeMin + (int)(num2 * (float)num3);
        }

        private void SaveToDrive(string name, ModOrder m)
        {
            string text = MHApplication.EXTERNAL_ASSETS;
            if (m != null)
            {
                text = m.GetPath();
            }
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            MemoryStream memoryStream = new MemoryStream();
            Serializer.Serialize(memoryStream, this);
            text = Path.Combine(text, name + ".bin");
            using (FileStream destination = new FileStream(text, FileMode.Create, FileAccess.Write))
            {
                memoryStream.Position = 0L;
                memoryStream.CopyTo(destination);
            }
            memoryStream.Dispose();
        }

        public void ExportToDrive(string name, ModOrder m)
        {
            string text = MHApplication.EXTERNAL_ASSETS;
            if (m != null)
            {
                text = m.GetPath();
            }
            if (!Directory.Exists(text))
            {
                Directory.CreateDirectory(text);
            }
            StringBuilder stringBuilder = new StringBuilder();
            StringBuilder stringBuilder2 = new StringBuilder();
            List<NNUnit> list = new List<NNUnit>();
            foreach (Multitype<BattleUnit, int> battleResult in this.battleResults)
            {
                list.Add(new NNUnit(battleResult.t0, 0));
            }
            stringBuilder.Append("X,");
            foreach (Multitype<BattleUnit, int> battleResult2 in this.battleResults)
            {
                stringBuilder.Append(battleResult2.t0.dbSource?.ToString() + ",");
            }
            stringBuilder.AppendLine("|");
            stringBuilder.Append("!Power!,");
            foreach (Multitype<BattleUnit, int> battleResult3 in this.battleResults)
            {
                stringBuilder.Append(battleResult3.t1 + ",");
                stringBuilder2.AppendLine(battleResult3.t0.dbSource.dbName + " \t\t " + battleResult3.t1);
            }
            stringBuilder.AppendLine("|");
            stringBuilder.Append("Figures,");
            foreach (Multitype<BattleUnit, int> battleResult4 in this.battleResults)
            {
                stringBuilder.Append(battleResult4.t0.GetMaxFigureCount() + ",");
            }
            stringBuilder.AppendLine("|");
            foreach (TAG unitParamsPair in NNUnit.unitParamsPairs)
            {
                stringBuilder.Append("TAG_" + unitParamsPair.ToString() + ",");
                foreach (Multitype<BattleUnit, int> battleResult5 in this.battleResults)
                {
                    stringBuilder.Append(battleResult5.t0.attributes.GetFinal(unitParamsPair).ToString() + ",");
                }
                stringBuilder.AppendLine("|");
            }
            foreach (TAG unitParam in NNUnit.unitParams)
            {
                stringBuilder.Append("TAG_" + unitParam.ToString() + ",");
                foreach (Multitype<BattleUnit, int> battleResult6 in this.battleResults)
                {
                    stringBuilder.Append(battleResult6.t0.attributes.GetFinal(unitParam).ToString() + ",");
                }
                stringBuilder.AppendLine("|");
            }
            foreach (string v in NNUnit.unitSkills)
            {
                stringBuilder.Append("TAG_" + v + ",");
                foreach (Multitype<BattleUnit, int> battleResult7 in this.battleResults)
                {
                    stringBuilder.Append(battleResult7.t0.GetSkills().Find((DBReference<Skill> o) => o.dbName.Contains(v))?.ToString() + ",");
                }
                stringBuilder.AppendLine("|");
            }
            File.WriteAllText(Path.Combine(text, name + ".txt"), stringBuilder.ToString());
            File.WriteAllText(Path.Combine(text, name + "_Short.txt"), stringBuilder2.ToString());
        }

        private static void LogToConsole()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < PowerEstimate.powerValues.Count; i++)
            {
                BattleFigure baseFigure = PowerEstimate.powerValues[i].t0.GetBaseFigure();
                stringBuilder.AppendLine("WCount: " + PowerEstimate.powerValues[i].t1 + " A:" + baseFigure.attack + " RA:" + baseFigure.rangedAttack + " HP:" + baseFigure.maxHitPoints + " Def:" + baseFigure.defence + " Fig:" + PowerEstimate.powerValues[i].t0.GetMaxFigureCount() + " " + PowerEstimate.powerValues[i].t0.dbSource.dbName);
            }
            Debug.Log(stringBuilder.ToString());
        }

        public static List<Multitype<BattleUnit, int>> GetList()
        {
            return PowerEstimate.powerValues;
        }

        public static IEnumerator SimulatedBattle(Battle source, int iterations, BattleResult returnResult, int pickBest = 0, bool aUseMana = true, bool dUseMana = true)
        {
            _ = Time.timeSinceLevelLoad;
            if (source != null)
            {
                source.simulation = true;
            }
            MemoryStream aMS = new MemoryStream();
            MemoryStream dMS = new MemoryStream();
            Serializer.Serialize(aMS, source.attackerUnits);
            Serializer.Serialize(dMS, source.defenderUnits);
            MemoryStream apMS = new MemoryStream();
            MemoryStream dpMS = new MemoryStream();
            Serializer.Serialize(apMS, source.GetPlayer(attacker: true));
            Serializer.Serialize(dpMS, source.GetPlayer(attacker: false));
            bool results = false;
            int chosenRank = 0;
            for (int i = 0; i < iterations; i++)
            {
                aMS.Position = 0L;
                dMS.Position = 0L;
                List<BattleUnit> aList = Serializer.Deserialize<List<BattleUnit>>(aMS);
                List<BattleUnit> dList = Serializer.Deserialize<List<BattleUnit>>(dMS);
                aList.ForEach(delegate(BattleUnit aUnit)
                {
                    aUnit.simulated = true;
                });
                dList.ForEach(delegate(BattleUnit dUnit)
                {
                    dUnit.simulated = true;
                });
                apMS.Position = 0L;
                dpMS.Position = 0L;
                BattlePlayer aPlayer = Serializer.Deserialize<BattlePlayer>(apMS);
                BattlePlayer dPlayer = Serializer.Deserialize<BattlePlayer>(dpMS);
                yield return PowerEstimate.ResolveSimlatedBattle(aList, dList, aPlayer, dPlayer, source, aUseMana, dUseMana);
                int resultRank;
                if (!results)
                {
                    results = true;
                    returnResult.attacker = aList;
                    returnResult.defender = dList;
                    returnResult.aManaLeft = aPlayer.mana;
                    returnResult.dManaLeft = dPlayer.mana;
                    resultRank = returnResult.GetResultRank(source.attackerUnits, source.defenderUnits);
                    chosenRank = resultRank;
                }
                else
                {
                    resultRank = new BattleResult
                    {
                        attacker = aList,
                        defender = dList
                    }.GetResultRank(source.attackerUnits, source.defenderUnits);
                    if ((pickBest > 0 && chosenRank < resultRank) || (pickBest < 0 && chosenRank > resultRank))
                    {
                        returnResult.attacker = aList;
                        returnResult.defender = dList;
                        returnResult.aManaLeft = aPlayer.mana;
                        returnResult.dManaLeft = dPlayer.mana;
                        chosenRank = resultRank;
                    }
                }
                if (resultRank > 0)
                {
                    returnResult.RegisterWinByRank(resultRank, attacker: true);
                    returnResult.RegisterWinByRank(1, attacker: false);
                }
                else
                {
                    returnResult.RegisterWinByRank(-resultRank, attacker: false);
                    returnResult.RegisterWinByRank(1, attacker: true);
                }
            }
            if (source != null)
            {
                source.simulation = false;
                List<BattleUnit> list = null;
                foreach (KeyValuePair<BattleUnit, Unit> item in source.buToSource)
                {
                    if (item.Key.simulated)
                    {
                        if (list == null)
                        {
                            list = new List<BattleUnit>();
                        }
                        list.Add(item.Key);
                    }
                }
                if (list != null)
                {
                    foreach (BattleUnit item2 in list)
                    {
                        source.buToSource.Remove(item2);
                    }
                }
            }
            aMS.Dispose();
            dMS.Dispose();
            apMS.Dispose();
            dpMS.Dispose();
        }

        private static IEnumerator ResolveSimlatedBattle(List<BattleUnit> aList, List<BattleUnit> dList, BattlePlayer aPlayer, BattlePlayer dPlayer, Battle forReadOnly_Battle, bool aUseMana, bool dUseMana)
        {
            if (PowerEstimate.spellTests != 0)
            {
                PowerEstimate.TestSpells(aPlayer, dPlayer);
                if (PowerEstimate.spellTests == SpellTests.eOnce)
                {
                    PowerEstimate.spellTests = SpellTests.eNone;
                }
            }
            aList.RandomSort();
            dList.RandomSort();
            BattleMultiTask battleMultiTask = new BattleMultiTask();
            battleMultiTask.random = new MHRandom(global::UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            battleMultiTask.defenders = dList;
            battleMultiTask.attackers = aList;
            battleMultiTask.aPlayer = aPlayer;
            battleMultiTask.dPlayer = dPlayer;
            battleMultiTask.forReadOnly_Battle = forReadOnly_Battle;
            battleMultiTask.aUseMana = aUseMana;
            battleMultiTask.dUseMana = dUseMana;
            battleMultiTask.Execute();
            EntityManager.PurgeSimulationUnits();
            yield break;
        }

        private static void TestSpells(BattlePlayer aPlayer, BattlePlayer dPlayer)
        {
            Debug.LogWarning("[DEBUG] TestSpells starts");
            List<Subrace> obj = ScriptLibrary.Call("SpellTestGroup") as List<Subrace>;
            Group group = new Group(World.GetArcanus(), aPlayer.GetID());
            List<BattleUnit> list = new List<BattleUnit>();
            Group group2 = new Group(World.GetArcanus(), dPlayer.GetID());
            List<BattleUnit> list2 = new List<BattleUnit>();
            foreach (Subrace item in obj)
            {
                group.AddUnit(Unit.CreateFrom(item));
                group2.AddUnit(Unit.CreateFrom(item));
            }
            foreach (Reference<Unit> unit in group.GetUnits())
            {
                list.Add(BattleUnit.Create(unit));
            }
            foreach (Reference<Unit> unit2 in group2.GetUnits())
            {
                list2.Add(BattleUnit.Create(unit2));
            }
            BattlePlayer battlePlayer = ((aPlayer.GetWizardOwner() != null) ? aPlayer : dPlayer);
            ScriptLibrary.Call("TestSpells", battlePlayer.GetWizardOwner(), list, list2);
            group.Destroy();
            group2.Destroy();
            EntityManager.PurgeSimulationUnits();
            Debug.LogWarning("[DEBUG] TestSpells ends");
        }
    }
}
