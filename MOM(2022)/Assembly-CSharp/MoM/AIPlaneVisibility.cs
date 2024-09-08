// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.AIPlaneVisibility
using System.Collections.Generic;
using MHUtils;
using MOM;
using WorldCode;

public class AIPlaneVisibility
{
    private Plane plane;

    private PlayerWizardAI owner;

    public List<Group> ownGroups;

    public List<Group> visibleGroups;

    public List<Group> sensedGroups;

    public List<Location> knownLocations;

    public HashSet<Vector3i> knownLocationPositions;

    public AIPlaneVisibility(Plane p, PlayerWizardAI owner)
    {
        this.plane = p;
        this.owner = owner;
        if (p.arcanusType)
        {
            PlayerWizardAI playerWizardAI = owner;
            if (playerWizardAI.arcanusKnownLocations == null)
            {
                playerWizardAI.arcanusKnownLocations = new HashSet<Vector3i>();
            }
            this.knownLocationPositions = owner.arcanusKnownLocations;
        }
        else
        {
            PlayerWizardAI playerWizardAI = owner;
            if (playerWizardAI.myrrorKnownLocations == null)
            {
                playerWizardAI.myrrorKnownLocations = new HashSet<Vector3i>();
            }
            this.knownLocationPositions = owner.myrrorKnownLocations;
        }
    }

    public void Update()
    {
        V3iRect area = this.plane.area;
        if (this.ownGroups == null)
        {
            this.ownGroups = new List<Group>();
        }
        else
        {
            this.ownGroups.Clear();
        }
        if (this.visibleGroups == null)
        {
            this.visibleGroups = new List<Group>();
        }
        else
        {
            this.visibleGroups.Clear();
        }
        if (this.sensedGroups == null)
        {
            this.sensedGroups = new List<Group>();
        }
        else
        {
            this.sensedGroups.Clear();
        }
        if (this.knownLocations == null)
        {
            this.knownLocations = new List<Location>();
        }
        else
        {
            this.knownLocations.Clear();
        }
        if (this.knownLocationPositions == null)
        {
            this.knownLocationPositions = new HashSet<Vector3i>();
        }
        List<Group> groupsOfPlane = GameManager.GetGroupsOfPlane(this.plane);
        foreach (Group item in groupsOfPlane)
        {
            if (item.GetOwnerID() == this.owner.ID)
            {
                this.ownGroups.Add(item);
            }
        }
        if (this.ownGroups.Count == 0)
        {
            return;
        }
        List<Location> locationsOfThePlane = GameManager.GetLocationsOfThePlane(this.plane);
        foreach (Group item2 in groupsOfPlane)
        {
            if (item2.GetOwnerID() == this.owner.ID)
            {
                continue;
            }
            bool flag = false;
            foreach (Group ownGroup in this.ownGroups)
            {
                int num = 7;
                int distanceTo = ownGroup.GetDistanceTo(item2);
                if (ownGroup.locationHost != null)
                {
                    num = ((ownGroup.GetLocationHostSmart() is TownLocation) ? 10 : 7);
                }
                if (distanceTo > num)
                {
                    continue;
                }
                if (!flag)
                {
                    this.sensedGroups.Add(item2);
                    flag = true;
                }
                if (distanceTo <= 4)
                {
                    this.visibleGroups.Add(item2);
                    if (item2.GetOwnerID() > 0 && item2.GetOwnerID() != this.owner.GetID())
                    {
                        this.owner.EnsureWizardIsKnown(item2.GetOwnerID());
                    }
                    break;
                }
            }
        }
        foreach (Location item3 in locationsOfThePlane)
        {
            if (item3.GetOwnerID() == this.owner.ID)
            {
                continue;
            }
            bool flag2 = false;
            foreach (Group ownGroup2 in this.ownGroups)
            {
                int num2 = 7;
                if (ownGroup2.locationHost != null)
                {
                    num2 = ((ownGroup2.GetLocationHostSmart() is TownLocation) ? 10 : 7);
                }
                if (this.knownLocationPositions.Contains(ownGroup2.GetPosition()))
                {
                    this.knownLocations.Add(item3);
                    flag2 = true;
                    break;
                }
                if (area.HexDistance(item3.GetPosition(), ownGroup2.GetPosition()) <= num2)
                {
                    this.knownLocations.Add(item3);
                    this.knownLocationPositions.Add(item3.GetPosition());
                    flag2 = true;
                    break;
                }
            }
            if (!flag2 || item3.owner <= 0 || (this.owner.GetDiscoveredWizards() != null && this.owner.GetDiscoveredWizards().Contains(item3.GetWizardOwner())))
            {
                continue;
            }
            foreach (Group ownGroup3 in this.ownGroups)
            {
                if (area.HexDistance(item3.GetPosition(), ownGroup3.GetPosition()) <= 5)
                {
                    this.owner.EnsureWizardIsKnown(item3.owner);
                    break;
                }
            }
        }
    }
}
