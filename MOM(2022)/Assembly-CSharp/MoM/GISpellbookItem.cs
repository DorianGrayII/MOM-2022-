// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.GISpellbookItem
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MHUtils.UI;
using MOM;
using UnityEngine;

public class GISpellbookItem : GridItemBase
{
    public GameObject[] lifeBooks;

    public GameObject[] deathBooks;

    public GameObject[] chaosBooks;

    public GameObject[] natureBooks;

    public GameObject[] sorceryBooks;

    public Dictionary<string, GameObject[]> booksByTag;

    private void Awake()
    {
        this.booksByTag = new Dictionary<string, GameObject[]>();
        this.booksByTag.Add(((Tag)TAG.LIFE_MAGIC_BOOK).dbName, this.lifeBooks);
        this.booksByTag.Add(((Tag)TAG.DEATH_MAGIC_BOOK).dbName, this.deathBooks);
        this.booksByTag.Add(((Tag)TAG.CHAOS_MAGIC_BOOK).dbName, this.chaosBooks);
        this.booksByTag.Add(((Tag)TAG.NATURE_MAGIC_BOOK).dbName, this.natureBooks);
        this.booksByTag.Add(((Tag)TAG.SORCERY_MAGIC_BOOK).dbName, this.sorceryBooks);
    }

    public void Set(object bookOrRealm, int bookShelfIndex = 0, Wizard w = null)
    {
        Tag tag = bookOrRealm as Tag;
        if (bookOrRealm is ERealm realm)
        {
            tag = MagicAndResearch.GetTagForRealm(realm);
        }
        foreach (KeyValuePair<string, GameObject[]> item in this.booksByTag)
        {
            GameObject[] value = item.Value;
            for (int i = 0; i < value.Length; i++)
            {
                value[i].SetActive(value: false);
            }
        }
        if (w == null)
        {
            w = GameManager.GetHumanWizard().GetBaseWizard();
        }
        MHRandom mHRandom = new MHRandom(w.dbName.GetHashCode());
        for (int j = 0; j < bookShelfIndex; j++)
        {
            mHRandom.GetInt(0, 100);
        }
        GameObject[] array = this.booksByTag[tag.dbName];
        int @int = mHRandom.GetInt(0, array.Length);
        array[@int].SetActive(value: true);
    }
}
