namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;

    [ActionCategory(ActionCategory.StateMachine), Tooltip("Sets Event Data before sending an event. Get the Event Data, along with sender information, using Get Event Info action.")]
    public class SetEventData : FsmStateAction
    {
        public FsmGameObject setGameObjectData;
        public FsmInt setIntData;
        public FsmFloat setFloatData;
        public FsmString setStringData;
        public FsmBool setBoolData;
        public FsmVector2 setVector2Data;
        public FsmVector3 setVector3Data;
        public FsmRect setRectData;
        public FsmQuaternion setQuaternionData;
        public FsmColor setColorData;
        public FsmMaterial setMaterialData;
        public FsmTexture setTextureData;
        public FsmObject setObjectData;

        public override void OnEnter()
        {
            Fsm.EventData.BoolData = this.setBoolData.Value;
            Fsm.EventData.IntData = this.setIntData.Value;
            Fsm.EventData.FloatData = this.setFloatData.Value;
            Fsm.EventData.Vector2Data = this.setVector2Data.get_Value();
            Fsm.EventData.Vector3Data = this.setVector3Data.get_Value();
            Fsm.EventData.StringData = this.setStringData.Value;
            Fsm.EventData.GameObjectData = this.setGameObjectData.get_Value();
            Fsm.EventData.RectData = this.setRectData.get_Value();
            Fsm.EventData.QuaternionData = this.setQuaternionData.get_Value();
            Fsm.EventData.ColorData = this.setColorData.get_Value();
            Fsm.EventData.MaterialData = this.setMaterialData.get_Value();
            Fsm.EventData.TextureData = this.setTextureData.get_Value();
            Fsm.EventData.ObjectData = this.setObjectData.get_Value();
            base.Finish();
        }

        public override void Reset()
        {
            FsmGameObject obj1 = new FsmGameObject();
            obj1.UseVariable = true;
            this.setGameObjectData = obj1;
            FsmInt num1 = new FsmInt();
            num1.UseVariable = true;
            this.setIntData = num1;
            FsmFloat num2 = new FsmFloat();
            num2.UseVariable = true;
            this.setFloatData = num2;
            FsmString text1 = new FsmString();
            text1.UseVariable = true;
            this.setStringData = text1;
            FsmBool bool1 = new FsmBool();
            bool1.UseVariable = true;
            this.setBoolData = bool1;
            FsmVector2 vector1 = new FsmVector2();
            vector1.UseVariable = true;
            this.setVector2Data = vector1;
            FsmVector3 vector2 = new FsmVector3();
            vector2.UseVariable = true;
            this.setVector3Data = vector2;
            FsmRect rect1 = new FsmRect();
            rect1.UseVariable = true;
            this.setRectData = rect1;
            FsmQuaternion quaternion1 = new FsmQuaternion();
            quaternion1.UseVariable = true;
            this.setQuaternionData = quaternion1;
            FsmColor color1 = new FsmColor();
            color1.UseVariable = true;
            this.setColorData = color1;
            FsmMaterial material1 = new FsmMaterial();
            material1.UseVariable = true;
            this.setMaterialData = material1;
            FsmTexture texture1 = new FsmTexture();
            texture1.UseVariable = true;
            this.setTextureData = texture1;
            FsmObject obj2 = new FsmObject();
            obj2.UseVariable = true;
            this.setObjectData = obj2;
        }
    }
}

