using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseCreationTool.FiveETools
{
    /// <summary>
    /// Resolves 5e.tools "_copy" entries (a monster defined as "monster X with these modifications"),
    /// including "_trait" / "_templates" templates from traits.json.
    /// Port of DataUtil.generic.copyApplier from the 5e.tools source.
    /// </summary>
    public class FiveEToolsCopyResolver
    {
        // Properties never inherited from the base unless "_preserve" asks for them.
        private static readonly HashSet<string> PreserveOnly = new HashSet<string>
        {
            "page", "otherSources", "referenceSources", "srd", "srd52", "basicRules", "basicRules2024",
            "reprintedAs", "hasFluff", "hasFluffImages", "hasToken", "tokenCredit",
            // monster specific
            "legendaryGroup", "environment", "soundClip", "altArt", "variant", "dragonCastingColor", "familiar",
        };

        // Properties targeted by the "*" modification key.
        private static readonly string[] CopyEntryProps =
        {
            "action", "bonus", "reaction", "trait", "legendary", "mythic", "variant", "spellcasting",
            "actionHeader", "bonusHeader", "reactionHeader", "legendaryHeader", "mythicHeader",
        };

        private static readonly Dictionary<string, string> SkillToAbility = new Dictionary<string, string>
        {
            ["athletics"] = "str",
            ["acrobatics"] = "dex", ["sleight of hand"] = "dex", ["stealth"] = "dex",
            ["arcana"] = "int", ["history"] = "int", ["investigation"] = "int", ["nature"] = "int", ["religion"] = "int",
            ["animal handling"] = "wis", ["insight"] = "wis", ["medicine"] = "wis", ["perception"] = "wis", ["survival"] = "wis",
            ["deception"] = "cha", ["intimidation"] = "cha", ["performance"] = "cha", ["persuasion"] = "cha",
        };

        private static readonly string[] SizeOrder = { "F", "D", "T", "S", "M", "L", "H", "G", "C", "V" };

        private readonly Dictionary<string, JObject> _byKey;
        private readonly List<JObject> _templates;
        private readonly Dictionary<string, JObject> _resolved = new Dictionary<string, JObject>();
        private readonly HashSet<string> _inProgress = new HashSet<string>();

        /// <summary>Problems met while resolving (unknown modes, missing items...). The monster is still imported.</summary>
        public List<string> Warnings { get; } = new List<string>();

        public FiveEToolsCopyResolver(IEnumerable<JObject> entities, IEnumerable<JObject> templates)
        {
            _byKey = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            foreach (var e in entities)
                _byKey[Key((string)e["name"], (string)e["source"])] = e;
            _templates = templates?.ToList() ?? new List<JObject>();
        }

        public static string Key(string name, string source) => (name ?? "").Trim() + "|" + (source ?? "").Trim();

        /// <summary>Returns the fully resolved entity (a new object when it is a copy).</summary>
        public JObject Resolve(JObject entity)
        {
            if (entity["_copy"] == null) return entity;

            string key = Key((string)entity["name"], (string)entity["source"]);
            if (_resolved.TryGetValue(key, out JObject done)) return done;
            if (!_inProgress.Add(key)) throw new InvalidOperationException("Circular _copy on " + key);

            try
            {
                var copyMeta = (JObject)entity["_copy"];
                string baseKey = Key((string)copyMeta["name"], (string)copyMeta["source"]);
                if (!_byKey.TryGetValue(baseKey, out JObject baseRaw))
                    throw new InvalidOperationException("Base monster not found: " + baseKey);

                JObject copyFrom = (JObject)Resolve(baseRaw).DeepClone();
                JObject copyTo = (JObject)entity.DeepClone();
                ApplyCopy(copyFrom, copyTo);
                _resolved[key] = copyTo;
                return copyTo;
            }
            finally
            {
                _inProgress.Remove(key);
            }
        }

        private void ApplyCopy(JObject copyFrom, JObject copyTo)
        {
            string who = $"{copyTo["name"]} ({copyTo["source"]})";
            var copyMeta = (JObject)copyTo["_copy"];
            JObject mods = NormaliseMods(copyMeta["_mod"] as JObject);

            // Templates: old format "_trait": {name, source}; new format "_templates": [...]
            var templateRefs = new List<JObject>();
            if (copyMeta["_trait"] is JObject t) templateRefs.Add(t);
            if (copyMeta["_templates"] is JArray ts) templateRefs.AddRange(ts.OfType<JObject>());

            var templates = new List<JObject>();
            foreach (var tr in templateRefs)
            {
                var template = _templates.FirstOrDefault(x =>
                    string.Equals((string)x["name"], (string)tr["name"], StringComparison.OrdinalIgnoreCase) &&
                    string.Equals((string)x["source"], (string)tr["source"], StringComparison.OrdinalIgnoreCase));
                if (template == null)
                {
                    Warnings.Add($"{who}: template \"{tr["name"]}\" not found.");
                    continue;
                }
                template = (JObject)template.DeepClone();
                templates.Add(template);

                var tMods = NormaliseMods(template["apply"]?["_mod"] as JObject);
                if (tMods == null) continue;
                if (mods == null) mods = new JObject();
                foreach (var p in tMods.Properties())
                {
                    if (mods[p.Name] is JArray existing) foreach (var v in (JArray)p.Value) existing.Add(v);
                    else mods[p.Name] = p.Value;
                }
            }

            var rootProps = new HashSet<string>(copyTo.Properties().Select(p => p.Name));
            var preserve = copyMeta["_preserve"] as JObject;

            // Copy over the base values the copy does not define.
            foreach (var p in copyFrom.Properties())
            {
                var existing = copyTo[p.Name];
                if (existing != null && existing.Type == JTokenType.Null) { copyTo.Remove(p.Name); continue; }
                if (existing != null) continue;

                if (PreserveOnly.Contains(p.Name))
                {
                    if (preserve != null && (preserve["*"] != null || preserve[p.Name] != null))
                        copyTo[p.Name] = p.Value.DeepClone();
                }
                else copyTo[p.Name] = p.Value.DeepClone();
            }

            // Template root values (never overwrite the copy's own root properties).
            foreach (var template in templates)
            {
                if (!(template["apply"]?["_root"] is JObject root)) continue;
                foreach (var p in root.Properties().Where(p => !rootProps.Contains(p.Name)))
                    copyTo[p.Name] = p.Value.DeepClone();
            }

            // Modifications: normal props first, then "_", then "*".
            if (mods != null)
            {
                foreach (var p in mods.Properties().ToList())
                    p.Value = ResolveVariables(p.Value, copyTo);

                foreach (var p in mods.Properties().OrderBy(p => p.Name == "_" ? 1 : p.Name == "*" ? 2 : 0))
                {
                    var modInfos = (JArray)p.Value;
                    try
                    {
                        if (p.Name == "*")
                            foreach (string prop in CopyEntryProps) HandleProp(copyTo, modInfos, prop, who);
                        else if (p.Name == "_")
                            HandleProp(copyTo, modInfos, null, who);
                        else
                            HandleProp(copyTo, modInfos, p.Name, who);
                    }
                    catch (Exception ex)
                    {
                        Warnings.Add($"{who}: modification on \"{p.Name}\" failed — {ex.Message}");
                    }
                }
            }

            copyTo.Remove("_copy");
        }

        private static JObject NormaliseMods(JObject mods)
        {
            if (mods == null) return null;
            foreach (var p in mods.Properties().ToList())
                if (!(p.Value is JArray)) p.Value = new JArray(p.Value);
            return mods;
        }

        // ── Modification modes ───────────────────────────────────────────────

        private void HandleProp(JObject copyTo, JArray modInfos, string prop, string who)
        {
            string[] path = prop?.Split('.');

            foreach (var modInfo in modInfos)
            {
                if (modInfo.Type == JTokenType.String)
                {
                    if ((string)modInfo == "remove" && path != null) RemovePath(copyTo, path);
                    else Warnings.Add($"{who}: unknown mode \"{modInfo}\".");
                    continue;
                }

                var mi = (JObject)modInfo;
                string mode = (string)mi["mode"];
                switch (mode)
                {
                    case "appendStr":
                    {
                        string existing = (string)GetPath(copyTo, path);
                        SetPath(copyTo, path, existing != null ? existing + ((string)mi["joiner"] ?? "") + (string)mi["str"] : (string)mi["str"]);
                        break;
                    }
                    case "replaceName": ReplaceText(copyTo, path, mi, onlyNames: true); break;
                    case "replaceTxt": ReplaceText(copyTo, path, mi, onlyNames: false); break;
                    case "prependArr":
                    {
                        var items = AsArray(mi["items"]);
                        var existing = GetPath(copyTo, path) as JArray;
                        SetPath(copyTo, path, existing != null ? new JArray(items.Concat(existing)) : items);
                        break;
                    }
                    case "appendArr":
                    {
                        var items = AsArray(mi["items"]);
                        var existing = GetPath(copyTo, path) as JArray;
                        SetPath(copyTo, path, existing != null ? new JArray(existing.Concat(items)) : items);
                        break;
                    }
                    case "appendIfNotExistsArr":
                    {
                        var items = AsArray(mi["items"]);
                        var existing = GetPath(copyTo, path) as JArray;
                        if (existing == null) SetPath(copyTo, path, items);
                        else foreach (var it in items) if (!existing.Any(x => JToken.DeepEquals(x, it))) existing.Add(it);
                        break;
                    }
                    case "replaceArr":
                        if (!ReplaceArr(copyTo, path, mi)) Warnings.Add($"{who}: replaceArr on \"{prop}\" found nothing to replace ({mi["replace"]}).");
                        break;
                    case "replaceOrAppendArr":
                        if (!ReplaceArr(copyTo, path, mi))
                        {
                            var existing = GetPath(copyTo, path) as JArray;
                            var items = AsArray(mi["items"]);
                            SetPath(copyTo, path, existing != null ? new JArray(existing.Concat(items)) : items);
                        }
                        break;
                    case "insertArr":
                    {
                        var existing = (JArray)GetPath(copyTo, path);
                        int index = (int)mi["index"];
                        if (index < 0 || index > existing.Count) index = existing.Count;
                        foreach (var it in AsArray(mi["items"]).Reverse()) existing.Insert(index, it);
                        break;
                    }
                    case "removeArr":
                    {
                        var existing = GetPath(copyTo, path) as JArray;
                        if (existing == null) break;
                        if (mi["names"] != null)
                            foreach (var n in AsArray(mi["names"]))
                            {
                                var hit = existing.FirstOrDefault(x => x is JObject xo && (string)xo["name"] == (string)n);
                                if (hit != null) hit.Remove();
                                else if ((bool?)mi["force"] != true) Warnings.Add($"{who}: removeArr could not find \"{n}\" in \"{prop}\".");
                            }
                        else
                            foreach (var item in AsArray(mi["items"]))
                            {
                                var hit = existing.FirstOrDefault(x => JToken.DeepEquals(x, item));
                                if (hit != null) hit.Remove();
                            }
                        break;
                    }
                    case "renameArr":
                    {
                        var existing = GetPath(copyTo, path) as JArray;
                        if (existing == null) break;
                        foreach (var r in AsArray(mi["renames"]))
                        {
                            var ent = existing.OfType<JObject>().FirstOrDefault(x => (string)x["name"] == (string)r["rename"]);
                            if (ent != null) ent["name"] = r["with"];
                        }
                        break;
                    }
                    case "calculateProp":
                    {
                        string formula = Regex.Replace((string)mi["formula"], @"<\$([^$]+)\$>", m =>
                            m.Groups[1].Value == "prof_bonus" ? CrToPb(copyTo["cr"]).ToString(CultureInfo.InvariantCulture)
                            : m.Groups[1].Value == "dex_mod" ? AbilityMod(copyTo, "dex").ToString(CultureInfo.InvariantCulture) : "0");
                        var target = GetOrCreateObject(copyTo, path);
                        target[(string)mi["prop"]] = (int)Math.Floor(Evaluate(formula));
                        break;
                    }
                    case "scalarAddProp":
                    case "scalarMultProp":
                    {
                        if (!(GetPath(copyTo, path) is JObject target)) break;
                        double scalar = (double)mi["scalar"];
                        bool floor = (bool?)mi["floor"] == true;
                        string p = (string)mi["prop"];
                        foreach (var k in (p == "*" ? target.Properties().Select(x => x.Name).ToList() : new List<string> { p }))
                        {
                            if (target[k] == null) continue;
                            bool isString = target[k].Type == JTokenType.String;
                            if (!double.TryParse((string)target[k], NumberStyles.Any, CultureInfo.InvariantCulture, out double v)) continue;
                            double outV = mode == "scalarAddProp" ? v + scalar : v * scalar;
                            if (floor || mode == "scalarAddProp") outV = Math.Floor(outV);
                            target[k] = isString ? (outV >= 0 ? "+" : "") + outV.ToString(CultureInfo.InvariantCulture) : (JToken)outV;
                        }
                        break;
                    }
                    case "setProp":
                    {
                        var full = new List<string>();
                        if (path != null && !(path.Length == 1 && path[0] == "*")) full.AddRange(path);
                        if (mi["prop"] != null) full.AddRange(((string)mi["prop"]).Split('.'));
                        SetPath(copyTo, full.ToArray(), mi["value"].DeepClone());
                        break;
                    }
                    case "prefixSuffixStringProp":
                    {
                        var full = new List<string>();
                        if (path != null && !(path.Length == 1 && path[0] == "*")) full.AddRange(path);
                        if (mi["prop"] != null) full.AddRange(((string)mi["prop"]).Split('.'));
                        if (GetPath(copyTo, full.ToArray()) is JValue sv && sv.Type == JTokenType.String)
                            SetPath(copyTo, full.ToArray(), ((string)mi["prefix"] ?? "") + (string)sv + ((string)mi["suffix"] ?? ""));
                        break;
                    }

                    // ── Bestiary specific ──
                    case "addSenses":
                    {
                        if (!(copyTo["senses"] is JArray senses)) copyTo["senses"] = senses = new JArray();
                        foreach (var sense in AsArray(mi["senses"]))
                        {
                            string type = (string)sense["type"];
                            int range = (int)sense["range"];
                            bool found = false;
                            for (int i = 0; i < senses.Count; i++)
                            {
                                var m = Regex.Match((string)senses[i], Regex.Escape(type) + @" (\d+)", RegexOptions.IgnoreCase);
                                if (!m.Success) continue;
                                found = true;
                                if (int.Parse(m.Groups[1].Value) < range) senses[i] = $"{type} {range} ft.";
                                break;
                            }
                            if (!found) senses.Add($"{type} {range} ft.");
                        }
                        break;
                    }
                    case "addSaves":
                        AddProficiencies(copyTo, "save", (JObject)mi["saves"], k => k);
                        break;
                    case "addAllSaves":
                        AddProficiencies(copyTo, "save", new JObject(new[] { "str", "dex", "con", "int", "wis", "cha" }.Select(a => new JProperty(a, mi["saves"]))), k => k);
                        break;
                    case "addSkills":
                        AddProficiencies(copyTo, "skill", (JObject)mi["skills"], k => SkillToAbility.TryGetValue(k, out string a) ? a : "int");
                        break;
                    case "addAllSkills":
                        AddProficiencies(copyTo, "skill", new JObject(SkillToAbility.Keys.Select(s => new JProperty(s, mi["skills"]))), k => SkillToAbility[k]);
                        break;
                    case "addSpells": AddSpells(copyTo, mi); break;
                    case "replaceSpells": ReplaceSpells(copyTo, mi, who); break;
                    case "removeSpells": RemoveSpells(copyTo, mi); break;
                    case "maxSize":
                    {
                        var sizes = AsArray(copyTo["size"]).Select(s => (string)s).ToList();
                        int max = Array.IndexOf(SizeOrder, (string)mi["max"]);
                        var kept = sizes.Where(s => Array.IndexOf(SizeOrder, s) <= max).ToList();
                        if (kept.Count == 0) kept.Add((string)mi["max"]);
                        copyTo["size"] = new JArray(kept);
                        break;
                    }
                    case "scalarMultXp":
                        break; // XP is not stored in DM Tools
                    case "scalarAddHit":
                        WalkStrings(copyTo, path, s => Regex.Replace(s, @"\{@hit ([-+]?\d+)\}", m => "{@hit " + (int.Parse(m.Groups[1].Value) + (int)mi["scalar"]) + "}"));
                        break;
                    case "scalarAddDc":
                        WalkStrings(copyTo, path, s => Regex.Replace(s, @"\{@dc (\d+)(?:\|[^}]+)?\}", m => "{@dc " + (int.Parse(m.Groups[1].Value) + (int)mi["scalar"]) + "}"));
                        break;
                    default:
                        Warnings.Add($"{who}: unsupported mode \"{mode}\" on \"{prop}\".");
                        break;
                }
            }
        }

        private void ReplaceText(JObject copyTo, string[] path, JObject mi, bool onlyNames)
        {
            if (!(GetPath(copyTo, path) is JArray ents)) return;

            var options = RegexOptions.None;
            string flags = (string)mi["flags"] ?? "";
            if (flags.Contains("i")) options |= RegexOptions.IgnoreCase;
            var re = new Regex((string)mi["replace"], options);
            string with = ConvertReplacement((string)mi["with"] ?? "");
            bool tagInsensitive = (bool?)mi["tagInsensitive"] == true;
            Func<string, string> replace = s => tagInsensitive ? re.Replace(s, with) : ReplaceOutsideTags(s, re, with);

            if (onlyNames)
            {
                foreach (var ent in ents.OfType<JObject>())
                    if (ent["name"] != null) ent["name"] = WalkToken(ent["name"], replace);
                return;
            }

            var props = mi["props"] is JArray pa ? pa.Select(p => p.Type == JTokenType.Null ? null : (string)p).ToList()
                                                  : new List<string> { null, "entries", "headerEntries", "footerEntries" };

            if (props.Contains(null))
                for (int i = 0; i < ents.Count; i++)
                    if (ents[i].Type == JTokenType.String) ents[i] = replace((string)ents[i]);

            foreach (var ent in ents.OfType<JObject>())
                foreach (var prop in props.Where(p => p != null))
                    if (ent[prop] != null) ent[prop] = WalkToken(ent[prop], replace);
        }

        /// <summary>JS replacement syntax → .NET ("$&amp;" works in both; "$$" stays a literal "$").</summary>
        private static string ConvertReplacement(string with) => with;

        private static string ReplaceOutsideTags(string s, Regex re, string with)
        {
            var parts = Regex.Split(s, @"(\{@[^{}]*\})");
            for (int i = 0; i < parts.Length; i++)
                if (!parts[i].StartsWith("{@")) parts[i] = re.Replace(parts[i], with);
            return string.Concat(parts);
        }

        private static JToken WalkToken(JToken token, Func<string, string> fn)
        {
            switch (token.Type)
            {
                case JTokenType.String: return fn((string)token);
                case JTokenType.Array:
                    var a = (JArray)token;
                    for (int i = 0; i < a.Count; i++) a[i] = WalkToken(a[i], fn);
                    return a;
                case JTokenType.Object:
                    foreach (var p in ((JObject)token).Properties().ToList()) p.Value = WalkToken(p.Value, fn);
                    return token;
                default: return token;
            }
        }

        private static void WalkStrings(JObject copyTo, string[] path, Func<string, string> fn)
        {
            var existing = GetPath(copyTo, path);
            if (existing != null) SetPath(copyTo, path, WalkToken(existing, fn));
        }

        private bool ReplaceArr(JObject copyTo, string[] path, JObject mi)
        {
            if (!(GetPath(copyTo, path) is JArray existing)) return false;
            var items = AsArray(mi["items"]);
            var replace = mi["replace"];

            int ix = -1;
            if (replace is JObject ro && ro["regex"] != null)
            {
                var re = new Regex((string)ro["regex"], ((string)ro["flags"] ?? "").Contains("i") ? RegexOptions.IgnoreCase : RegexOptions.None);
                ix = existing.ToList().FindIndex(x => x is JObject xo && xo["name"] != null ? re.IsMatch((string)xo["name"]) : x.Type == JTokenType.String && re.IsMatch((string)x));
            }
            else if (replace is JObject ri && ri["index"] != null) ix = (int)ri["index"];
            else
            {
                string name = (string)replace;
                ix = existing.ToList().FindIndex(x => x is JObject xo && xo["name"] != null ? (string)xo["name"] == name : x.Type == JTokenType.String && (string)x == name);
            }

            if (ix < 0 || ix >= existing.Count) return false;
            existing.RemoveAt(ix);
            foreach (var it in items.Reverse()) existing.Insert(ix, it);
            return true;
        }

        private static void AddProficiencies(JObject copyTo, string prop, JObject entries, Func<string, string> abilityOf)
        {
            if (!(copyTo[prop] is JObject target)) copyTo[prop] = target = new JObject();
            int pb = CrToPb(copyTo["cr"]);
            foreach (var p in entries.Properties())
            {
                int total = (int)p.Value * pb + AbilityMod(copyTo, abilityOf(p.Name));
                string asText = total >= 0 ? "+" + total : total.ToString();
                if (target[p.Name] != null && int.TryParse(((string)target[p.Name]).Replace("+", ""), out int cur) && cur >= total) continue;
                target[p.Name] = asText;
            }
        }

        private static readonly string[] SpellUseProps = { "recharge", "legendary", "charges", "rest", "restLong", "daily", "weekly", "monthly", "yearly" };

        private void AddSpells(JObject copyTo, JObject mi)
        {
            if (!(copyTo["spellcasting"] is JArray scs) || scs.Count == 0) throw new InvalidOperationException("no spellcasting to add spells to");
            var sc = mi["name"] != null ? scs.OfType<JObject>().FirstOrDefault(x => (string)x["name"] == (string)mi["name"]) : (JObject)scs[0];
            if (sc == null) throw new InvalidOperationException("spellcasting \"" + mi["name"] + "\" not found");

            if (mi["spells"] is JObject newSpells)
            {
                if (!(sc["spells"] is JObject spells)) sc["spells"] = spells = new JObject();
                foreach (var lvl in newSpells.Properties())
                {
                    if (!(spells[lvl.Name] is JObject oldLvl)) { spells[lvl.Name] = lvl.Value.DeepClone(); continue; }
                    foreach (var kk in ((JObject)lvl.Value).Properties())
                    {
                        if (oldLvl[kk.Name] is JArray oldArr && kk.Value is JArray newArr)
                            oldLvl[kk.Name] = new JArray(oldArr.Concat(newArr).OrderBy(x => (string)x, StringComparer.OrdinalIgnoreCase));
                        else oldLvl[kk.Name] = kk.Value.DeepClone();
                    }
                }
            }

            foreach (var prop in new[] { "constant", "will", "ritual" })
            {
                if (!(mi[prop] is JArray add)) continue;
                if (!(sc[prop] is JArray arr)) sc[prop] = arr = new JArray();
                foreach (var s in add) arr.Add(s.DeepClone());
            }

            foreach (var prop in SpellUseProps)
            {
                if (!(mi[prop] is JObject add)) continue;
                if (!(sc[prop] is JObject target)) sc[prop] = target = new JObject();
                foreach (var k in add.Properties())
                {
                    if (!(target[k.Name] is JArray arr)) target[k.Name] = arr = new JArray();
                    foreach (var s in (JArray)k.Value) arr.Add(s.DeepClone());
                }
            }
        }

        private void ReplaceSpells(JObject copyTo, JObject mi, string who)
        {
            if (!(copyTo["spellcasting"] is JArray scs) || scs.Count == 0) throw new InvalidOperationException("no spellcasting to replace spells in");
            var sc = (JObject)scs[0];

            Action<JObject, string, JObject> handle = (container, key, meta) =>
            {
                if (!(container?[key] is JArray list)) return;
                string toReplace = (string)meta["replace"];
                var with = AsArray(meta["with"]);
                int ix = list.ToList().FindIndex(x => (string)x == toReplace);
                if (ix < 0) { Warnings.Add($"{who}: spell \"{toReplace}\" not found to replace."); return; }
                list.RemoveAt(ix);
                foreach (var w in with.Reverse()) list.Insert(ix, w);
                container[key] = new JArray(list.OrderBy(x => (string)x, StringComparer.OrdinalIgnoreCase));
            };

            if (mi["spells"] is JObject lvls)
                foreach (var lvl in lvls.Properties())
                    if (sc["spells"]?[lvl.Name] is JObject lvlObj)
                        foreach (var meta in AsArray(lvl.Value).OfType<JObject>()) handle(lvlObj, "spells", meta);

            if (mi["daily"] is JObject daily)
                foreach (var k in daily.Properties())
                    foreach (var meta in AsArray(k.Value).OfType<JObject>()) handle(sc["daily"] as JObject, k.Name, meta);
        }

        private static void RemoveSpells(JObject copyTo, JObject mi)
        {
            if (!(copyTo["spellcasting"] is JArray scs) || scs.Count == 0) return;
            var sc = (JObject)scs[0];
            Func<JArray, JArray, JArray> without = (list, remove) => new JArray(list.Where(x => !remove.Any(r => JToken.DeepEquals(r, x))));

            if (mi["spells"] is JObject lvls)
                foreach (var lvl in lvls.Properties())
                    if (sc["spells"]?[lvl.Name]?["spells"] is JArray list)
                        sc["spells"][lvl.Name]["spells"] = without(list, AsArray(lvl.Value));

            foreach (var prop in new[] { "constant", "will", "ritual" })
                if (mi[prop] is JArray rm && sc[prop] is JArray list) sc[prop] = without(list, rm);

            foreach (var prop in SpellUseProps)
                if (mi[prop] is JObject rm && sc[prop] is JObject target)
                    foreach (var k in rm.Properties())
                        if (target[k.Name] is JArray list) target[k.Name] = without(list, AsArray(k.Value));
        }

        // ── Variables: <$name$>, <$short_name$>, <$spell_dc__cha$>, <$to_hit__str$>... ──

        private static JToken ResolveVariables(JToken token, JObject ent)
            => WalkToken(token.DeepClone(), s => Regex.Replace(s, @"<\$([^$]+)\$>", m => ResolveVariable(m.Groups[1].Value, ent)));

        private static string ResolveVariable(string variable, JObject ent)
        {
            string[] parts = variable.Split(new[] { "__" }, 2, StringSplitOptions.None);
            string mode = parts[0];
            string detail = parts.Length > 1 ? parts[1] : "";

            switch (mode)
            {
                case "name": return (string)ent["name"];
                case "short_name": return ShortName(ent, false);
                case "title_short_name": return ShortName(ent, true);
                case "title_name": return FiveEToolsText.TitleCase((string)ent["name"]); // old data: "Sylvira Savikas can innately cast..."
                case "dc":
                case "spell_dc": return (8 + AbilityMod(ent, detail) + CrToPb(ent["cr"])).ToString();
                case "to_hit":
                {
                    int total = CrToPb(ent["cr"]) + AbilityMod(ent, detail);
                    return total >= 0 ? "+" + total : total.ToString();
                }
                case "damage_mod":
                {
                    int total = AbilityMod(ent, detail);
                    return total == 0 ? "" : total > 0 ? " + " + total : " - " + Math.Abs(total);
                }
                case "damage_avg":
                {
                    string expr = Regex.Replace(detail, @"\b(str|dex|con|int|wis|cha)\b", m => AbilityMod(ent, m.Value).ToString(), RegexOptions.IgnoreCase);
                    expr = Regex.Replace(expr, @"\bsize_mult\b", SizeMult(ent).ToString());
                    return ((int)Math.Floor(Evaluate(expr))).ToString();
                }
                case "size_mult":
                    return detail.Length == 0 ? SizeMult(ent).ToString() : ((int)Math.Floor(SizeMult(ent) * Evaluate(detail))).ToString();
                default:
                    return "<$" + variable + "$>";
            }
        }

        /// <summary>5e.tools Renderer.monster.getShortName: "the aarakocra", "The adult red dragon" → "the dragon"...</summary>
        public static string ShortName(JObject mon, bool titleCase)
        {
            bool named = (bool?)mon["isNamedCreature"] == true;
            string prefix = named ? "" : titleCase ? "The " : "the ";
            string name = (string)mon["name"] ?? "";
            var shortName = mon["shortName"];

            if (shortName != null && shortName.Type == JTokenType.Boolean && (bool)shortName) return prefix + name;
            if (shortName != null && shortName.Type == JTokenType.String)
            {
                string sn = (string)shortName;
                return prefix + (prefix.Length == 0 && titleCase ? FiveEToolsText.TitleCase(sn) : sn.ToLowerInvariant());
            }

            string b = name.Split(',')[0];
            b = Regex.Replace(b, @"(?:adult|ancient|young) \w+ (dragon|dracolich)", "$1", RegexOptions.IgnoreCase);
            return prefix + (named ? b.Split(' ')[0] : b.ToLowerInvariant());
        }

        // ── Numbers ──────────────────────────────────────────────────────────

        public static double CrToNumber(JToken cr)
        {
            if (cr == null) return 0;
            if (cr is JObject o) cr = o["cr"];
            string s = (string)cr ?? "";
            var frac = Regex.Match(s, @"^\s*(\d+)\s*/\s*(\d+)");
            if (frac.Success) return double.Parse(frac.Groups[1].Value) / double.Parse(frac.Groups[2].Value);
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v) ? v : 0;
        }

        public static int CrToPb(JToken cr)
        {
            double n = CrToNumber(cr);
            return n < 5 ? 2 : (int)Math.Ceiling(n / 4) + 1;
        }

        public static int AbilityMod(JObject ent, string ability)
        {
            var v = ent[(ability ?? "").ToLowerInvariant()];
            int score = v != null && v.Type == JTokenType.Integer ? (int)v : 10;
            return (int)Math.Floor((score - 10) / 2.0);
        }

        private static int SizeMult(JObject ent)
        {
            string size = (string)AsArray(ent["size"]).FirstOrDefault() ?? "M";
            switch (size) { case "L": return 2; case "H": return 3; case "G": return 4; default: return 1; }
        }

        private static double Evaluate(string expr)
        {
            object result = new DataTable().Compute(expr, "");
            return Convert.ToDouble(result, CultureInfo.InvariantCulture);
        }

        // ── JSON path helpers ────────────────────────────────────────────────

        private static JArray AsArray(JToken t)
        {
            if (t == null) return new JArray();
            if (t is JArray a) return a;
            return new JArray(t.DeepClone());
        }

        private static JToken GetPath(JObject root, string[] path)
        {
            if (path == null) return root;
            JToken cur = root;
            foreach (string p in path)
            {
                cur = cur?[p];
                if (cur == null) return null;
            }
            return cur;
        }

        private static void SetPath(JObject root, string[] path, JToken value)
        {
            if (path == null || path.Length == 0) return;
            JObject cur = root;
            for (int i = 0; i < path.Length - 1; i++)
            {
                if (!(cur[path[i]] is JObject next)) cur[path[i]] = next = new JObject();
                cur = next;
            }
            cur[path[path.Length - 1]] = value;
        }

        private static void RemovePath(JObject root, string[] path)
        {
            var parent = path.Length == 1 ? root : GetPath(root, path.Take(path.Length - 1).ToArray()) as JObject;
            parent?.Remove(path[path.Length - 1]);
        }

        private static JObject GetOrCreateObject(JObject root, string[] path)
        {
            if (path == null) return root;
            if (GetPath(root, path) is JObject o) return o;
            var created = new JObject();
            SetPath(root, path, created);
            return created;
        }
    }
}
