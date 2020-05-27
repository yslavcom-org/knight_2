﻿using UnityEngine;
using Kryz.CharacterStats;

namespace Iar.StackedInventory
{
    public enum EquipmentType
    {
        Health,
        HomingMissile,
        ForcefieldArmour
    }

    [CreateAssetMenu]
    public class EquipableStackedItem : StackedItem
    {
        public int StrengthBonus;
        public int AgilityBonus;
        public int IntelligenceBonus;
        public int VitalityBonus;
        [Space]
        public float StrengthPercentBonus;
        public float AgilityPercentBonus;
        public float IntelligencePercentBonus;
        public float VitalityPercentBonus;
        [Space]
        public EquipmentType EquipmentType;

        public void Equip(StackedCharacter c)
        {
            #region flat
            if (StrengthBonus != 0)
            {
                c.Strength.AddModifier(new StatModifier(StrengthBonus, StatModType.Flat, this));
            }

            if (AgilityBonus != 0)
            {
                c.Agility.AddModifier(new StatModifier(AgilityBonus, StatModType.Flat, this));
            }

            if (IntelligenceBonus != 0)
            {
                c.Intelligence.AddModifier(new StatModifier(IntelligenceBonus, StatModType.Flat, this));
            }

            if (VitalityBonus != 0)
            {
                c.Vitality.AddModifier(new StatModifier(VitalityBonus, StatModType.Flat, this));
            }
            #endregion

            #region percent
            if (StrengthPercentBonus != 0)
            {
                c.Strength.AddModifier(new StatModifier(StrengthPercentBonus, StatModType.PercentMult, this));
            }

            if (AgilityPercentBonus != 0)
            {
                c.Strength.AddModifier(new StatModifier(AgilityPercentBonus, StatModType.PercentMult, this));
            }

            if (IntelligencePercentBonus != 0)
            {
                c.Strength.AddModifier(new StatModifier(IntelligencePercentBonus, StatModType.PercentMult, this));
            }

            if (VitalityPercentBonus != 0)
            {
                c.Strength.AddModifier(new StatModifier(VitalityPercentBonus, StatModType.PercentMult, this));
            }
            #endregion
        }

        public void Unequip(StackedCharacter c)
        {
            c.Strength.RemoveAllModifiersFromSource(this);
            c.Agility.RemoveAllModifiersFromSource(this);
            c.Intelligence.RemoveAllModifiersFromSource(this);
            c.Vitality.RemoveAllModifiersFromSource(this);
        }
    }
}
