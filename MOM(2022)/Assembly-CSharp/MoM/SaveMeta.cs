using System;
using System.Xml.Serialization;
using WorldCode;

namespace MOM
{
    public class SaveMeta
    {
        [XmlAttribute]
        public string wizardName;

        [XmlAttribute]
        public string saveName;

        [XmlAttribute]
        public int turn;

        [XmlAttribute]
        public int worldSeed;

        [XmlAttribute]
        public long saveDate;

        [XmlAttribute]
        public string gameVersion;

        [XmlAttribute]
        public int gameID;

        [XmlAttribute]
        public int worldSizeSetting;

        [XmlAttribute]
        public int dlc;

        public SaveMeta()
        {
        }

        public SaveMeta(string saveName)
        {
            this.CollectData();
            this.saveName = saveName;
        }

        public void CollectData()
        {
            this.wizardName = GameManager.GetWizard(PlayerWizard.HumanID()).name;
            this.turn = TurnManager.GetTurnNumber();
            this.worldSeed = World.Get().seed;
            this.worldSizeSetting = World.Get().worldSizeSetting;
            this.gameID = World.Get().gameID;
            this.saveDate = DateTime.Now.Ticks;
            this.gameVersion = GameVersion.GetGameVersionFull();
            this.dlc = GameManager.Get().dlcSettings;
        }

        public string GetTimeStamp()
        {
            return new DateTime(this.saveDate).ToString();
        }
    }
}
