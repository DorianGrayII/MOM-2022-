using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [Tooltip("Creates a Game Object at a spawn point.\nUse a Game Object and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
    public class CreateEmptyObject : FsmStateAction
    {
        [Tooltip("Optional GameObject to create. Usually a Prefab.")]
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
            Vector3 vector = Vector3.zero;
            Vector3 eulerAngles = Vector3.zero;
            if (this.spawnPoint.Value != null)
            {
                vector = this.spawnPoint.Value.transform.position;
                if (!this.position.IsNone)
                {
                    vector += this.position.Value;
                }
                eulerAngles = (this.rotation.IsNone ? this.spawnPoint.Value.transform.eulerAngles : this.rotation.Value);
            }
            else
            {
                if (!this.position.IsNone)
                {
                    vector = this.position.Value;
                }
                if (!this.rotation.IsNone)
                {
                    eulerAngles = this.rotation.Value;
                }
            }
            GameObject value2 = this.storeObject.Value;
            if (value != null)
            {
                value2 = Object.Instantiate(value);
                this.storeObject.Value = value2;
            }
            else
            {
                value2 = new GameObject("EmptyObjectFromNull");
                this.storeObject.Value = value2;
            }
            if (value2 != null)
            {
                value2.transform.position = vector;
                value2.transform.eulerAngles = eulerAngles;
            }
            base.Finish();
        }
    }
}
