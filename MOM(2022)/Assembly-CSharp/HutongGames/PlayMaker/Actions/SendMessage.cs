using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Sends a Message to a Game Object. See Unity docs for SendMessage.")]
    public class SendMessage : FsmStateAction
    {
        public enum MessageType
        {
            SendMessage = 0,
            SendMessageUpwards = 1,
            BroadcastMessage = 2
        }

        [RequiredField]
        [Tooltip("GameObject that sends the message.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Where to send the message.\nSee Unity docs.")]
        public MessageType delivery;

        [Tooltip("Send options.\nSee Unity docs.")]
        public SendMessageOptions options;

        [RequiredField]
        public FunctionCall functionCall;

        public override void Reset()
        {
            this.gameObject = null;
            this.delivery = MessageType.SendMessage;
            this.options = SendMessageOptions.DontRequireReceiver;
            this.functionCall = null;
        }

        public override void OnEnter()
        {
            this.DoSendMessage();
            base.Finish();
        }

        private void DoSendMessage()
        {
            GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if (!(ownerDefaultTarget == null))
            {
                object obj = null;
                switch (this.functionCall.ParameterType)
                {
                case "bool":
                    obj = this.functionCall.BoolParameter.Value;
                    break;
                case "int":
                    obj = this.functionCall.IntParameter.Value;
                    break;
                case "float":
                    obj = this.functionCall.FloatParameter.Value;
                    break;
                case "string":
                    obj = this.functionCall.StringParameter.Value;
                    break;
                case "Vector2":
                    obj = this.functionCall.Vector2Parameter.Value;
                    break;
                case "Vector3":
                    obj = this.functionCall.Vector3Parameter.Value;
                    break;
                case "Rect":
                    obj = this.functionCall.RectParamater.Value;
                    break;
                case "GameObject":
                    obj = this.functionCall.GameObjectParameter.Value;
                    break;
                case "Material":
                    obj = this.functionCall.MaterialParameter.Value;
                    break;
                case "Texture":
                    obj = this.functionCall.TextureParameter.Value;
                    break;
                case "Color":
                    obj = this.functionCall.ColorParameter.Value;
                    break;
                case "Quaternion":
                    obj = this.functionCall.QuaternionParameter.Value;
                    break;
                case "Object":
                    obj = this.functionCall.ObjectParameter.Value;
                    break;
                case "Enum":
                    obj = this.functionCall.EnumParameter.Value;
                    break;
                case "Array":
                    obj = this.functionCall.ArrayParameter.Values;
                    break;
                }
                switch (this.delivery)
                {
                case MessageType.SendMessage:
                    ownerDefaultTarget.SendMessage(this.functionCall.FunctionName, obj, this.options);
                    break;
                case MessageType.SendMessageUpwards:
                    ownerDefaultTarget.SendMessageUpwards(this.functionCall.FunctionName, obj, this.options);
                    break;
                case MessageType.BroadcastMessage:
                    ownerDefaultTarget.BroadcastMessage(this.functionCall.FunctionName, obj, this.options);
                    break;
                }
            }
        }
    }
}
