using DBEnum;
using MHUtils;
using MOM;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WorldCode;

[Extension]
public static class PlanePositionExtension
{
    public const int MaxDistance = 0x2710;

    [Extension]
    public static int GetDistanceTo(IPlanePosition obj, IPlanePosition ipp)
    {
        return (((obj == null) || (ipp == null)) ? 0x2710 : (ReferenceEquals(obj.GetPlane(), ipp.GetPlane()) ? GetDistanceTo(obj, ipp.GetPosition()) : 0x2710));
    }

    [Extension]
    public static int GetDistanceTo(IPlanePosition obj, Vector3i pos)
    {
        return obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos);
    }

    [Extension]
    public static int GetDistanceTo(IPlanePosition obj, Vector3i pos, Plane plane)
    {
        Location location = obj as Location;
        return (((location == null) || (location.otherPlaneLocation == null)) ? (!ReferenceEquals(obj.GetPlane(), plane) ? 0x2710 : obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos)) : obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos));
    }

    [Extension]
    public static int GetMultiPlanarDistanceTo(IPlanePosition obj, IPlanePosition ipp, int seekerWizard)
    {
        if ((obj == null) || (ipp == null))
        {
            return 0x2710;
        }
        if (ReferenceEquals(obj.GetPlane(), ipp.GetPlane()))
        {
            return GetDistanceTo(obj, ipp);
        }
        int num = 0x2710;
        foreach (Location location in GameManager.GetLocationsOfThePlane(true))
        {
            if ((location.otherPlaneLocation != null) && ((location.GetOwnerID() == seekerWizard) || (location.GetUnits().Count == 0)))
            {
                int num2 = GetDistanceTo(obj, location.GetPosition()) + GetDistanceTo(ipp, location.GetPosition());
                if (num2 < num)
                {
                    num = num2;
                }
            }
        }
        return num;
    }

    [Extension]
    public static List<Vector3i> GetSurroundingArea(IPlanePosition obj, int range)
    {
        List<Vector3i> pos = HexNeighbors.GetRange(obj.GetPosition(), range);
        if (obj.GetPlane().area.horizontalWrap)
        {
            pos = obj.GetPlane().GetPositionWrapping(pos);
        }
        return pos.FindAll(o => obj.GetPlane().area.IsInside(o, false));
    }

    [Extension]
    public static bool IsDistanceTo_Zero(IPlanePosition obj, Vector3i pos, Plane plane)
    {
        Location location = obj as Location;
        return (((location == null) || (location.otherPlaneLocation == null)) ? (ReferenceEquals(obj.GetPlane(), plane) && (obj.GetPosition() == pos)) : (obj.GetPosition() == pos));
    }

    [Extension]
    public static bool IsHumanPlayerFocusedOnPlane(IPlanePosition obj)
    {
        return ReferenceEquals(World.GetActivePlane(), obj.GetPlane());
    }

    [Extension]
    public static bool IsSwitchPlaneDestinationValid(IPlanePosition obj, List<Unit> selectedUnits)
    {
        if (!(obj is Group))
        {
            return false;
        }
        Group group = obj as Group;
        Plane otherPlane = World.GetOtherPlane(group.GetPlane());
        Hex hexAt = otherPlane.GetHexAt(group.GetPosition());
        PlayerWizard wizard = GameManager.GetWizard(group.GetOwnerID());
        if (!GameManager.Get().allowPlaneSwitch)
        {
            if (!(wizard is PlayerWizardAI))
            {
                PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_UNAVAILABLE", "UI_OKAY", null, null, null, null, null, null);
            }
            return false;
        }
        Group group = GameManager.GetGroupsOfPlane(otherPlane).Find(o => o.GetPosition() == group.GetPosition());
        if ((group != null) && (group.GetOwnerID() != group.GetOwnerID()))
        {
            if (!(wizard is PlayerWizardAI))
            {
                PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_OCCUPIED", "UI_OKAY", null, null, null, null, null, null);
            }
            return false;
        }
        if ((selectedUnits == null) || (selectedUnits.Count >= group.GetUnits().Count))
        {
            if (group.locationHost != null)
            {
                group.ForceUpdateMovementFlags();
            }
            if (!hexAt.IsLand() && !group.waterMovement)
            {
                if (!(wizard is PlayerWizardAI))
                {
                    PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY", null, null, null, null, null, null);
                }
                return false;
            }
            if (hexAt.IsLand() && !group.landMovement)
            {
                if (!(wizard is PlayerWizardAI))
                {
                    PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY", null, null, null, null, null, null);
                }
                return false;
            }
        }
        else
        {
            List<Unit>.Enumerator enumerator;
            bool flag;
            if (hexAt.IsLand())
            {
                if (!hexAt.IsLand())
                {
                    goto TR_0009;
                }
                else
                {
                    using (enumerator = selectedUnits.GetEnumerator())
                    {
                        while (true)
                        {
                            if (!enumerator.MoveNext())
                            {
                                break;
                            }
                            Unit current = enumerator.Current;
                            if (current.GetAttributes().Contains(TAG.TRANSPORTER) && ((current.GetAttributes().GetFinal(TAG.CAN_WALK) > 0) || (current.GetAttributes().GetFinal(TAG.CAN_FLY) > 0)))
                            {
                                return true;
                            }
                        }
                    }
                    using (enumerator = selectedUnits.GetEnumerator())
                    {
                        while (true)
                        {
                            if (enumerator.MoveNext())
                            {
                                Unit current = enumerator.Current;
                                if ((current.GetAttributes().GetFinal(TAG.CAN_WALK) > 0) || (current.GetAttributes().GetFinal(TAG.CAN_FLY) > 0))
                                {
                                    continue;
                                }
                                if (!(wizard is PlayerWizardAI))
                                {
                                    PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY", null, null, null, null, null, null);
                                }
                                flag = false;
                            }
                            else
                            {
                                goto TR_0009;
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                using (enumerator = selectedUnits.GetEnumerator())
                {
                    while (true)
                    {
                        if (!enumerator.MoveNext())
                        {
                            break;
                        }
                        Unit current = enumerator.Current;
                        if (current.GetAttributes().Contains(TAG.TRANSPORTER) && ((current.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0) || (current.GetAttributes().GetFinal(TAG.CAN_FLY) > 0)))
                        {
                            return true;
                        }
                    }
                }
                using (enumerator = selectedUnits.GetEnumerator())
                {
                    while (true)
                    {
                        if (enumerator.MoveNext())
                        {
                            Unit current = enumerator.Current;
                            if ((current.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0) || (current.GetAttributes().GetFinal(TAG.CAN_FLY) > 0))
                            {
                                continue;
                            }
                            if (!(wizard is PlayerWizardAI))
                            {
                                PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY", null, null, null, null, null, null);
                            }
                            flag = false;
                        }
                        else
                        {
                            goto TR_0009;
                        }
                        break;
                    }
                }
            }
            return flag;
        }
    TR_0009:
        return true;
    }
}

