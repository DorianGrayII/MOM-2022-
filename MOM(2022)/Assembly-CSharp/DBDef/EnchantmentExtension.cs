namespace DBDef
{
    using MHUtils;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class EnchantmentExtension
    {
        private static Dictionary<string, Spell> spellsByEnchantment;

        [Extension]
        public static Spell GetSpell(Enchantment enchantment)
        {
            if (spellsByEnchantment == null)
            {
                InitSBE();
            }
            Spell spell = null;
            spellsByEnchantment.TryGetValue(enchantment.dbName, out spell);
            return spell;
        }

        private static void InitSBE()
        {
            spellsByEnchantment = new Dictionary<string, Spell>();
            List<string> list = new List<string>();
            foreach (Spell spell in DataBase.GetType<Spell>())
            {
                if ((spell.enchantmentData != null) && (spell.enchantmentData.Length != 0))
                {
                    Enchantment[] enchantmentData = spell.enchantmentData;
                    for (int i = 0; i < enchantmentData.Length; i++)
                    {
                        string dbName = enchantmentData[i].dbName;
                        if (spellsByEnchantment.ContainsKey(dbName))
                        {
                            list.Add(dbName);
                        }
                        else
                        {
                            spellsByEnchantment.Add(dbName, spell);
                        }
                    }
                }
            }
            foreach (string str2 in list)
            {
                spellsByEnchantment.Remove(str2);
            }
        }
    }
}

