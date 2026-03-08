using DBCD;
using WoWTools.SpellDescParser;

namespace wow.tools.local.Services
{
    public class SpellDataSupplier : ISupplier
    {
        private DBCManager dbcManager;
        private string build;
        private byte level;
        private sbyte difficulty;
        private short mapID;
        private sbyte expansion = -2;

        public SpellDataSupplier(DBCManager dbcManager, string build, byte level = 60, sbyte difficulty = -1, short mapID = -1)
        {
            this.build = build;
            this.dbcManager = dbcManager;
            this.level = level;
            this.difficulty = difficulty;
            this.mapID = mapID;
        }

        public DBCDRow? SupplyEffectRow(int spellID, uint? effectIndex)
        {
            effectIndex ??= 1;

            var spellEffects = dbcManager.FindRecords("SpellEffect", build, "SpellID", spellID).Result;
            if (spellEffects.Count > 0)
            {
                foreach (var spellEffect in spellEffects)
                {
                    // TODO: LINQ as well as proper fallbacks to Difficulty::FallbackDifficultyID if SpellEffect does not have DifficultyID for this EffectIndex
                    if (uint.Parse(spellEffect["EffectIndex"].ToString()!) == (short)effectIndex - 1 && uint.Parse(spellEffect["DifficultyID"].ToString()!) == 0)
                    {
                        return spellEffect;
                    }
                }
            }

            return null;
        }

        // Stat/Effect Point parsing based on work done by simc & https://github.com/TrinityCore/SpellWork 
        public double? SupplyEffectPoint(int spellID, uint? effectIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            if (spellEffect == null)
                return null;

            var effectPoints = (float)spellEffect["EffectBasePointsF"];

            var spellMiscEntry = dbcManager.FindRecords("SpellMisc", build, "SpellID", spellID, true).Result;
            var spellAttributes = spellMiscEntry.Count == 0 ? new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } : spellMiscEntry[0].FieldAs<int[]>("Attributes");

            if ((float)spellEffect["Coefficient"] != 0.0f)
            {
                // TODO: Not yet implemented
                var spellScalingEntry = dbcManager.FindRecords("SpellScaling", build, "SpellID", spellID, true).Result;
                if (spellScalingEntry == null)
                    return 0.0f;

                return effectPoints;
            }
            else
            {
                var miscValue = spellEffect.FieldAs<int[]>("EffectMiscValue");

                var expectedStatRows = dbcManager.FindRecords("ExpectedStat", build, "Lvl", level).Result;
                var expectedStatRow = expectedStatRows.Where(x => (int)x["ExpansionID"] == this.expansion);

                var expectedStatType =
                    TooltipUtils.GetExpectedStatTypeBySpellEffect(int.Parse(spellEffect["Effect"].ToString()), (short)spellEffect["EffectAura"], miscValue[0]);
                if (expectedStatType != TooltipUtils.ExpectedStatType.None)
                {
                    if ((spellAttributes[0] & 0x80000) == 0x80000)
                        expectedStatType = TooltipUtils.ExpectedStatType.CreatureAutoAttackDps;


                }

                return effectPoints;
            }
        }

        public int? SupplyEffectAmplitude(int spellID, uint? effectIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            return (int?)spellEffect?["EffectAmplitude"];
        }

        public int? SupplyAuraPeriod(int spellID, uint? effectIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            return (int?)spellEffect?["EffectAuraPeriod"];
        }

        public int? SupplyChainTargets(int spellID, uint? effectIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            return (int?)spellEffect?["EffectChainTargets"];
        }

        public int? SupplyMaxTargetLevel(int spellID)
        {
            var spellTargetRestrictions = dbcManager.FindRecords("SpellTargetRestrictions", build, "SpellID", spellID, true).Result;
            if (spellTargetRestrictions.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in SpellTargetRestrictions");
                return null;
            }

            return (int)spellTargetRestrictions[0]["MaxTargetLevel"];
        }

        public int? SupplyMaxTargets(int spellID)
        {
            var spellTargetRestrictions = dbcManager.FindRecords("SpellTargetRestrictions", build, "SpellID", spellID, true).Result;
            if (spellTargetRestrictions.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in SpellTargetRestrictions");
                return null;
            }

            return (byte)spellTargetRestrictions[0]["MaxTargets"];
        }

        public int? SupplyProcCharges(int spellID)
        {
            var spellAuraOptions = dbcManager.FindRecords("SpellAuraOptions", build, "SpellID", spellID, true).Result;
            if (spellAuraOptions.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spellAuraOptions");
                return null;
            }

            return (int)spellAuraOptions[0]["ProcCharges"];
        }

        public int? SupplyDuration(int spellID)
        {
            var spellMiscRow = dbcManager.FindRecords("spellMisc", build, "SpellID", spellID, true).Result;
            if (spellMiscRow.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spell misc");
                return null;
            }

            var spellDurationID = (ushort)spellMiscRow[0]["DurationIndex"];
            if (spellDurationID == 0)
            {
                Console.WriteLine("Unable to find duration for Spell ID " + spellID);
                return null;
            }

            var spellDurationDB = dbcManager.GetOrLoad("SpellDuration", build).Result;
            if (spellDurationDB.TryGetValue(spellDurationID, out var durationRow))
            {
                return (int)durationRow["Duration"];
            }

            Console.WriteLine("Unable to find duration for Spell ID " + spellID);
            return null;
        }

        public double? SupplyRadius(int spellID, uint? effectIndex, int radiusIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            if (spellEffect == null)
                return null;

            var radiusIndexArray = spellEffect.FieldAs<int[]>("EffectRadiusIndex");

            // $a is for first array entry, $A for second
            var spellRadiusID = radiusIndexArray[radiusIndex];

            // TODO: Temp fix, something wrong here.
            if (spellRadiusID == 0)
                spellRadiusID = radiusIndexArray[radiusIndex == 0 ? 1 : 0];

            var spellRadiusDB = dbcManager.GetOrLoad("SpellRadius", build).Result;
            if (spellRadiusDB.TryGetValue(spellRadiusID, out var radiusRow))
            {
                return (float)radiusRow["Radius"];
            }

            Console.WriteLine("Unable to find radius for Spell ID " + spellID + " index " + effectIndex + " radiusIndex " + radiusIndex);
            return null;
        }

        public int? SupplyMaxStacks(int spellID)
        {
            var spellAuraOptions = dbcManager.FindRecords("SpellAuraOptions", build, "SpellID", spellID, true).Result;
            if (spellAuraOptions.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spellAuraOptions");
                return null;
            }

            return (ushort)spellAuraOptions[0]["CumulativeAura"];
        }

        public int? SupplyProcChance(int spellID)
        {
            var spellAuraOptions = dbcManager.FindRecords("SpellAuraOptions", build, "SpellID", spellID, true).Result;
            if (spellAuraOptions.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spellAuraOptions");
                return null;
            }

            return (byte)spellAuraOptions[0]["ProcChance"];
        }

        public int? SupplyMinRange(int spellID)
        {
            var spellMiscRow = dbcManager.FindRecords("spellMisc", build, "SpellID", spellID, true).Result;
            if (spellMiscRow.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spell misc");
                return null;
            }

            var spellRangeID = (ushort)spellMiscRow[0]["RangeIndex"];
            if (spellRangeID == 0)
            {
                Console.WriteLine("Unable to find range for Spell ID " + spellID);
                return null;
            }

            var spellRangeDB = dbcManager.GetOrLoad("SpellRange", build).Result;
            if (spellRangeDB.TryGetValue(spellRangeID, out var rangeRow))
            {
                var rangeMin = rangeRow.FieldAs<float[]>("RangeMin");

                if ((int)rangeMin[0] != 0)
                {
                    return (int)rangeMin[0];
                }

                if ((int)rangeMin[1] != 0)
                {
                    return (int)rangeMin[1];
                }
            }

            return null;
        }

        public int? SupplyMaxRange(int spellID)
        {
            var spellMiscRow = dbcManager.FindRecords("spellMisc", build, "SpellID", spellID, true).Result;
            if (spellMiscRow.Count == 0)
            {
                Console.WriteLine("Unable to find Spell ID " + spellID + " in spell misc");
                return null;
            }

            var spellRangeID = (ushort)spellMiscRow[0]["RangeIndex"];
            if (spellRangeID == 0)
            {
                Console.WriteLine("Unable to find range for Spell ID " + spellID);
                return null;
            }

            var spellRangeDB = dbcManager.GetOrLoad("SpellRange", build).Result;
            if (spellRangeDB.TryGetValue(spellRangeID, out var rangeRow))
            {
                var rangeMax = rangeRow.FieldAs<float[]>("RangeMax");

                if ((int)rangeMax[0] != 0)
                {
                    return (int)rangeMax[0];
                }

                if ((int)rangeMax[1] != 0)
                {
                    return (int)rangeMax[1];
                }
            }

            return null;
        }

        public int? SupplyEffectMisc(int spellID, uint? effectIndex)
        {
            var spellEffect = SupplyEffectRow(spellID, effectIndex);
            if (spellEffect == null)
                return null;

            return (int)spellEffect.FieldAs<int[]>("EffectMiscValue")[0];
        }

        public string? SupplySpellName(int spellID)
        {
            var spellNameDB = dbcManager.GetOrLoad("SpellName", build).Result;
            if (spellNameDB.TryGetValue(spellID, out var spellNameRow))
            {
                return (string)spellNameRow["Name_lang"];
            }

            return null;
        }
    }
}