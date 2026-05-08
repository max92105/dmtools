using Controllers.Factories;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Controllers.Controllers
{
    /// <summary>
    /// One-time migration of legacy Monster string fields into structured objects.
    /// Safe to run on every startup — skips monsters that are already migrated.
    /// </summary>
    public static class LegacyMigrationController
    {
        public static void MigrateArmorClass()
        {
            var monsterFactory = new MonsterFactory();
            var acFactory = new ArmorClassEntryFactory();

            foreach (var monster in monsterFactory.GetObjects())
            {
                var entries = acFactory.GetObjectsByMonsterId(monster.Id);
                var primary = entries.FirstOrDefault();
                if (primary != null && monster.ArmorClass != primary.Value)
                {
                    monster.ArmorClass = primary.Value;
                    monsterFactory.SaveObject(monster);
                }
            }
        }

        public static void MigrateTypesCasing()
        {
            var typeMap    = ConfigurationPageDataController.LoadMonsterTypes()
                                .ToDictionary(t => t.Name.ToLowerInvariant(), t => t.Name);
            var subtypeMap = ConfigurationPageDataController.LoadMonsterSubtypes()
                                .ToDictionary(t => t.Name.ToLowerInvariant(), t => t.Name);

            var monsterFactory = new MonsterFactory();
            foreach (var monster in monsterFactory.GetObjects())
            {
                string newType    = Remap(monster.Type, typeMap);
                string newSubtype = Remap(monster.Subtype, subtypeMap);

                if (newType != monster.Type || newSubtype != monster.Subtype)
                {
                    monster.Type    = newType    ?? monster.Type;
                    monster.Subtype = newSubtype ?? monster.Subtype;
                    monsterFactory.SaveObject(monster);
                }
            }
        }

        private static string Remap(string value, Dictionary<string, string> map)
        {
            if (string.IsNullOrEmpty(value)) return value;
            string key = value.ToLowerInvariant();
            return map.TryGetValue(key, out string canonical) ? canonical : value;
        }

        public static void MigrateLegacyData()
        {
            var monsterFactory = new MonsterFactory();
            var speedFactory = new SpeedFactory();
            var senseFactory = new SenseFactory();
            var dmFactory = new DamageModifierFactory();
            var acFactory = new ArmorClassEntryFactory();

            var monsters = monsterFactory.GetObjects();

            foreach (var monster in monsters)
            {
                bool anyExistingSpeeds = speedFactory.GetObjectsByMonsterId(monster.Id).Count > 0;
                bool anyExistingSenses = senseFactory.GetObjectsByMonsterId(monster.Id).Count > 0;
                bool anyExistingDm = dmFactory.GetObjectsByMonsterId(monster.Id).Count > 0;
                bool anyExistingAc = acFactory.GetObjectsByMonsterId(monster.Id).Count > 0;

                if (!anyExistingAc && monster.ArmorClass > 0)
                {
                    var entry = new ArmorClassEntry
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = monster.Id,
                        Label = "Default",
                        Value = monster.ArmorClass
                    };
                    acFactory.SaveObject(entry);
                }

                if (!anyExistingSpeeds && !string.IsNullOrWhiteSpace(monster.Speed))
                {
                    foreach (var speed in ParseSpeeds(monster.Speed, monster.Id))
                        speedFactory.SaveObject(speed);
                }

                if (!anyExistingSenses && !string.IsNullOrWhiteSpace(monster.Senses))
                {
                    foreach (var sense in ParseSenses(monster.Senses, monster.Id))
                        senseFactory.SaveObject(sense);
                }

                if (!anyExistingDm)
                {
                    short diceCount, diceSize;
                    DamageModifierDefaults.GetDefaultDice(monster.ChallengeRating, out diceCount, out diceSize);

                    foreach (string dt in ParseDamageTypes(monster.DamageVulnerabilities))
                        dmFactory.SaveObject(MakeDamageModifier(monster.Id, dt, "Vulnerability", diceCount, diceSize));

                    foreach (string dt in ParseDamageTypes(monster.DamageResistances))
                        dmFactory.SaveObject(MakeDamageModifier(monster.Id, dt, "Resistance", diceCount, diceSize));

                    foreach (string dt in ParseDamageTypes(monster.DamageImmunities))
                        dmFactory.SaveObject(MakeDamageModifier(monster.Id, dt, "Immunity", 0, 0));
                }
            }
        }

        private static DamageModifier MakeDamageModifier(Guid monsterId, string damageType, string modifierType, short diceCount, short diceSize)
        {
            return new DamageModifier
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                DamageType = damageType,
                ModifierType = modifierType,
                DiceCount = diceCount,
                DiceSize = diceSize
            };
        }

        private static List<Speed> ParseSpeeds(string input, Guid monsterId)
        {
            var result = new List<Speed>();
            if (string.IsNullOrWhiteSpace(input)) return result;

            // Remove parenthetical notes like "(hover)", "(in spider form)"
            string cleaned = Regex.Replace(input, @"\([^)]*\)", "");

            var parts = cleaned.Split(',');
            foreach (var part in parts)
            {
                string token = part.Trim();
                var numMatch = Regex.Match(token, @"(\d+)\s*ft", RegexOptions.IgnoreCase);
                if (!numMatch.Success) continue;
                if (!short.TryParse(numMatch.Groups[1].Value, out short value)) continue;

                string lower = token.ToLowerInvariant();
                string speedType = "Walk";
                if (lower.Contains("fly")) speedType = "Fly";
                else if (lower.Contains("swim")) speedType = "Swim";
                else if (lower.Contains("climb")) speedType = "Climb";
                else if (lower.Contains("burrow")) speedType = "Burrow";
                else if (lower.Contains("hover")) speedType = "Hover";

                result.Add(new Speed
                {
                    Id = Guid.NewGuid(),
                    MonsterId = monsterId,
                    SpeedType = speedType,
                    Value = value
                });
            }

            return result;
        }

        private static List<Sense> ParseSenses(string input, Guid monsterId)
        {
            var result = new List<Sense>();
            if (string.IsNullOrWhiteSpace(input)) return result;

            var parts = input.Split(',');
            foreach (var part in parts)
            {
                string token = part.Trim();

                // "Passive Perception" stays as a Sense entry (it's in SenseTypes.All)
                if (token.IndexOf("passive", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var ppMatch = Regex.Match(token, @"(\d+)");
                    if (ppMatch.Success && short.TryParse(ppMatch.Groups[1].Value, out short ppValue))
                    {
                        result.Add(new Sense
                        {
                            Id = Guid.NewGuid(),
                            MonsterId = monsterId,
                            SenseType = "Passive Perception",
                            Value = ppValue
                        });
                    }
                    continue;
                }

                var numMatch = Regex.Match(token, @"(\d+)\s*ft", RegexOptions.IgnoreCase);
                if (!numMatch.Success) continue;
                if (!short.TryParse(numMatch.Groups[1].Value, out short value)) continue;

                string lower = token.ToLowerInvariant();
                string senseType;
                if (lower.Contains("blindsight")) senseType = "Blindsight";
                else if (lower.Contains("darkvision")) senseType = "Darkvision";
                else if (lower.Contains("tremorsense")) senseType = "Tremorsense";
                else if (lower.Contains("truesight")) senseType = "Truesight";
                else senseType = token.Split(' ')[0]; // best guess from first word

                result.Add(new Sense
                {
                    Id = Guid.NewGuid(),
                    MonsterId = monsterId,
                    SenseType = senseType,
                    Value = value
                });
            }

            return result;
        }

        private static List<string> ParseDamageTypes(string input)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(input)) return result;

            // Match known damage types from longest to shortest to correctly handle
            // compound entries like "Bludgeoning, Piercing, and Slashing from Nonmagical Attacks"
            string remaining = input;
            foreach (string knownType in DamageTypes.All.OrderByDescending(t => t.Length))
            {
                if (remaining.IndexOf(knownType, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(knownType);
                    remaining = Regex.Replace(remaining, Regex.Escape(knownType), "", RegexOptions.IgnoreCase);
                }
            }

            // Pick up anything left that didn't match a known type (custom damage types)
            var leftover = remaining.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in leftover)
            {
                string trimmed = part.Trim().Trim(';').Trim(',').Trim();
                // Remove filler words left from compound removals
                trimmed = Regex.Replace(trimmed, @"\b(and|from|nonmagical|attacks|weapons)\b", "", RegexOptions.IgnoreCase).Trim();
                if (trimmed.Length > 2)
                    result.Add(trimmed);
            }

            return result;
        }
    }
}
