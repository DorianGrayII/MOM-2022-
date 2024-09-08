using UnityEngine;

public class AnimSpeedController : MonoBehaviour
{
    public Animator animator;

    public float idleSpeedMultiplier = 1f;

    public float moveSpeedMultiplier = 1f;

    public float attackMeleeSpeedMultiplier = 1f;

    public float attackMagicSpeedMultiplier = 1f;

    public float attackRangedSpeedMultiplier = 1f;

    public float dieSpeedMultiplier = 1f;

    public float getHitSpeedMultiplier = 1f;

    public float buildHitSpeedMultiplier = 1f;

    private void OnEnable()
    {
        this.animator = base.GetComponent<Animator>();
        if (this.animator == null)
        {
            Debug.LogError(base.gameObject.name + "Missing Animator");
        }
        else
        {
            this.ChangeStateSpeed();
        }
    }

    private void ChangeStateSpeed()
    {
        this.animator.SetFloat("Idle_SpeedMultiplier", this.idleSpeedMultiplier);
        this.animator.SetFloat("Move_SpeedMultiplier", this.moveSpeedMultiplier);
        this.animator.SetFloat("AttackMelee_SpeedMultiplier", this.attackMeleeSpeedMultiplier);
        this.animator.SetFloat("AttackMagic_SpeedMultiplier", this.attackMagicSpeedMultiplier);
        this.animator.SetFloat("AttackRanged_SpeedMultiplier", this.attackRangedSpeedMultiplier);
        this.animator.SetFloat("Die_SpeedMultiplier", this.dieSpeedMultiplier);
        this.animator.SetFloat("GetHit_SpeedMultiplier", this.getHitSpeedMultiplier);
        this.animator.SetFloat("Build_SpeedMultiplier", this.buildHitSpeedMultiplier);
    }
}
