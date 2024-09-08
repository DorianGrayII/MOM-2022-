using System.Collections.Generic;
using MOM.Adventures;
using ProtoBuf;

namespace MOM
{
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
            if (AdventureManager.lastOccurence == null)
            {
                return;
            }
            foreach (KeyValuePair<Adventure, int> item in AdventureManager.lastOccurence)
            {
                SaveAdventureRecord saveAdventureRecord = new SaveAdventureRecord(item.Key, item.Value);
                if (AdventureManager.visitedNodes != null && AdventureManager.visitedNodes.ContainsKey(item.Key))
                {
                    saveAdventureRecord.AddVisitedNodes(AdventureManager.visitedNodes[item.Key]);
                }
                this.adventureOccuranceRecord.Add(saveAdventureRecord);
            }
        }

        public void RestoreAdventureData()
        {
            AdventureManager.lastOccurence = new Dictionary<Adventure, int>();
            AdventureManager.visitedNodes = new Dictionary<Adventure, List<int>>();
            if (this.adventureOccuranceRecord == null)
            {
                return;
            }
            foreach (SaveAdventureRecord item in this.adventureOccuranceRecord)
            {
                Adventure adventure = item.advRef.Get();
                if (adventure != null)
                {
                    AdventureManager.lastOccurence[adventure] = item.turn;
                    if (item.visitedNodes != null)
                    {
                        AdventureManager.visitedNodes[adventure] = item.visitedNodes;
                    }
                }
            }
        }
    }
}
