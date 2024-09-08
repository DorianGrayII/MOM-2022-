using System.Collections;
using System.Collections.Generic;
using DBDef;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheRoom : ScreenBase
{
    public enum RoomEvents
    {
        SpellResearched = 0,
        UnitSummoned = 1,
        DefeatByWizard = 2,
        DefeatByNeutrals = 3,
        EnemyDefeated = 4,
        EnemiesDefeated = 5
    }

    private class QueueItem
    {
        public RoomEvents roomEvent;

        public PlayerWizard wizard;

        public object o;

        public QueueItem(PlayerWizard w, RoomEvents e, object o)
        {
            this.roomEvent = e;
            this.wizard = w;
            this.o = o;
        }
    }

    public Button buttonClose;

    public TextMeshProUGUI textButton;

    public TextMeshProUGUI textHeading;

    public TextMeshProUGUI spellName;

    public GameObject backGround;

    private GameObject theRoomModels;

    public TextMeshProUGUI textHeadingRed;

    public GameObject backGroundRed;

    public GameObject spellGraphics;

    public GameObject research;

    public GameObject summon;

    public RawImage spellIcon;

    public RawImage unitImage1;

    public RawImage unitImage2;

    public RawImage unitImage3;

    private static Queue<QueueItem> eventQueue = new Queue<QueueItem>();

    private static TheRoom theRoom;

    public static TheRoom Open(PlayerWizard w, RoomEvents e, object o)
    {
        TheRoom.eventQueue.Enqueue(new QueueItem(w, e, o));
        if (TheRoom.eventQueue.Count == 1)
        {
            GameObject original = AssetManager.Get<GameObject>("TheRoomPrefab");
            VerticalMarkerManager.Get().doUpdate = false;
            if (HUD.Get() != null && HUD.Get().gameObject != null)
            {
                HUD.Get().gameObject.SetActive(value: false);
            }
            CameraController.Get().gameObject.SetActive(value: false);
            TheRoom.theRoom = UIManager.Open<TheRoom>(UIManager.Layer.Popup);
            TheRoom.theRoom.theRoomModels = Object.Instantiate(original);
            TheRoom.theRoom.theRoomModels.name = "TheRoom";
            TheRoom.theRoom.Set(TheRoom.eventQueue.Peek());
            AmbientLightController.ChangeAmbientColor();
        }
        else
        {
            TheRoom.theRoom.textButton.text = global::DBUtils.Localization.Get("UI_NEXT", true);
        }
        return TheRoom.theRoom;
    }

    public IEnumerator WaitWhileOpen()
    {
        while (base.stateStatus < StateStatus.Closing)
        {
            yield return null;
        }
    }

    public IEnumerator WaitWhileOpenThenEndGame()
    {
        while (base.stateStatus < StateStatus.Closing)
        {
            yield return null;
        }
        UIManager.Open<Victory>(UIManager.Layer.Popup).SetMessage("UI_YOU_ARE_MASTER_OF_MAGIC");
    }

    private void Close()
    {
        TheRoom.eventQueue.Dequeue();
        if (TheRoom.eventQueue.Count > 0)
        {
            this.Set(TheRoom.eventQueue.Peek());
            return;
        }
        VerticalMarkerManager.Get().doUpdate = true;
        if (HUD.Get() != null && HUD.Get().gameObject != null)
        {
            HUD.Get().gameObject.SetActive(value: true);
        }
        CameraController.Get().gameObject.SetActive(value: true);
        UIManager.Close(this);
        AmbientLightController.ChangeAmbientColor();
    }

    public override IEnumerator PostClose()
    {
        yield return base.PostClose();
        if (this.theRoomModels != null)
        {
            Object.Destroy(this.theRoomModels);
        }
    }

    private void Set(QueueItem q)
    {
        PlayerWizard w = q.wizard;
        RoomEvents roomEvent = q.roomEvent;
        object obj = q.o;
        if (obj is DBRefBase dBRefBase)
        {
            obj = dBRefBase.GetObject();
        }
        this.theRoomModels.transform.Find("Mirror_AnimationHolder/Mirror/Wizard/WizardPortrait").GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = w.Graphic;
        this.theRoomModels.transform.Find("LostScr_AnimationHolder/Mirror/Wizard/WizardPortrait2").GetComponent<SkinnedMeshRenderer>().materials[0].mainTexture = w.Graphic;
        string text = w.familiar.ToString();
        Transform transform = this.theRoomModels.transform.Find("Familiars");
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj2 = transform.GetChild(i).gameObject;
            obj2.SetActive(obj2.name == text);
        }
        string text2 = "";
        Texture2D texture = null;
        if (obj is global::MOM.Unit unit)
        {
            text2 = unit.GetName();
            texture = null;
        }
        else if (obj is IDescriptionInfoType t)
        {
            text2 = t.GetDILocalizedName();
            texture = t.GetTexture();
        }
        else if (obj is PlayerWizard playerWizard)
        {
            text2 = playerWizard.GetName();
            texture = null;
        }
        bool flag = false;
        switch (roomEvent)
        {
        case RoomEvents.UnitSummoned:
        {
            this.theRoomModels.transform.Find("SummoningPortal_AnimationHolder/SummoningPortal/SummoningMonster");
            global::MOM.Unit unit2 = obj as global::MOM.Unit;
            flag = true;
            this.textHeadingRed.text = global::DBUtils.Localization.Get("UI_ROOM_UNITSUMMONED", true, text2);
            this.textHeading.text = null;
            this.spellName.text = null;
            this.spellGraphics.gameObject.SetActive(value: false);
            this.unitImage1.texture = unit2.GetDescriptionInfo().GetTextureLarge();
            this.unitImage2.texture = unit2.GetDescriptionInfo().GetTextureLarge();
            this.unitImage3.texture = unit2.GetDescriptionInfo().GetTextureLarge();
            this.summon.SetActive(value: true);
            AudioLibrary.RequestSFX("CutsceneSummon");
            break;
        }
        case RoomEvents.SpellResearched:
            this.textHeading.text = global::DBUtils.Localization.Get("UI_ROOM_SPELLRESEARCHED", true);
            this.spellName.text = global::DBUtils.Localization.Get("UI_ROOM_SPELLRESEARCHED2", true, text2);
            this.textHeadingRed.text = null;
            this.spellGraphics.gameObject.SetActive(value: true);
            this.spellIcon.texture = texture;
            AudioLibrary.RequestSFX("CutsceneResearch");
            this.research.SetActive(value: true);
            break;
        case RoomEvents.EnemyDefeated:
            if (obj is PlayerWizard playerWizard2)
            {
                this.textHeading.text = global::DBUtils.Localization.Get("UI_WIZARD_DEFEATED", true, playerWizard2.GetName());
                this.textHeadingRed.text = null;
                this.theRoomModels.transform.Find("EnemyDefeated_AnimationHolder/Enemy1/DefeatedEnemyPortrait").GetComponent<MeshRenderer>().materials[0].mainTexture = playerWizard2.Graphic;
                PlayMusic.Play("SOUND_LIST-CUTSCENE_ENEMY_DEFEATED1", this);
            }
            else
            {
                Debug.LogWarning("Variable 'o' is not a PlayerWizard");
            }
            break;
        case RoomEvents.EnemiesDefeated:
            if (obj is List<PlayerWizard> list && list != null)
            {
                int num = 0;
                List<PlayerWizard> list2 = list.FindAll((PlayerWizard o) => o.isAlive && o != w);
                if (list2.Count == 1)
                {
                    this.textHeading.text = global::DBUtils.Localization.Get("UI_WIZARD_DEFEATED", true, list2[0].GetName());
                }
                else
                {
                    this.textHeading.text = global::DBUtils.Localization.Get("UI_WIZARDS_DEFEATED", true);
                }
                this.textHeadingRed.text = null;
                foreach (PlayerWizard item in list2)
                {
                    num++;
                    Transform obj3 = this.theRoomModels.transform.Find("EnemyDefeated_AnimationHolder/Enemy" + num);
                    obj3.gameObject.SetActive(value: true);
                    obj3.Find("DefeatedEnemyPortrait").GetComponent<MeshRenderer>().materials[0].mainTexture = item.Graphic;
                }
                PlayMusic.Play("SOUND_LIST-CUTSCENE_ENEMY_DEFEATED" + num, this);
            }
            else
            {
                Debug.LogWarning("Variable 'o' is not a List<PlayerWizard>");
            }
            break;
        default:
            this.textHeading.text = global::DBUtils.Localization.Get("UI_ROOM_" + roomEvent.ToString().ToUpperInvariant(), true, text2);
            this.textHeadingRed.text = null;
            this.spellName.text = null;
            this.spellGraphics.gameObject.SetActive(value: false);
            break;
        }
        this.backGroundRed.SetActive(flag);
        this.backGround.SetActive(!flag);
        this.theRoomModels.GetComponent<Animator>().SetTrigger(roomEvent.ToString());
        this.textButton.text = global::DBUtils.Localization.Get((TheRoom.eventQueue.Count > 1) ? "UI_NEXT" : "UI_CLOSE", true);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.buttonClose)
        {
            this.Close();
        }
    }
}
