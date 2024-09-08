namespace MOM
{
    using MOM.Adventures;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract]
    public class SaveBlock
    {
        [ProtoMember(1)]
        public EntityManager entityManager;
        [ProtoMember(2)]
        public int worldSeed;
        [ProtoMember(3)]
        public DifficultySettingsData settings;
        [ProtoMember(4)]
        public int turnNumber;
        [ProtoMember(5)]
        public float[] arcanusData;
        [ProtoMember(6)]
        public float[] myrrorData;
        [ProtoMember(7)]
        public List<SaveAdventureRecord> adventureOccuranceRecord;
        [ProtoMember(8)]
        public int worldSizeSetting;

        public void CollectAdventureData()
        {
            this.adventureOccuranceRecord = new List<SaveAdventureRecord>();
            if (AdventureManager.lastOccurence != null)
            {
                foreach (KeyValuePair<Adventure, int> pair in AdventureManager.lastOccurence)
                {
                    SaveAdventureRecord item = new SaveAdventureRecord(pair.Key, pair.Value);
                    if ((AdventureManager.visitedNodes != null) && AdventureManager.visitedNodes.ContainsKey(pair.Key))
                    {
                        item.AddVisitedNodes(AdventureManager.visitedNodes[pair.Key]);
                    }
                    this.adventureOccuranceRecord.Add(item);
                }
            }
        }

        public void RestoreAdventureData()
        {
            AdventureManager.lastOccurence = new Dictionary<Adventure, int>();
            AdventureManager.visitedNodes = new Dictionary<Adventure, List<int>>();
            if (this.adventureOccuranceRecord != null)
            {
                foreach (SaveAdventureRecord record in this.adventureOccuranceRecord)
                {
                    Adventure adventure = record.advRef.Get();
                    if (adventure != null)
                    {
                        AdventureManager.lastOccurence[adventure] = record.turn;
                        if (record.visitedNodes != null)
                        {
                            AdventureManager.visitedNodes[adventure] = record.visitedNodes;
                        }
                    }
                }
            }
        }
    }
}

