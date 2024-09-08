namespace MOM
{
    using MOM.Adventures;
    using ProtoBuf;
    using System;
    using System.Collections.Generic;

    [ProtoContract]
    public class SaveAdventureRecord
    {
        [ProtoMember(1)]
        public AdventureTrigger advRef;
        [ProtoMember(2)]
        public int turn;
        [ProtoMember(3)]
        public List<int> visitedNodes;

        public SaveAdventureRecord()
        {
        }

        public SaveAdventureRecord(Adventure a, int turn)
        {
            this.advRef = new AdventureTrigger();
            this.advRef.Set(a);
            this.turn = turn;
        }

        public void AddVisitedNodes(List<int> visitedNodes)
        {
            this.visitedNodes = visitedNodes;
        }
    }
}

