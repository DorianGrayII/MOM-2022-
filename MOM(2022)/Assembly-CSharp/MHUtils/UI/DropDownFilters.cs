// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MHUtils.UI.DropDownFilters
using System;
using System.Collections;
using System.Collections.Generic;
using DBUtils;
using MHUtils;
using MHUtils.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownFilters : MonoBehaviour
{
    public enum FilterStringCasing
    {
        UniversalCase = 0,
        UseUpperCase = 1,
        UseLowerCase = 2,
        UseTypedCasing = 3
    }

    public static DropDownFilters singleOpenedDropDown;

    public TMP_InputField inputField;

    public GameObject dropDownTemplate;

    public GameObject itemTemplate;

    public Button previousSelectionItem;

    public Scrollbar scrollbar;

    public FilterStringCasing caseMode;

    public int dropdownItemCount = 1;

    public global::MHUtils.Callback onChange;

    public global::MHUtils.Callback onPreOpen;

    private bool dropDownOpened;

    private List<string> originalOptions = new List<string>();

    private List<string> options = new List<string>();

    private List<string> filteredOutOptions = new List<string>();

    private string selectedOption = "";

    private GameObject[] itemInstances;

    private int scrollingOffest;

    private float mouseWheelScroll;

    private bool clearInputField;

    private Coroutine autoCloserByClicks;

    protected void Start()
    {
        GameObjectUtils.CanvasDrawLayer(this.dropDownTemplate, 10000);
        this.itemTemplate.SetActive(value: false);
        this.dropDownTemplate.SetActive(value: false);
        this.previousSelectionItem.gameObject.SetActive(value: false);
        this.inputField.onValueChanged.AddListener(delegate
        {
            this.UpdateComponent(initialization: false);
        });
        this.inputField.onSelect.AddListener(OnSelect);
        this.scrollbar.value = 0f;
        this.scrollbar.onValueChanged.AddListener(delegate(float x)
        {
            this.ScrollFromScrollbar(x);
        });
        this.UpdateComponent();
        Button component = base.GetComponent<Button>();
        this.inputField.interactable = false;
        if (!(component != null))
        {
            return;
        }
        component.onClick.AddListener(delegate
        {
            if (!this.dropDownOpened)
            {
                this.inputField.interactable = true;
                this.inputField.Select();
            }
        });
    }

    public void SetOptions(List<string> options, bool doUpdate = true, bool localize = true)
    {
        if (localize)
        {
            this.originalOptions = options;
            this.options = new List<string>(options);
            List<string> list = new List<string>(options.Count);
            foreach (string option in options)
            {
                list.Add(this.Localize(option));
            }
            this.options = list;
        }
        else
        {
            this.originalOptions = new List<string>();
            this.options = options;
        }
        if (doUpdate)
        {
            this.UpdateComponent();
        }
    }

    public void SetOptions(Type e, bool doUpdate = true, bool cleanOptionList = true, bool localizeNames = false, bool acceptDefaultOption = true)
    {
        if (!e.IsEnum)
        {
            Debug.LogError("[ERROR]Drop down is used for type which is not enum!");
        }
        string[] names = Enum.GetNames(e);
        if (cleanOptionList)
        {
            this.options = new List<string>(names);
        }
        else
        {
            this.options.AddRange(names);
        }
        if (!acceptDefaultOption && names.Length != 0 && this.options.Contains(names[0]))
        {
            this.options.Remove(names[0]);
        }
        if (localizeNames)
        {
            this.originalOptions = this.options;
            List<string> list = new List<string>(this.options.Count);
            foreach (string option in this.options)
            {
                list.Add(this.Localize(option));
            }
            this.options = list;
        }
        else
        {
            this.originalOptions = new List<string>();
        }
        if (!this.options.Contains(this.selectedOption) && this.selectedOption != "")
        {
            this.SelectOption("", fallbackToFirst: true);
        }
        this.UpdateComponent();
    }

    private string Localize(string name)
    {
        if (!name.StartsWith("UI_") && !char.IsNumber(name[0]))
        {
            name = "UI_OPT_" + name.ToUpperInvariant().Replace(' ', '_');
        }
        return Localization.SimpleGet(name);
    }

    protected void Update()
    {
        if (!this.dropDownOpened)
        {
            return;
        }
        DropDownFilters.singleOpenedDropDown = this;
        float axis = Input.GetAxis("Mouse ScrollWheel");
        this.mouseWheelScroll += axis * 7f;
        if (this.mouseWheelScroll > 1f)
        {
            this.mouseWheelScroll -= 1f;
            if (this.scrollingOffest > 0)
            {
                this.scrollingOffest--;
                this.UpdateComponent(initialization: false);
            }
        }
        if (this.mouseWheelScroll < -1f)
        {
            this.mouseWheelScroll += 1f;
            if (this.scrollingOffest < this.filteredOutOptions.Count - this.dropdownItemCount)
            {
                this.scrollingOffest++;
                this.UpdateComponent(initialization: false);
            }
        }
        if (this.autoCloserByClicks == null)
        {
            this.autoCloserByClicks = base.StartCoroutine(this.ExternalClick());
        }
    }

    private IEnumerator ExternalClick()
    {
        yield return null;
        while (true)
        {
            if (this.dropDownOpened)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    bool cancelClosing = false;
                    int startScrollingOffset = this.scrollingOffest;
                    while (Input.GetMouseButton(0))
                    {
                        if (startScrollingOffset != this.scrollingOffest)
                        {
                            cancelClosing = true;
                        }
                        yield return null;
                    }
                    yield return null;
                    if (!cancelClosing && this.dropDownOpened && !this.inputField.isFocused)
                    {
                        this.SelectOption(this.selectedOption);
                        base.StopCoroutine(this.autoCloserByClicks);
                        this.autoCloserByClicks = null;
                    }
                }
            }
            else
            {
                base.StopCoroutine(this.autoCloserByClicks);
                this.autoCloserByClicks = null;
            }
            yield return null;
        }
    }

    private void InputFieldValueChanged(string s)
    {
        if (this.options == null)
        {
            return;
        }
        string inputValue;
        switch (this.caseMode)
        {
        case FilterStringCasing.UseUpperCase:
            inputValue = s.ToUpperInvariant();
            break;
        case FilterStringCasing.UseLowerCase:
            inputValue = s.ToLowerInvariant();
            break;
        case FilterStringCasing.UniversalCase:
            inputValue = s.ToUpperInvariant();
            break;
        default:
            inputValue = s;
            break;
        }
        if (this.caseMode == FilterStringCasing.UniversalCase)
        {
            this.filteredOutOptions = this.options.FindAll((string o) => o.ToUpperInvariant().Contains(inputValue));
        }
        else
        {
            this.filteredOutOptions = this.options.FindAll((string o) => o.Contains(inputValue));
        }
    }

    public void UpdateComponent(bool initialization = true, bool updateScrollbar = true)
    {
        if (initialization)
        {
            this.inputField.text = this.selectedOption;
        }
        this.dropDownTemplate.SetActive(this.dropDownOpened);
        if (this.dropDownOpened)
        {
            this.InputFieldValueChanged(this.inputField.text);
            if (this.itemInstances == null)
            {
                this.itemInstances = new GameObject[this.dropdownItemCount];
                this.itemInstances[0] = this.itemTemplate;
                for (int i = 1; i < this.dropdownItemCount; i++)
                {
                    this.itemInstances[i] = global::UnityEngine.Object.Instantiate(this.itemTemplate, this.itemTemplate.transform.parent);
                }
            }
            for (int j = 0; j < this.dropdownItemCount; j++)
            {
                Button b = this.itemInstances[j].GetComponent<Button>();
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(delegate
                {
                    this.SelectOption(b.GetComponentInChildren<TextMeshProUGUI>().text);
                });
            }
            this.previousSelectionItem.onClick.RemoveAllListeners();
            this.previousSelectionItem.onClick.AddListener(delegate
            {
                this.SelectOption(this.previousSelectionItem.GetComponentInChildren<TextMeshProUGUI>().text);
            });
            int num = this.filteredOutOptions.Count - this.dropdownItemCount;
            if (num < this.scrollingOffest)
            {
                this.scrollingOffest = Mathf.Max(0, num);
            }
            for (int k = 0; k < this.dropdownItemCount; k++)
            {
                int num2 = k + this.scrollingOffest;
                if (this.filteredOutOptions.Count <= num2)
                {
                    this.itemInstances[k].SetActive(value: false);
                    continue;
                }
                this.itemInstances[k].SetActive(value: true);
                this.itemInstances[k].GetComponentInChildren<TextMeshProUGUI>().text = this.filteredOutOptions[num2];
            }
            if (!string.IsNullOrEmpty(this.selectedOption) && this.options.Contains(this.selectedOption))
            {
                this.previousSelectionItem.gameObject.SetActive(value: true);
                this.previousSelectionItem.GetComponentInChildren<TextMeshProUGUI>().text = this.selectedOption;
            }
        }
        if (updateScrollbar)
        {
            if (this.filteredOutOptions.Count <= this.dropdownItemCount)
            {
                this.scrollbar.gameObject.SetActive(value: false);
                return;
            }
            this.scrollbar.gameObject.SetActive(value: true);
            float size = (float)this.dropdownItemCount / (float)this.filteredOutOptions.Count;
            float num3 = (float)this.scrollingOffest / (float)(this.filteredOutOptions.Count - this.dropdownItemCount);
            this.scrollbar.size = size;
            this.scrollbar.value = num3;
            this.scrollbar.numberOfSteps = this.filteredOutOptions.Count - this.dropdownItemCount + 1;
            this.scrollbar.onValueChanged.Invoke(num3);
        }
    }

    public void ScrollFromScrollbar(float v)
    {
        int num = Mathf.RoundToInt((float)(this.filteredOutOptions.Count - this.dropdownItemCount) * v);
        if (num != this.scrollingOffest)
        {
            this.scrollingOffest = num;
            this.UpdateComponent(initialization: false, updateScrollbar: false);
        }
    }

    public void SelectOption(string option, bool fallbackToFirst = false, bool alreadyLocalized = true)
    {
        if (!string.IsNullOrEmpty(option) && (this.options.Contains(option) || this.originalOptions.Contains(option)))
        {
            if (!alreadyLocalized && this.originalOptions.Count > 0)
            {
                option = this.Localize(option);
            }
            this.selectedOption = option;
            if (this.onChange != null)
            {
                this.onChange(this.GetSelection());
            }
        }
        else if (fallbackToFirst && this.options != null && this.options.Count > 0)
        {
            this.selectedOption = this.options[0];
            if (this.onChange != null)
            {
                this.onChange(this.GetSelection());
            }
        }
        this.SetDropDownOpenened(value: false);
        this.UpdateComponent();
    }

    public void SelectOption(int option, bool fallbackToFirst = false, bool alreadyLocalized = true)
    {
        if (option >= 0 && this.options.Count > option)
        {
            this.SelectOption(this.options[option], fallbackToFirst, alreadyLocalized);
        }
        else
        {
            Debug.LogWarning("Trying to select option outside allowed index!");
        }
    }

    public void SelectOption(Enum option, bool fallbackToFirst = false)
    {
        if (this.options != null && this.options.Count > 0)
        {
            string a = option.ToString();
            if (this.originalOptions.Count > 0)
            {
                a = this.Localize(a);
            }
            int num = this.options.FindIndex((string o) => o == a);
            if (num > -1)
            {
                this.SelectOption(this.options[num], fallbackToFirst);
            }
            else
            {
                this.SelectOption(0, fallbackToFirst);
            }
        }
    }

    public void OnSelect(string s)
    {
        this.SetDropDownOpenened(value: true);
        this.UpdateComponent();
        base.StartCoroutine(this.ClearInputField());
    }

    private void SetDropDownOpenened(bool value)
    {
        if (this.dropDownOpened != value)
        {
            this.dropDownOpened = value;
            this.inputField.interactable = value;
            if (this.dropDownOpened && this.onPreOpen != null)
            {
                this.onPreOpen(this);
            }
            if (!this.dropDownOpened && DropDownFilters.singleOpenedDropDown == this)
            {
                DropDownFilters.singleOpenedDropDown = null;
            }
        }
    }

    private IEnumerator ClearInputField()
    {
        yield return null;
        Debug.Log("ClearInputField");
        this.inputField.text = "";
    }

    public string GetSelection()
    {
        if (this.originalOptions == null || this.originalOptions.Count < 1)
        {
            return this.selectedOption;
        }
        return this.originalOptions.Find((string o) => this.Localize(o) == this.selectedOption);
    }

    public int GetSelectionNR()
    {
        return this.options.IndexOf(this.selectedOption);
    }
}
