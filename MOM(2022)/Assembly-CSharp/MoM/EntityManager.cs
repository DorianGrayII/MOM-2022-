using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class EntityManager
    {
        private static EntityManager instance;

        [ProtoIgnore]
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        [ProtoMember(1)]
        public int nextEntityID;

        [ProtoMember(2)]
        private List<Entity> _entities;

        private EntityManager()
        {
            EntityManager.instance = this;
        }

        [ProtoBeforeSerialization]
        public void BeforeSerialization()
        {
            this._entities = new List<Entity>(this.entities.Values);
        }

        [ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            this.entities = new Dictionary<int, Entity>();
            if (this._entities == null)
            {
                return;
            }
            foreach (Entity entity in this._entities)
            {
                this.entities[entity.GetID()] = entity;
            }
            this._entities = null;
        }

        public static void Reset()
        {
            GameManager.instance = null;
            EntityManager.instance = null;
        }

        public static EntityManager Get()
        {
            if (SaveManager.loadingMode)
            {
                return null;
            }
            if (EntityManager.instance == null)
            {
                new EntityManager();
            }
            return EntityManager.instance;
        }

        public static int GetNextID()
        {
            if (EntityManager.Get().nextEntityID == 0)
            {
                EntityManager.Get().nextEntityID = 100;
            }
            EntityManager.Get().nextEntityID++;
            _ = EntityManager.Get().nextEntityID;
            _ = 11552;
            return EntityManager.Get().nextEntityID;
        }

        public static void RegisterEntity(Entity e, int forcedID = 0)
        {
            if (e.GetID() != 0)
            {
                Debug.LogError("Registration of registered entity!");
                return;
            }
            if (forcedID > 0)
            {
                e.SetID(forcedID);
            }
            else
            {
                e.SetID(EntityManager.GetNextID());
            }
            EntityManager.Get().entities[e.GetID()] = e;
        }

        public static void UnregisterEntity(Entity e)
        {
            if (EntityManager.Get().entities.ContainsKey(e.GetID()))
            {
                EntityManager.Get().entities.Remove(e.GetID());
            }
        }

        public static Entity GetEntity(int ID)
        {
            if (EntityManager.Get().entities != null && EntityManager.Get().entities.ContainsKey(ID))
            {
                return EntityManager.Get().entities[ID];
            }
            return null;
        }

        public static T GetEntityOfType<T>() where T : Entity
        {
            if (EntityManager.Get().entities != null)
            {
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value is T)
                    {
                        return entity.Value as T;
                    }
                }
            }
            return null;
        }

        public static List<T> GetEntitiesType<T>() where T : Entity
        {
            List<T> list = new List<T>();
            if (EntityManager.Get().entities != null)
            {
                foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
                {
                    if (entity.Value is T)
                    {
                        list.Add(entity.Value as T);
                    }
                }
            }
            return list;
        }

        public static void CleanupSequence()
        {
            GameManager.Destroy();
            EntityManager.instance = null;
        }

        public static void PurgeSimulationUnits()
        {
            List<Entity> list = null;
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (entity.Value is Unit && (entity.Value as Unit).simulationUnit)
                {
                    if (list == null)
                    {
                        list = new List<Entity>();
                    }
                    list.Add(entity.Value);
                }
            }
            if (list == null)
            {
                return;
            }
            foreach (Entity item in list)
            {
                (item as Unit).Destroy();
            }
        }

        internal static bool IsCoherent()
        {
            foreach (KeyValuePair<int, Entity> entity in EntityManager.Get().entities)
            {
                if (entity.Value is Unit)
                {
                    Unit unit = (Unit)entity.Value;
                    if (unit.group == null || !EntityManager.Get().entities.ContainsKey(unit.group.ID))
                    {
                        Debug.LogError("(Unit) Entity Sanity error " + unit.ID);
                        return false;
                    }
                }
                else if (entity.Value is Group)
                {
                    Group group = (Group)entity.Value;
                    if (group.transporter != null && !EntityManager.Get().entities.ContainsKey(group.transporter.ID))
                    {
                        Debug.LogError("(A) Entity Sanity error " + group.GetReportForLog());
                        return false;
                    }
                    if (group.beforeMovingAway != null)
                    {
                        Debug.LogError("(B) Entity Sanity error " + group.GetReportForLog());
                        return false;
                    }
                    if (group.locationHost != null && (group.GetLocationHostSmart() == null || !EntityManager.Get().entities.ContainsKey(group.GetLocationHostSmart().ID)))
                    {
                        Debug.LogError("(C) Entity Sanity error " + group.GetReportForLog());
                        return false;
                    }
                    if (group.GetUnits() == null)
                    {
                        continue;
                    }
                    foreach (Reference<Unit> unit2 in group.GetUnits())
                    {
                        if (!EntityManager.Get().entities.ContainsKey(unit2.ID))
                        {
                            Debug.LogError("(D) Entity Sanity error " + group.GetReportForLog());
                            return false;
                        }
                    }
                }
                else
                {
                    if (!(entity.Value is PlayerWizard))
                    {
                        continue;
                    }
                    PlayerWizard playerWizard = (PlayerWizard)entity.Value;
                    if (playerWizard.heroes == null)
                    {
                        continue;
                    }
                    foreach (Reference<Unit> hero in playerWizard.heroes)
                    {
                        if (!EntityManager.Get().entities.ContainsKey(hero.ID))
                        {
                            Debug.LogError("(Hero) Entity Sanity error, hero ID: " + hero.ID + " removed incorrectly for wizard " + playerWizard.ID);
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
