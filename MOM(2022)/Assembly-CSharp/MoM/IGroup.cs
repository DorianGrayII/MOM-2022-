using System.Collections.Generic;

namespace MOM
{
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
}
