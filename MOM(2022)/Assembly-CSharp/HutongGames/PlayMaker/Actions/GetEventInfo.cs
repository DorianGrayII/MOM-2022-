namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using UnityEngine;

    [ActionCategory(ActionCategory.StateMachine), HutongGames.PlayMaker.Tooltip("Gets info on the last event that caused a state change. See also Set Event Data action.")]
    public class GetEventInfo : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public FsmGameObject sentByGameObject;
        [UIHint(UIHint.Variable)]
        public FsmString fsmName;
        [UIHint(UIHint.Variable)]
        public FsmBool getBoolData;
        [UIHint(UIHint.Variable)]
        public FsmInt getIntData;
        [UIHint(UIHint.Variable)]
        public FsmFloat getFloatData;
        [UIHint(UIHint.Variable)]
        public FsmVector2 getVector2Data;
        [UIHint(UIHint.Variable)]
        public FsmVector3 getVector3Data;
        [UIHint(UIHint.Variable)]
        public FsmString getStringData;
        [UIHint(UIHint.Variable)]
        public FsmGameObject getGameObjectData;
        [UIHint(UIHint.Variable)]
        public FsmRect getRectData;
        [UIHint(UIHint.Variable)]
        public FsmQuaternion getQuaternionData;
        [UIHint(UIHint.Variable)]
        public FsmMaterial getMaterialData;
        [UIHint(UIHint.Variable)]
        public FsmTexture getTextureData;
        [UIHint(UIHint.Variable)]
        public FsmColor getColorData;
        [UIHint(UIHint.Variable)]
        public FsmObject getObjectData;

        public override void OnEnter()
        {
            if (Fsm.EventData.SentByGameObject != null)
            {
                this.sentByGameObject.set_Value(Fsm.EventData.SentByGameObject);
            }
            else if (Fsm.EventData.SentByFsm != null)
            {
                this.sentByGameObject.set_Value(Fsm.EventData.SentByFsm.get_GameObject());
                this.fsmName.Value = Fsm.EventData.SentByFsm.Name;
            }
            else
            {
                this.sentByGameObject.set_Value((GameObject) null);
                this.fsmName.Value = "";
            }
            this.getBoolData.Value = Fsm.EventData.BoolData;
            this.getIntData.Value = Fsm.EventData.IntData;
            this.getFloatData.Value = Fsm.EventData.FloatData;
            this.getVector2Data.set_Value(Fsm.EventData.Vector2Data);
            this.getVector3Data.set_Value(Fsm.EventData.Vector3Data);
            this.getStringData.Value = Fsm.EventData.StringData;
            this.getGameObjectData.set_Value(Fsm.EventData.GameObjectData);
            this.getRectData.set_Value(Fsm.EventData.RectData);
            this.getQuaternionData.set_Value(Fsm.EventData.QuaternionData);
            this.getMaterialData.set_Value(Fsm.EventData.MaterialData);
            this.getTextureData.set_Value(Fsm.EventData.TextureData);
            this.getColorData.set_Value(Fsm.EventData.ColorData);
            this.getObjectData.set_Value(Fsm.EventData.ObjectData);
            base.Finish();
        }

        public override void Reset()
        {
            this.sentByGameObject = null;
            this.fsmName = null;
            this.getBoolData = null;
            this.getIntData = null;
            this.getFloatData = null;
            this.getVector2Data = null;
            this.getVector3Data = null;
            this.getStringData = null;
            this.getGameObjectData = null;
            this.getRectData = null;
            this.getQuaternionData = null;
            this.getMaterialData = null;
            this.getTextureData = null;
            this.getColorData = null;
            this.getObjectData = null;
        }
    }
}

