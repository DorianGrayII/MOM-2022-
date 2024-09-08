using System.Collections.Generic;
using WorldCode;

namespace MOM.Adventures
{
    public class AdventureData
    {
        public Dictionary<string, AdvList> publicLits;

        public Dictionary<string, AdvList> temporaryLists;

        public int mainPlayerWizard;

        public IGroup mainPlayerGroup;

        public Plane adventurePlane;

        public List<int> visitedNodes = new List<int>();

        public List<int> avaliableOutputs = new List<int>();

        public List<KeyValuePair<Unit, IGroup>> heroes = new List<KeyValuePair<Unit, IGroup>>();

        public string imageName;

        public List<AdventureChange> adventureChanges;

        public IGroup advSource;

        public AdvList GetListByName(string name, Dictionary<string, AdvList> tempList2)
        {
            if (tempList2 != null && tempList2.ContainsKey(name))
            {
                return tempList2[name];
            }
            if (this.temporaryLists != null && this.temporaryLists.ContainsKey(name))
            {
                return this.temporaryLists[name];
            }
            if (this.publicLits != null && this.publicLits.ContainsKey(name))
            {
                return this.publicLits[name];
            }
            return null;
        }
    }
}
