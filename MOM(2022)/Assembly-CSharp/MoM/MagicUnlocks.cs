using System;
using System.Collections.Generic;
using DBDef;
using MHUtils;
using ProtoBuf;
using UnityEngine;

namespace MOM
{
    [ProtoContract]
    public class MagicUnlocks
    {
        [ProtoMember(1)]
        public ERealm realm;

        [ProtoMember(2)]
        public DBReference<BooksAdvantage> booksAdvantages;

        [ProtoMember(3)]
        public List<Multitype<ERarity, int>> researchedCount;

        [ProtoMember(4)]
        public int seed;

        public MagicUnlocks()
        {
        }

        public MagicUnlocks(Tag book, FInt level)
        {
            this.seed = global::UnityEngine.Random.Range(0, int.MaxValue);
            this.researchedCount = new List<Multitype<ERarity, int>>();
            this.realm = MagicAndResearch.GetRealmFromTag(book);
            this.booksAdvantages = null;
            this.UpdateTo(level.ToInt());
        }

        public void UpdateTo(int level)
        {
            List<BooksAdvantage> type = DataBase.GetType<BooksAdvantage>();
            BooksAdvantage booksAdvantage = type.Find((BooksAdvantage o) => o.bookCount == level);
            if (booksAdvantage == null)
            {
                booksAdvantage = type[type.Count - 1];
                Debug.LogError("count " + level + " not found in BooksAdvantage database");
            }
            this.booksAdvantages = booksAdvantage;
        }

        public int GetResearchedCount(ERarity r)
        {
            if (this.researchedCount == null)
            {
                return 0;
            }
            return this.researchedCount.Find((Multitype<ERarity, int> o) => o.t0 == r)?.t1 ?? 0;
        }

        public int GetResearchLimit(ERarity r)
        {
            if (this.booksAdvantages == null)
            {
                return 0;
            }
            SpellsSection[] researchLimit = this.booksAdvantages.Get().researchLimit;
            if (researchLimit == null)
            {
                return 0;
            }
            return Array.Find(researchLimit, (SpellsSection o) => o.rarity == r)?.count ?? 0;
        }

        public void SetResearched(ERarity r)
        {
            if (this.researchedCount == null)
            {
                this.researchedCount = new List<Multitype<ERarity, int>>();
            }
            Multitype<ERarity, int> multitype = this.researchedCount.Find((Multitype<ERarity, int> o) => o.t0 == r);
            if (multitype == null)
            {
                multitype = new Multitype<ERarity, int>(r, 0);
                this.researchedCount.Add(multitype);
            }
            multitype.t1++;
        }
    }
}
