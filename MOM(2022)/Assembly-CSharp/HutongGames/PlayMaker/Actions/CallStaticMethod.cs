namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Call a static method in a class.")]
    public class CallStaticMethod : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Full path to the class that contains the static method.")]
        public FsmString className;
        [HutongGames.PlayMaker.Tooltip("The static method to call.")]
        public FsmString methodName;
        [HutongGames.PlayMaker.Tooltip("Method parameters. NOTE: these must match the method's signature!")]
        public FsmVar[] parameters;
        [ActionSection("Store Result"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result of the method call.")]
        public FsmVar storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        private System.Type cachedType;
        private string cachedClassName;
        private string cachedMethodName;
        private MethodInfo cachedMethodInfo;
        private ParameterInfo[] cachedParameterInfo;
        private object[] parametersArray;
        private string errorString;

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
            List<System.Type> list = new List<System.Type>(this.parameters.Length);
            foreach (FsmVar var in this.parameters)
            {
                list.Add(var.RealType);
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

        private void DoMethodCall()
        {
            if ((this.className == null) || string.IsNullOrEmpty(this.className.Value))
            {
                base.Finish();
            }
            else
            {
                if ((this.cachedClassName != this.className.Value) || (this.cachedMethodName != this.methodName.Value))
                {
                    this.errorString = string.Empty;
                    if (!this.DoCache())
                    {
                        Debug.LogError(this.errorString);
                        base.Finish();
                        return;
                    }
                }
                object obj2 = null;
                if (this.cachedParameterInfo.Length == 0)
                {
                    obj2 = this.cachedMethodInfo.Invoke(null, null);
                }
                else
                {
                    int index = 0;
                    while (true)
                    {
                        if (index >= this.parameters.Length)
                        {
                            obj2 = this.cachedMethodInfo.Invoke(null, this.parametersArray);
                            break;
                        }
                        FsmVar var = this.parameters[index];
                        var.UpdateValue();
                        this.parametersArray[index] = var.GetValue();
                        index++;
                    }
                }
                this.storeResult.SetValue(obj2);
            }
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
                string[] textArray1 = new string[] { "Parameter count does not match method.\nMethod has ", this.cachedParameterInfo.Length.ToString(), " parameters.\nYou specified ", this.parameters.Length.ToString(), " paramaters." };
                return string.Concat(textArray1);
            }
            for (int i = 0; i < this.parameters.Length; i++)
            {
                System.Type realType = this.parameters[i].RealType;
                System.Type parameterType = this.cachedParameterInfo[i].ParameterType;
                if (!ReferenceEquals(realType, parameterType))
                {
                    string text2;
                    string text3;
                    string[] textArray2 = new string[6];
                    textArray2[0] = "Parameters do not match method signature.\nParameter ";
                    textArray2[1] = (i + 1).ToString();
                    textArray2[2] = " (";
                    string[] textArray3 = textArray2;
                    if (realType != null)
                    {
                        text2 = realType.ToString();
                    }
                    else
                    {
                        System.Type local1 = realType;
                        text2 = null;
                    }
                    textArray2[3] = text2;
                    string[] local2 = textArray2;
                    local2[4] = ") should be of type: ";
                    string[] textArray4 = local2;
                    if (parameterType != null)
                    {
                        text3 = parameterType.ToString();
                    }
                    else
                    {
                        System.Type local3 = parameterType;
                        text3 = null;
                    }
                    local2[5] = text3;
                    return string.Concat(local2);
                }
            }
            if (ReferenceEquals(this.cachedMethodInfo.ReturnType, typeof(void)))
            {
                if (!string.IsNullOrEmpty(this.storeResult.variableName))
                {
                    return "Method does not have return.\nSpecify 'none' in Store Result.";
                }
            }
            else if (!ReferenceEquals(this.cachedMethodInfo.ReturnType, this.storeResult.RealType))
            {
                string text1;
                System.Type returnType = this.cachedMethodInfo.ReturnType;
                if (returnType != null)
                {
                    text1 = returnType.ToString();
                }
                else
                {
                    System.Type local4 = returnType;
                    text1 = null;
                }
                return ("Store Result is of the wrong type.\nIt should be of type: " + text1);
            }
            return string.Empty;
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
    }
}

