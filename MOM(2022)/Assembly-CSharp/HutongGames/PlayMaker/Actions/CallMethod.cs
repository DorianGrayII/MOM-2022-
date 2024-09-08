namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [ActionCategory(ActionCategory.ScriptControl), HutongGames.PlayMaker.Tooltip("Call a method in a component on a GameObject.")]
    public class CallMethod : FsmStateAction
    {
        [ObjectType(typeof(Component)), HutongGames.PlayMaker.Tooltip("The behaviour on a GameObject that has the method you want to call. Drag the script component from the Unity inspector into this slot. HINT: Use Lock if the script is on another GameObject.\n\nNOTE: Unity Object fields only show the GameObject name, so for clarity we show the Behaviour name in a readonly field below.")]
        public FsmObject behaviour;
        [HutongGames.PlayMaker.Tooltip("Name of the method to call on the component")]
        public FsmString methodName;
        [HutongGames.PlayMaker.Tooltip("Method parameters. NOTE: these must match the method's signature!")]
        public FsmVar[] parameters;
        [ActionSection("Store Result"), UIHint(UIHint.Variable), HutongGames.PlayMaker.Tooltip("Store the result of the method call.")]
        public FsmVar storeResult;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyFrame;
        [HutongGames.PlayMaker.Tooltip("Use the old manual editor UI.")]
        public bool manualUI;
        private FsmObject cachedBehaviour;
        private FsmString cachedMethodName;
        private System.Type cachedType;
        private MethodInfo cachedMethodInfo;
        private ParameterInfo[] cachedParameterInfo;
        private object[] parametersArray;
        private string errorString;

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
            if (this.cachedBehaviour.get_Value() == null)
            {
                if (!this.behaviour.UsesVariable || Application.isPlaying)
                {
                    this.errorString = this.errorString + "Behaviour is invalid!\n";
                }
                base.Finish();
                return false;
            }
            this.cachedType = this.behaviour.get_Value().GetType();
            List<System.Type> list = new List<System.Type>(this.parameters.Length);
            foreach (FsmVar var in this.parameters)
            {
                list.Add(var.RealType);
            }
            this.cachedMethodInfo = this.cachedType.GetMethod(this.methodName.Value, list.ToArray());
            if (this.cachedMethodInfo != null)
            {
                this.cachedParameterInfo = this.cachedMethodInfo.GetParameters();
                return true;
            }
            this.errorString = this.errorString + "Invalid Method Name or Parameters: " + this.methodName.Value + "\n";
            base.Finish();
            return false;
        }

        private void DoMethodCall()
        {
            if (this.behaviour.get_Value() == null)
            {
                base.Finish();
            }
            else if (this.NeedToUpdateCache() && !this.DoCache())
            {
                Debug.LogError(this.errorString);
                base.Finish();
            }
            else
            {
                object obj2;
                if (this.cachedParameterInfo.Length == 0)
                {
                    obj2 = this.cachedMethodInfo.Invoke(this.cachedBehaviour.get_Value(), null);
                }
                else
                {
                    int index = 0;
                    while (true)
                    {
                        if (index >= this.parameters.Length)
                        {
                            obj2 = this.cachedMethodInfo.Invoke(this.cachedBehaviour.get_Value(), this.parametersArray);
                            break;
                        }
                        FsmVar var = this.parameters[index];
                        var.UpdateValue();
                        if (var.Type != VariableType.Array)
                        {
                            var.UpdateValue();
                            this.parametersArray[index] = var.GetValue();
                        }
                        else
                        {
                            var.UpdateValue();
                            object[] objArray = var.GetValue() as object[];
                            Array array = Array.CreateInstance(this.cachedParameterInfo[index].ParameterType.GetElementType(), objArray.Length);
                            int num2 = 0;
                            while (true)
                            {
                                if (num2 >= objArray.Length)
                                {
                                    this.parametersArray[index] = array;
                                    break;
                                }
                                array.SetValue(objArray[num2], num2);
                                num2++;
                            }
                        }
                        index++;
                    }
                }
                if ((this.storeResult != null) && (!this.storeResult.IsNone && (this.storeResult.Type != VariableType.Unknown)))
                {
                    this.storeResult.SetValue(obj2);
                }
            }
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

        private bool NeedToUpdateCache()
        {
            return ((this.cachedBehaviour == null) || ((this.cachedMethodName == null) || ((this.cachedBehaviour.get_Value() != this.behaviour.get_Value()) || ((this.cachedBehaviour.Name != this.behaviour.Name) || ((this.cachedMethodName.Value != this.methodName.Value) || (this.cachedMethodName.Name != this.methodName.Name))))));
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

        public override void Reset()
        {
            this.behaviour = null;
            this.methodName = null;
            this.parameters = null;
            this.storeResult = null;
            this.everyFrame = false;
        }
    }
}

