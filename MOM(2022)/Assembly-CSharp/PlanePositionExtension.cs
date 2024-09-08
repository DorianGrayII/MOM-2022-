using System.Collections.Generic;
using DBEnum;
using MHUtils;
using MOM;
using WorldCode;

public static class PlanePositionExtension
{
    public const int MaxDistance = 10000;

    public static List<Vector3i> GetSurroundingArea(this IPlanePosition obj, int range = 1)
    {
        List<Vector3i> list = HexNeighbors.GetRange(obj.GetPosition(), range);
        if (obj.GetPlane().area.horizontalWrap)
        {
            list = obj.GetPlane().GetPositionWrapping(list);
        }
        return list.FindAll((Vector3i o) => obj.GetPlane().area.IsInside(o));
    }

    public static bool IsSwitchPlaneDestinationValid(this IPlanePosition obj, List<Unit> selectedUnits = null)
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
                PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_UNAVAILABLE", "UI_OKAY");
            }
            return false;
        }
        Group group2 = GameManager.GetGroupsOfPlane(otherPlane).Find((Group o) => o.GetPosition() == group.GetPosition());
        if (group2 != null && group2.GetOwnerID() != group.GetOwnerID())
        {
            if (!(wizard is PlayerWizardAI))
            {
                PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_OCCUPIED", "UI_OKAY");
            }
            return false;
        }
        if (selectedUnits != null && selectedUnits.Count < group.GetUnits().Count)
        {
            if (!hexAt.IsLand())
            {
                foreach (Unit selectedUnit in selectedUnits)
                {
                    if (selectedUnit.GetAttributes().Contains(TAG.TRANSPORTER) && (selectedUnit.GetAttributes().GetFinal(TAG.CAN_SWIM) > 0 || selectedUnit.GetAttributes().GetFinal(TAG.CAN_FLY) > 0))
                    {
                        return true;
                    }
                }
                foreach (Unit selectedUnit2 in selectedUnits)
                {
                    if (selectedUnit2.GetAttributes().GetFinal(TAG.CAN_SWIM) <= 0 && selectedUnit2.GetAttributes().GetFinal(TAG.CAN_FLY) <= 0)
                    {
                        if (!(wizard is PlayerWizardAI))
                        {
                            PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY");
                        }
                        return false;
                    }
                }
            }
            else if (hexAt.IsLand())
            {
                foreach (Unit selectedUnit3 in selectedUnits)
                {
                    if (selectedUnit3.GetAttributes().Contains(TAG.TRANSPORTER) && (selectedUnit3.GetAttributes().GetFinal(TAG.CAN_WALK) > 0 || selectedUnit3.GetAttributes().GetFinal(TAG.CAN_FLY) > 0))
                    {
                        return true;
                    }
                }
                foreach (Unit selectedUnit4 in selectedUnits)
                {
                    if (selectedUnit4.GetAttributes().GetFinal(TAG.CAN_WALK) <= 0 && selectedUnit4.GetAttributes().GetFinal(TAG.CAN_FLY) <= 0)
                    {
                        if (!(wizard is PlayerWizardAI))
                        {
                            PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY");
                        }
                        return false;
                    }
                }
            }
        }
        else
        {
            if (group.locationHost != null)
            {
                group.ForceUpdateMovementFlags();
            }
            if (!hexAt.IsLand() && !group.waterMovement)
            {
                if (!(wizard is PlayerWizardAI))
                {
                    PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY");
                }
                return false;
            }
            if (hexAt.IsLand() && !group.landMovement)
            {
                if (!(wizard is PlayerWizardAI))
                {
                    PopupGeneral.OpenPopup(HUD.Get(), "UI_SHIFT_FAILED", "UI_DESTINATION_INVALID", "UI_OKAY");
                }
                return false;
            }
        }
        return true;
    }

    public static bool IsHumanPlayerFocusedOnPlane(this IPlanePosition obj)
    {
        if (World.GetActivePlane() != obj.GetPlane())
        {
            return false;
        }
        return true;
    }

    public static int GetDistanceTo(this IPlanePosition obj, Vector3i pos)
    {
        return obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos);
    }

    public static int GetDistanceTo(this IPlanePosition obj, Vector3i pos, Plane plane)
    {
        if (obj is Location location && location.otherPlaneLocation != null)
        {
            return obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos);
        }
        if (obj.GetPlane() == plane)
        {
            return obj.GetPlane().GetDistanceWrapping(obj.GetPosition(), pos);
        }
        return 10000;
    }

    public static bool IsDistanceTo_Zero(this IPlanePosition obj, Vector3i pos, Plane plane)
    {
        if (obj is Location location && location.otherPlaneLocation != null)
        {
            return obj.GetPosition() == pos;
        }
        if (obj.GetPlane() == plane)
        {
            return obj.GetPosition() == pos;
        }
        return false;
    }

    public static int GetDistanceTo(this IPlanePosition obj, IPlanePosition ipp)
    {
        if (obj == null || ipp == null)
        {
            return 10000;
        }
        if (obj.GetPlane() != ipp.GetPlane())
        {
            return 10000;
        }
        return obj.GetDistanceTo(ipp.GetPosition());
    }

    public static int GetMultiPlanarDistanceTo(this IPlanePosition obj, IPlanePosition ipp, int seekerWizard)
    {
        if (obj == null || ipp == null)
        {
            return 10000;
        }
        if (obj.GetPlane() == ipp.GetPlane())
        {
            return obj.GetDistanceTo(ipp);
        }
        List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(arcanus: true);
        int num = 10000;
        foreach (Location item in locationsOfThePlane)
        {
            if (item.otherPlaneLocation != null && (item.GetOwnerID() == seekerWizard || item.GetUnits().Count == 0))
            {
                int num2 = obj.GetDistanceTo(item.GetPosition()) + ipp.GetDistanceTo(item.GetPosition());
                if (num2 < num)
                {
                    num = num2;
                }
            }
        }
        return num;
    }
}
