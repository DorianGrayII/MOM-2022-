namespace MOM.Adventures
{
    using MOM;
    using System;
    using System.Collections.Generic;
    using WorldCode;

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
            return (((tempList2 == null) || !tempList2.ContainsKey(name)) ? (((this.temporaryLists == null) || !this.temporaryLists.ContainsKey(name)) ? (((this.publicLits == null) || !this.publicLits.ContainsKey(name)) ? null : this.publicLits[name]) : this.temporaryLists[name]) : tempList2[name]);
        }
    }
}

