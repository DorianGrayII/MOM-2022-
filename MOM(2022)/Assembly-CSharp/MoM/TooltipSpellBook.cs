// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.TooltipSpellBook
using System;
using System.Collections.Generic;
using DBDef;
using MHUtils.UI;
using MOM;
using TMPro;
using UnityEngine;

public class TooltipSpellBook : TooltipBase
{
    [Serializable]
    public class Books
    {
        public ERealm realm;

        public TextMeshProUGUI labelValue;

        [Tooltip("If set to an object the object will have it's active state set to false if the number of books == 0")]
        public GameObject visibility;
    }

    [Tooltip("Use Realm None for the total row")]
    public List<Books> books = new List<Books>();

    private static TooltipSpellBook instance;

    public static void Update()
    {
        if ((bool)TooltipSpellBook.instance)
        {
            TooltipSpellBook.instance.Populate(null);
        }
    }

    public void Awake()
    {
        ScreenBase.LocalizeTextFields(base.gameObject);
        TooltipSpellBook.instance = this;
    }

    public void OnDestroy()
    {
        if (TooltipSpellBook.instance == this)
        {
            TooltipSpellBook.instance = null;
        }
    }

    public override void Populate(object o)
    {
        PlayerWizard playerWizard = o as PlayerWizard;
        Books books = null;
        int num = 0;
        SelectWizard selectWizard = SelectWizard.Get();
        foreach (Books book in this.books)
        {
            if (book.realm == ERealm.None)
            {
                books = book;
                continue;
            }
            int num2 = 0;
            if (selectWizard != null)
            {
                num2 = selectWizard.GetNumBooks(book.realm);
            }
            else
            {
                if (playerWizard == null)
                {
                    playerWizard = GameManager.GetHumanWizard();
                }
                num2 = playerWizard.GetAttributes().GetFinal(MagicAndResearch.GetTagForRealm(book.realm)).ToInt();
            }
            num += num2;
            book.labelValue.text = num2.ToString();
            if ((bool)book.visibility)
            {
                book.visibility.SetActive(num2 > 0);
            }
        }
        if (books != null)
        {
            books.labelValue.text = num.ToString();
        }
    }
}
