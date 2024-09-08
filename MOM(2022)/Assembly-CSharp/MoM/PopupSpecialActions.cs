// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.PopupSpecialActions
using System.Collections.Generic;
using DBDef;
using DBEnum;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WorldCode;

public class PopupSpecialActions : ScreenBase
{
    public Button btCancel;

    public Button btBuildOutpost;

    public Button btBuildRoad;

    public Button btMeldWithNode;

    public Button btPurify;

    public TextMeshProUGUI labelOutpostTime;

    public TextMeshProUGUI labelRoadTime;

    public TextMeshProUGUI labelMeldTime;

    public TextMeshProUGUI labelMeldChance;

    public TextMeshProUGUI labelPurifyTime;

    public GameObject activeEnchWarning;

    public PopupSpecialActions()
    {
        MHEventSystem.RegisterListener<FSMSelectionManager>(SelectionChanged, this);
    }

    private void SelectionChanged(object sender, object e)
    {
        this.Close();
    }

    public static void OpenPopup(ScreenBase parent, global::MOM.Group g)
    {
        PopupSpecialActions popup = UIManager.Open<PopupSpecialActions>(UIManager.Layer.Popup, parent);
        popup.BuildRoad(g);
        popup.BuildTown(g);
        popup.PurifyLand(g);
        popup.MeldWithNode(g);
        popup.ActiveEnchWarning();
        popup.btCancel.onClick.AddListener(delegate
        {
            popup.Close();
        });
    }

    private void Close()
    {
        UIManager.Close(this);
    }

    private void BuildRoad(global::MOM.Group g)
    {
        List<Reference<global::MOM.Unit>> list = g.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().IsEngineer());
        Hex hexAt = g.GetPlane().GetHexAt(g.GetPosition());
        if (list.Count == 0 || !hexAt.IsLand() || list.FindAll((Reference<global::MOM.Unit> o) => o.Get().Mp <= 0).Count == list.Count)
        {
            this.labelRoadTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
            this.btBuildRoad.interactable = false;
        }
        else
        {
            this.btBuildRoad.interactable = true;
            if (g.engineerManager != null)
            {
                this.labelRoadTime.text = g.engineerManager.TurnsOfWorkLeft().ToString();
            }
            else if (list.Count > 0)
            {
                EngineerManager engineerManager = new EngineerManager(g, virtualVariant: true);
                engineerManager.AddRoadNode(g.GetPosition());
                this.labelRoadTime.text = engineerManager.TurnsOfWorkLeft().ToString();
                engineerManager.Destroy();
            }
        }
        this.btBuildRoad.onClick.AddListener(delegate
        {
            global::MOM.Group group = g;
            if (group.engineerManager == null)
            {
                group.engineerManager = new EngineerManager(g);
            }
            g.engineerManager.AddRoadNode(g.GetPosition());
            if (FSMSelectionManager.Get().GetSelectedGroup() == g)
            {
                FSMSelectionManager.SetRoadPathMode(b: true);
            }
            g.engineerManager.TurnUpdate();
            g.UpdateMapFormation(createIfMissing: false);
            g.destination = Vector3i.invalid;
            g.Action = global::MOM.Group.GroupActions.None;
            this.Close();
        });
    }

    private void BuildTown(global::MOM.Group g)
    {
        this.btBuildOutpost.interactable = g.GetUnits().FindIndex((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) > -1;
        if (!this.btBuildOutpost.interactable)
        {
            this.labelOutpostTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
        }
        this.btBuildOutpost.onClick.AddListener(delegate
        {
            this.Close();
            if (g.CanBuildTown())
            {
                g.BuildTown();
            }
            else
            {
                int townDistance = DifficultySettingsData.GetTownDistance();
                string message = global::DBUtils.Localization.Get("UI_INVALID_NEW_TOWN_LOCATION", true, townDistance);
                PopupGeneral.OpenPopup(this, "UI_WARNING", message, "UI_OK");
            }
        });
    }

    private void PurifyLand(global::MOM.Group g)
    {
        Hex h = g.GetPlane().GetHexAt(g.GetPosition());
        List<Reference<global::MOM.Unit>> purifiers = g.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().IsPurifier());
        this.btPurify.interactable = purifiers.Count > 0 && h != null && h.IsLand() && !h.ActiveHex;
        if (this.btPurify.interactable)
        {
            if (g.purificationManager != null)
            {
                this.labelPurifyTime.text = g.purificationManager.TurnsOfWorkLeft().ToString();
            }
            else if (purifiers.Count > 0)
            {
                PurificationManager purificationManager = new PurificationManager(g, virtualVariant: true);
                purificationManager.AddNode(g.GetPosition());
                this.labelPurifyTime.text = purificationManager.TurnsOfWorkLeft().ToString();
                if (GameManager.GetLocationsOfThePlane(g.GetPlane()).Find((global::MOM.Location o) => o.GetPosition() == g.GetPosition()) != null)
                {
                    this.labelPurifyTime.text = "1";
                }
                purificationManager.Destroy();
            }
        }
        else
        {
            this.labelPurifyTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
        }
        this.btPurify.onClick.AddListener(delegate
        {
            global::MOM.Group group = g;
            if (group.purificationManager == null)
            {
                group.purificationManager = new PurificationManager(g);
            }
            if (GameManager.GetLocationsOfThePlane(g.GetPlane()).Find((global::MOM.Location o) => o.GetPosition() == g.GetPosition()) != null)
            {
                h.ActiveHex = true;
                g.purificationManager.Destroy();
            }
            else
            {
                g.purificationManager.AddNode(g.GetPosition());
                g.UpdateMapFormation(createIfMissing: false);
                g.destination = Vector3i.invalid;
                g.Action = global::MOM.Group.GroupActions.None;
            }
            purifiers.ForEach(delegate(Reference<global::MOM.Unit> o)
            {
                o.Get().Mp = FInt.ZERO;
            });
            this.Close();
        });
    }

    private void MeldWithNode(global::MOM.Group g)
    {
        g.GetPlane().GetHexAt(g.GetPosition());
        List<Reference<global::MOM.Unit>> melders = g.GetUnits().FindAll((Reference<global::MOM.Unit> o) => o.Get().IsMelder());
        if (melders.Count > 0)
        {
            List<global::MOM.Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(g.GetPlane());
            global::MOM.Location loc = locationsOfThePlane.Find((global::MOM.Location o) => o.GetPosition() == g.GetPosition() && o.locationType == ELocationType.Node);
            int num = 100;
            if (loc == null)
            {
                this.btMeldWithNode.interactable = false;
                this.labelMeldTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
                this.labelMeldChance.text = global::DBUtils.Localization.Get("UI_MELD_WITH_NODE", true);
            }
            else if (loc.melding != null && loc.melding.meldOwner == g.GetOwnerID())
            {
                Reference<global::MOM.Unit> reference = melders.Find((Reference<global::MOM.Unit> o) => o.Get().IsAdvMelder());
                if (reference != null && loc.melding != null && loc.melding.strength <= 1)
                {
                    this.btMeldWithNode.interactable = true;
                    this.labelMeldChance.text = global::DBUtils.Localization.Get("UI_UPGRADE_NODE", true);
                    melders[0] = reference;
                }
                else
                {
                    this.btMeldWithNode.interactable = false;
                    this.labelMeldTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
                    this.labelMeldChance.text = global::DBUtils.Localization.Get("UI_MELD_WITH_NODE", true);
                }
            }
            else
            {
                if (loc.melding != null && loc.melding.strength > 1)
                {
                    num = 25;
                }
                this.btMeldWithNode.interactable = true;
                this.labelMeldChance.text = global::DBUtils.Localization.Get("UI_MELD_WITH_NODE", true) + " (" + num + "%)";
            }
            this.btMeldWithNode.onClick.AddListener(delegate
            {
                this.Close();
                Reference<global::MOM.Unit> reference2 = melders[0];
                loc.MeldAttempt(reference2);
                reference2.Get().Destroy();
            });
        }
        else
        {
            this.btMeldWithNode.interactable = false;
            this.labelMeldTime.text = global::DBUtils.Localization.Get("UI_NOT_AVAILABLE", true);
            this.labelMeldChance.text = global::DBUtils.Localization.Get("UI_MELD_WITH_NODE", true);
        }
    }

    public static bool AnyActionPossible(global::MOM.Group g)
    {
        if (g == null || g.GetUnits().Count == 0)
        {
            return false;
        }
        if (PopupSpecialActions.MeldingUnitPresent(g) || PopupSpecialActions.PurifierPresent(g))
        {
            return true;
        }
        if (g.GetLocationHostSmart() != null)
        {
            return false;
        }
        if (!PopupSpecialActions.SettlerPresent(g))
        {
            return PopupSpecialActions.EngineerPresent(g);
        }
        return true;
    }

    private static bool SettlerPresent(global::MOM.Group g)
    {
        return g.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsSettler()) != null;
    }

    private static bool EngineerPresent(global::MOM.Group g)
    {
        return g.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsEngineer()) != null;
    }

    private static bool MeldingUnitPresent(global::MOM.Group g)
    {
        return g.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsMelder()) != null;
    }

    private static bool PurifierPresent(global::MOM.Group g)
    {
        return g.GetUnits().Find((Reference<global::MOM.Unit> o) => o.Get().IsPurifier()) != null;
    }

    private void ActiveEnchWarning()
    {
        if (GameManager.GetHumanWizard().GetAttFinal(TAG.OUTPOST_WARNING) > 0)
        {
            this.activeEnchWarning.SetActive(value: true);
        }
    }
}
