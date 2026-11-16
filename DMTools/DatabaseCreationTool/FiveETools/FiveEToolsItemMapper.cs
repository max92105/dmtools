using Data.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using T = DatabaseCreationTool.FiveETools.FiveEToolsText;

namespace DatabaseCreationTool.FiveETools
{
    /// <summary>Converts a resolved 5e.tools item (items.json / items-base.json) to a DM Tools <see cref="Item"/>.</summary>
    public static class FiveEToolsItemMapper
    {
        private static readonly Dictionary<string, string> Categories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["M"] = "Weapon", ["R"] = "Weapon", ["A"] = "Ammunition", ["AF"] = "Ammunition",
            ["LA"] = "Armor", ["MA"] = "Armor", ["HA"] = "Armor", ["S"] = "Shield",
            ["P"] = "Potion", ["RG"] = "Ring", ["RD"] = "Rod", ["SC"] = "Scroll", ["WD"] = "Wand",
            ["$"] = "Treasure", ["G"] = "Adventuring Gear", ["AT"] = "Tools", ["T"] = "Tools", ["GS"] = "Tools",
            ["INS"] = "Instrument", ["SCF"] = "Spellcasting Focus", ["TG"] = "Trade Good", ["FD"] = "Food & Drink",
            ["TAH"] = "Mounts & Vehicles", ["MNT"] = "Mounts & Vehicles", ["VEH"] = "Mounts & Vehicles",
            ["SHP"] = "Mounts & Vehicles", ["AIR"] = "Mounts & Vehicles", ["EXP"] = "Explosive", ["OTH"] = "Other",
        };

        private static readonly Dictionary<string, string> DamageTypes = new Dictionary<string, string>
        {
            ["A"] = "acid", ["B"] = "bludgeoning", ["C"] = "cold", ["F"] = "fire", ["O"] = "force", ["L"] = "lightning",
            ["N"] = "necrotic", ["P"] = "piercing", ["I"] = "poison", ["Y"] = "psychic", ["R"] = "radiant",
            ["S"] = "slashing", ["T"] = "thunder",
        };

        private static readonly Dictionary<string, string> WeaponProperties = new Dictionary<string, string>
        {
            ["A"] = "ammunition", ["AF"] = "ammunition", ["BF"] = "burst fire", ["F"] = "finesse", ["H"] = "heavy",
            ["L"] = "light", ["LD"] = "loading", ["R"] = "reach", ["RLD"] = "reload", ["S"] = "special",
            ["T"] = "thrown", ["2H"] = "two-handed", ["V"] = "versatile",
        };

        public static Item Map(JObject it)
        {
            string rarity = (string)it["rarity"] ?? "None";
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = (string)it["name"],
                Source = (string)it["source"],
                Category = MapCategory(it),
                Rarity = MapRarity(rarity),
                IsMagic = !string.Equals(rarity, "None", StringComparison.OrdinalIgnoreCase),
                Properties = MapProperties(it),
            };

            // Attunement: true, or a restriction such as "by a wizard"
            var attune = it["reqAttune"];
            if (attune != null && attune.Type != JTokenType.Null && !(attune.Type == JTokenType.Boolean && !(bool)attune))
            {
                item.RequiresAttunement = true;
                if (attune.Type == JTokenType.String) item.AttunementNote = T.StripTags((string)attune).TrimEnd('.');
            }

            if (it["value"] != null && decimal.TryParse((string)it["value"], System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal copper))
                item.ValueGp = copper / 100m;
            if (it["weight"] != null && decimal.TryParse((string)it["weight"], System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out decimal weight))
                item.Weight = weight;

            var description = new List<string>();
            string text = T.Render(it["entries"]);
            if (text.Length > 0) description.Add(text);
            string additional = T.Render(it["additionalEntries"]);
            if (additional.Length > 0) description.Add(additional);

            // Item groups ("Ioun Stone", "Figurine of Wondrous Power"...): list the members
            if (it["items"] is JArray members && members.Count > 0)
                description.Add("Includes: " + string.Join(", ", members.Select(m => ((string)m).Split('|')[0])) + ".");

            item.Description = string.Join("\n", description);
            return item;
        }

        private static string MapCategory(JObject it)
        {
            if ((bool?)it["staff"] == true) return "Staff";
            string type = ((string)it["type"] ?? "").Split('|')[0];
            if (type.Length > 0 && Categories.TryGetValue(type, out string category)) return category;
            if ((bool?)it["wondrous"] == true) return "Wondrous Item";
            if ((bool?)it["weapon"] == true) return "Weapon";
            if ((bool?)it["armor"] == true) return "Armor";
            return "Other";
        }

        private static string MapRarity(string rarity)
        {
            switch ((rarity ?? "").ToLowerInvariant())
            {
                case "none": return "Mundane";
                case "common": return "Common";
                case "uncommon": return "Uncommon";
                case "rare": return "Rare";
                case "very rare": return "Very Rare";
                case "legendary": return "Legendary";
                case "artifact": return "Artifact";
                case "varies": return "Varies";
                default: return T.TitleCase(rarity); // "Unknown", "Unknown (Magic)"
            }
        }

        /// <summary>"1d8 slashing, versatile (1d10)", "AC 15 + Dex (max 2), stealth disadvantage", "7 charges"...</summary>
        private static string MapProperties(JObject it)
        {
            var parts = new List<string>();

            if (it["dmg1"] != null)
            {
                string dmgType = DamageTypes.TryGetValue((string)it["dmgType"] ?? "", out string dt) ? " " + dt : "";
                parts.Add(T.StripTags((string)it["dmg1"]) + dmgType);
            }

            foreach (var p in ((it["property"] as JArray) ?? new JArray()).Select(x => ((string)x).Split('|')[0]))
            {
                if (!WeaponProperties.TryGetValue(p, out string name)) continue;
                if (p == "V" && it["dmg2"] != null) name += " (" + T.StripTags((string)it["dmg2"]) + ")";
                if ((p == "T" || p == "A" || p == "AF") && it["range"] != null) name += " (range " + it["range"] + " ft.)";
                parts.Add(name);
            }

            if (it["ac"] != null)
            {
                string type = ((string)it["type"] ?? "").Split('|')[0];
                if (type == "S") parts.Add("AC +" + it["ac"]);
                else parts.Add("AC " + it["ac"] + (type == "LA" ? " + Dex" : type == "MA" ? " + Dex (max 2)" : ""));
            }
            if (it["strength"] != null) parts.Add("Str " + it["strength"] + " required");
            if ((bool?)it["stealth"] == true) parts.Add("stealth disadvantage");
            if (it["charges"] != null) parts.Add(it["charges"] + " charges");

            return string.Join(", ", parts);
        }
    }
}
