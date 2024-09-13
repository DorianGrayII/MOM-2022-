using System;
using System.Collections.Generic;
using System.IO;
using MHUtils;
using MHUtils.UI;
using MOM;
using ProtoBuf;
using UnityEngine;
using WorldCode;

[ProtoContract]
public class SettingsBlock
{
    [ProtoMember(1)]
    public List<string> optionNames;

    [ProtoMember(2)]
    public List<string> optionValues;

    [ProtoMember(3)]
    public int animationSpeed;

    [ProtoMember(4)]
    public int battleSpeed;

    [ProtoMember(5)]
    public bool followAIMovement;

    [ProtoMember(6)]
    public bool onlyManualBattleCamera;

    [ProtoMember(7)]
    public bool autoEndTurn;

    [ProtoMember(8)]
    public bool edgescrolling;

    [ProtoMember(9)]
    public NetDictionary<Settings.KeyActions, KeyCode> curentKeyMapping;

    [ProtoMember(10)]
    public int experimentalLoading;

    [ProtoMember(11)]
    public bool townMap;

    [ProtoMember(12)]
    public string townSortOption;

    [ProtoMember(13)]
    public bool autoNextUnit;

    private Dictionary<string, object> values;

    public static SettingsBlock Factory()
    {
        return new SettingsBlock
        {
            followAIMovement = true,
            onlyManualBattleCamera = false,
            autoEndTurn = false,
            // changing the defalut to 4
            // animationSpeed = 1,
            animationSpeed = 4,
            // changing the default to 4
            // battleSpeed = 1,
            battleSpeed = 4,
            edgescrolling = true,
            autoNextUnit = false,
            experimentalLoading = 2,
            townMap = false,
            townSortOption = null
        };
    }

    [ProtoAfterDeserialization]
    public void AfterDeserialize()
    {
        if (this.battleSpeed == 0)
        {
            this.battleSpeed = 1;
        }
        if (this.animationSpeed == 0)
        {
            this.animationSpeed = 1;
        }
        if (this.experimentalLoading == 0)
        {
            this.experimentalLoading = 2;
        }
    }

    public T Get<T>(Settings.Name e)
    {
        return this.Get<T>(e.ToString());
    }

    public T Get<T>(string s)
    {
        if (this.values == null)
        {
            this.values = new Dictionary<string, object>();
        }
        if (!this.values.ContainsKey(s))
        {
            int num = -1;
            if (this.optionNames != null)
            {
                num = this.optionNames.IndexOf(s);
            }
            if (num < 0)
            {
                if (s == Settings.Name.aiAnimationSpeed.ToString())
                {
                    return (T)(object)1;
                }
                if (s == Settings.Name.playerAnimationSpeed.ToString())
                {
                    return (T)(object)0;
                }
                if (s == Settings.Name.musicVolume.ToString())
                {
                    return (T)(object)50;
                }
                if (s == Settings.Name.sfxVolume.ToString())
                {
                    return (T)(object)75;
                }
                if (s == Settings.Name.arcanusGamma.ToString())
                {
                    return (T)(object)60;
                }
                if (s == Settings.Name.myrrorGamma.ToString())
                {
                    return (T)(object)70;
                }
                return default(T);
            }
            try
            {
                if (typeof(T) == typeof(int))
                {
                    this.values[s] = Convert.ToInt32(this.optionValues[num]);
                }
                else if (typeof(T) == typeof(float))
                {
                    this.values[s] = Convert.ToSingle(this.optionValues[num]);
                }
                else if (typeof(T) == typeof(bool))
                {
                    this.values[s] = Convert.ToBoolean(this.optionValues[num]);
                }
                else if (typeof(T) == typeof(string))
                {
                    this.values[s] = this.optionValues[num];
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(this.optionValues[num] + " for " + s + "invalid conversion \n" + ex);
            }
        }
        if (!(this.values[s] is T))
        {
            this.values[s] = (T)this.values[s];
        }
        return (T)this.values[s];
    }

    public int GetExperimentalLoading()
    {
        return this.experimentalLoading;
    }

    public int GetAnimationSpeed()
    {
        return this.animationSpeed;
    }

    public int GetBattleAnimationSpeed()
    {
        return this.battleSpeed;
    }

    public void UpdateAnimationSpeed()
    {
        if (World.Get() != null && World.GetActivePlane() != null && World.GetActivePlane().battlePlane)
        {
            this.SetBattleAnimationSpeed();
        }
        else if (TurnManager.Get(allowNull: true) != null && TurnManager.Get().playerTurn)
        {
            this.animationSpeed = this.GetPlayerAnimationSpeedValue();
        }
        else
        {
            this.animationSpeed = this.Get<int>(Settings.Name.aiAnimationSpeed);
        }
    }

    public int GetPlayerAnimationSpeedValue()
    {
        switch (this.Get<int>(Settings.Name.playerAnimationSpeed))
        {
        case 0:
            return 1;
        case 1:
            return 2;
        case 2:
            return 4;
        case 3:
            return 200;
        default:
            return 1;
        }
    }

    public void SetExperimentalLoading(int value)
    {
        this.experimentalLoading = value;
    }

    public void SetBattleAnimationSpeed()
    {
        this.animationSpeed = this.battleSpeed;
    }

    public void SetBattleAnimationSpeed(int x)
    {
        this.battleSpeed = x;
        this.SetBattleAnimationSpeed();
    }

    public void SetFollowEnemyMovement(bool follow)
    {
        this.followAIMovement = follow;
    }

    public void SetBattleCameraFollow(bool follow)
    {
        this.onlyManualBattleCamera = !follow;
    }

    public void SetAutoEndTurn(bool autoEnd)
    {
        this.autoEndTurn = autoEnd;
    }

    public void SetEdgescrolling(bool scroll)
    {
        this.edgescrolling = scroll;
    }

    public void SetAutoNextUnit(bool autoNext)
    {
        this.autoNextUnit = autoNext;
    }

    public void SetTownMap(bool map)
    {
        this.townMap = map;
    }

    public void SetTownSortOption(string sortOpt)
    {
        this.townSortOption = sortOpt;
    }

    public void Set<T>(T key, string value) where T : struct
    {
        this.Set(key.ToString(), value);
    }

    public void Set(string key, string value)
    {
        this.EnsureInitialization();
        int num = this.optionNames.FindIndex((string o) => o == key);
        if (num < 0)
        {
            this.optionNames.Add(key);
            this.optionValues.Add(value);
        }
        else
        {
            this.optionValues[num] = value;
        }
        this.values = null;
    }

    public void RemoveKey(string key)
    {
        if (this.optionNames != null)
        {
            int num = this.optionNames.FindIndex((string o) => o == key);
            if (num >= 0)
            {
                this.optionNames.RemoveAt(num);
                this.optionValues.RemoveAt(num);
            }
        }
    }

    public void EnsureInitialization()
    {
        if (this.optionNames == null)
        {
            this.optionNames = new List<string>();
            this.optionValues = new List<string>();
        }
    }

    public static KeyCode GetKeyForAction(Settings.KeyActions action)
    {
        SettingsBlock data = Settings.GetData();
        if (data.curentKeyMapping == null)
        {
            data.curentKeyMapping = new NetDictionary<Settings.KeyActions, KeyCode>();
        }
        if (!Settings.GetData().curentKeyMapping.ContainsKey(action))
        {
            if (!Settings.defaultMapping.ContainsKey(action))
            {
                Debug.LogError("unknown action! mapping will not be created for it");
                return KeyCode.KeypadMinus;
            }
            Settings.GetData().curentKeyMapping[action] = Settings.defaultMapping[action];
        }
        return Settings.GetData().curentKeyMapping[action];
    }

    public static bool IsKeyDown(Settings.KeyActions action)
    {
        if (UIManager.IsInputConsumed())
        {
            return false;
        }
        if (action == Settings.KeyActions.None)
        {
            return false;
        }
        return Input.GetKeyDown(SettingsBlock.GetKeyForAction(action));
    }

    public static bool IsKeyUp(Settings.KeyActions action)
    {
        if (UIManager.IsInputConsumed())
        {
            return false;
        }
        if (action == Settings.KeyActions.None)
        {
            return false;
        }
        return Input.GetKeyUp(SettingsBlock.GetKeyForAction(action));
    }

    public static bool IsKey(Settings.KeyActions action)
    {
        if (UIManager.IsInputConsumed())
        {
            return false;
        }
        if (action == Settings.KeyActions.None)
        {
            return false;
        }
        return Input.GetKey(SettingsBlock.GetKeyForAction(action));
    }

    public void SetKey(Settings.KeyActions action, KeyCode keyCode)
    {
        foreach (KeyValuePair<Settings.KeyActions, KeyCode> item in Settings.defaultMapping)
        {
            if (SettingsBlock.GetKeyForAction(item.Key) == keyCode)
            {
                this.curentKeyMapping[item.Key] = KeyCode.None;
            }
        }
        this.curentKeyMapping[action] = keyCode;
        SettingsBlock.Save(this);
    }

    public void ClearPrefix(string prefix)
    {
        if (this.optionNames != null)
        {
            for (int num = this.optionNames.Count - 1; num >= 0; num--)
            {
                if (this.optionNames[num].StartsWith(prefix))
                {
                    this.optionNames.RemoveAt(num);
                    this.optionValues.RemoveAt(num);
                }
            }
        }
        this.values = null;
    }

    public bool HavePrefix(string prefix)
    {
        if (this.optionNames != null)
        {
            for (int num = this.optionNames.Count - 1; num >= 0; num--)
            {
                if (this.optionNames[num].StartsWith(prefix))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool GetFollowAIMovement()
    {
        return this.followAIMovement;
    }

    public bool GetBattleCameraFollow()
    {
        return !this.onlyManualBattleCamera;
    }

    public bool GetAutoEndTurn()
    {
        return this.autoEndTurn;
    }

    public bool GetEdgescrolling()
    {
        return this.edgescrolling;
    }

    public bool GetAutoNextUnit()
    {
        return this.edgescrolling;
    }

    public bool GetTownMap()
    {
        return this.townMap;
    }

    public string GetTownSortOption()
    {
        return this.townSortOption;
    }

    public static void Save(SettingsBlock block)
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        string path = Path.Combine(pROFILES, "settings.bin");
        using (MemoryStream memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, block);
            memoryStream.Position = 0L;
            byte[] bytes = memoryStream.ToArray();
            File.WriteAllBytes(path, bytes);
        }
    }

    public static SettingsBlock Load()
    {
        string pROFILES = MHApplication.PROFILES;
        if (!Directory.Exists(pROFILES))
        {
            Directory.CreateDirectory(pROFILES);
        }
        string path = Path.Combine(pROFILES, "settings.bin");
        if (!File.Exists(path))
        {
            return SettingsBlock.Factory();
        }
        try
        {
            byte[] array = File.ReadAllBytes(path);
            using (MemoryStream memoryStream = new MemoryStream(array))
            {
                memoryStream.Write(array, 0, array.Length);
                memoryStream.Position = 0L;
                return Serializer.Deserialize<SettingsBlock>(memoryStream);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Warning! Settings cannot be loaded: \n" + ex);
            return SettingsBlock.Factory();
        }
    }
}
