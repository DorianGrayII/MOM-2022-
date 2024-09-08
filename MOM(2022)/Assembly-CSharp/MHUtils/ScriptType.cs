using System;

namespace MHUtils
{
    public class ScriptType : Attribute
    {
        public enum Type
        {
            EnchantmentApplyScript = 0,
            EnchantmentActivatorIntScript = 1,
            EnchantmentActivatorFIntScript = 2,
            EnchantmentActivatorFloatScript = 3,
            EnchantmentActivatorScript = 4,
            EditorWizardFilter = 5,
            EditorCityFilter = 6,
            EditorHeroFilter = 7,
            EditorUnitFilter = 8,
            EditorEnchantmentFilter = 9,
            EditorGroupFilter = 10,
            EditorEquipmentFilter = 11,
            EditorFilterListProcessing = 12,
            EditorListResult = 13,
            EditorWizardResult = 14,
            EditorHeroResult = 15,
            EditorCityResult = 16,
            EditorSharedTagResult = 17,
            EditorFamiliarResult = 18,
            EditorModifier = 19,
            EditorSpawnLocation = 20,
            EditorBattleNode = 21,
            OtherCriteria = 22,
            UISortScript = 23,
            UIFilterScript = 24
        }

        public Type eType;

        public ScriptType(Type eType)
        {
            this.eType = eType;
        }
    }
}
