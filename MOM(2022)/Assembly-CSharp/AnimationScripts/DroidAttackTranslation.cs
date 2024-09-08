using System.Collections;
using UnityEngine;

namespace AnimationScripts
{
    public class DroidAttackTranslation : MonoBehaviour
    {
        public Transform target;

        public Vector3 destination = new Vector3(0f, 0f, 1.7f);

        public int startAnimationFrame = 7;

        public int endAnimationFrame = 40;

        public int framesPerSecond = 40;

        public void EventAttack()
        {
            base.StartCoroutine(this.MoveForward());
        }

        private IEnumerator MoveForward()
        {
            float progress = 0f;
            float totalTime = (float)(this.endAnimationFrame - this.startAnimationFrame) / (float)this.framesPerSecond;
            while (totalTime > 0f && progress < 1f)
            {
                progress = Mathf.Min(1f, progress + Time.deltaTime / totalTime);
                Vector3 localPosition = Vector3.Lerp(Vector3.zero, this.destination, progress);
                this.target.transform.localPosition = localPosition;
                yield return null;
            }
            this.target.transform.localPosition = this.destination;
        }
    }
}
