using DBUtils;
using MHUtils;
using UnityEngine;

namespace MOM
{
    public class BattleWall
    {
        public bool standing;

        public bool gate;

        public Vector3i position;

        public GameObject mapModel;

        public Vector3 defenceNormal;

        public Animator animator;

        public void AnimateDestroy()
        {
            if (this.animator == null)
            {
                this.animator = this.mapModel.GetComponent<Animator>();
            }
            if (this.animator == null)
            {
                Debug.LogError("Missing animator for wall " + this.mapModel);
            }
            this.animator.SetBool("Destroy", value: true);
            AudioLibrary.RequestSFX("DestroyWall");
            BattleHUD.CombatLogAdd(Localization.Get("UI_COMBAT_LOG_WALL_DESTROYED", true));
            this.standing = false;
        }

        public void AnimateOpen()
        {
            if (!this.gate)
            {
                Debug.LogError("Asking to animate open not-gate" + this.mapModel);
            }
            if (this.animator == null)
            {
                this.animator = this.mapModel.GetComponent<Animator>();
            }
            if (this.animator == null)
            {
                Debug.LogError("Missing animator for wall " + this.mapModel);
            }
            this.animator.SetBool("Open", value: true);
            AudioLibrary.RequestSFX("OpenGate");
        }

        public void AnimateClose()
        {
            if (!this.gate)
            {
                Debug.LogError("Asking to animate close not-gate" + this.mapModel);
            }
            if (this.animator == null)
            {
                this.animator = this.mapModel.GetComponent<Animator>();
            }
            if (this.animator == null)
            {
                Debug.LogError("Missing animator for wall " + this.mapModel);
            }
            this.animator.SetBool("Close", value: true);
            AudioLibrary.RequestSFX("CloseGate");
        }
    }
}
