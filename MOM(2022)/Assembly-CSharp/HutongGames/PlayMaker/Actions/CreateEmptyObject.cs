﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Creates a Game Object at a spawn point.\nUse a Game Object and/or Position/Rotation for the Spawn Point. If you specify a Game Object, Position is used as a local offset, and Rotation will override the object's rotation.")]
    public class CreateEmptyObject : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Optional GameObject to create. Usually a Prefab.")]
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
            Vector3 zero = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            if (this.spawnPoint.get_Value() != null)
            {
                zero = this.spawnPoint.get_Value().transform.position;
                if (!this.position.IsNone)
                {
                    zero += this.position.get_Value();
                }
                vector2 = this.rotation.IsNone ? this.spawnPoint.get_Value().transform.eulerAngles : this.rotation.get_Value();
            }
            else
            {
                if (!this.position.IsNone)
                {
                    zero = this.position.get_Value();
                }
                if (!this.rotation.IsNone)
                {
                    vector2 = this.rotation.get_Value();
                }
            }
            GameObject obj3 = this.storeObject.get_Value();
            if (original != null)
            {
                obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                this.storeObject.set_Value(obj3);
            }
            else
            {
                obj3 = new GameObject("EmptyObjectFromNull");
                this.storeObject.set_Value(obj3);
            }
            if (obj3 != null)
            {
                obj3.transform.position = zero;
                obj3.transform.eulerAngles = vector2;
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

