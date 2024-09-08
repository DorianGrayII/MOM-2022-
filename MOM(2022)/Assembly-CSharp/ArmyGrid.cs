using System;
using System.Collections.Generic;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

[RequireComponent(typeof(GridItemManager))]
public class ArmyGrid : MonoBehaviour
{
    public Action<ArmyGrid> onSelectionChange;

    public List<BaseUnit> selectedList = new List<BaseUnit>();

    private List<BaseUnit> units = new List<BaseUnit>();

    private GridItemManager gridManager;

    private void OnDestroy()
    {
        MHEventSystem.UnRegisterListenersLinkedToObject(this);
    }

    public bool IsSelected(BaseUnit unit)
    {
        return this.selectedList.Contains(unit);
    }

    public void UpdateToggleState(BaseUnit u, bool selected)
    {
        if (this.IsSelected(u) != selected)
        {
            if (selected)
            {
                this.selectedList.Add(u);
            }
            else
            {
                this.selectedList.Remove(u);
            }
            this.InvokeSelectionChanged();
        }
    }

    public void SetUnits(IEnumerable<object> units)
    {
        this.units.Clear();
        if (units != null)
        {
            foreach (object unit in units)
            {
                BaseUnit bU = this.GetBU(unit);
                if (!bU.IsInvisibleUnit() || bU.GetWizardOwner() == GameManager.GetHumanWizard())
                {
                    this.units.Add(bU);
                }
            }
        }
        bool flag = false;
        for (int num = this.selectedList.Count - 1; num >= 0; num--)
        {
            BaseUnit item = this.selectedList[num];
            if (!this.units.Contains(item))
            {
                this.selectedList.RemoveAt(num);
                flag = true;
            }
        }
        this.gridManager.UpdateGrid(this.units);
        if (flag)
        {
            this.InvokeSelectionChanged();
        }
    }

    private void InvokeSelectionChanged()
    {
        if (this.onSelectionChange != null)
        {
            this.onSelectionChange(this);
        }
    }

    public void Awake()
    {
        MHEventSystem.RegisterListener<Unit>(DoRemoveUnit, this);
        this.gridManager = base.GetComponent<GridItemManager>();
        this.gridManager.CustomDynamicItem(delegate(GameObject itemSource, object source, object data, int index)
        {
            itemSource.GetComponent<ArmyListItem>().Unit = this.GetBU(source);
        }, delegate
        {
            this.gridManager.UpdateGrid(this.units);
        });
    }

    private BaseUnit GetBU(object source)
    {
        Reference<Unit> reference = source as Reference<Unit>;
        if (reference != null)
        {
            return reference.Get();
        }
        Reference<BattleUnit> reference2 = source as Reference<BattleUnit>;
        if (reference2 != null)
        {
            return reference2.Get();
        }
        return source as BaseUnit;
    }

    public List<object> GetObjectsInGrid()
    {
        return new List<object>(this.gridManager.dataToGameObjectDictionary.Keys);
    }

    public void DoRemoveUnit(object unit, object notUsed)
    {
        BaseUnit u = unit as BaseUnit;
        int num = this.units.FindIndex((BaseUnit o) => o == u);
        if (num >= 0)
        {
            if (this.IsSelected(u))
            {
                this.selectedList.Remove(u);
            }
            this.units.RemoveAt(num);
            this.gridManager.UpdateGrid(this.units);
        }
    }

    public List<BaseUnit> GetUnits()
    {
        return this.units;
    }
}
