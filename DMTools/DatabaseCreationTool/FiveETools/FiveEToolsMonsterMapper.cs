using Data.Constants;
using Data.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using T = DatabaseCreationTool.FiveETools.FiveEToolsText;

namespace DatabaseCreationTool.FiveETools
{
    /// <summary>A monster converted to DM Tools objects, ready to be inserted.</summary>
    public class MappedMonster
    {
        public Monster Monster;
        public List<Characteristic> Characteristics = new List<Characteristic>();
        public List<Skill> Skills = new List<Skill>();
        public List<SpecialAbility> SpecialAbilities = new List<SpecialAbility>();
        public List<Data.Objects.Action> Actions = new List<Data.Objects.Action>();
        public List<ArmorClassEntry> ArmorClassEntries = new List<ArmorClassEntry>();
        public List<Speed> Speeds = new List<Speed>();
        public List<Sense> Senses = new List<Sense>();
        public List<DamageModifier> DamageModifiers = new List<DamageModifier>();
        public List<string> Warnings = new List<string>();
    }

    /// <summary>Converts a resolved 5e.tools monster (JSON) to DM Tools objects.</summary>
    public static class FiveEToolsMonsterMapper
    {
        private const string NonMagicalBps = "Bludgeoning, Piercing, and Slashing from Nonmagical Attacks";

        private static readonly Dictionary<string, string> Sizes = new Dictionary<string, string>
        {
            ["T"] = "Tiny", ["S"] = "Small", ["M"] = "Medium", ["L"] = "Large", ["H"] = "Huge", ["G"] = "Gargantuan",
        };

        private static readonly Dictionary<string, Guid> AbilityIds = new Dictionary<string, Guid>
        {
            ["str"] = Characteristics.StrenghtId,
            ["dex"] = Characteristics.DexterityId,
            ["con"] = Characteristics.ConstitutionId,
            ["int"] = Characteristics.IntelligenceId,
            ["wis"] = Characteristics.WisdomId,
            ["cha"] = Characteristics.CharismaId,
        };

        private static readonly Dictionary<string, Guid> SkillIds = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
        {
            ["acrobatics"] = SkillTypes.AcrobaticsId, ["animal handling"] = SkillTypes.AnimalHandlingId,
            ["arcana"] = SkillTypes.ArcanaId, ["athletics"] = SkillTypes.AthleticsId, ["deception"] = SkillTypes.DeceptionId,
            ["history"] = SkillTypes.HistoryId, ["insight"] = SkillTypes.InsightId, ["intimidation"] = SkillTypes.IntimidationId,
            ["investigation"] = SkillTypes.InvestigationId, ["medicine"] = SkillTypes.MedicineId, ["nature"] = SkillTypes.NatureId,
            ["perception"] = SkillTypes.PerceptionId, ["performance"] = SkillTypes.PerformanceId, ["persuasion"] = SkillTypes.PersuasionId,
            ["religion"] = SkillTypes.ReligionId, ["sleight of hand"] = SkillTypes.SleightOfHandId, ["stealth"] = SkillTypes.StealthId,
            ["survival"] = SkillTypes.SurvivalId,
        };

        private static readonly string[] SpeedOrder = { "walk", "burrow", "climb", "fly", "swim" };

        /// <param name="mon">Resolved monster (no "_copy").</param>
        /// <param name="legendaryGroup">Matching entry of meta.json (lair actions / regional effects), or null.</param>
        public static MappedMonster Map(JObject mon, JObject legendaryGroup)
        {
            var r = new MappedMonster();
            Guid id = Guid.NewGuid();
            var traits = new List<KeyValuePair<string, string>>(); // name, description
            var notes = new List<KeyValuePair<string, string>>();  // extra info that has no dedicated field

            var monster = new Monster
            {
                Id = id,
                Name = (string)mon["name"],
                Source = (string)mon["source"],
                Size = MapSize(mon["size"]),
                Alignment = MapAlignment(mon["alignment"]),
                Languages = mon["languages"] is JArray langs ? string.Join(", ", langs.Select(l => T.StripTags((string)l))) : T.StripTags((string)mon["languages"] ?? ""),
            };
            r.Monster = monster;
            MapType(mon["type"], monster);

            // ── Challenge rating ──
            monster.ChallengeRating = (decimal)FiveEToolsCopyResolver.CrToNumber(mon["cr"]);
            if (mon["cr"] is JObject crObj)
            {
                var extra = new List<string>();
                if (crObj["lair"] != null) extra.Add($"{crObj["lair"]} when encountered in its lair");
                if (crObj["coven"] != null) extra.Add($"{crObj["coven"]} when part of a coven");
                if (extra.Count > 0) notes.Add(Kv("Challenge", $"{crObj["cr"]} ({string.Join("; ", extra)})."));
            }

            // ── Hit points ──
            if (mon["hp"] is JObject hp)
            {
                if (hp["average"] != null)
                {
                    monster.HitPoints = (short)(int)hp["average"];
                    monster.HitDice = (string)hp["formula"];
                }
                else if (hp["special"] != null)
                {
                    string special = T.StripTags((string)hp["special"]);
                    var lead = Regex.Match(special, @"^\s*(\d+)");
                    if (lead.Success) monster.HitPoints = short.Parse(lead.Groups[1].Value);
                    notes.Add(Kv("Hit Points", T.CapitalizeFirst(special)));
                }
            }

            MapArmorClass(mon["ac"], r, notes);
            MapSpeeds(mon["speed"], r, notes);
            MapSenses(mon, r);
            MapCharacteristics(mon, r);
            MapSkills(mon["skill"], r);
            MapDamage(mon, r);
            monster.ConditionImmunities = MapConditions(mon["conditionImmune"]);

            // ── Traits ──
            foreach (var t in Entries(mon["trait"]))
                traits.Add(Kv(Name(t), T.Render(t["entries"])));

            foreach (var sc in Entries(mon["spellcasting"]).Where(s => ((string)s["displayAs"] ?? "trait") == "trait"))
                traits.Add(Kv(Name(sc, "Spellcasting"), T.RenderSpellcasting(sc)));

            traits.AddRange(notes);

            foreach (var v in Entries(mon["variant"]))
                traits.Add(Kv("Variant: " + Name(v, "Variant"), T.Render(v["entries"])));

            // ── Actions ──
            int order = 0;
            void AddAction(string name, string text, bool bonus = false, bool reaction = false, bool legendary = false, bool lair = false)
            {
                var action = new Data.Objects.Action
                {
                    Id = Guid.NewGuid(),
                    MonsterId = id,
                    Name = name,
                    Description = text,
                    IsBonus = bonus,
                    IsReaction = reaction,
                    IsLegendary = legendary,
                    IsLair = lair,
                    SortOrder = order++
                };
                FillAttackFields(action, text);
                r.Actions.Add(action);
            }
            string ActionText(JObject a) => (string)a["type"] == "spellcasting" ? T.RenderSpellcasting(a) : T.Render(a["entries"] ?? a["entry"]);

            var spellActions = Entries(mon["spellcasting"]).Where(s => (string)s["displayAs"] == "action").ToList();
            spellActions.ForEach(s => { if (s["type"] == null) s["type"] = "spellcasting"; });

            foreach (var a in Entries(mon["action"]).Concat(spellActions)) AddAction(Name(a, "Action"), ActionText(a));
            foreach (var a in Entries(mon["bonus"])) AddAction(Name(a, "Action"), ActionText(a), bonus: true);
            foreach (var a in Entries(mon["reaction"])) AddAction(Name(a, "Action"), ActionText(a), reaction: true);
            foreach (var a in Entries(mon["legendary"])) AddAction(Name(a, "Action"), ActionText(a), legendary: true);
            foreach (var a in Entries(mon["mythic"])) AddAction(Name(a, "Action") + " (Mythic)", ActionText(a), legendary: true);

            // Legendary actions per round (the stat block writes the intro sentence from it)
            if (Entries(mon["legendary"]).Any())
                monster.LegendaryActionCount = (short)((int?)mon["legendaryActions"] ?? 3);

            // ── Lair actions: intro sentences on the monster, each bullet is a lair action ──
            if (legendaryGroup?["lairActions"] is JArray lairEntries)
            {
                var intro = new List<string>();
                int lairNumber = 0;
                foreach (var part in lairEntries)
                {
                    if (part is JObject lo && (string)lo["type"] == "list")
                    {
                        foreach (var item in (lo["items"] as JArray) ?? new JArray())
                        {
                            if (item is JObject io && io["name"] != null)
                                AddAction(T.StripTags((string)io["name"]).TrimEnd('.', ' '), T.Render(io["entry"] ?? io["entries"]), lair: true);
                            else
                                AddAction("Lair Action " + (++lairNumber), T.Render(item), lair: true);
                        }
                    }
                    else intro.Add(T.Render(part));
                }
                monster.LairActionsIntro = string.Join("\n", intro.Where(s => s.Length > 0));
            }

            // Multiattack is always the first action.
            r.Actions = r.Actions.OrderBy(a => (a.Name ?? "").StartsWith("Multiattack", StringComparison.OrdinalIgnoreCase) ? 0 : 1).ToList();
            for (int i = 0; i < r.Actions.Count; i++) r.Actions[i].SortOrder = i;

            // ── Regional effects: their own text on the monster ──
            if (legendaryGroup?["regionalEffects"] != null)
                monster.RegionalEffects = T.Render(legendaryGroup["regionalEffects"]);

            int saOrder = 0;
            foreach (var t in traits.Where(t => !string.IsNullOrWhiteSpace(t.Value) || !string.IsNullOrWhiteSpace(t.Key)))
            {
                r.SpecialAbilities.Add(new SpecialAbility
                {
                    Id = Guid.NewGuid(),
                    MonsterId = id,
                    Name = t.Key,
                    Description = t.Value,
                    SortOrder = saOrder++
                });
            }

            return r;
        }

        // ── Header ───────────────────────────────────────────────────────────

        private static string MapSize(JToken size)
        {
            string code = size is JArray a ? (string)a.FirstOrDefault() : (string)size;
            return code != null && Sizes.TryGetValue(code, out string full) ? full : "Medium";
        }

        private static void MapType(JToken type, Monster monster)
        {
            if (type == null) { monster.Type = "Humanoid"; return; }
            if (type.Type == JTokenType.String) { monster.Type = T.TitleCase((string)type); return; }

            var o = (JObject)type;
            var inner = o["type"];
            if (inner is JObject choose && choose["choose"] is JArray options)
                monster.Type = T.TitleCase(string.Join(" or ", options.Select(x => (string)x)));
            else
                monster.Type = T.TitleCase((string)inner ?? "Humanoid");

            var tags = ((o["tags"] as JArray) ?? new JArray())
                .Select(t => t.Type == JTokenType.String ? (string)t : ((string)t["prefix"] + " " + (string)t["tag"]).Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(T.TitleCase)
                .ToList();
            if (o["swarmSize"] != null) tags.Insert(0, "Swarm");
            if (tags.Count > 0) monster.Subtype = string.Join(", ", tags);
        }

        /// <summary>5e.tools alignment codes → "Lawful Evil", "Any Non-Good Alignment"... (Parser.alignmentListToFull).</summary>
        private static string MapAlignment(JToken alignment)
        {
            string text = AlignmentListToFull(alignment as JArray);
            if (string.IsNullOrWhiteSpace(text)) return "Unaligned";
            string known = Alignments.All.FirstOrDefault(a => string.Equals(a, text, StringComparison.OrdinalIgnoreCase));
            return known ?? T.TitleCase(text);
        }

        private static string AlignmentAbvToFull(JToken a)
        {
            if (a is JObject o)
            {
                if (o["special"] != null) return (string)o["special"];
                return AlignmentListToFull(o["alignment"] as JArray)
                    + (o["chance"] != null ? $" ({o["chance"]}%)" : "")
                    + (o["note"] != null ? $" ({o["note"]})" : "");
            }
            switch (((string)a ?? "").ToUpperInvariant())
            {
                case "L": return "lawful";
                case "N": return "neutral";
                case "NX": return "neutral (law/chaos axis)";
                case "NY": return "neutral (good/evil axis)";
                case "C": return "chaotic";
                case "G": return "good";
                case "E": return "evil";
                case "U": return "unaligned";
                case "A": return "any alignment";
                default: return (string)a;
            }
        }

        private static string AlignmentListToFull(JArray list)
        {
            if (list == null || list.Count == 0) return "";
            if (list.Any(x => x.Type != JTokenType.String))
                return string.Join(" or ", list.OfType<JObject>()
                    .Select(o => o["special"] != null || o["chance"] != null || o["note"] != null ? AlignmentAbvToFull(o) : AlignmentListToFull(o["alignment"] as JArray)));

            var codes = list.Select(x => (string)x).ToList();
            if (codes.Count == 1) return AlignmentAbvToFull(codes[0]);
            if (codes.Count == 2) return string.Join(" ", codes.Select(c => AlignmentAbvToFull(c)));
            if (codes.Count == 3 && codes.Contains("NX") && codes.Contains("NY") && codes.Contains("N")) return "any neutral alignment";
            if (codes.Count == 5)
            {
                if (!codes.Contains("G")) return "any non-good alignment";
                if (!codes.Contains("E")) return "any non-evil alignment";
                if (!codes.Contains("L")) return "any non-lawful alignment";
                if (!codes.Contains("C")) return "any non-chaotic alignment";
            }
            if (codes.Count == 4)
            {
                if (!codes.Contains("L") && !codes.Contains("NX")) return "any chaotic alignment";
                if (!codes.Contains("G") && !codes.Contains("NY")) return "any evil alignment";
                if (!codes.Contains("C") && !codes.Contains("NX")) return "any lawful alignment";
                if (!codes.Contains("E") && !codes.Contains("NY")) return "any good alignment";
            }
            if (codes.Count == 6) return "any alignment";
            return string.Join(" ", codes.Select(c => AlignmentAbvToFull(c)));
        }

        // ── Armor class / speed / senses ─────────────────────────────────────

        private static void MapArmorClass(JToken ac, MappedMonster r, List<KeyValuePair<string, string>> notes)
        {
            int order = 0;
            foreach (var entry in (ac as JArray) ?? new JArray())
            {
                short value;
                string label;
                if (entry.Type == JTokenType.Integer)
                {
                    value = (short)(int)entry;
                    label = order == 0 ? "Default" : "Alternate";
                }
                else if (entry is JObject o && o["ac"] != null)
                {
                    value = (short)(int)o["ac"];
                    var parts = new List<string>();
                    if (o["from"] is JArray from) parts.Add(string.Join(", ", from.Select(f => T.StripTags((string)f))));
                    if (o["condition"] != null) parts.Add(T.StripTags((string)o["condition"]));
                    label = parts.Count > 0 ? T.CapitalizeFirst(string.Join(" ", parts)) : (order == 0 ? "Default" : "Alternate");
                }
                else
                {
                    if (entry is JObject so && so["special"] != null) notes.Add(Kv("Armor Class", T.CapitalizeFirst(T.StripTags((string)so["special"]))));
                    continue;
                }

                if (order == 0) r.Monster.ArmorClass = value;
                r.ArmorClassEntries.Add(new ArmorClassEntry { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, Label = label, Value = value, SortOrder = order++ });
            }
        }

        private static void MapSpeeds(JToken speedToken, MappedMonster r, List<KeyValuePair<string, string>> notes)
        {
            if (!(speedToken is JObject speed)) return;
            int order = 0;
            bool hasNuance = speed["alternate"] != null || speed["choose"] != null;
            var textParts = new List<string>();
            int? walkValue = SpeedValue(speed["walk"], null, out _);

            foreach (string key in SpeedOrder)
            {
                if (speed[key] == null) continue;
                int? value = SpeedValue(speed[key], walkValue, out string condition);
                if (value == null) continue;
                if (condition != null) hasNuance = true;

                r.Speeds.Add(new Speed { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, SpeedType = T.TitleCase(key), Value = (short)value.Value, SortOrder = order++ });
                if (key == "fly" && (bool?)speed["canHover"] == true)
                    r.Speeds.Add(new Speed { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, SpeedType = "Hover", Value = (short)value.Value, SortOrder = order++ });

                string part = (key == "walk" ? "" : key + " ") + value + " ft." + (condition != null ? " " + condition : "")
                              + (key == "fly" && (bool?)speed["canHover"] == true ? " (hover)" : "");
                textParts.Add(part);
            }

            if (speed["alternate"] is JObject alt)
                foreach (var p in alt.Properties())
                    foreach (var v in (p.Value as JArray) ?? new JArray(p.Value))
                    {
                        int? value = SpeedValue(v, walkValue, out string condition);
                        if (value != null) textParts.Add((p.Name == "walk" ? "" : p.Name + " ") + value + " ft." + (condition != null ? " " + condition : ""));
                    }
            if (speed["choose"] is JObject choose)
                textParts.Add(T.StripTags($"{string.Join(" or ", ((choose["from"] as JArray) ?? new JArray()).Select(x => (string)x))} {choose["amount"]} ft. {choose["note"]}").Trim());

            if (hasNuance) notes.Add(Kv("Speed", string.Join(", ", textParts)));
        }

        private static int? SpeedValue(JToken token, int? walk, out string condition)
        {
            condition = null;
            if (token == null) return null;
            if (token.Type == JTokenType.Integer) return (int)token;
            if (token.Type == JTokenType.Boolean) return (bool)token ? walk : null;
            if (token is JObject o && o["number"] != null)
            {
                if (o["condition"] != null) condition = T.StripTags((string)o["condition"]);
                return (int)o["number"];
            }
            return null;
        }

        private static void MapSenses(JObject mon, MappedMonster r)
        {
            int order = 0;
            var senses = mon["senses"] is JArray a ? a.Select(s => (string)s) : mon["senses"] != null ? new[] { (string)mon["senses"] } : new string[0];
            foreach (string raw in senses)
            {
                string s = T.StripTags(raw);
                var m = Regex.Match(s, @"^(blindsight|darkvision|tremorsense|truesight)\s*(\d+)", RegexOptions.IgnoreCase);
                if (m.Success)
                    r.Senses.Add(new Sense { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, SenseType = T.TitleCase(m.Groups[1].Value), Value = short.Parse(m.Groups[2].Value), SortOrder = order++ });
                else
                    r.Warnings.Add("Sense not mapped: " + s);
            }

            var passive = Regex.Match((string)mon["passive"] ?? "", @"\d+");
            if (passive.Success)
                r.Senses.Add(new Sense { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, SenseType = "Passive Perception", Value = short.Parse(passive.Value), SortOrder = order++ });
        }

        // ── Characteristics / skills ─────────────────────────────────────────

        private static void MapCharacteristics(JObject mon, MappedMonster r)
        {
            var saves = mon["save"] as JObject;
            foreach (var kv in AbilityIds)
            {
                short score = mon[kv.Key] != null && mon[kv.Key].Type == JTokenType.Integer ? (short)(int)mon[kv.Key] : (short)10;
                short save = 0;
                var sm = Regex.Match((string)saves?[kv.Key] ?? "", @"[+-]?\d+");
                if (sm.Success) save = short.Parse(sm.Value);
                r.Characteristics.Add(new Characteristic
                {
                    Id = Guid.NewGuid(),
                    MonsterId = r.Monster.Id,
                    CharacteristicTypeId = kv.Value,
                    Score = score,
                    Save = save
                });
            }
        }

        private static void MapSkills(JToken skillToken, MappedMonster r)
        {
            if (!(skillToken is JObject skills)) return;
            foreach (var p in skills.Properties())
            {
                if (!SkillIds.TryGetValue(p.Name, out Guid skillId)) continue; // "other" / "special"
                var m = Regex.Match((string)p.Value ?? "", @"[+-]?\d+");
                if (!m.Success) continue;
                r.Skills.Add(new Skill { Id = Guid.NewGuid(), MonsterId = r.Monster.Id, SkillTypeId = skillId, Save = short.Parse(m.Value) });
            }
        }

        // ── Damage modifiers / conditions ────────────────────────────────────

        private static void MapDamage(JObject mon, MappedMonster r)
        {
            DamageModifierDefaults.GetDefaultDice(r.Monster.ChallengeRating, out short diceCount, out short diceSize);
            int order = 0;

            foreach (var spec in new[]
            {
                new { Prop = "vulnerable", Type = "Vulnerability" },
                new { Prop = "resist", Type = "Resistance" },
                new { Prop = "immune", Type = "Immunity" },
            })
            {
                if (!(mon[spec.Prop] is JArray list)) continue;

                var damageTypes = new List<string>();
                CollectDamage(list, spec.Prop, damageTypes);

                bool immune = spec.Type == "Immunity";
                foreach (string dt in damageTypes.Distinct())
                {
                    if (CanonicalDamageType(dt) == null) r.Warnings.Add("Damage type not in the list: " + dt);
                    r.DamageModifiers.Add(new DamageModifier
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = r.Monster.Id,
                        DamageType = CanonicalDamageType(dt) ?? dt,
                        ModifierType = spec.Type,
                        DiceCount = immune ? (short)0 : diceCount,
                        DiceSize = immune ? (short)0 : diceSize,
                        SortOrder = order++
                    });
                }
            }
        }

        /// <summary>
        /// Turns a resist / immune / vulnerable list into damage type names. Qualified entries become their own
        /// damage type, named like the existing one: "Bludgeoning, Piercing, and Slashing from Nonmagical Attacks
        /// that aren't Silvered", "Bludgeoning, Piercing, and Slashing while in Dim Light or Darkness"...
        /// </summary>
        private static void CollectDamage(JArray list, string prop, List<string> damageTypes)
        {
            foreach (var item in list)
            {
                if (item.Type == JTokenType.String)
                {
                    damageTypes.Add(CanonicalDamageType((string)item) ?? T.TitleCase((string)item));
                    continue;
                }

                if (!(item is JObject o)) continue;
                if (o["special"] != null) { damageTypes.Add(T.CapitalizeFirst(StyleDamageText(T.StripTags((string)o["special"])))); continue; }

                var inner = (o[prop] as JArray) ?? new JArray();
                string note = T.StripTags((string)o["note"] ?? "").Trim().ToLowerInvariant();
                string preNote = T.StripTags((string)o["preNote"] ?? "").Trim().ToLowerInvariant();

                // Item-granted ("While wearing the Ring of Winter:", "(granted by the blast scepter...)", "(from stoneskin)"):
                // the NPC has the item, so these are plain entries.
                note = Regex.Replace(note, @"\([^)]*\)", "").Trim();
                if (preNote.StartsWith("while wearing")) preNote = "";

                if (preNote == "nonmagical") { preNote = ""; note = "from nonmagical attacks"; }
                if (note == "from") note = "from nonmagical attacks"; // truncated in the data

                var names = inner.Where(x => x.Type == JTokenType.String)
                                 .Select(x => CanonicalDamageType((string)x) ?? T.TitleCase((string)x)).ToList();

                if (preNote.StartsWith("one of the following"))
                {
                    damageTypes.Add("One of: " + T.JoinConjunct(names, "or"));
                    continue;
                }

                if (note.Length == 0 && preNote.Length == 0)
                {
                    CollectDamage(inner, prop, damageTypes); // plain grouping
                    continue;
                }

                // Equivalent wordings → one name
                note = note.Replace("damage from", "from")
                           .Replace("from nonmagical weapons", "from nonmagical attacks")
                           .Replace("not made with silvered weapons", "that aren't silvered")
                           .Replace("not made with adamantine weapons", "that aren't adamantine");
                if (note.StartsWith("that ") || note.StartsWith("not made with")) note = "from nonmagical attacks " + note;

                damageTypes.Add(T.JoinConjunct(names) + " " + StyleDamageText(note));
                CollectDamage(new JArray(inner.OfType<JObject>()), prop, damageTypes); // nested qualified entries
            }
        }

        private static readonly HashSet<string> LowerWords = new HashSet<string>
            { "from", "that", "aren't", "and", "or", "with", "by", "in", "not", "made", "of", "while", "the" };

        /// <summary>"from nonmagical attacks that aren't silvered" → "from Nonmagical Attacks that aren't Silvered".</summary>
        private static string StyleDamageText(string s)
        {
            var words = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(w => LowerWords.Contains(w.ToLowerInvariant()) ? w.ToLowerInvariant() : T.CapitalizeFirst(w));
            return string.Join(" ", words);
        }

        private static string CanonicalDamageType(string s)
            => DamageTypes.All.FirstOrDefault(d => string.Equals(d, (s ?? "").Trim(), StringComparison.OrdinalIgnoreCase));

        private static string MapConditions(JToken token)
        {
            var names = new List<string>();
            Action<JArray> collect = null;
            collect = arr =>
            {
                foreach (var item in arr)
                {
                    if (item.Type == JTokenType.String) names.Add(T.TitleCase(T.StripTags((string)item)));
                    else if (item is JObject o && o["conditionImmune"] is JArray inner) collect(inner);
                }
            };
            if (token is JArray list) collect(list);
            return string.Join(", ", names.Distinct());
        }

        // ── Actions ──────────────────────────────────────────────────────────

        /// <summary>Fills attack bonus, range and damage from the rendered description (used by the stat block tools).</summary>
        private static void FillAttackFields(Data.Objects.Action action, string text)
        {
            string t = (text ?? "").Replace('−', '-').Replace('–', '-');

            var atk = Regex.Match(t, @"\b(Melee or Ranged|Melee|Ranged)\s+(?:Weapon\s+|Spell\s+)?Attack(?:\s+Roll)?:\s*([+-]?\d+)", RegexOptions.IgnoreCase);
            if (atk.Success)
            {
                action.AttackBonus = short.Parse(atk.Groups[2].Value);
                action.OverrideAttackBonus = true;

                string kind = atk.Groups[1].Value.ToLowerInvariant();
                var reach = Regex.Match(t, @"reach\s+(\d+\s*ft\.?)", RegexOptions.IgnoreCase);
                var range = Regex.Match(t, @"range\s+(\d+(?:/\d+)?\s*ft\.?)", RegexOptions.IgnoreCase);
                if (kind == "melee" && reach.Success) action.Range = "Melee (" + reach.Groups[1].Value + ")";
                else if (kind == "ranged" && range.Success) action.Range = "Ranged (" + range.Groups[1].Value + ")";
                else if (kind == "melee or ranged" && reach.Success && range.Success)
                    action.Range = "Melee or Ranged (" + reach.Groups[1].Value + " or " + range.Groups[1].Value + ")";
            }

            var hit = Regex.Match(t, @"Hit:\s*\d+\s*\((\d+d\d+)(?:\s*([+-])\s*(\d+))?\)\s+([A-Za-z]+)\s+damage", RegexOptions.IgnoreCase);
            if (hit.Success)
            {
                action.DamageDice = hit.Groups[1].Value;
                if (hit.Groups[3].Success)
                    action.DamageBonus = (short)((hit.Groups[2].Value == "-" ? -1 : 1) * int.Parse(hit.Groups[3].Value));
                action.DamageType = CanonicalDamageType(hit.Groups[4].Value);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static IEnumerable<JObject> Entries(JToken token) => ((token as JArray) ?? new JArray()).OfType<JObject>();

        private static string Name(JObject o, string fallback = "")
        {
            string n = T.StripTags((string)o["name"] ?? "").Trim();
            return n.Length > 0 ? n : fallback;
        }

        private static KeyValuePair<string, string> Kv(string k, string v) => new KeyValuePair<string, string>(k, v);
    }
}
