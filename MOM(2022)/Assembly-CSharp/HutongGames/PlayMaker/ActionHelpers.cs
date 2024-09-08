namespace HutongGames.PlayMaker
{
    using HutongGames.PlayMaker.AnimationEnums;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class ActionHelpers
    {
        public static RaycastHit mousePickInfo;
        private static float mousePickRaycastTime;
        private static float mousePickDistanceUsed;
        private static int mousePickLayerMaskUsed;

        public static void AddAnimationClip(GameObject go, AnimationClip animClip)
        {
            if (animClip != null)
            {
                Animation component = go.GetComponent<Animation>();
                if (component != null)
                {
                    component.AddClip(animClip, animClip.name);
                }
            }
        }

        public static string AutoName(FsmStateAction action, params INamedVariable[] exposedFields)
        {
            return ((action == null) ? null : AutoName(action.GetType().Name, exposedFields));
        }

        public static string AutoName(string actionName, params INamedVariable[] exposedFields)
        {
            string str = actionName + " :";
            foreach (INamedVariable variable in exposedFields)
            {
                str = str + " " + GetValueLabel(variable);
            }
            return str;
        }

        public static string AutoNameConvert(FsmStateAction action, NamedVariable fromVariable, NamedVariable toVariable)
        {
            return ((action == null) ? null : AutoNameConvert(action.GetType().Name, fromVariable, toVariable));
        }

        public static string AutoNameConvert(string actionName, NamedVariable fromVariable, NamedVariable toVariable)
        {
            string[] textArray1 = new string[] { actionName.Replace("Convert", ""), " : ", GetValueLabel(fromVariable), " to ", GetValueLabel(toVariable) };
            return string.Concat(textArray1);
        }

        public static string AutoNameGetProperty(FsmStateAction action, NamedVariable property, NamedVariable store)
        {
            return ((action == null) ? null : AutoNameGetProperty(action.GetType().Name, property, store));
        }

        public static string AutoNameGetProperty(string actionName, NamedVariable property, NamedVariable store)
        {
            string[] textArray1 = new string[] { actionName, " : ", GetValueLabel(property), " -> ", GetValueLabel(store) };
            return string.Concat(textArray1);
        }

        public static string AutoNameRange(FsmStateAction action, NamedVariable min, NamedVariable max)
        {
            return ((action == null) ? null : AutoNameRange(action.GetType().Name, min, max));
        }

        public static string AutoNameRange(string actionName, NamedVariable min, NamedVariable max)
        {
            string[] textArray1 = new string[] { actionName, " : ", GetValueLabel(min), " - ", GetValueLabel(max) };
            return string.Concat(textArray1);
        }

        public static string AutoNameSetVar(FsmStateAction action, NamedVariable var, NamedVariable value)
        {
            return ((action == null) ? null : AutoNameSetVar(action.GetType().Name, var, value));
        }

        public static string AutoNameSetVar(string actionName, NamedVariable var, NamedVariable value)
        {
            string[] textArray1 = new string[] { actionName, " : ", GetValueLabel(var), " = ", GetValueLabel(value) };
            return string.Concat(textArray1);
        }

        public static Color BlendColor(ColorBlendMode blendMode, Color c1, Color c2)
        {
            switch (blendMode)
            {
                case ColorBlendMode.Normal:
                    return Color.Lerp(c1, c2, c2.a);

                case ColorBlendMode.Multiply:
                    return Color.Lerp(c1, c1 * c2, c2.a);

                case ColorBlendMode.Screen:
                {
                    Color b = Color.white - ((Color.white - c1) * (Color.white - c2));
                    return Color.Lerp(c1, b, c2.a);
                }
            }
            throw new ArgumentOutOfRangeException();
        }

        public static bool CanEditTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
        {
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    return false;

                case PositionOptions.WorldPosition:
                case PositionOptions.LocalPosition:
                case PositionOptions.WorldOffset:
                case PositionOptions.LocalOffset:
                    return !position.IsNone;

                case PositionOptions.TargetGameObject:
                    return (target.get_Value() != null);
            }
            throw new ArgumentOutOfRangeException();
        }

        private static bool CanEditTargetRotation(RotationOptions option, NamedVariable rotation, FsmGameObject target)
        {
            switch (option)
            {
                case RotationOptions.CurrentRotation:
                    return false;

                case RotationOptions.WorldRotation:
                case RotationOptions.LocalRotation:
                case RotationOptions.WorldOffsetRotation:
                case RotationOptions.LocalOffsetRotation:
                    return !rotation.IsNone;

                case RotationOptions.MatchGameObjectRotation:
                    return (target.get_Value() != null);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static string CheckForValidEvent(FsmState state, string eventName)
        {
            int num;
            if (state == null)
            {
                return "Invalid State!";
            }
            if (string.IsNullOrEmpty(eventName))
            {
                return "";
            }
            FsmTransition[] globalTransitions = state.Fsm.GlobalTransitions;
            for (num = 0; num < globalTransitions.Length; num++)
            {
                if (globalTransitions[num].EventName == eventName)
                {
                    return "";
                }
            }
            globalTransitions = state.Transitions;
            for (num = 0; num < globalTransitions.Length; num++)
            {
                if (globalTransitions[num].EventName == eventName)
                {
                    return "";
                }
            }
            return ("Fsm will not respond to Event: " + eventName);
        }

        public static string CheckOwnerPhysics2dSetup(GameObject gameObject)
        {
            return CheckPhysics2dSetup(gameObject);
        }

        public static string CheckOwnerPhysicsSetup(GameObject gameObject)
        {
            return CheckPhysicsSetup(gameObject);
        }

        public static string CheckPhysics2dSetup(FsmOwnerDefault ownerDefault)
        {
            return ((ownerDefault != null) ? CheckPhysics2dSetup(ownerDefault.GameObject.get_Value()) : "");
        }

        public static string CheckPhysics2dSetup(GameObject gameObject)
        {
            string str = string.Empty;
            if ((gameObject != null) && ((gameObject.GetComponent<Collider2D>() == null) && (gameObject.GetComponent<Rigidbody2D>() == null)))
            {
                str = str + "GameObject requires a RigidBody2D or Collider2D component!\n";
            }
            return str;
        }

        public static string CheckPhysicsSetup(FsmOwnerDefault ownerDefault)
        {
            return ((ownerDefault != null) ? CheckPhysicsSetup(ownerDefault.GameObject.get_Value()) : "");
        }

        public static string CheckPhysicsSetup(GameObject gameObject)
        {
            string str = string.Empty;
            if ((gameObject != null) && ((gameObject.GetComponent<Collider>() == null) && (gameObject.GetComponent<Rigidbody>() == null)))
            {
                str = str + "GameObject requires RigidBody/Collider!\n";
            }
            return str;
        }

        public static string CheckRayDistance(float rayDistance)
        {
            return ((rayDistance <= 0f) ? "Ray Distance should be greater than zero!\n" : "");
        }

        public static void DebugLog(Fsm fsm, HutongGames.PlayMaker.LogLevel logLevel, string text, bool sendToUnityLog)
        {
            if (!Application.isEditor & sendToUnityLog)
            {
                string message = FormatUnityLogString(text);
                if (logLevel == HutongGames.PlayMaker.LogLevel.Warning)
                {
                    Debug.LogWarning(message);
                }
                else if (logLevel != HutongGames.PlayMaker.LogLevel.Error)
                {
                    Debug.Log(message);
                }
                else
                {
                    Debug.LogError(message);
                }
            }
            if (FsmLog.LoggingEnabled && (fsm != null))
            {
                switch (logLevel)
                {
                    case HutongGames.PlayMaker.LogLevel.Info:
                        fsm.MyLog.LogAction(FsmLogType.Info, text, sendToUnityLog);
                        return;

                    case HutongGames.PlayMaker.LogLevel.Warning:
                        fsm.MyLog.LogAction(FsmLogType.Warning, text, sendToUnityLog);
                        return;

                    case HutongGames.PlayMaker.LogLevel.Error:
                        fsm.MyLog.LogAction(FsmLogType.Error, text, sendToUnityLog);
                        return;
                }
            }
        }

        private static void DoMousePick(float distance, int layerMask)
        {
            if (Camera.main != null)
            {
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mousePickInfo, distance, layerMask);
                mousePickLayerMaskUsed = layerMask;
                mousePickDistanceUsed = distance;
                mousePickRaycastTime = Time.frameCount;
            }
        }

        public static string FormatUnityLogString(string text)
        {
            if (FsmExecutionStack.ExecutingFsm == null)
            {
                return text;
            }
            string fullFsmLabel = Fsm.GetFullFsmLabel(FsmExecutionStack.ExecutingFsm);
            if (FsmExecutionStack.ExecutingState != null)
            {
                fullFsmLabel = fullFsmLabel + " : " + FsmExecutionStack.ExecutingStateName;
            }
            if (FsmExecutionStack.ExecutingAction != null)
            {
                fullFsmLabel = fullFsmLabel + FsmExecutionStack.ExecutingAction.Name;
            }
            return (fullFsmLabel + " : " + text);
        }

        public static PlayMakerFSM GetGameObjectFsm(GameObject go, string fsmName)
        {
            if (!string.IsNullOrEmpty(fsmName))
            {
                PlayMakerFSM[] components = go.GetComponents<PlayMakerFSM>();
                int index = 0;
                while (true)
                {
                    if (index >= components.Length)
                    {
                        Debug.LogWarning("Could not find FSM: " + fsmName);
                        break;
                    }
                    PlayMakerFSM rfsm = components[index];
                    if (rfsm.FsmName == fsmName)
                    {
                        return rfsm;
                    }
                    index++;
                }
            }
            return go.GetComponent<PlayMakerFSM>();
        }

        public static GameObject GetOwnerDefault(FsmStateAction action, FsmOwnerDefault ownerDefault)
        {
            return action.Fsm.GetOwnerDefaultTarget(ownerDefault);
        }

        public static Vector3 GetPosition(FsmGameObject fsmGameObject, FsmVector3 fsmVector3)
        {
            return ((fsmGameObject.get_Value() == null) ? fsmVector3.get_Value() : (!fsmVector3.IsNone ? fsmGameObject.get_Value().transform.TransformPoint(fsmVector3.get_Value()) : fsmGameObject.get_Value().transform.position));
        }

        public static int GetRandomWeightedIndex(FsmFloat[] weights)
        {
            float maxInclusive = 0f;
            foreach (FsmFloat num4 in weights)
            {
                maxInclusive += num4.Value;
            }
            float num2 = UnityEngine.Random.Range(0f, maxInclusive);
            for (int i = 0; i < weights.Length; i++)
            {
                if (num2 < weights[i].Value)
                {
                    return i;
                }
                num2 -= weights[i].Value;
            }
            return -1;
        }

        public static Vector3 GetTargetPosition(PositionOptions option, Transform owner, Transform target, Vector3 position)
        {
            if (owner == null)
            {
                return Vector3.zero;
            }
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    return owner.position;

                case PositionOptions.WorldPosition:
                    return position;

                case PositionOptions.LocalPosition:
                    return ((owner.parent != null) ? owner.parent.TransformPoint(position) : position);

                case PositionOptions.WorldOffset:
                    return (owner.position + position);

                case PositionOptions.LocalOffset:
                    return owner.TransformPoint(position);

                case PositionOptions.TargetGameObject:
                    return ((target != null) ? (!(position != Vector3.zero) ? target.position : target.TransformPoint(position)) : owner.position);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static bool GetTargetPosition(PositionOptions option, Transform owner, FsmVector3 position, FsmGameObject target, out Vector3 targetPosition)
        {
            targetPosition = Vector3.zero;
            if ((owner == null) || !IsValidTargetPosition(option, position, target))
            {
                return false;
            }
            targetPosition = GetTargetPosition(option, owner, ((target == null) || (target.get_Value() == null)) ? null : target.get_Value().transform, position.get_Value());
            return true;
        }

        public static Quaternion GetTargetRotation(RotationOptions option, Transform owner, Transform target, Vector3 rotation)
        {
            if (owner == null)
            {
                return Quaternion.identity;
            }
            switch (option)
            {
                case RotationOptions.CurrentRotation:
                    return owner.rotation;

                case RotationOptions.WorldRotation:
                    return Quaternion.Euler(rotation);

                case RotationOptions.LocalRotation:
                    return ((owner.parent != null) ? (owner.parent.rotation * Quaternion.Euler(rotation)) : Quaternion.Euler(rotation));

                case RotationOptions.WorldOffsetRotation:
                    return (Quaternion.Euler(rotation) * owner.rotation);

                case RotationOptions.LocalOffsetRotation:
                    return (owner.rotation * Quaternion.Euler(rotation));

                case RotationOptions.MatchGameObjectRotation:
                    return ((target != null) ? (target.rotation * Quaternion.Euler(rotation)) : owner.rotation);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static bool GetTargetRotation(RotationOptions option, Transform owner, FsmVector3 rotation, FsmGameObject target, out Quaternion targetRotation)
        {
            targetRotation = Quaternion.identity;
            if ((owner == null) || !CanEditTargetRotation(option, rotation, target))
            {
                return false;
            }
            targetRotation = GetTargetRotation(option, owner, target.get_Value()?.transform, rotation.get_Value());
            return true;
        }

        public static Vector3 GetTargetScale(ScaleOptions option, Transform owner, Transform target, Vector3 scale)
        {
            if (owner == null)
            {
                return Vector3.one;
            }
            switch (option)
            {
                case ScaleOptions.CurrentScale:
                    return owner.localScale;

                case ScaleOptions.LocalScale:
                    return scale;

                case ScaleOptions.MultiplyScale:
                    return new Vector3(owner.localScale.x * scale.x, owner.localScale.y * scale.y, owner.localScale.z * scale.z);

                case ScaleOptions.AddToScale:
                    return new Vector3(owner.localScale.x + scale.x, owner.localScale.y + scale.y, owner.localScale.z + scale.z);

                case ScaleOptions.MatchGameObject:
                    return ((target != null) ? target.localScale : owner.localScale);
            }
            return owner.localScale;
        }

        public static string GetValueLabel(INamedVariable variable)
        {
            return "";
        }

        public static string GetValueLabel(Fsm fsm, FsmOwnerDefault ownerDefault)
        {
            return ((ownerDefault != null) ? ((ownerDefault.OwnerOption != OwnerDefaultOption.UseOwner) ? GetValueLabel(ownerDefault.GameObject) : "Owner") : "[null]");
        }

        public static bool HasAnimationFinished(AnimationState anim, float prevTime, float currentTime)
        {
            if ((anim.wrapMode == WrapMode.Loop) || (anim.wrapMode == WrapMode.PingPong))
            {
                return false;
            }
            if ((((anim.wrapMode != WrapMode.Default) && (anim.wrapMode != WrapMode.Once)) || (prevTime <= 0f)) || !currentTime.Equals((float) 0f))
            {
                return ((prevTime < anim.length) && (currentTime >= anim.length));
            }
            return true;
        }

        public static bool IsLoopingWrapMode(WrapMode wrapMode)
        {
            return ((wrapMode == WrapMode.Loop) || (wrapMode == WrapMode.PingPong));
        }

        public static bool IsMouseOver(GameObject gameObject, float distance, int layerMask)
        {
            return ((gameObject != null) ? (gameObject == MouseOver(distance, layerMask)) : false);
        }

        private static bool IsValidTargetPosition(PositionOptions option, NamedVariable position, FsmGameObject target)
        {
            switch (option)
            {
                case PositionOptions.CurrentPosition:
                    return true;

                case PositionOptions.WorldPosition:
                case PositionOptions.LocalPosition:
                case PositionOptions.WorldOffset:
                case PositionOptions.LocalOffset:
                    return !position.IsNone;

                case PositionOptions.TargetGameObject:
                    return (target.get_Value() != null);
            }
            throw new ArgumentOutOfRangeException();
        }

        public static bool IsVisible(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            Renderer component = go.GetComponent<Renderer>();
            return ((component != null) && component.isVisible);
        }

        public static int LayerArrayToLayerMask(FsmInt[] layers, bool invert)
        {
            int num = 0;
            foreach (FsmInt num3 in layers)
            {
                num |= 1 << (num3.Value & 0x1f);
            }
            if (invert)
            {
                num = ~num;
            }
            return ((num == 0) ? -5 : num);
        }

        public static void LogError(string text)
        {
            DebugLog(FsmExecutionStack.ExecutingFsm, HutongGames.PlayMaker.LogLevel.Error, text, true);
        }

        public static void LogWarning(string text)
        {
            DebugLog(FsmExecutionStack.ExecutingFsm, HutongGames.PlayMaker.LogLevel.Warning, text, true);
        }

        public static GameObject MouseOver(float distance, int layerMask)
        {
            if (!mousePickRaycastTime.Equals((float) Time.frameCount) || ((mousePickDistanceUsed < distance) || (mousePickLayerMaskUsed != layerMask)))
            {
                DoMousePick(distance, layerMask);
            }
            return (((mousePickInfo.collider == null) || (mousePickInfo.distance >= distance)) ? null : mousePickInfo.collider.gameObject);
        }

        public static RaycastHit MousePick(float distance, int layerMask)
        {
            if (!mousePickRaycastTime.Equals((float) Time.frameCount) || ((mousePickDistanceUsed < distance) || (mousePickLayerMaskUsed != layerMask)))
            {
                DoMousePick(distance, layerMask);
            }
            return mousePickInfo;
        }

        [Obsolete("Use LogError instead.")]
        public static void RuntimeError(FsmStateAction action, string error)
        {
            string text1;
            if (action != null)
            {
                text1 = action.ToString();
            }
            else
            {
                FsmStateAction local1 = action;
                text1 = null;
            }
            action.LogError(text1 + " : " + error);
        }

        public static Texture2D WhiteTexture
        {
            get
            {
                return Texture2D.whiteTexture;
            }
        }
    }
}

