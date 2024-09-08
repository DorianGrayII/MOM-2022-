using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Call a method in a component on a GameObject.")]
    public class CallMethod : FsmStateAction
    {
        [ObjectType(typeof(Component))]
        [Tooltip("The behaviour on a GameObject that has the method you want to call. Drag the script component from the Unity inspector into this slot. HINT: Use Lock if the script is on another GameObject.\n\nNOTE: Unity Object fields only show the GameObject name, so for clarity we show the Behaviour name in a readonly field below.")]
        public FsmObject behaviour;

        [Tooltip("Name of the method to call on the component")]
        public FsmString methodName;

        [Tooltip("Method parameters. NOTE: these must match the method's signature!")]
        public FsmVar[] parameters;

        [ActionSection("Store Result")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result of the method call.")]
        public FsmVar storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        [Tooltip("Use the old manual editor UI.")]
        public bool manualUI;

        private FsmObject cachedBehaviour;

        private FsmString cachedMethodName;

        private Type cachedType;

        private MethodInfo cachedMethodInfo;

        private ParameterInfo[] cachedParameterInfo;

        private object[] parametersArray;

        private string errorString;

        public override void Reset()
        {
            this.behaviour = null;
            this.methodName = null;
            this.parameters = null;
            this.storeResult = null;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.parametersArray = new object[this.parameters.Length];
            this.DoMethodCall();
            if (!this.everyFrame)
            {
                base.Finish();
            }
        }

        public override void OnUpdate()
        {
            this.DoMethodCall();
        }

        private void DoMethodCall()
        {
            if (this.behaviour.Value == null)
            {
                base.Finish();
                return;
            }
            if (this.NeedToUpdateCache() && !this.DoCache())
            {
                Debug.LogError(this.errorString);
                base.Finish();
                return;
            }
            object value;
            if (this.cachedParameterInfo.Length == 0)
            {
                value = this.cachedMethodInfo.Invoke(this.cachedBehaviour.Value, null);
            }
            else
            {
                for (int i = 0; i < this.parameters.Length; i++)
                {
                    FsmVar fsmVar = this.parameters[i];
                    fsmVar.UpdateValue();
                    if (fsmVar.Type == VariableType.Array)
                    {
                        fsmVar.UpdateValue();
                        object[] array = fsmVar.GetValue() as object[];
                        Array array2 = Array.CreateInstance(this.cachedParameterInfo[i].ParameterType.GetElementType(), array.Length);
                        for (int j = 0; j < array.Length; j++)
                        {
                            array2.SetValue(array[j], j);
                        }
                        this.parametersArray[i] = array2;
                    }
                    else
                    {
                        fsmVar.UpdateValue();
                        this.parametersArray[i] = fsmVar.GetValue();
                    }
                }
                value = this.cachedMethodInfo.Invoke(this.cachedBehaviour.Value, this.parametersArray);
            }
            if (this.storeResult != null && !this.storeResult.IsNone && this.storeResult.Type != VariableType.Unknown)
            {
                this.storeResult.SetValue(value);
            }
        }

        private bool NeedToUpdateCache()
        {
            if (this.cachedBehaviour != null && this.cachedMethodName != null && !(this.cachedBehaviour.Value != this.behaviour.Value) && !(this.cachedBehaviour.Name != this.behaviour.Name) && !(this.cachedMethodName.Value != this.methodName.Value))
            {
                return this.cachedMethodName.Name != this.methodName.Name;
            }
            return true;
        }

        private void ClearCache()
        {
            this.cachedBehaviour = null;
            this.cachedMethodName = null;
            this.cachedType = null;
            this.cachedMethodInfo = null;
            this.cachedParameterInfo = null;
        }

        private bool DoCache()
        {
            this.ClearCache();
            this.errorString = string.Empty;
            this.cachedBehaviour = new FsmObject(this.behaviour);
            this.cachedMethodName = new FsmString(this.methodName);
            if (this.cachedBehaviour.Value == null)
            {
                if (!this.behaviour.UsesVariable || Application.isPlaying)
                {
                    this.errorString += "Behaviour is invalid!\n";
                }
                base.Finish();
                return false;
            }
            this.cachedType = this.behaviour.Value.GetType();
            List<Type> list = new List<Type>(this.parameters.Length);
            FsmVar[] array = this.parameters;
            foreach (FsmVar fsmVar in array)
            {
                list.Add(fsmVar.RealType);
            }
            this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, list.ToArray());
            if (this.cachedMethodInfo == null)
            {
                this.errorString = this.errorString + "Invalid Method Name or Parameters: " + this.methodName.Value + "\n";
                base.Finish();
                return false;
            }
            this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
            return true;
        }

        public override string ErrorCheck()
        {
            if (Application.isPlaying)
            {
                return this.errorString;
            }
            if (!this.DoCache())
            {
                return this.errorString;
            }
            if (this.parameters.Length != this.cachedParameterInfo.Length)
            {
                return "Parameter count does not match method.\nMethod has " + this.cachedParameterInfo.Length + " parameters.\nYou specified " + this.parameters.Length + " paramaters.";
            }
            for (int i = 0; i < this.parameters.Length; i++)
            {
                Type realType = this.parameters[i].RealType;
                Type parameterType = this.cachedParameterInfo[i].ParameterType;
                if ((object)realType != parameterType)
                {
                    return "Parameters do not match method signature.\nParameter " + (i + 1) + " (" + realType?.ToString() + ") should be of type: " + parameterType;
                }
            }
            if ((object)this.cachedMethodInfo.ReturnType == typeof(void))
            {
                if (!string.IsNullOrEmpty(this.storeResult.variableName))
                {
                    return "Method does not have return.\nSpecify 'none' in Store Result.";
                }
            }
            else if ((object)this.cachedMethodInfo.ReturnType != this.storeResult.RealType)
            {
                return "Store Result is of the wrong type.\nIt should be of type: " + this.cachedMethodInfo.ReturnType;
            }
            return string.Empty;
        }
    }
}
