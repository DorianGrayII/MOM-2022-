// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// AnimSpeedController4UI
using UnityEngine;

public class AnimSpeedController4UI : MonoBehaviour
{
    public Animator animator;

    public float speedMultiplier = 1f;

    private void Start()
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
        this.animator.SetFloat("speedMultiplier", this.speedMultiplier);
    }
}
