// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.SelectRace
using System.Collections;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectRace : ScreenBase
{
    public Button btContinue;

    public Button btCancel;

    public Button btNextArcanusRace;

    public Button btPreviousArcanusRace;

    public Button btNextMyrrorRace;

    public Button btPreviousMyrrorRace;

    public RawImage riRacePortrait;

    public RawImage riRacePortraitOverdraw;

    public RawImage riWizard;

    public GridItemManager gmArcanianRaces;

    public GridItemManager gmMyrranRaces;

    [FormerlySerializedAs("production")]
    public RaceInfo info;

    public Toggle tgBannerGreen;

    public Toggle tgBannerBlue;

    public Toggle tgBannerRed;

    public Toggle tgBannerPurple;

    public Toggle tgBannerYellow;

    public Animator animUnitPortrait;

    private Race selected;

    private List<Race> arcanusRaces;

    private List<Race> myrranRaces;

    private bool dedicatedArcanusRace;

    private Race wDedicatedRace;

    public override IEnumerator PreStart()
    {
        List<Race> type = DataBase.GetType<Race>();
        this.arcanusRaces = type.FindAll((Race o) => o.baseRace && o.arcanusRace);
        this.myrranRaces = type.FindAll((Race o) => o.baseRace && !o.arcanusRace);
        this.gmArcanianRaces.CustomDynamicItem(RaceItem, UpdateArcaneGrid);
        this.gmMyrranRaces.CustomDynamicItem(RaceItem, UpdateMyrranGrid);
        this.UpdateArcaneGrid();
        this.UpdateMyrranGrid();
        PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
        this.riWizard.texture = AssetManager.Get<Texture2D>(wizard.Background);
        this.btNextArcanusRace = this.gmArcanianRaces.pageingNext;
        this.btPreviousArcanusRace = this.gmArcanianRaces.pageingPrev;
        this.btNextMyrrorRace = this.gmMyrranRaces.pageingNext;
        this.btPreviousMyrrorRace = this.gmMyrranRaces.pageingPrev;
        yield return base.PreStart();
    }

    public override void OnStart()
    {
        base.OnStart();
        PlayerWizard w = GameManager.GetWizard(PlayerWizard.HumanID());
        if (!string.IsNullOrEmpty(w.dedicatedRace))
        {
            int num = -1;
            List<Race> type = DataBase.GetType<Race>();
            this.wDedicatedRace = type.Find((Race o) => o.dbName == w.dedicatedRace);
            this.dedicatedArcanusRace = type.Find((Race o) => o == this.wDedicatedRace).arcanusRace;
            num = ((!this.dedicatedArcanusRace) ? this.myrranRaces.FindIndex((Race o) => o == this.wDedicatedRace) : this.arcanusRaces.FindIndex((Race o) => o == this.wDedicatedRace));
            int num2 = num / 12;
            if (this.dedicatedArcanusRace)
            {
                this.gmArcanianRaces.SetPageNr(num2);
                this.gmArcanianRaces.Unselect();
                this.gmArcanianRaces.SelectItem(num - 12 * num2);
            }
            else
            {
                this.gmMyrranRaces.SetPageNr(num2);
                this.gmMyrranRaces.Unselect();
                this.gmMyrranRaces.SelectItem(num - 12 * num2);
            }
        }
        else
        {
            this.gmArcanianRaces.SelectItem(0);
        }
        this.tgBannerGreen.isOn = true;
    }

    private void UpdateArcaneGrid()
    {
        this.gmArcanianRaces.UpdateGrid(this.arcanusRaces);
    }

    private void UpdateMyrranGrid()
    {
        this.gmMyrranRaces.UpdateGrid(this.myrranRaces);
    }

    private void RaceItem(GameObject itemSource, object source, object data, int index)
    {
        PlayerWizard wizard = GameManager.GetWizard(PlayerWizard.HumanID());
        TextMeshProUGUI componentInChildren = itemSource.GetComponentInChildren<TextMeshProUGUI>();
        RawImage componentInChildren2 = itemSource.GetComponentInChildren<RawImage>();
        GameObject gameObject = GameObjectUtils.FindByName(itemSource, "Disabled");
        Race race = source as Race;
        componentInChildren.text = race.GetDescriptionInfo().GetLocalizedName();
        if (componentInChildren2 != null)
        {
            componentInChildren2.texture = race.GetDescriptionInfo().GetTexture();
        }
        Toggle component = itemSource.GetComponent<Toggle>();
        if (!string.IsNullOrEmpty(wizard.dedicatedRace))
        {
            if (this.wDedicatedRace == null)
            {
                this.wDedicatedRace = DataBase.Get<Race>(wizard.dedicatedRace, reportMissing: false);
            }
            if (this.wDedicatedRace != race)
            {
                component.interactable = false;
                component.isOn = false;
            }
            else
            {
                gameObject.SetActive(value: false);
                component.interactable = true;
                component.isOn = true;
            }
        }
        else
        {
            component.interactable = race.arcanusRace || wizard.myrranRaces || wizard.myrranRefugee;
        }
        if (!component.interactable)
        {
            gameObject.SetActive(value: true);
        }
        component.onValueChanged.RemoveAllListeners();
        component.onValueChanged.AddListener(delegate(bool b)
        {
            if (b)
            {
                this.RaceSelected(race);
            }
        });
    }

    private void RaceSelected(Race race)
    {
        this.selected = race;
        global::DBDef.Unit representativeUnit = race.representativeUnit;
        if (representativeUnit != null && this.riRacePortrait.texture != representativeUnit.GetDescriptionInfo().GetTextureLarge())
        {
            this.animUnitPortrait.SetTrigger("show");
            this.riRacePortrait.texture = representativeUnit.GetDescriptionInfo().GetTextureLarge();
            this.riRacePortraitOverdraw.texture = representativeUnit.GetDescriptionInfo().GetTextureLarge();
        }
        this.info.Set(race);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btContinue)
        {
            if (this.selected != null)
            {
                GameManager.GetHumanWizard().mainRace = this.selected;
            }
            else
            {
                Debug.LogError("No race selected");
            }
            if (this.tgBannerGreen.isOn)
            {
                this.SetColor(PlayerWizard.Color.Green);
            }
            else if (this.tgBannerBlue.isOn)
            {
                this.SetColor(PlayerWizard.Color.Blue);
            }
            else if (this.tgBannerRed.isOn)
            {
                this.SetColor(PlayerWizard.Color.Red);
            }
            else if (this.tgBannerPurple.isOn)
            {
                this.SetColor(PlayerWizard.Color.Purple);
            }
            else if (this.tgBannerYellow.isOn)
            {
                this.SetColor(PlayerWizard.Color.Yellow);
            }
            MHEventSystem.TriggerEvent(this, "Advance");
        }
        else if (s == this.btNextArcanusRace || s == this.btPreviousArcanusRace)
        {
            Race selectedObject = this.gmArcanianRaces.GetSelectedObject<Race>();
            if (selectedObject == null)
            {
                Dictionary<Selectable, object> toggleToDataDictionary = this.gmArcanianRaces.toggleToDataDictionary;
                if (toggleToDataDictionary == null)
                {
                    return;
                }
                {
                    foreach (KeyValuePair<Selectable, object> item in toggleToDataDictionary)
                    {
                        if (item.Key is Toggle toggle && toggle.interactable && toggle.gameObject.activeInHierarchy)
                        {
                            this.gmArcanianRaces.SelectItemByGameObject(toggle.gameObject);
                            this.RaceSelected(item.Value as Race);
                            break;
                        }
                    }
                    return;
                }
            }
            this.RaceSelected(selectedObject);
        }
        else if (s == this.btNextMyrrorRace || s == this.btPreviousMyrrorRace)
        {
            Race selectedObject2 = this.gmMyrranRaces.GetSelectedObject<Race>();
            if (selectedObject2 == null)
            {
                Dictionary<Selectable, object> toggleToDataDictionary2 = this.gmMyrranRaces.toggleToDataDictionary;
                if (toggleToDataDictionary2 == null)
                {
                    return;
                }
                {
                    foreach (KeyValuePair<Selectable, object> item2 in toggleToDataDictionary2)
                    {
                        if (item2.Key is Toggle toggle2 && toggle2.interactable && toggle2.gameObject.activeInHierarchy)
                        {
                            this.gmMyrranRaces.SelectItemByGameObject(toggle2.gameObject);
                            this.RaceSelected(item2.Value as Race);
                            break;
                        }
                    }
                    return;
                }
            }
            this.RaceSelected(selectedObject2);
        }
        else if (s == this.btCancel)
        {
            MHEventSystem.TriggerEvent(this, "Back");
        }
    }

    private void SetColor(PlayerWizard.Color col)
    {
        PlayerWizard.Color color = GameManager.GetHumanWizard().color;
        foreach (PlayerWizard wizard in GameManager.GetWizards())
        {
            if (wizard.color == col)
            {
                wizard.color = color;
                break;
            }
        }
        GameManager.GetHumanWizard().color = col;
    }
}
