using System.Collections.Generic;
using MHUtils;

namespace DBDef
{
    public static class EnchantmentExtension
    {
        private static Dictionary<string, Spell> spellsByEnchantment;

        private static void InitSBE()
        {
            EnchantmentExtension.spellsByEnchantment = new Dictionary<string, Spell>();
            List<Spell> type = DataBase.GetType<Spell>();
            List<string> list = new List<string>();
            foreach (Spell item in type)
            {
                if (item.enchantmentData == null || item.enchantmentData.Length == 0)
                {
                    continue;
                }
                Enchantment[] enchantmentData = item.enchantmentData;
                for (int i = 0; i < enchantmentData.Length; i++)
                {
                    string dbName = enchantmentData[i].dbName;
                    if (EnchantmentExtension.spellsByEnchantment.ContainsKey(dbName))
                    {
                        list.Add(dbName);
                    }
                    else
                    {
                        EnchantmentExtension.spellsByEnchantment.Add(dbName, item);
                    }
                }
            }
            foreach (string item2 in list)
            {
                EnchantmentExtension.spellsByEnchantment.Remove(item2);
            }
        }

        public static Spell GetSpell(this Enchantment enchantment)
        {
            if (EnchantmentExtension.spellsByEnchantment == null)
            {
                EnchantmentExtension.InitSBE();
            }
            Spell value = null;
            EnchantmentExtension.spellsByEnchantment.TryGetValue(enchantment.dbName, out value);
            return value;
        }
    }
}
