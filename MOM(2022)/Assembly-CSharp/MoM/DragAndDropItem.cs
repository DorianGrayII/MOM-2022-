using MHUtils;
using UnityEngine;

namespace MOM
{
    public class DragAndDropItem : MonoBehaviour
    {
        public DragAndDrop source;

        private const float TRANSITION_TIME = 0.3f;

        private float timer = 0.3f;

        private CanvasGroup cg;

        public void Update()
        {
            if ((object)this.cg == null)
            {
                this.cg = base.gameObject.GetComponentInChildren<CanvasGroup>();
            }
            this.cg.alpha = Mathf.Min(1f, (0.3f - this.timer) / 0.3f);
            this.timer -= Time.deltaTime;
            base.transform.position = Input.mousePosition;
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButton(0))
            {
                MHEventSystem.TriggerEvent<DragAndDropItem>(this, this.source);
                this.Destroy();
            }
        }

        public void Destroy()
        {
            Object.Destroy(base.gameObject);
        }
    }
}
