namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("Mesh"), HutongGames.PlayMaker.Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
    public class GetVertexCount : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(MeshFilter)), HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the vertex count in a variable.")]
        public FsmInt storeCount;
        public bool everyFrame;

        private void DoGetVertexCount()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (ownerDefaultTarget != null)
            {
                MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
                if (component == null)
                {
                    base.LogError("Missing MeshFilter!");
                }
                else
                {
                    this.storeCount.Value = component.mesh.vertexCount;
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetVertexCount();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetVertexCount();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.storeCount = null;
            this.everyFrame = false;
        }
    }
}

