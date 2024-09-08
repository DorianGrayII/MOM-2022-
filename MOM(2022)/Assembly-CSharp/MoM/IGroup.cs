// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.IGroup
using System.Collections.Generic;
using MOM;

public interface IGroup
{
    int GetOwnerID();

    void SetOwnerID(int id, int attackerID = -1, bool additionalLosses = false);

    List<Reference<Unit>> GetUnits();

    AdventureTrigger GetAdventureTrigger();

    void AddUnit(Unit u, bool updateMovementFlags = true);

    void RemoveUnit(Unit u, bool allowGroupDestruction = true, bool updateGroup = true);

    Group GetGroup();
}
