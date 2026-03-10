using wow.tools.local.Controllers;
using wow.tools.local.Providers;

namespace wow.tools.local.Utils
{
    public static class TooltipUtils
    {
        public enum InventoryType : sbyte
        {
            NonEquippable = 0,
            Head = 1,
            Neck = 2,
            Shoulder = 3,
            Shirt = 4,
            Chest = 5,
            Waist = 6,
            Legs = 7,
            Feet = 8,
            Wrist = 9,
            Hands = 10,
            Finger = 11,
            Trinket = 12,
            OneHand = 13,
            Shield = 14,
            Ranged = 15,
            Back = 16,
            TwoHand = 17,
            Bag = 18,
            Tabard = 19,
            Robe = 20,
            MainHand = 21,
            OffHand = 22,
            HeldInOffhand = 23,
            Ammo = 24,
            Thrown = 25,
            RangedRight = 26,
            Quiver = 27,
            Relic = 28,
            ProfessionTool = 29,
            ProfessionGear = 30,
            EquippableSpelOffensive = 31,
            EquippableSpellUtility = 32,
            EquippableSpellDefensive = 33,
            EquippableSpellWeapon = 34
        }

        public enum ItemStatType : sbyte
        {
            MANA = 0,
            HEALTH = 1,
            AGILITY = 3,
            STRENGTH = 4,
            INTELLECT = 5,
            SPIRIT = 6,
            STAMINA = 7,
            DEFENSE_SKILL_RATING = 12,
            DODGE_RATING = 13,
            PARRY_RATING = 14,
            BLOCK_RATING = 15,
            HIT_MELEE_RATING = 16,
            HIT_RANGED_RATING = 17,
            HIT_SPELL_RATING = 18,
            CRIT_MELEE_RATING = 19,
            CRIT_RANGED_RATING = 20,
            CRIT_SPELL_RATING = 21,
            CORRUPTION = 22,
            CORRUPTION_RESISTANCE = 23,
            MODIFIED_CRAFTING_STAT_1 = 24,
            MODIFIED_CRAFTING_STAT_2 = 25,
            CRIT_TAKEN_RANGED_RATING = 26,
            CRIT_TAKEN_SPELL_RATING = 27,
            HASTE_MELEE_RATING = 28,
            HASTE_RANGED_RATING = 29,
            HASTE_SPELL_RATING = 30,
            HIT_RATING = 31,
            CRIT_RATING = 32,
            HIT_TAKEN_RATING = 33,
            CRIT_TAKEN_RATING = 34,
            RESILIENCE_RATING = 35,
            HASTE_RATING = 36,
            EXPERTISE_RATING = 37,
            ATTACK_POWER = 38,
            RANGED_ATTACK_POWER = 39,
            VERSATILITY = 40,
            SPELL_HEALING_DONE = 41,
            SPELL_DAMAGE_DONE = 42,
            MANA_REGENERATION = 43,
            ARMOR_PENETRATION_RATING = 44,
            SPELL_POWER = 45,
            HEALTH_REGEN = 46,
            SPELL_PENETRATION = 47,
            BLOCK_VALUE = 48,
            MASTERY_RATING = 49,
            EXTRA_ARMOR = 50,
            FIRE_RESISTANCE = 51,
            FROST_RESISTANCE = 52,
            HOLY_RESISTANCE = 53,
            SHADOW_RESISTANCE = 54,
            NATURE_RESISTANCE = 55,
            ARCANE_RESISTANCE = 56,
            PVP_POWER = 57,
            CR_AMPLIFY = 58,
            CR_MULTISTRIKE = 59,
            CR_READINESS = 60,
            CR_SPEED = 61,
            CR_LIFESTEAL = 62,
            CR_AVOIDANCE = 63,
            CR_STURDINESS = 64
        }

        public enum ExpectedStatType : byte
        {
            CreatureHealth = 0,
            PlayerHealth = 1,
            CreatureAutoAttackDps = 2,
            CreatureArmor = 3,
            PlayerMana = 4,
            PlayerPrimaryStat = 5,
            PlayerSecondaryStat = 6,
            ArmorConstant = 7,
            None = 8,
            CreatureSpellDamage = 9
        }

        public static bool IsStatCombatRating(sbyte StatTypeID)
        {
            switch ((ItemStatType)StatTypeID)
            {
                case ItemStatType.DODGE_RATING:
                case ItemStatType.PARRY_RATING:
                case ItemStatType.BLOCK_RATING:
                case ItemStatType.HIT_MELEE_RATING:
                case ItemStatType.HIT_RANGED_RATING:
                case ItemStatType.HIT_SPELL_RATING:
                case ItemStatType.CRIT_MELEE_RATING:
                case ItemStatType.CRIT_RANGED_RATING:
                case ItemStatType.HIT_RATING:
                case ItemStatType.CRIT_RATING:
                case ItemStatType.RESILIENCE_RATING:
                case ItemStatType.HASTE_RATING:
                case ItemStatType.EXPERTISE_RATING:
                case ItemStatType.VERSATILITY:
                case ItemStatType.MASTERY_RATING:
                case ItemStatType.CR_MULTISTRIKE:
                case ItemStatType.CR_SPEED:
                case ItemStatType.CR_LIFESTEAL:
                case ItemStatType.CR_AVOIDANCE:
                case ItemStatType.CR_STURDINESS:
                    return true;
                default:
                    return false;
            }
        }

        public static ExpectedStatType GetExpectedStatTypeBySpellEffect(int effectType, int effectAuraType, int effectMiscValue0)
        {
            switch (effectType)
            {
                case 10:
                case 75:
                    return ExpectedStatType.PlayerHealth;
                case 8:
                    return ExpectedStatType.PlayerMana;
                case 2:
                case 7:
                case 9:
                case 17:
                case 58:
                    return ExpectedStatType.CreatureSpellDamage;
                case 30:
                case 62:
                    return effectMiscValue0 == 0 ? ExpectedStatType.PlayerMana : ExpectedStatType.None;
                case 6:
                case 27:
                case 35:
                case 65:
                case 119:
                case 128:
                case 129:
                case 143:
                case 174:
                case 202:
                    switch (effectAuraType)
                    {
                        case 8:
                        case 14:
                        case 34:
                        case 69:
                        case 84:
                        case 97:
                        case 115:
                        case 135:
                        case 161:
                        case 230:
                        case 250:
                        case 301:
                            return ExpectedStatType.PlayerHealth;
                        case 64:
                            return ExpectedStatType.PlayerMana;
                        case 29:
                        case 99:
                        case 124:
                            return ExpectedStatType.PlayerPrimaryStat;
                        case 189:
                            return ExpectedStatType.PlayerSecondaryStat;
                        case 22:
                        case 83:
                        case 123:
                        case 465:
                            return ExpectedStatType.ArmorConstant;
                        case 3:
                        case 13:
                        case 15:
                        case 43:
                        case 53:
                        case 59:
                        case 62:
                        case 102:
                        case 131:
                        case 180:
                            return ExpectedStatType.CreatureSpellDamage;
                        case 24:
                        case 35:
                        case 73:
                        case 85:
                        case 162:
                        case 418:
                            return effectMiscValue0 == 0 ? ExpectedStatType.PlayerMana : ExpectedStatType.None;
                        default:
                            return ExpectedStatType.None;
                    }
                default:
                    return ExpectedStatType.None;
            }
        }

        public static (string TargetField, sbyte TargetIndex) GetRandomPropertyByInventoryType(byte overallQualityID, sbyte inventoryTypeID, byte subClassID, string build)
        {
            var inventoryType = (InventoryType)inventoryTypeID;

            sbyte targetIndex = -1;
            switch (inventoryType)
            {
                case InventoryType.Head:
                case InventoryType.Shirt:
                case InventoryType.Chest:
                case InventoryType.Legs:
                case InventoryType.Ranged:
                case InventoryType.TwoHand:
                case InventoryType.Robe:
                case InventoryType.Thrown:
                    targetIndex = 0;
                    break;
                case InventoryType.Shoulder:
                case InventoryType.Waist:
                case InventoryType.Feet:
                case InventoryType.Hands:
                case InventoryType.Trinket:
                    targetIndex = 1;
                    break;
                case InventoryType.Neck:
                case InventoryType.Wrist:
                case InventoryType.Finger:
                case InventoryType.Back:
                    targetIndex = 2;
                    break;
                case InventoryType.OneHand:
                case InventoryType.Shield:
                case InventoryType.MainHand:
                case InventoryType.OffHand:
                case InventoryType.HeldInOffhand:
                    targetIndex = 3;
                    break;
                case InventoryType.RangedRight:
                    targetIndex = 3;
                    if (subClassID != 19) // Wands
                        targetIndex = 0;
                    break;
                case InventoryType.Relic:
                // TODO: Figure out correct values for the below types?
                case InventoryType.ProfessionTool:
                case InventoryType.ProfessionGear:
                case InventoryType.EquippableSpelOffensive:
                case InventoryType.EquippableSpellUtility:
                case InventoryType.EquippableSpellDefensive:
                case InventoryType.EquippableSpellWeapon:
                    targetIndex = 4;
                    break;
                default:
                    Console.WriteLine($"Unsupported inventory type for random property: {inventoryType}");
                    break;
            }

            string targetField = overallQualityID switch
            {
                2 => "Good",
                3 => "Superior",
                4 => "Epic",
                _ => "Good",
            };

            return (targetField, targetIndex);
        }

        public static TTItemStat CalculateItemStat(sbyte StatTypeID, int randProp, ushort ItemLevel, int statAlloc, float socketPenalty, byte QualityID, sbyte InventoryTypeID, byte SubClassID, string build)
        {
            // TODO: Socket penalty

            var inventoryType = (InventoryType)InventoryTypeID;

            var baseStat = (int)((statAlloc * randProp) * 0.000099999997f - socketPenalty + 0.5f);

            var multiplierRow = new GameTableProvider.MultByILVLRow()
            {
                JewelryMultiplier = 1.0d,
                TrinketMultiplier = 1.0d,
                WeaponMultiplier = 1.0d,
                ArmorMultiplier = 1.0d,
            };

            if (IsStatCombatRating(StatTypeID))
            {
                multiplierRow = GameTableProvider.GetCombatRatingsMultByILVLRow(ItemLevel - 1, build);
            }
            else if ((ItemStatType)StatTypeID == ItemStatType.STAMINA)
            {
                multiplierRow = GameTableProvider.GetStaminaMultByILVLRow(ItemLevel - 1, build);
            }

            double multiplier;
            switch (inventoryType)
            {
                case InventoryType.Neck:
                case InventoryType.Finger:
                    multiplier = multiplierRow.JewelryMultiplier;
                    break;
                case InventoryType.Trinket:
                    multiplier = multiplierRow.TrinketMultiplier;
                    break;
                case InventoryType.OneHand:
                case InventoryType.Shield:
                case InventoryType.Ranged:
                case InventoryType.TwoHand:
                case InventoryType.MainHand:
                case InventoryType.OffHand:
                case InventoryType.HeldInOffhand:
                case InventoryType.RangedRight:
                    multiplier = multiplierRow.WeaponMultiplier;
                    break;
                default:
                    multiplier = multiplierRow.ArmorMultiplier;
                    break;
            }

            int calculatedValue = (int)(multiplier * baseStat);

            return new TTItemStat()
            {
                StatTypeID = StatTypeID,
                Value = calculatedValue,
                IsCombatRating = IsStatCombatRating(StatTypeID)
            };
        }
    }
}