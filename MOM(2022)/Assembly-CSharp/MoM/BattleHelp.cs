// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.BattleHelp
using System.Collections;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;
using UnityEngine.UI;

public class BattleHelp : ScreenBase
{
    public Button btClose;

    public GameObjectEnabler<PlayerWizard.Familiar> familiar;

    public GameObject pageRoot;

    public GameObject p_Melee;

    public GameObject p_Ranged;

    public GameObject p_Casting;

    public GameObject p_MovementAllowance;

    public GameObject p_ArmourAndResistance;

    public Toggle tgMelee;

    public Toggle tgRanged;

    public Toggle tgCasting;

    public Toggle tgMovementAllowance;

    public Toggle tgArmourAndResistance;

    public override IEnumerator PreStart()
    {
        yield return base.PreStart();
        this.familiar.Set(GameManager.GetHumanWizard().familiar);
        this.Open(this.p_Melee);
    }

    protected override void ButtonClick(Selectable s)
    {
        base.ButtonClick(s);
        if (s == this.btClose)
        {
            UIManager.Close(this);
        }
        Toggle toggle = s as Toggle;
        if (toggle != null && toggle.isOn)
        {
            if (toggle == this.tgMelee)
            {
                this.Open(this.p_Melee);
            }
            if (toggle == this.tgRanged)
            {
                this.Open(this.p_Ranged);
            }
            if (toggle == this.tgCasting)
            {
                this.Open(this.p_Casting);
            }
            if (toggle == this.tgMovementAllowance)
            {
                this.Open(this.p_MovementAllowance);
            }
            if (toggle == this.tgArmourAndResistance)
            {
                this.Open(this.p_ArmourAndResistance);
            }
        }
    }

    private void Open(GameObject source)
    {
        GameObjectUtils.RemoveChildren(this.pageRoot.transform);
        GameObject gameObject = GameObjectUtils.InstantiateWithLocalization(source, this.pageRoot.transform);
        if (!(gameObject != null))
        {
            return;
        }
        Animator[] componentsInChildren = gameObject.GetComponentsInChildren<Animator>();
        foreach (Animator animator in componentsInChildren)
        {
            if (animator != null)
            {
                animator.speed = 0f;
            }
        }
    }
}
