namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.Physics), Tooltip("Gets info on the last RaycastAll and store in array variables.")]
    public class GetRaycastAllInfo : FsmStateAction
    {
        [Tooltip("Store the GameObjects hit in an array variable."), UIHint(UIHint.Variable), ArrayEditor(VariableType.GameObject, "", 0, 0, 0x10000)]
        public FsmArray storeHitObjects;
        [Tooltip("Get the world position of all ray hit point and store them in an array variable."), UIHint(UIHint.Variable), ArrayEditor(VariableType.Vector3, "", 0, 0, 0x10000)]
        public FsmArray points;
        [Tooltip("Get the normal at all hit points and store them in an array variable."), UIHint(UIHint.Variable), ArrayEditor(VariableType.Vector3, "", 0, 0, 0x10000)]
        public FsmArray normals;
        [Tooltip("Get the distance along the ray to all hit points and store them in an array variable."), UIHint(UIHint.Variable), ArrayEditor(VariableType.Float, "", 0, 0, 0x10000)]
        public FsmArray distances;
        [Tooltip("Repeat every frame. Warning, this could be affecting performances")]
        public bool everyFrame;

        public override void OnEnter()
        {
            this.StoreRaycastAllInfo();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.StoreRaycastAllInfo();
        }

        public override void Reset()
        {
            this.storeHitObjects = null;
            this.points = null;
            this.normals = null;
            this.distances = null;
            this.everyFrame = false;
        }

        private void StoreRaycastAllInfo()
        {
            if (RaycastAll.RaycastAllHitInfo != null)
            {
                this.storeHitObjects.Resize(RaycastAll.RaycastAllHitInfo.Length);
                this.points.Resize(RaycastAll.RaycastAllHitInfo.Length);
                this.normals.Resize(RaycastAll.RaycastAllHitInfo.Length);
                this.distances.Resize(RaycastAll.RaycastAllHitInfo.Length);
                for (int i = 0; i < RaycastAll.RaycastAllHitInfo.Length; i++)
                {
                    this.storeHitObjects.Values[i] = RaycastAll.RaycastAllHitInfo[i].collider.gameObject;
                    this.points.Values[i] = RaycastAll.RaycastAllHitInfo[i].point;
                    this.normals.Values[i] = RaycastAll.RaycastAllHitInfo[i].normal;
                    this.distances.Values[i] = RaycastAll.RaycastAllHitInfo[i].distance;
                }
            }
        }
    }
}

