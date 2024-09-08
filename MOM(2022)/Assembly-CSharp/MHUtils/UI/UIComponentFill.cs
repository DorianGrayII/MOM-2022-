using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MHUtils.UI
{
    public class UIComponentFill : MonoBehaviour
    {
        public static void LinkInputField<T>(GameObject gameObject, object source, string tfName, string fieldName, Callback callback = null, Callback finalCallback = null)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            TMP_InputField tf = GameObjectUtils.FindByNameGetComponent<TMP_InputField>(gameObject, tfName);
            tf.onValueChanged.RemoveAllListeners();
            tf.onEndEdit.RemoveAllListeners();
            object value = field.GetValue(source);
            string text = ((value == null) ? "" : value.ToString());
            if (string.IsNullOrEmpty(text))
            {
                tf.text = "";
            }
            else
            {
                tf.text = text;
            }
            tf.onValueChanged.AddListener(delegate(string newValue)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }
                try
                {
                    object value2 = Convert.ChangeType(newValue, typeof(T));
                    field.SetValue(source, value2);
                    callback?.Invoke(source);
                }
                catch (Exception ex2)
                {
                    Debug.LogWarning("Value " + newValue + " cannot be implanted into field " + fieldName + " of type " + typeof(T)?.ToString() + " due to conversion error. \n " + ex2);
                }
            });
            tf.onEndEdit.AddListener(delegate(string newValue)
            {
                if (string.IsNullOrEmpty(newValue) && tf.contentType == TMP_InputField.ContentType.IntegerNumber)
                {
                    field.SetValue(source, 0);
                    tf.text = "0";
                }
                if (finalCallback != null)
                {
                    try
                    {
                        finalCallback?.Invoke(source);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning("Value " + newValue + " cannot be implanted into field " + fieldName + " of type " + typeof(T)?.ToString() + " due to conversion error. \n " + ex);
                    }
                }
            });
        }

        public static void LinkToggle(GameObject gameObject, object source, string toggleName, string fieldName, Callback callback = null)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            Toggle toggle = GameObjectUtils.FindByNameGetComponentReport<Toggle>(gameObject, toggleName);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.isOn = (bool)field.GetValue(source);
            toggle.onValueChanged.AddListener(delegate(bool newValue)
            {
                field.SetValue(source, newValue);
                callback?.Invoke(source);
            });
        }

        public static void LinkDropdownEnum<T>(GameObject gameObject, object source, string dropdownName, string fieldName, Callback callback = null) where T : struct
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            string option = Convert.ToString(field.GetValue(source));
            DropDownFilters dropDownFilters = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName);
            dropDownFilters.onChange = null;
            dropDownFilters.SetOptions(typeof(T), doUpdate: false);
            dropDownFilters.SelectOption(option, fallbackToFirst: true, alreadyLocalized: false);
            dropDownFilters.onChange = delegate(object obj)
            {
                string text = obj as string;
                try
                {
                    if (text == null)
                    {
                        field.SetValue(source, default(T));
                    }
                    else
                    {
                        T val = (T)Enum.Parse(typeof(T), text);
                        field.SetValue(source, val);
                    }
                    callback?.Invoke(source);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[ERROR]" + ex);
                }
            };
        }

        public static void LinkDropdownEnumStringField(Type type, GameObject gameObject, object source, string dropdownName, string fieldName, Callback callback = null)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            string option = field.GetValue(source) as string;
            DropDownFilters dropDownFilters = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName);
            dropDownFilters.onChange = null;
            dropDownFilters.SetOptions(type, doUpdate: false);
            dropDownFilters.SelectOption(option, fallbackToFirst: false, alreadyLocalized: false);
            dropDownFilters.onChange = delegate(object obj)
            {
                string text = obj as string;
                try
                {
                    if (text == null)
                    {
                        field.SetValue(source, null);
                    }
                    else
                    {
                        field.SetValue(source, text);
                    }
                    callback?.Invoke(source);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[ERROR]" + ex);
                }
            };
        }

        public static void LinkDropdownEnum<T>(DropDownFilters filter, Callback callback = null, bool acceptDefaultOption = true, string selectedOption = null, bool localize = false) where T : struct
        {
            filter.onChange = null;
            filter.SetOptions(typeof(T), doUpdate: false, cleanOptionList: true, localize, acceptDefaultOption);
            filter.SelectOption(selectedOption, fallbackToFirst: true, alreadyLocalized: false);
            filter.onChange = callback;
        }

        public static void LinkDropdown(GameObject gameObject, object source, string dropdownName, string fieldName, List<string> options, Callback callback = null, bool localize = false)
        {
            FieldInfo field = source.GetType().GetField(fieldName);
            PropertyInfo property = null;
            if (field == null)
            {
                property = source.GetType().GetProperty(fieldName);
            }
            if (field == null && property == null)
            {
                return;
            }
            object obj2 = ((field != null) ? field.GetValue(source) : null);
            obj2 = ((property != null) ? property.GetValue(source) : obj2);
            string option = Convert.ToString(obj2);
            DropDownFilters dropDownFilters = GameObjectUtils.FindByNameGetComponent<DropDownFilters>(gameObject, dropdownName);
            dropDownFilters.onChange = null;
            dropDownFilters.SetOptions(options, doUpdate: true, localize);
            dropDownFilters.SelectOption(option, fallbackToFirst: false, alreadyLocalized: false);
            dropDownFilters.onChange = delegate(object obj)
            {
                string value = obj as string;
                try
                {
                    if (field != null)
                    {
                        field.SetValue(source, value);
                    }
                    else
                    {
                        property.SetValue(source, value);
                    }
                    callback?.Invoke(source);
                }
                catch (Exception ex)
                {
                    Debug.LogError("[ERROR]" + ex);
                }
            };
        }

        public static void LinkDropdownCustom(DropDownFilters filter, CallbackRet customFill, object data, CallbackRet selectedItem, Callback callback = null, bool localize = false)
        {
            filter.onChange = null;
            List<string> list = customFill(data) as List<string>;
            filter.SetOptions(list, doUpdate: true, localize);
            object obj2 = selectedItem(list);
            if (obj2 is string)
            {
                filter.SelectOption(obj2 as string, fallbackToFirst: false, alreadyLocalized: false);
            }
            else if (obj2 is int)
            {
                filter.SelectOption((int)obj2);
            }
            else if (obj2 is Enum)
            {
                filter.SelectOption((Enum)obj2);
            }
            filter.onChange = delegate(object obj)
            {
                callback?.Invoke(obj);
            };
        }
    }
}
