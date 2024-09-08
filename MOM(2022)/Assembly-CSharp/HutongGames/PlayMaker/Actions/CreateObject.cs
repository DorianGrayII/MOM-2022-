namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), ActionTarget(typeof(GameObject), "gameObject", true), HutongGames.PlayMaker.Tooltip("Creates a Game Object, usually using a Prefab.")]
    public class CreateObject : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("GameObject to create. Usually a Prefab.")]
        public FsmGameObject gameObject;
        [HutongGames.PlayMaker.Tooltip("Optional Spawn Point.")]
        public FsmGameObject spawnPoint;
        [HutongGames.PlayMaker.Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
        public FsmVector3 position;
        [HutongGames.PlayMaker.Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
        public FsmVector3 rotation;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Optionally store the created object.")]
        public FsmGameObject storeObject;

        public override void OnEnter()
        {
            GameObject original = this.gameObject.get_Value();
            if (original != null)
            {
                Vector3 zero = Vector3.zero;
                Vector3 euler = Vector3.zero;
                if (this.spawnPoint.get_Value() != null)
                {
                    zero = this.spawnPoint.get_Value().transform.position;
                    if (!this.position.IsNone)
                    {
                        zero += this.position.get_Value();
                    }
                    euler = !this.rotation.IsNone ? this.rotation.get_Value() : this.spawnPoint.get_Value().transform.eulerAngles;
                }
                else
                {
                    if (!this.position.IsNone)
                    {
                        zero = this.position.get_Value();
                    }
                    if (!this.rotation.IsNone)
                    {
                        euler = this.rotation.get_Value();
                    }
                }
                GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original, zero, Quaternion.Euler(euler));
                this.storeObject.set_Value(obj3);
            }
            base.Finish();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.spawnPoint = null;
            FsmVector3 vector1 = new FsmVector3();
            vector1.UseVariable = true;
            this.position = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.rotation = vector2;
            this.storeObject = null;
        }
    }
}

