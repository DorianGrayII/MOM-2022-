using System.Collections.Generic;
using DBDef;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class SpellManager
    {
        public ISpellCaster owner;

        [ProtoMember(1)]
        public List<DBReference<Spell>> spells;

        [ProtoMember(2)]
        private List<DBReference<Spell>> worldFav;

        [ProtoMember(3)]
        private List<DBReference<Spell>> battleFav;

        public SpellManager()
        {
        }

        public SpellManager(ISpellCaster owner)
        {
            this.owner = owner;
            this.spells = new List<DBReference<Spell>>();
            this.worldFav = new List<DBReference<Spell>>();
            this.battleFav = new List<DBReference<Spell>>();
        }

        public void Add(Spell spell)
        {
            if (spell == null)
            {
                Debug.LogWarning("spell is null");
            }
            else
            {
                this.GetSpells().Add(spell);
            }
        }

        public void Remove(Spell spell)
        {
            for (int i = 0; i < this.GetSpells().Count; i++)
            {
                if (this.GetSpells()[i].Get() == spell)
                {
                    this.GetSpells().RemoveAt(i);
                    break;
                }
            }
        }

        public List<DBReference<Spell>> GetSpells()
        {
            if (this.spells == null)
            {
                this.spells = new List<DBReference<Spell>>();
            }
            return this.spells;
        }

        public List<DBReference<Spell>> GetWorldFav()
        {
            if (this.worldFav == null)
            {
                this.worldFav = new List<DBReference<Spell>>();
            }
            return this.worldFav;
        }

        public List<DBReference<Spell>> GetBattleFav()
        {
            if (this.battleFav == null)
            {
                this.battleFav = new List<DBReference<Spell>>();
            }
            return this.battleFav;
        }

        public SpellManager CopySpellManager(ISpellCaster newOwner)
        {
            return new SpellManager
            {
                owner = newOwner,
                spells = new List<DBReference<Spell>>(this.GetSpells())
            };
        }
    }
}
