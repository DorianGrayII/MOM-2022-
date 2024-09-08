namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory("Mesh"), HutongGames.PlayMaker.Tooltip("Gets the position of a vertex in a GameObject's mesh. Hint: Use GetVertexCount to get the number of vertices in a mesh.")]
    public class GetVertexPosition : FsmStateAction
    {
        [RequiredField, CheckForComponent(typeof(MeshFilter)), HutongGames.PlayMaker.Tooltip("The GameObject to check.")]
        public FsmOwnerDefault gameObject;
        [RequiredField, HutongGames.PlayMaker.Tooltip("The index of the vertex.")]
        public FsmInt vertexIndex;
        [HutongGames.PlayMaker.Tooltip("Coordinate system to use.")]
        public Space space;
        [RequiredField, UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the vertex position in a variable.")]
        public FsmVector3 storePosition;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the mesh is animated.")]
        public bool everyFrame;

        private void DoGetVertexPosition()
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
                    Space space = this.space;
                    if (space == Space.World)
                    {
                        Vector3 position = component.mesh.vertices[this.vertexIndex.Value];
                        this.storePosition.set_Value(ownerDefaultTarget.transform.TransformPoint(position));
                    }
                    else if (space == Space.Self)
                    {
                        this.storePosition.set_Value(component.mesh.vertices[this.vertexIndex.Value]);
                    }
                }
            }
        }

        public override void OnEnter()
        {
            this.DoGetVertexPosition();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoGetVertexPosition();
        }

        public override void Reset()
        {
            this.gameObject = null;
            this.space = Space.World;
            this.storePosition = null;
            this.everyFrame = false;
        }
    }
}

