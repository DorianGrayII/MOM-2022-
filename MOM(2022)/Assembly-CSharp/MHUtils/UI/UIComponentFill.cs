namespace MHUtils.UI
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using TMPro;
    using UnityEngine;

    public class UIComponentFill : MonoBehaviour
    {
        public static void LinkDropdown(GameObject gameObject, object source, string dropdownName, string fieldName, List<string> options, Callback callback, bool localize)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            PropertyInfo property = null;
            if (field == null)
            {
                property = source.GetType().GetProperty(fieldName);
            }
            if ((field != null) || (property != null))
            {
                object obj2 = field?.GetValue(source);
                string option = Convert.ToString((property != null) ? property.GetValue(source) : obj2);
                DropDownFilters local1 = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName, false);
                local1.onChange = null;
                local1.SetOptions(options, true, localize);
                local1.SelectOption(option, false, false);
                local1.onChange = delegate (object obj) {
                    string str = obj as string;
                    try
                    {
                        if (field != null)
                        {
                            field.SetValue(source, str);
                        }
                        else
                        {
                            property.SetValue(source, str);
                        }
                        if (callback == null)
                        {
                            Callback local1 = callback;
                        }
                        else
                        {
                            callback(source);
                        }
                    }
                    catch (Exception exception)
                    {
                        string text1;
                        if (exception != null)
                        {
                            text1 = exception.ToString();
                        }
                        else
                        {
                            Exception local2 = exception;
                            text1 = null;
                        }
                        Debug.LogError("[ERROR]" + text1);
                    }
                };
            }
        }

        public static void LinkDropdownCustom(DropDownFilters filter, CallbackRet customFill, object data, CallbackRet selectedItem, Callback callback, bool localize)
        {
            filter.onChange = null;
            List<string> options = customFill(data) as List<string>;
            filter.SetOptions(options, true, localize);
            object obj2 = selectedItem(options);
            if (obj2 is string)
            {
                filter.SelectOption(obj2 as string, false, false);
            }
            else if (obj2 is int)
            {
                filter.SelectOption((int) obj2, false, true);
            }
            else if (obj2 is Enum)
            {
                filter.SelectOption((Enum) obj2, false);
            }
            filter.onChange = delegate (object obj) {
                if (callback == null)
                {
                    Callback local1 = callback;
                }
                else
                {
                    callback(obj);
                }
            };
        }

        public static void LinkDropdownEnum<T>(DropDownFilters filter, Callback callback, bool acceptDefaultOption, string selectedOption, bool localize) where T: struct
        {
            filter.onChange = null;
            filter.SetOptions(typeof(T), false, true, localize, acceptDefaultOption);
            filter.SelectOption(selectedOption, true, false);
            filter.onChange = callback;
        }

        public static void LinkDropdownEnum<T>(GameObject gameObject, object source, string dropdownName, string fieldName, Callback callback) where T: struct
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            string option = Convert.ToString(field.GetValue(source));
            DropDownFilters local1 = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName, false);
            local1.onChange = null;
            local1.SetOptions(typeof(T), false, true, false, true);
            local1.SelectOption(option, true, false);
            local1.onChange = delegate (object obj) {
                string str = obj as string;
                try
                {
                    if (str == null)
                    {
                        T local = default(T);
                        field.SetValue(source, local);
                    }
                    else
                    {
                        T local2 = (T) Enum.Parse(typeof(T), str);
                        field.SetValue(source, local2);
                    }
                    if (callback == null)
                    {
                        Callback local1 = callback;
                    }
                    else
                    {
                        callback(source);
                    }
                }
                catch (Exception exception)
                {
                    string text1;
                    if (exception != null)
                    {
                        text1 = exception.ToString();
                    }
                    else
                    {
                        Exception local3 = exception;
                        text1 = null;
                    }
                    Debug.LogError("[ERROR]" + text1);
                }
            };
        }

        public static void LinkDropdownEnumStringField(System.Type type, GameObject gameObject, object source, string dropdownName, string fieldName, Callback callback)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            string option = field.GetValue(source) as string;
            DropDownFilters local1 = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName, false);
            local1.onChange = null;
            local1.SetOptions(type, false, true, false, true);
            local1.SelectOption(option, false, false);
            local1.onChange = delegate (object obj) {
                string str = obj as string;
                try
                {
                    if (str == null)
                    {
                        field.SetValue(source, null);
                    }
                    else
                    {
                        field.SetValue(source, str);
                    }
                    if (callback == null)
                    {
                        Callback local1 = callback;
                    }
                    else
                    {
                        callback(source);
                    }
                }
                catch (Exception exception)
                {
                    string text1;
                    if (exception != null)
                    {
                        text1 = exception.ToString();
                    }
                    else
                    {
                        Exception local2 = exception;
                        text1 = null;
                    }
                    Debug.LogError("[ERROR]" + text1);
                }
            };
        }

        public static void LinkInputField<T>(GameObject gameObject, object source, string tfName, string fieldName, Callback callback, Callback finalCallback)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            TMP_InputField tf = GameObjectUtils.FindByNameGetComponent<TMP_InputField>(gameObject, tfName, false);
            tf.onValueChanged.RemoveAllListeners();
            tf.onEndEdit.RemoveAllListeners();
            object obj2 = field.GetValue(source);
            string str = (obj2 == null) ? "" : obj2.ToString();
            tf.text = !string.IsNullOrEmpty(str) ? str : "";
            tf.onValueChanged.AddListener(delegate (string newValue) {
                if (!string.IsNullOrEmpty(newValue))
                {
                    try
                    {
                        object obj2 = Convert.ChangeType(newValue, typeof(T));
                        field.SetValue(source, obj2);
                        if (callback == null)
                        {
                            Callback local1 = callback;
                        }
                        else
                        {
                            callback(source);
                        }
                    }
                    catch (Exception exception)
                    {
                        string text1;
                        string text2;
                        string[] textArray1 = new string[8];
                        textArray1[0] = "Value ";
                        textArray1[1] = newValue;
                        textArray1[2] = " cannot be implanted into field ";
                        textArray1[3] = fieldName;
                        textArray1[4] = " of type ";
                        System.Type type1 = typeof(T);
                        string[] textArray2 = textArray1;
                        if (type1 != null)
                        {
                            text1 = type1.ToString();
                        }
                        else
                        {
                            System.Type local2 = type1;
                            text1 = null;
                        }
                        textArray1[5] = text1;
                        string[] local3 = textArray1;
                        local3[6] = " due to conversion error. \n ";
                        string[] textArray3 = local3;
                        if (exception != null)
                        {
                            text2 = exception.ToString();
                        }
                        else
                        {
                            Exception local4 = exception;
                            text2 = null;
                        }
                        local3[7] = text2;
                        Debug.LogWarning(string.Concat(local3));
                    }
                }
            });
            tf.onEndEdit.AddListener(delegate (string newValue) {
                if (string.IsNullOrEmpty(newValue) && (tf.contentType == TMP_InputField.ContentType.IntegerNumber))
                {
                    field.SetValue(source, 0);
                    tf.text = "0";
                }
                if (finalCallback != null)
                {
                    try
                    {
                        if (finalCallback == null)
                        {
                            Callback local1 = finalCallback;
                        }
                        else
                        {
                            finalCallback(source);
                        }
                    }
                    catch (Exception exception)
                    {
                        string text1;
                        string text2;
                        string[] textArray1 = new string[8];
                        textArray1[0] = "Value ";
                        textArray1[1] = newValue;
                        textArray1[2] = " cannot be implanted into field ";
                        textArray1[3] = fieldName;
                        textArray1[4] = " of type ";
                        System.Type type1 = typeof(T);
                        string[] textArray2 = textArray1;
                        if (type1 != null)
                        {
                            text1 = type1.ToString();
                        }
                        else
                        {
                            System.Type local2 = type1;
                            text1 = null;
                        }
                        textArray1[5] = text1;
                        string[] local3 = textArray1;
                        local3[6] = " due to conversion error. \n ";
                        string[] textArray3 = local3;
                        if (exception != null)
                        {
                            text2 = exception.ToString();
                        }
                        else
                        {
                            Exception local4 = exception;
                            text2 = null;
                        }
                        local3[7] = text2;
                        Debug.LogWarning(string.Concat(local3));
                    }
                }
            });
        }

        public static void LinkToggle(GameObject gameObject, object source, string toggleName, string fieldName, Callback callback)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            Toggle local1 = GameObjectUtils.FindByNameGetComponentReport<Toggle>(gameObject, toggleName, false);
            local1.onValueChanged.RemoveAllListeners();
            local1.isOn = (bool) field.GetValue(source);
            local1.onValueChanged.AddListener(delegate (bool newValue) {
                field.SetValue(source, newValue);
                if (callback == null)
                {
                    Callback local1 = callback;
                }
                else
                {
                    callback(source);
                }
            });
        }
    }
}

