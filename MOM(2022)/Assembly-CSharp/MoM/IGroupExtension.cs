using System.Collections.Generic;
using MHUtils;

namespace MOM
{
    public static class IGroupExtension
    {
        public static bool AddUnitsIfPossible(this IGroup obj, List<Reference<Unit>> units)
        {
            List<Reference<Unit>> units2 = obj.GetUnits();
            if (units == null || units2.Count + units.Count > 9)
            {
                return false;
            }
            for (int num = units.Count - 1; num >= 0; num--)
            {
                Reference<Unit> reference = units[num];
                reference.Get().group.Get().RemoveUnit(reference);
                obj.AddUnit(reference);
            }
            return true;
        }

        public static bool ExchangeOptimizedForSelf(this IGroup obj, IGroup otherGroup)
        {
            if (otherGroup == null)
            {
                return false;
            }
            int count = obj.GetUnits().Count;
            int count2 = otherGroup.GetUnits().Count;
            if (count + count2 - 9 <= 0)
            {
                obj.AddUnitsIfPossible(otherGroup.GetUnits());
            }
            else
            {
                List<Multitype<Unit, int>> list = new List<Multitype<Unit, int>>();
                foreach (Reference<Unit> unit3 in obj.GetUnits())
                {
                    Unit unit = unit3.Get();
                    list.Add(new Multitype<Unit, int>(unit, unit.GetWorldUnitValue()));
                }
                foreach (Reference<Unit> unit4 in otherGroup.GetUnits())
                {
                    Unit unit2 = unit4.Get();
                    list.Add(new Multitype<Unit, int>(unit2, unit2.GetWorldUnitValue()));
                }
                list.Sort(delegate(Multitype<Unit, int> a, Multitype<Unit, int> b)
                {
                    int num = -a.t1.CompareTo(b.t1);
                    if (num == 0)
                    {
                        num = a.t0.ID.CompareTo(b.t0.ID);
                    }
                    return num;
                });
                for (int i = 0; i < list.Count; i++)
                {
                    if (i < 9)
                    {
                        otherGroup.RemoveUnit(list[i].t0, allowGroupDestruction: false, updateGroup: false);
                    }
                    else
                    {
                        obj.RemoveUnit(list[i].t0, allowGroupDestruction: false, updateGroup: false);
                    }
                }
                for (int j = 0; j < list.Count; j++)
                {
                    if (j < 9)
                    {
                        obj.AddUnit(list[j].t0, updateMovementFlags: false);
                    }
                    else
                    {
                        otherGroup.AddUnit(list[j].t0, updateMovementFlags: false);
                    }
                }
                if (obj.GetGroup().GetUnits().Count == 0 && !obj.GetGroup().IsHosted())
                {
                    obj.GetGroup().Destroy();
                }
                else
                {
                    obj.GetGroup().Dirty = true;
                    obj.GetGroup().UpdateGroupMarkersAfterSkip();
                }
                if (otherGroup.GetGroup().GetUnits().Count == 0 && !otherGroup.GetGroup().IsHosted())
                {
                    otherGroup.GetGroup().Destroy();
                }
                else
                {
                    otherGroup.GetGroup().Dirty = true;
                    otherGroup.GetGroup().UpdateGroupMarkersAfterSkip();
                }
            }
            return true;
        }
    }
}
