using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseCreationTool.FiveETools
{
    /// <summary>
    /// Converts 5e.tools data text to plain text: inline tags ({@hit 5}, {@spell fireball}...) and
    /// nested "entries" structures (lists, tables, sub-entries, spellcasting blocks).
    /// Follows the plain-text rules of the 5e.tools renderer (Renderer.stripTags).
    /// </summary>
    public static class FiveEToolsText
    {
        private static readonly Regex InnermostTag = new Regex(@"\{@(\w+)(?: ([^{}]*))?\}", RegexOptions.Compiled);

        // ── Inline tags ──────────────────────────────────────────────────────

        /// <summary>Replaces every {@tag ...} with its plain-text form (innermost first, so nesting works).</summary>
        public static string StripTags(string s)
        {
            if (string.IsNullOrEmpty(s)) return s ?? "";
            string prev;
            do
            {
                prev = s;
                s = InnermostTag.Replace(s, m => TagToText(m.Groups[1].Value, m.Groups[2].Success ? m.Groups[2].Value : ""));
            } while (s != prev);
            return s;
        }

        private static string TagToText(string tag, string text)
        {
            string[] parts = text.Split('|');
            string first = parts[0];
            string display = parts.Length > 1 ? parts[1] : "";

            switch (tag)
            {
                case "h": return "Hit: ";
                case "m": return "Miss: ";
                case "hom": return "Hit or Miss: ";
                case "atk": return AttackTagToFull(text, false);
                case "atkr": return AttackTagToFull(text, true);

                case "hit":
                case "d20":
                case "initiative":
                    if (display.Length > 0) return display;
                    return int.TryParse(first, out int n) ? (n >= 0 ? "+" + n : n.ToString()) : first;

                case "damage":
                case "dice":
                case "autodice":
                    return display.Length > 0 ? display : first.Replace(";", "/");

                case "recharge":
                {
                    if (first.Length == 0) return "(Recharge 6)";
                    if (!int.TryParse(first, out int r)) return "(Recharge ?)";
                    return r < 6 ? $"(Recharge {r}–6)" : $"(Recharge {r})";
                }

                case "chance": return display.Length > 0 ? display : first + " percent";
                case "dc": return "DC " + (display.Length > 0 ? display : first);
                case "dcYourSpellSave": return first.Length > 0 ? first : "your spell save DC";
                case "hitYourSpellAttack": return first.Length > 0 ? first : "your spell attack modifier";
                case "actSave": return AbilityFull(first) + " Saving Throw:";
                case "actSaveSuccess": return "Success:";
                case "actSaveFail": return "Failure:";
                case "actSaveSuccessOrFail": return "Failure or Success:";
                case "actTrigger": return "Trigger:";
                case "actResponse": return "Response:";

                case "scaledice":
                case "scaledamage":
                    return parts.Length > 4 && parts[4].Length > 0 ? parts[4] : (parts.Length > 2 ? parts[2] : first);

                // Text-only tags
                case "b": case "bold": case "i": case "italic": case "s": case "strike": case "u": case "underline":
                case "sup": case "sub": case "kbd": case "code": case "note": case "tip": case "style": case "font":
                case "highlight": case "color": case "help": case "comic":
                    return first;

                // Links whose first part is the display text
                case "book": case "adventure": case "filter": case "link": case "loader": case "5etools": case "footnote":
                case "homebrew": case "area": case "cite":
                    return first;

                // Entity links: {@spell name|source|display text}
                default:
                    return parts.Length > 2 && parts[2].Length > 0 ? parts[2] : first;
            }
        }

        /// <summary>"mw" → "Melee Weapon Attack:", "mw,rw" → "Melee or Ranged Weapon Attack:".</summary>
        private static string AttackTagToFull(string tagStr, bool isRoll)
        {
            var groups = tagStr.ToLowerInvariant().Split(',').Select(t => t.Trim()).Where(t => t.Length > 0)
                .Select(t => t.ToCharArray().ToList()).ToList();

            // Remove letters repeated in later groups ("mw,rw" → "m" + "rw" → "Melee or Ranged Weapon")
            if (groups.Count > 1)
            {
                var seen = new HashSet<char>(groups.Last());
                for (int i = groups.Count - 2; i >= 0; i--)
                {
                    groups[i] = groups[i].Where(c => { bool keep = !seen.Contains(c); seen.Add(c); return keep; }).ToList();
                }
            }

            Func<List<char>, string> render = g =>
            {
                string type = g.Contains('m') ? "Melee " : g.Contains('r') ? "Ranged " : g.Contains('g') ? "Magical " : g.Contains('a') ? "Area " : "";
                string method = g.Contains('w') ? "Weapon " : g.Contains('s') ? "Spell " : g.Contains('p') ? "Power " : "";
                return type + method;
            };

            return string.Join("or ", groups.Select(render)) + "Attack" + (isRoll ? " Roll" : "") + ":";
        }

        public static string AbilityFull(string abv)
        {
            switch ((abv ?? "").ToLowerInvariant())
            {
                case "str": return "Strength";
                case "dex": return "Dexterity";
                case "con": return "Constitution";
                case "int": return "Intelligence";
                case "wis": return "Wisdom";
                case "cha": return "Charisma";
                default: return abv;
            }
        }

        // ── Entries ──────────────────────────────────────────────────────────

        /// <summary>Renders an entries array / entry object to plain text (paragraphs separated by new lines).</summary>
        public static string Render(JToken token)
        {
            var lines = new List<string>();
            RenderInto(token, lines);
            return string.Join("\n", lines.Select(l => l.Trim()).Where(l => l.Length > 0));
        }

        private static void RenderInto(JToken token, List<string> lines)
        {
            if (token == null || token.Type == JTokenType.Null) return;

            if (token.Type == JTokenType.String)
            {
                lines.Add(StripTags((string)token));
                return;
            }

            if (token is JArray arr)
            {
                foreach (var child in arr) RenderInto(child, lines);
                return;
            }

            if (!(token is JObject o))
            {
                lines.Add(token.ToString());
                return;
            }

            string type = (string)o["type"] ?? "entries";
            string name = o["name"] != null ? StripTags((string)o["name"]) : null;

            switch (type)
            {
                case "list":
                    foreach (var item in (o["items"] as JArray) ?? new JArray())
                        lines.Add("• " + RenderInline(item));
                    return;

                case "item":
                case "itemSpell":
                case "itemSub":
                {
                    string body = o["entry"] != null ? RenderInline(o["entry"]) : RenderInline(o["entries"]);
                    lines.Add(((name ?? "") + " " + body).Trim());
                    return;
                }

                case "table":
                    RenderTable(o, lines);
                    return;

                case "inline":
                case "inlineBlock":
                    lines.Add(string.Concat(((o["entries"] as JArray) ?? new JArray()).Select(e => RenderInline(e))));
                    return;

                case "spellcasting":
                    lines.Add(RenderSpellcasting(o, includeName: true));
                    return;

                case "link":
                    lines.Add(StripTags((string)o["text"] ?? ""));
                    return;

                case "abilityDc":
                    lines.Add($"{name} save DC = 8 + your proficiency bonus + your {AttributesText(o["attributes"])} modifier");
                    return;

                case "abilityAttackMod":
                    lines.Add($"{name} attack modifier = your proficiency bonus + your {AttributesText(o["attributes"])} modifier");
                    return;

                case "dice":
                    lines.Add(o["toRoll"] is JArray rolls
                        ? string.Join(" + ", rolls.Select(r => $"{(int?)r["number"] ?? 1}d{(int?)r["faces"] ?? 6}"))
                        : "");
                    return;

                default:
                {
                    // entries, inset, insetReadaloud, variant, variantInner, variantSub, section, options, quote...
                    var inner = new List<string>();
                    RenderInto(o["entries"] ?? o["entry"] ?? o["items"], inner);
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (inner.Count == 0) lines.Add(name + ".");
                        else
                        {
                            inner[0] = name + ". " + inner[0];
                            lines.AddRange(inner);
                        }
                    }
                    else lines.AddRange(inner);
                    return;
                }
            }
        }

        /// <summary>Renders on one line (used for list items and inline content).</summary>
        public static string RenderInline(JToken token) => Render(token).Replace("\n", " ");

        private static void RenderTable(JObject o, List<string> lines)
        {
            if (o["caption"] != null) lines.Add(StripTags((string)o["caption"]));
            if (o["colLabels"] is JArray labels)
                lines.Add(string.Join(" | ", labels.Select(l => RenderInline(l))));
            foreach (var row in (o["rows"] as JArray) ?? new JArray())
            {
                JToken cells = row is JObject ro && ro["row"] != null ? ro["row"] : row;
                if (cells is JArray ca) lines.Add(string.Join(" | ", ca.Select(RenderCell)));
            }
        }

        private static string RenderCell(JToken cell)
        {
            if (cell is JObject c && (string)c["type"] == "cell")
            {
                if (c["roll"] is JObject roll)
                {
                    if (roll["exact"] != null) return (string)roll["exact"];
                    return $"{roll["min"]}–{roll["max"]}";
                }
                return RenderInline(c["entry"]);
            }
            return RenderInline(cell);
        }

        private static string AttributesText(JToken attrs)
        {
            if (!(attrs is JArray a)) return "";
            var names = a.Select(x => AbilityFull((string)x)).ToList();
            return names.Count <= 1 ? string.Concat(names) : string.Join(", ", names.Take(names.Count - 1)) + " or " + names.Last();
        }

        // ── Spellcasting ─────────────────────────────────────────────────────

        private static readonly string[] DurationProps = { "recharge", "legendary", "charges", "rest", "restLong", "daily", "weekly", "monthly", "yearly" };

        /// <summary>Renders a spellcasting block like the 5e.tools stat block (header, spell lists, footer).</summary>
        public static string RenderSpellcasting(JObject sc, bool includeName = false)
        {
            var hidden = new HashSet<string>(((sc["hidden"] as JArray) ?? new JArray()).Select(h => (string)h));
            var lines = new List<string>();

            string header = Render(sc["headerEntries"]);
            if (includeName && sc["name"] != null) header = StripTags((string)sc["name"]) + ". " + header;
            if (header.Length > 0) lines.Add(header);

            if (sc["constant"] != null && !hidden.Contains("constant")) lines.Add("Constant: " + SpellList(sc["constant"]));
            if (sc["will"] != null && !hidden.Contains("will")) lines.Add("At will: " + SpellList(sc["will"]));

            foreach (string prop in DurationProps)
            {
                if (!(sc[prop] is JObject per) || hidden.Contains(prop)) continue;
                foreach (var kv in per.Properties().OrderByDescending(p => KeyNumber(p.Name)))
                {
                    bool each = kv.Name.EndsWith("e");
                    int n = KeyNumber(kv.Name);
                    string label;
                    switch (prop)
                    {
                        case "recharge": label = StripTags("{@recharge " + n + "}"); break;
                        case "legendary": label = n + " legendary action" + (n == 1 ? "" : "s"); break;
                        case "charges": label = n + " charge" + (n == 1 ? "" : "s"); break;
                        case "rest": label = n + "/rest"; break;
                        case "restLong": label = n + "/long rest"; break;
                        case "daily": label = n + "/day"; break;
                        case "weekly": label = n + "/week"; break;
                        case "monthly": label = n + "/month"; break;
                        default: label = n + "/year"; break;
                    }
                    lines.Add(label + (each ? " each" : "") + ": " + SpellList(kv.Value));
                }
            }

            if (sc["ritual"] != null && !hidden.Contains("ritual")) lines.Add("Rituals: " + SpellList(sc["ritual"]));

            if (sc["spells"] is JObject spells && !hidden.Contains("spells"))
            {
                foreach (var lvlProp in spells.Properties().OrderBy(p => KeyNumber(p.Name)))
                {
                    int lvl = KeyNumber(lvlProp.Name);
                    var info = lvlProp.Value as JObject;
                    if (info == null) continue;

                    string levelText = lvl == 0 ? "Cantrips" : Ordinal(lvl) + " level";
                    int? slots = (int?)info["slots"];
                    string slotText = slots == null ? " (at will)" : slots > 0 ? $" ({slots} slot{(slots > 1 ? "s" : "")})" : "";
                    int? lower = (int?)info["lower"];
                    if (lower != null && lower != lvl)
                    {
                        levelText = Ordinal(lower.Value) + "-" + levelText;
                        if (slots != null) slotText = slots > 0 ? $" ({slots} {Ordinal(lvl)}-level slot{(slots > 1 ? "s" : "")})" : "";
                    }
                    string list = SpellList(info["spells"]);
                    lines.Add(levelText + slotText + ": " + (list.Length > 0 ? list : "—"));
                }
            }

            string footer = Render(sc["footerEntries"]);
            if (footer.Length > 0) lines.Add(footer);

            return string.Join("\n", lines);
        }

        private static string SpellList(JToken list)
        {
            if (!(list is JArray a)) return "";
            return string.Join(", ", a
                .Where(s => !(s is JObject so && (bool?)so["hidden"] == true))
                .Select(s => s is JObject so2 ? StripTags((string)so2["entry"]) : StripTags((string)s)));
        }

        private static int KeyNumber(string key)
        {
            var m = Regex.Match(key ?? "", @"\d+");
            return m.Success ? int.Parse(m.Value) : 0;
        }

        public static string Ordinal(int n)
        {
            switch (n)
            {
                case 1: return "1st";
                case 2: return "2nd";
                case 3: return "3rd";
                default: return n + "th";
            }
        }

        // ── Small helpers ────────────────────────────────────────────────────

        public static string TitleCase(string s)
            => string.IsNullOrWhiteSpace(s) ? s : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.Trim().ToLowerInvariant());

        public static string CapitalizeFirst(string s)
        {
            s = (s ?? "").Trim();
            return s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s.Substring(1);
        }

        /// <summary>"a, b, and c" (5e.tools joinConjunct).</summary>
        public static string JoinConjunct(IList<string> items, string conjunction = "and")
        {
            if (items.Count == 0) return "";
            if (items.Count == 1) return items[0];
            if (items.Count == 2) return items[0] + " " + conjunction + " " + items[1];
            return string.Join(", ", items.Take(items.Count - 1)) + ", " + conjunction + " " + items.Last();
        }
    }
}
