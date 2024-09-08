using System;
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class TownMap : MonoBehaviour
{
    public Material selectedMaterial;

    private Image selected;

    private Dictionary<string, Image> buildingImages = new Dictionary<string, Image>();

    public void SetTown(TownLocation town, Action<DBReference<Building>> onClick = null)
    {
        this.buildingImages.Clear();
        List<string> bNames = new List<string>();
        town.buildings.ForEach(delegate(DBReference<Building> o)
        {
            bNames.Add(o.Get().GetDescriptionInfo().graphic);
        });
        int popUnits = town.GetPopUnits();
        string text = "";
        Race race = (town.source.Get() as Town).race;
        text = race.visualGroup;
        if (string.IsNullOrEmpty(text))
        {
            if (race == (Race)RACE.GNOLLS || race == (Race)RACE.NOMADS || race == (Race)RACE.BARBARIANS)
            {
                text = "Primitive";
            }
            if (race == (Race)RACE.TROLLS || race == (Race)RACE.KLACKONS || race == (Race)RACE.LIZARDMEN)
            {
                text = "Nature";
            }
            if (race == (Race)RACE.DRACONIANS || race == (Race)RACE.HIGH_ELVES || race == (Race)RACE.DARK_ELVES)
            {
                text = "Magical";
            }
            if (race == (Race)RACE.DWARVES || race == (Race)RACE.ORCS || race == (Race)RACE.BEASTMEN)
            {
                text = "Warlike";
            }
            else if (race == (Race)RACE.HALFLINGS || race == (Race)RACE.HIGH_MEN || string.IsNullOrEmpty(text))
            {
                text = "";
            }
        }
        foreach (Transform item in base.transform)
        {
            string text2 = item.gameObject.name;
            if (text2.StartsWith("X"))
            {
                continue;
            }
            switch (text2)
            {
            case "MapBaseGrassland":
            {
                Hex hexAt = town.GetPlane().GetHexAt(town.GetPosition());
                bool active = !hexAt.HaveFlag(ETerrainType.Desert) && !hexAt.HaveFlag(ETerrainType.Tundra) && !hexAt.HaveFlag(ETerrainType.Swamp);
                item.gameObject.SetActive(active);
                continue;
            }
            case "MapBaseDesert":
            {
                bool active3 = town.GetPlane().GetHexAt(town.GetPosition()).HaveFlag(ETerrainType.Desert);
                item.gameObject.SetActive(active3);
                continue;
            }
            case "MapBaseTundra":
            {
                bool active5 = town.GetPlane().GetHexAt(town.GetPosition()).HaveFlag(ETerrainType.Tundra);
                item.gameObject.SetActive(active5);
                continue;
            }
            case "MapBaseSwamp":
            {
                bool active2 = town.GetPlane().GetHexAt(town.GetPosition()).HaveFlag(ETerrainType.Swamp);
                item.gameObject.SetActive(active2);
                continue;
            }
            case "ForestTrees":
            {
                Hex hexAt2 = town.GetPlane().GetHexAt(town.GetPosition());
                bool active4 = hexAt2.HaveFlag(ETerrainType.Forest) || hexAt2.HaveFlag(ETerrainType.GrassLand) || hexAt2.HaveFlag(ETerrainType.Swamp);
                item.gameObject.SetActive(active4);
                continue;
            }
            case "SummoningCircle":
            {
                PlayerWizard wizard = GameManager.GetWizard(town.owner);
                item.gameObject.SetActive(wizard?.GetSummoningLocation() == town);
                DBReference<Building> dBReference3 = new DBReference<Building>();
                dBReference3.dbName = "SUMMONING_CIRCLE";
                AddRollover(item.gameObject, dBReference3);
                continue;
            }
            case "EnchantmentNaturesEye":
            {
                DBReference<Building> dBReference2 = new DBReference<Building>();
                dBReference2.dbName = "NATURES_EYE";
                AddRollover(item.gameObject, dBReference2);
                continue;
            }
            case "EnchantmentAltarOfBattle":
            {
                DBReference<Building> dBReference4 = new DBReference<Building>();
                dBReference4.dbName = "ALTAR_OF_BATTLE";
                AddRollover(item.gameObject, dBReference4);
                continue;
            }
            case "EnchantmentStreamOfLife":
            {
                DBReference<Building> dBReference = new DBReference<Building>();
                dBReference.dbName = "STREAM_OF_LIFE";
                AddRollover(item.gameObject, dBReference);
                continue;
            }
            }
            if (text2.Contains("PopHouse"))
            {
                int num = text2.IndexOf("_");
                if (num == -1 && !text2.StartsWith(text + "PopHouse"))
                {
                    item.gameObject.SetActive(value: false);
                    continue;
                }
                int num2 = ((num <= -1) ? (text + "PopHouse").Length : "PopHouse".Length);
                string text3 = ((num != -1) ? text2.Substring(num2, num - num2) : text2.Substring(num2));
                int num3 = 1;
                try
                {
                    num3 = Convert.ToInt32(text3);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed number conversion for " + text3 + " at " + text2 + " \n" + ex);
                }
                if (num3 <= popUnits)
                {
                    item.gameObject.SetActive(num == -1);
                    continue;
                }
                if (num == -1)
                {
                    item.gameObject.SetActive(value: false);
                    continue;
                }
                Hex hexAt3 = town.GetPlane().GetHexAt(town.GetPosition());
                bool active6 = !hexAt3.HaveFlag(ETerrainType.Desert) && !hexAt3.HaveFlag(ETerrainType.GrassLand) && !hexAt3.HaveFlag(ETerrainType.Swamp) && !hexAt3.HaveFlag(ETerrainType.Tundra);
                item.gameObject.SetActive(active6);
                continue;
            }
            if (text2.StartsWith("Enchantment"))
            {
                if (town.GetEnchantments().Count > 0)
                {
                    int num4 = text2.IndexOf("_");
                    int num5 = 0;
                    string sName2;
                    if (num4 == -1)
                    {
                        sName2 = text2.Substring(num5);
                    }
                    else
                    {
                        sName2 = text2.Substring(num5, num4 - num5);
                    }
                    if (town.GetEnchantments().Find((EnchantmentInstance o) => o.source.Get().GetDescriptionInfo().graphic == sName2) != null)
                    {
                        item.gameObject.SetActive(num4 == -1);
                    }
                    else
                    {
                        item.gameObject.SetActive(num4 != -1);
                    }
                }
                else
                {
                    item.gameObject.SetActive(value: false);
                }
                continue;
            }
            if (text2.StartsWith("Building"))
            {
                int num6 = text2.IndexOf("_");
                int num7 = 0;
                string sName;
                if (num6 == -1)
                {
                    sName = text2.Substring(num7);
                }
                else
                {
                    sName = text2.Substring(num7, num6 - num7);
                }
                DBReference<Building> dBReference5 = town.buildings.Find((DBReference<Building> o) => o.Get().GetDescriptionInfo().graphic == sName);
                if (dBReference5 != null)
                {
                    GameObject gameObject = item.gameObject;
                    if (num6 == -1)
                    {
                        gameObject.SetActive(value: true);
                        AddRollover(gameObject, dBReference5);
                    }
                    else
                    {
                        gameObject.SetActive(value: false);
                    }
                }
                else
                {
                    Hex hexAt4 = town.GetPlane().GetHexAt(town.GetPosition());
                    bool flag = !hexAt4.HaveFlag(ETerrainType.Desert) && !hexAt4.HaveFlag(ETerrainType.GrassLand) && !hexAt4.HaveFlag(ETerrainType.Swamp) && !hexAt4.HaveFlag(ETerrainType.Tundra);
                    item.gameObject.SetActive(num6 != -1 && flag);
                }
                continue;
            }
            if (!text2.StartsWith("World"))
            {
                continue;
            }
            bool active7 = false;
            if (text2 == "WorldSea")
            {
                global::WorldCode.Plane plane = town.GetPlane();
                Vector3i[] neighbours = HexNeighbors.neighbours;
                for (int i = 0; i < neighbours.Length; i++)
                {
                    Vector3i pos = neighbours[i] + town.GetPosition();
                    Hex hexAt5 = plane.GetHexAt(pos);
                    if (hexAt5 != null && hexAt5.HaveFlag(ETerrainType.Sea))
                    {
                        active7 = true;
                        break;
                    }
                }
            }
            else if (text2 == "WorldRiver")
            {
                Hex hexAt6 = town.GetPlane().GetHexAt(town.GetPosition());
                if (hexAt6.viaRiver != null)
                {
                    bool[] viaRiver = hexAt6.viaRiver;
                    for (int i = 0; i < viaRiver.Length; i++)
                    {
                        if (viaRiver[i])
                        {
                            active7 = true;
                        }
                    }
                }
            }
            item.gameObject.SetActive(active7);
        }
        void AddRollover(GameObject go, DBReference<Building> en)
        {
            if (!(en == null))
            {
                Image component = go.GetComponent<Image>();
                if (component != null)
                {
                    if (this.buildingImages.ContainsKey(en.dbName))
                    {
                        Debug.LogError("Building has more than one image " + en.dbName);
                    }
                    else
                    {
                        this.buildingImages.Add(en.dbName, component);
                        component.alphaHitTestMinimumThreshold = 0.5f;
                    }
                    if (!component.GetComponent<TownMapBuilding>())
                    {
                        TownMapBuilding townMapBuilding = component.gameObject.AddComponent<TownMapBuilding>();
                        townMapBuilding.townMap = this;
                        townMapBuilding.building = en;
                    }
                    if (onClick != null && component.GetComponent<Button>() == null)
                    {
                        component.gameObject.AddComponent<Button>().onClick.AddListener(delegate
                        {
                            onClick(en);
                        });
                    }
                }
            }
        }
    }

    public void ClearSelection()
    {
        if ((bool)this.selected)
        {
            this.selected.material = null;
        }
        this.selected = null;
    }

    public void Select(DBReference<Building> b)
    {
        this.ClearSelection();
        this.buildingImages.TryGetValue(b.dbName, out this.selected);
        if (this.selected != null)
        {
            this.selected.material = this.selectedMaterial;
            this.selected.SetMaterialDirty();
        }
    }
}
