using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Call a static method in a class.")]
    public class CallStaticMethod : FsmStateAction
    {
        [Tooltip("Full path to the class that contains the static method.")]
        public FsmString className;

        [Tooltip("The static method to call.")]
        public FsmString methodName;

        [Tooltip("Method parameters. NOTE: these must match the method's signature!")]
        public FsmVar[] parameters;

        [ActionSection("Store Result")]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result of the method call.")]
        public FsmVar storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        private Type cachedType;

        private string cachedClassName;

        private string cachedMethodName;

        private MethodInfo cachedMethodInfo;

        private ParameterInfo[] cachedParameterInfo;

        private object[] parametersArray;

        private string errorString;

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
            if (this.className == null || string.IsNullOrEmpty(this.className.Value))
            {
                base.Finish();
                return;
            }
            if (this.cachedClassName != this.className.Value || this.cachedMethodName != this.methodName.Value)
            {
                this.errorString = string.Empty;
                if (!this.DoCache())
                {
                    Debug.LogError(this.errorString);
                    base.Finish();
                    return;
                }
            }
            object obj = null;
            if (this.cachedParameterInfo.Length == 0)
            {
                obj = this.cachedMethodInfo.Invoke(null, null);
            }
            else
            {
                for (int i = 0; i < this.parameters.Length; i++)
                {
                    FsmVar fsmVar = this.parameters[i];
                    fsmVar.UpdateValue();
                    this.parametersArray[i] = fsmVar.GetValue();
                }
                obj = this.cachedMethodInfo.Invoke(null, this.parametersArray);
            }
            this.storeResult.SetValue(obj);
        }

        private bool DoCache()
        {
            this.cachedType = ReflectionUtils.GetGlobalType(this.className.Value);
            if (this.cachedType == null)
            {
                this.errorString = this.errorString + "Class is invalid: " + this.className.Value + "\n";
                base.Finish();
                return false;
            }
            this.cachedClassName = this.className.Value;
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
            this.cachedMethodName = this.methodName.Value;
            this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
            return true;
        }

        public override string ErrorCheck()
        {
            this.errorString = string.Empty;
            this.DoCache();
            if (!string.IsNullOrEmpty(this.errorString))
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
