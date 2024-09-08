using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using MHUtils;
using MOM.Adventures;
using ProtoBuf;
using UnityEngine;
using UnrealByte.EasyJira;
using WorldCode;

namespace MOM
{
    public class SaveManager
    {
        public static bool loadingMode;

        private static string[] extensions = new string[3] { "save", "metasave", "save_caches" };

        private static int lastSave = -1;

        private static SaveBlock CollectData()
        {
            SaveBlock saveBlock = new SaveBlock();
            GameManager.Get().customDBUsed |= GameVersion.IsDBModified();
            saveBlock.entityManager = EntityManager.Get();
            saveBlock.worldSeed = World.Get().seed;
            saveBlock.worldSizeSetting = World.Get().worldSizeSetting;
            saveBlock.turnNumber = TurnManager.GetTurnNumber();
            saveBlock.settings = DifficultySettingsData.Current;
            Color[] arcanusData = FOW.Get().GetArcanusData();
            Color[] myrrorData = FOW.Get().GetMyrrorData();
            float[] array = new float[arcanusData.Length * 4];
            float[] array2 = new float[myrrorData.Length * 4];
            for (int i = 0; i < arcanusData.Length; i++)
            {
                Color color = arcanusData[i];
                array[i * 4] = color.r;
                array[i * 4 + 1] = color.g;
                array[i * 4 + 2] = color.b;
                array[i * 4 + 3] = color.a;
                color = myrrorData[i];
                array2[i * 4] = color.r;
                array2[i * 4 + 1] = color.g;
                array2[i * 4 + 2] = color.b;
                array2[i * 4 + 3] = color.a;
            }
            saveBlock.arcanusData = array;
            saveBlock.myrrorData = array2;
            saveBlock.CollectAdventureData();
            return saveBlock;
        }

        public static void UpdateLastSave()
        {
            SaveManager.lastSave = TurnManager.Get().turnNumber;
        }

        public static IEnumerator AutoSave()
        {
            while (!GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Movement) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Adventure) || !GameManager.Get().IsFocusFreeFrom(GameManager.FocusFlag.Battle))
            {
                yield return null;
            }
            if (GameManager.GetHumanWizard().GetWizardStatus(updateTracker: false) == PlayerWizard.WizardStatus.Killed)
            {
                yield break;
            }
            int num = TurnManager.Get().turnNumber - SaveManager.lastSave;
            int autosaveFrequency = Settings.GetAutosaveFrequency();
            if (autosaveFrequency == 0 || num < autosaveFrequency || !string.IsNullOrEmpty(TLog.firstCriticalHeader) || !EntityManager.IsCoherent())
            {
                yield break;
            }
            string text = Path.Combine(MHApplication.PROFILES, "Autosave_");
            for (int num2 = 4; num2 > 0; num2--)
            {
                string text2 = text + num2;
                string text3 = text + (num2 + 1);
                bool openWarning = false;
                string[] array = SaveManager.extensions;
                foreach (string text4 in array)
                {
                    try
                    {
                        string text5 = text3 + "." + text4;
                        string text6 = text2 + "." + text4;
                        if (File.Exists(text5))
                        {
                            File.Delete(text5);
                        }
                        if (File.Exists(text6))
                        {
                            if (!text6.EndsWith(".metasave"))
                            {
                                goto IL_019c;
                            }
                            SaveMeta saveMeta = SaveManager.LoadMeta(text6);
                            if (saveMeta != null)
                            {
                                saveMeta.saveName = "Autosave_" + (num2 + 1);
                                SaveManager.SaveMeta(text6, saveMeta);
                                goto IL_019c;
                            }
                            File.Delete(text6);
                        }
                        goto end_IL_0115;
                        IL_019c:
                        File.Move(text6, text5);
                        end_IL_0115:;
                    }
                    catch (Exception ex)
                    {
                        if (ex is IOException || ex is UnauthorizedAccessException)
                        {
                            openWarning = true;
                            PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_FILE_WRITE_FORBIDDEN", "UI_OK", delegate
                            {
                                openWarning = false;
                            });
                        }
                        else
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
                if (openWarning)
                {
                    while (openWarning)
                    {
                        yield return null;
                    }
                    yield break;
                }
            }
            Exception ex2 = SaveManager.SaveGame(World.Get().seed, "Autosave_1");
            if (ex2 != null)
            {
                PopupGeneral.OpenPopup(HUD.Get(), "UI_AUTOSAVE_ERROR", ex2.Message, "UI_OKAY");
            }
            yield return null;
        }

        public static Exception SaveGame(int gameID, string appliedName, bool checkForError = true)
        {
            try
            {
                if (checkForError && (!string.IsNullOrEmpty(TLog.firstCriticalHeader) || !EntityManager.IsCoherent()))
                {
                    PopupGeneral.OpenPopup(null, "UI_SAVE_FAILED", "UI_GAME_IS_DAMAGED_CANNOT_SAVE", "UI_OKAY");
                    return null;
                }
                string pROFILES = MHApplication.PROFILES;
                if (!Directory.Exists(pROFILES))
                {
                    Directory.CreateDirectory(pROFILES);
                }
                SaveManager.Save(SaveManager.CollectData(), appliedName + ".save", pROFILES, getHash: false);
                SaveMeta m = new SaveMeta(appliedName);
                SaveManager.SaveMeta(Path.Combine(pROFILES, appliedName) + ".metasave", m);
                SaveManager.UpdateLastSave();
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is UnauthorizedAccessException)
                {
                    PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_FILE_WRITE_FORBIDDEN", "UI_OK");
                    return null;
                }
                Debug.LogError("InnerException: " + ex.InnerException?.ToString() + "\n" + ex);
                return ex;
            }
            return null;
        }

        private static void SaveMeta(string path, SaveMeta m)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveMeta));
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlWriter xmlWriter = new XmlTextWriter(memoryStream, Encoding.Unicode)
                {
                    Formatting = Formatting.Indented
                };
                xmlSerializer.Serialize(xmlWriter, m);
                memoryStream.Position = 0L;
                byte[] bytes = memoryStream.ToArray();
                try
                {
                    File.WriteAllBytes(path, bytes);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_FILE_WRITE_FORBIDDEN", "UI_OK");
                        return;
                    }
                }
                memoryStream.Close();
            }
        }

        private static void DeleteFile(string path, string extension)
        {
            path = path + "." + extension;
            File.Delete(path);
        }

        public static void DeleteSaveGame(SaveMeta info)
        {
            string path = Path.Combine(MHApplication.PROFILES, info.saveName);
            SaveManager.DeleteFile(path, "save");
            SaveManager.DeleteFile(path, "metasave");
        }

        private static void Save(SaveBlock data, string name, string path, bool getHash)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, data);
                memoryStream.Position = 0L;
                string path2 = Path.Combine(path, name);
                byte[] bytes = memoryStream.ToArray();
                try
                {
                    File.WriteAllBytes(path2, bytes);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        PopupGeneral.OpenPopup(null, "UI_WARNING", "UI_FILE_WRITE_FORBIDDEN", "UI_OK");
                    }
                }
            }
        }

        public static List<SaveMeta> GetAvaliableSaves()
        {
            List<SaveMeta> list = new List<SaveMeta>();
            try
            {
                string pROFILES = MHApplication.PROFILES;
                if (!Directory.Exists(pROFILES))
                {
                    return null;
                }
                string[] files = Directory.GetFiles(pROFILES);
                if (files == null)
                {
                    return null;
                }
                string[] array = files;
                foreach (string text in array)
                {
                    if (text.EndsWith(".metasave"))
                    {
                        SaveMeta saveMeta = SaveManager.LoadMeta(text);
                        if (saveMeta != null)
                        {
                            list.Add(saveMeta);
                        }
                    }
                }
                return list;
            }
            catch (Exception message)
            {
                Debug.LogError(message);
                return null;
            }
        }

        public static bool IsAnySaveAvailable()
        {
            string pROFILES = MHApplication.PROFILES;
            if (!Directory.Exists(pROFILES))
            {
                return false;
            }
            string[] files = Directory.GetFiles(pROFILES);
            if (files == null || files.Length < 2)
            {
                return false;
            }
            return true;
        }

        private static SaveMeta LoadMeta(string path)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveMeta));
                using (Stream stream = new FileStream(path, FileMode.Open))
                {
                    stream.Position = 0L;
                    SaveMeta obj = (SaveMeta)xmlSerializer.Deserialize(stream);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
                    obj.saveName = fileNameWithoutExtension;
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("One of the meta files is damaged! Warning! you may want to remove or fix save  \n" + path + "\n" + ex);
                return null;
            }
        }

        public static SaveBlock Load(SaveMeta meta)
        {
            string saveName = meta.saveName;
            SaveBlock saveBlock = null;
            try
            {
                if (AdventureLibrary.currentLibrary != null)
                {
                    AdventureLibrary.currentLibrary.AdventureLocalization();
                }
                byte[] array = File.ReadAllBytes(Path.Combine(MHApplication.PROFILES, saveName + ".save"));
                using (MemoryStream memoryStream = new MemoryStream(array))
                {
                    memoryStream.Write(array, 0, array.Length);
                    memoryStream.Position = 0L;
                    saveBlock = Serializer.Deserialize<SaveBlock>(memoryStream);
                    saveBlock.RestoreAdventureData();
                    return saveBlock;
                }
            }
            catch (Exception message)
            {
                Debug.LogError(message);
                return null;
            }
        }

        public static IEnumerator LoadCache()
        {
            if (World.Get().gameID == 0)
            {
                yield break;
            }
            string cACHE = MHApplication.CACHE;
            string sFile = Path.Combine(cACHE, World.Get().gameID + "_" + Settings.GetMeshQuality() + ".bin");
            if (File.Exists(sFile))
            {
                Thread t = new Thread((ThreadStart)delegate
                {
                    MemoryStream memoryStream = null;
                    try
                    {
                        memoryStream = new MemoryStream(File.ReadAllBytes(sFile))
                        {
                            Position = 0L
                        };
                        ProtoLibrary protoLibrary = Serializer.Deserialize<ProtoLibrary>(memoryStream);
                        protoLibrary.serialized = memoryStream;
                        ProtoLibrary.SetInstance(protoLibrary);
                    }
                    catch (Exception)
                    {
                        memoryStream?.Dispose();
                        ProtoLibrary.Clear();
                    }
                });
                t.Start();
                while (t.IsAlive)
                {
                    yield return null;
                }
            }
            else
            {
                ProtoLibrary.Clear();
            }
        }
    }
}
