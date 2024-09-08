using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [ActionTarget(typeof(GameObject), "gameObject", true)]
    [Tooltip("Creates a Game Object, usually using a Prefab.")]
    public class CreateObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("GameObject to create. Usually a Prefab.")]
        public FsmGameObject gameObject;

        [Tooltip("Optional Spawn Point.")]
        public FsmGameObject spawnPoint;

        [Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
        public FsmVector3 position;

        [Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
        public FsmVector3 rotation;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optionally store the created object.")]
        public FsmGameObject storeObject;

        public override void Reset()
        {
            this.gameObject = null;
            this.spawnPoint = null;
            this.position = new FsmVector3
            {
                UseVariable = true
            };
            this.rotation = new FsmVector3
            {
                UseVariable = true
            };
            this.storeObject = null;
        }

        public override void OnEnter()
        {
            GameObject value = this.gameObject.Value;
            if (value != null)
            {
                Vector3 vector = Vector3.zero;
                Vector3 euler = Vector3.zero;
                if (this.spawnPoint.Value != null)
                {
                    vector = this.spawnPoint.Value.transform.position;
                    if (!this.position.IsNone)
                    {
                        vector += this.position.Value;
                    }
                    euler = ((!this.rotation.IsNone) ? this.rotation.Value : this.spawnPoint.Value.transform.eulerAngles);
                }
                else
                {
                    if (!this.position.IsNone)
                    {
                        vector = this.position.Value;
                    }
                    if (!this.rotation.IsNone)
                    {
                        euler = this.rotation.Value;
                    }
                }
                GameObject value2 = Object.Instantiate(value, vector, Quaternion.Euler(euler));
                this.storeObject.Value = value2;
            }
            base.Finish();
        }
    }
}
