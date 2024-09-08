namespace MHUtils
{
    using System;

    public class ScriptType : Attribute
    {
        public Type eType;

        public ScriptType(Type eType)
        {
            this.eType = eType;
        }

        public enum Type
        {
            EnchantmentApplyScript,
            EnchantmentActivatorIntScript,
            EnchantmentActivatorFIntScript,
            EnchantmentActivatorFloatScript,
            EnchantmentActivatorScript,
            EditorWizardFilter,
            EditorCityFilter,
            EditorHeroFilter,
            EditorUnitFilter,
            EditorEnchantmentFilter,
            EditorGroupFilter,
            EditorEquipmentFilter,
            EditorFilterListProcessing,
            EditorListResult,
            EditorWizardResult,
            EditorHeroResult,
            EditorCityResult,
            EditorSharedTagResult,
            EditorFamiliarResult,
            EditorModifier,
            EditorSpawnLocation,
            EditorBattleNode,
            OtherCriteria,
            UISortScript,
            UIFilterScript
        }
    }
}

