using System.Globalization;

namespace wow.tools.local.Services
{
    public static class GameTableProvider
    {
        private static Dictionary<string, Dictionary<int, MultByILVLRow>> combatRatingMultiByILVL;
        private static Dictionary<string, Dictionary<int, MultByILVLRow>> staminaMultiByILVL;
        private static Dictionary<string, Dictionary<int, SpellScalingRow>> spellScalingByLVL;

        public struct MultByILVLRow
        {
            public double ArmorMultiplier;
            public double WeaponMultiplier;
            public double TrinketMultiplier;
            public double JewelryMultiplier;
        }

        public struct SpellScalingRow
        {
            public double Rogue;
            public double Druid;
            public double Hunter;
            public double Mage;
            public double Paladin;
            public double Priest;
            public double Shaman;
            public double Warlock;
            public double Warrior;
            public double DeathKnight;
            public double Monk;
            public double DemonHunter;
            public double Item;
            public double Consumable;
            public double Gem1;
            public double Gem2;
            public double Gem3;
            public double Health;
            public double DamageReplaceStat;
            public double DamageSecondary;
        }

        static GameTableProvider()
        {
            combatRatingMultiByILVL = new Dictionary<string, Dictionary<int, MultByILVLRow>>();
            staminaMultiByILVL = new Dictionary<string, Dictionary<int, MultByILVLRow>>();
        }

        public static MultByILVLRow GetStaminaMultByILVLRow(int itemLevel, string build)
        {
            if (!staminaMultiByILVL.ContainsKey(build))
            {
                using (var client = new HttpClient())
                {
                    var tempDict = new Dictionary<int, MultByILVLRow>();
                    var gameTable = new StreamReader(CASC.GetFileByID(CASC.GetFileDataIDByName("gametables/staminamultbyilvl.txt"))!).ReadToEnd();
                    var lines = gameTable.Split("\r\n");
                    for (var i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Length == 0)
                            continue;

                        var fields = lines[i].Split('\t');
                        tempDict.Add(int.Parse(fields[0]),
                            new MultByILVLRow()
                            {
                                ArmorMultiplier = double.Parse(fields[1], CultureInfo.InvariantCulture),
                                WeaponMultiplier = double.Parse(fields[2], CultureInfo.InvariantCulture),
                                TrinketMultiplier = double.Parse(fields[3], CultureInfo.InvariantCulture),
                                JewelryMultiplier = double.Parse(fields[4], CultureInfo.InvariantCulture)
                            });
                    }
                    staminaMultiByILVL.Add(build, tempDict);
                }
            }

            if (staminaMultiByILVL.TryGetValue(build, out var buildDict))
            {
                // TODO: This broke.
                if (itemLevel == 0)
                    itemLevel = 1;

                if (buildDict.TryGetValue(itemLevel, out var row))
                {
                    return row;
                }
                else
                {
                    throw new Exception("Target itemLevel not found in gametable!");
                }
            }
            else
            {
                throw new Exception("Target build not found in gametable cache!");
            }
        }

        public static MultByILVLRow GetCombatRatingsMultByILVLRow(int itemLevel, string build)
        {
            if (!combatRatingMultiByILVL.ContainsKey(build))
            {
                using (var client = new HttpClient())
                {
                    var tempDict = new Dictionary<int, MultByILVLRow>();
                    var gameTable = new StreamReader(CASC.GetFileByID(CASC.GetFileDataIDByName("gametables/combatratingsmultbyilvl.txt"))!).ReadToEnd();
                    var lines = gameTable.Split("\r\n");
                    for (var i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Length == 0)
                            continue;

                        var fields = lines[i].Split('\t');
                        tempDict.Add(int.Parse(fields[0]),
                            new MultByILVLRow()
                            {
                                ArmorMultiplier = double.Parse(fields[1], CultureInfo.InvariantCulture),
                                WeaponMultiplier = double.Parse(fields[2], CultureInfo.InvariantCulture),
                                TrinketMultiplier = double.Parse(fields[3], CultureInfo.InvariantCulture),
                                JewelryMultiplier = double.Parse(fields[4], CultureInfo.InvariantCulture)
                            });
                    }
                    combatRatingMultiByILVL.Add(build, tempDict);
                }
            }

            if (combatRatingMultiByILVL.TryGetValue(build, out var buildDict))
            {
                // TODO: This broke.
                if (itemLevel == 0)
                    itemLevel = 1;

                if (buildDict.TryGetValue(itemLevel, out var row))
                {
                    return row;
                }
                else
                {
                    throw new Exception("Target itemLevel not found in gametable!");
                }
            }
            else
            {
                throw new Exception("Target build not found in gametable cache!");
            }
        }

        public static SpellScalingRow GetSpellScalingByLVLRow(int level, string build)
        {
            if (!spellScalingByLVL.ContainsKey(build))
            {
                using (var client = new HttpClient())
                {
                    var tempDict = new Dictionary<int, SpellScalingRow>();
                    var gameTable = new StreamReader(CASC.GetFileByID(CASC.GetFileDataIDByName("gametables/spellscaling.txt"))!).ReadToEnd();
                    var lines = gameTable.Split("\r\n");
                    for (var i = 1; i < lines.Length; i++)
                    {
                        if (lines[i].Length == 0)
                            continue;

                        var fields = lines[i].Split('\t');
                        tempDict.Add(int.Parse(fields[0]),
                            new SpellScalingRow()
                            {
                                Rogue = double.Parse(fields[1], CultureInfo.InvariantCulture),
                                Druid = double.Parse(fields[2], CultureInfo.InvariantCulture),
                                Hunter = double.Parse(fields[3], CultureInfo.InvariantCulture),
                                Mage = double.Parse(fields[4], CultureInfo.InvariantCulture),
                                Paladin = double.Parse(fields[5], CultureInfo.InvariantCulture),
                                Priest = double.Parse(fields[6], CultureInfo.InvariantCulture),
                                Shaman = double.Parse(fields[7], CultureInfo.InvariantCulture),
                                Warlock = double.Parse(fields[8], CultureInfo.InvariantCulture),
                                Warrior = double.Parse(fields[9], CultureInfo.InvariantCulture),
                                DeathKnight = double.Parse(fields[10], CultureInfo.InvariantCulture),
                                Monk = double.Parse(fields[11], CultureInfo.InvariantCulture),
                                DemonHunter = double.Parse(fields[12], CultureInfo.InvariantCulture),
                                Item = double.Parse(fields[13], CultureInfo.InvariantCulture),
                                Consumable = double.Parse(fields[14], CultureInfo.InvariantCulture),
                                Gem1 = double.Parse(fields[15], CultureInfo.InvariantCulture),
                                Gem2 = double.Parse(fields[16], CultureInfo.InvariantCulture),
                                Gem3 = double.Parse(fields[17], CultureInfo.InvariantCulture),
                                Health = double.Parse(fields[18], CultureInfo.InvariantCulture),
                                DamageReplaceStat = double.Parse(fields[19], CultureInfo.InvariantCulture),
                                DamageSecondary = double.Parse(fields[20], CultureInfo.InvariantCulture)
                            });
                    }
                    spellScalingByLVL.Add(build, tempDict);
                }
            }

            if (spellScalingByLVL.TryGetValue(build, out var buildDict))
            {
                if (buildDict.TryGetValue(level, out var row))
                {
                    return row;
                }
                else
                {
                    throw new Exception("Target level not found in gametable!");
                }
            }
            else
            {
                throw new Exception("Target build not found in gametable cache!");
            }
        }
    }
}