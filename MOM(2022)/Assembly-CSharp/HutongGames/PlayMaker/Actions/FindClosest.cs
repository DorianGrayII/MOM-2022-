namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.GameObject), HutongGames.PlayMaker.Tooltip("Finds the closest object to the specified Game Object.\nOptionally filter by Tag and Visibility.")]
    public class FindClosest : FsmStateAction
    {
        [RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to measure from.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Tag), HutongGames.PlayMaker.Tooltip("Only consider objects with this Tag. NOTE: It's generally a lot quicker to find objects with a Tag!")]
        public FsmString withTag;
        [HutongGames.PlayMaker.Tooltip("If checked, ignores the object that owns this FSM.")]
        public FsmBool ignoreOwner;
        [HutongGames.PlayMaker.Tooltip("Only consider objects visible to the camera.")]
        public FsmBool mustBeVisible;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the closest object.")]
        public FsmGameObject storeObject;
        [UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the distance to the closest object.")]
        public FsmFloat storeDistance;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame")]
        public bool everyFrame;

        private void DoFindClosest()
        {
            GameObject obj2 = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.get_Owner() : this.gameObject.GameObject.get_Value();
            GameObject[] objArray = (string.IsNullOrEmpty(this.withTag.Value) || (this.withTag.Value == "Untagged")) ? ((GameObject[]) UnityEngine.Object.FindObjectsOfType(typeof(GameObject))) : GameObject.FindGameObjectsWithTag(this.withTag.Value);
            GameObject obj3 = null;
            float positiveInfinity = float.PositiveInfinity;
            foreach (GameObject obj4 in objArray)
            {
                if ((!this.ignoreOwner.Value || (obj4 != base.get_Owner())) && (!this.mustBeVisible.Value || ActionHelpers.IsVisible(obj4)))
                {
                    float sqrMagnitude = (obj2.transform.position - obj4.transform.position).sqrMagnitude;
                    if (sqrMagnitude < positiveInfinity)
                    {
                        positiveInfinity = sqrMagnitude;
                        obj3 = obj4;
                    }
                }
            }
            this.storeObject.set_Value(obj3);
            if (!this.storeDistance.IsNone)
            {
                this.storeDistance.Value = Mathf.Sqrt(positiveInfinity);
            }
        }

        public override void OnEnter()
        {
            this.DoFindClosest();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoFindClosest();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.withTag = "Untagged";
            this.ignoreOwner = true;
            this.mustBeVisible = false;
            this.storeObject = null;
            this.storeDistance = null;
            this.everyFrame = false;
        }
    }
}

