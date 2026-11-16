using Data.Objects;
using DatabaseCreationTool.FiveETools;
using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using T = DatabaseCreationTool.FiveETools.FiveEToolsText;

namespace DatabaseCreationTool.Migrations
{
    /// <summary>
    /// Imports the 5e.tools item data ("5etools - Items" folder): every item, the mundane base items and the
    /// magic item groups, plus the Dungeon Master's Guide loot tables (Magic Item Tables A-I, gemstones, art objects).
    /// </summary>
    public class Migration_1794787300 : LiteDbMigration
    {
        public override long Version => 1794787300;
        public override string Description => "Import the 5e.tools items and the DMG loot tables";

        private const string DataFolderName = "5etools - Items";

        public override void Up(LiteDatabase db)
        {
            string folder = FindDataFolder();
            Console.WriteLine("  Data folder: " + folder);

            var itemsCol = db.GetCollection<Item>("items");
            var tablesCol = db.GetCollection<LootTable>("lootTables");
            var entriesCol = db.GetCollection<LootTableEntry>("lootTableEntries");

            // ── Items ──
            var itemsJson = JObject.Parse(File.ReadAllText(Path.Combine(folder, "items.json")));
            var baseJson = File.Exists(Path.Combine(folder, "items-base.json"))
                ? JObject.Parse(File.ReadAllText(Path.Combine(folder, "items-base.json"))) : new JObject();

            var raw = new List<JObject>();
            raw.AddRange(((itemsJson["item"] as JArray) ?? new JArray()).OfType<JObject>());
            raw.AddRange(((baseJson["baseitem"] as JArray) ?? new JArray()).OfType<JObject>());
            // Magic item groups ("Ioun Stone", "Figurine of Wondrous Power"...) are referenced by the DMG tables.
            raw.AddRange(((itemsJson["itemGroup"] as JArray) ?? new JArray()).OfType<JObject>()
                .Where(g => !string.Equals((string)g["rarity"], "None", StringComparison.OrdinalIgnoreCase)));

            var items = new List<Item>();
            if (itemsCol.Count() == 0)
            {
                var resolver = new FiveEToolsCopyResolver(raw, null);
                foreach (var entry in raw)
                {
                    try { items.Add(FiveEToolsItemMapper.Map(resolver.Resolve(entry))); }
                    catch (Exception ex) { Console.WriteLine($"  {entry["name"]} ({entry["source"]}) not imported: {ex.Message}"); }
                }
                foreach (string w in resolver.Warnings) Console.WriteLine("  " + w);

                itemsCol.InsertBulk(items);
                itemsCol.EnsureIndex(i => i.Name);
                Console.WriteLine($"  Imported {items.Count} items ({items.Count(i => i.IsMagic)} magic).");
            }
            else
            {
                items = itemsCol.FindAll().ToList();
                Console.WriteLine("  Items already present — skipping item import.");
            }

            // ── DMG loot tables ──
            if (tablesCol.Count(t => t.Source == "DMG") > 0)
            {
                Console.WriteLine("  DMG loot tables already present — skipping.");
                return;
            }

            string lootPath = Path.Combine(folder, "loot.json");
            if (!File.Exists(lootPath)) return;
            var loot = JObject.Parse(File.ReadAllText(lootPath));

            // Name → item (prefer the DMG version when a name exists in several books)
            var byName = items.GroupBy(i => i.Name.ToLowerInvariant())
                              .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Source == "DMG" ? 0 : 1).First());

            var tables = new List<LootTable>();
            var entries = new List<LootTableEntry>();

            foreach (var t in ((loot["magicitems"] as JArray) ?? new JArray()).OfType<JObject>())
            {
                var table = NewTable(tables, "DMG " + (string)t["name"], $"Dungeon Master's Guide p. {t["page"]} — roll d100.");
                int order = 0;
                foreach (var row in ((t["table"] as JArray) ?? new JArray()).OfType<JObject>())
                {
                    int min = (int?)row["min"] ?? 1, max = (int?)row["max"] ?? min;
                    string name;
                    if (row["item"] != null) name = T.StripTags((string)row["item"]);
                    else if (row["choose"]?["fromGeneric"] is JArray gen) name = string.Join(" or ", gen.Select(x => T.StripTags((string)x)));
                    else if (row["choose"]?["fromGroup"] is JArray grp) name = string.Join(" or ", grp.Select(x => T.StripTags((string)x)));
                    else continue;

                    byName.TryGetValue(name.ToLowerInvariant(), out Item linked);
                    entries.Add(new LootTableEntry
                    {
                        Id = Guid.NewGuid(), LootTableId = table.Id,
                        ItemId = linked?.Id ?? Guid.Empty,
                        Name = linked?.Name ?? name,
                        Weight = Math.Max(1, max - min + 1),
                        Quantity = "1",
                        SortOrder = order++
                    });
                }
            }

            AddListTables(loot["gemstones"], "Gemstones", tables, entries, byName);
            AddListTables(loot["artobjects"], "Art Objects", tables, entries, byName);

            tablesCol.InsertBulk(tables);
            entriesCol.InsertBulk(entries);
            entriesCol.EnsureIndex(e => e.LootTableId);
            Console.WriteLine($"  Created {tables.Count} DMG loot tables ({entries.Count} entries, {entries.Count(e => e.ItemId != Guid.Empty)} linked to items).");
        }

        public override void Down(LiteDatabase db)
        {
            var tables = db.GetCollection<LootTable>("lootTables");
            var entries = db.GetCollection<LootTableEntry>("lootTableEntries");
            foreach (var t in tables.Find(t => t.Source == "DMG").ToList())
            {
                entries.DeleteMany(e => e.LootTableId == t.Id);
                tables.Delete(t.Id);
            }
            db.DropCollection("items");
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static LootTable NewTable(List<LootTable> tables, string name, string description)
        {
            var table = new LootTable { Id = Guid.NewGuid(), Name = name, Description = description, Source = "DMG" };
            tables.Add(table);
            return table;
        }

        /// <summary>Gemstone / art object tables: a plain list, every line equally likely.</summary>
        private static void AddListTables(JToken source, string label, List<LootTable> tables, List<LootTableEntry> entries,
            Dictionary<string, Item> byName)
        {
            foreach (var t in ((source as JArray) ?? new JArray()).OfType<JObject>())
            {
                var table = NewTable(tables, $"DMG {label} ({t["type"]} gp)", $"Dungeon Master's Guide p. {t["page"]} — each worth {t["type"]} gp.");
                int order = 0;
                foreach (var line in ((t["table"] as JArray) ?? new JArray()))
                {
                    string text = line.Type == JTokenType.String ? T.StripTags((string)line) : T.RenderInline(line);
                    // "Azurite (opaque mottled deep blue)" → item "Azurite"
                    byName.TryGetValue(text.ToLowerInvariant(), out Item linked);
                    if (linked == null) byName.TryGetValue(text.Split(new[] { " (" }, StringSplitOptions.None)[0].ToLowerInvariant(), out linked);

                    entries.Add(new LootTableEntry
                    {
                        Id = Guid.NewGuid(), LootTableId = table.Id,
                        ItemId = linked?.Id ?? Guid.Empty,
                        Name = text,
                        Weight = 1,
                        Quantity = "1",
                        SortOrder = order++
                    });
                }
            }
        }

        /// <summary>DMTOOLS_ITEMS_PATH, else "5etools - Items" next to the tool or in a parent folder, else asked.</summary>
        private static string FindDataFolder()
        {
            string fromEnv = Environment.GetEnvironmentVariable("DMTOOLS_ITEMS_PATH");
            if (!string.IsNullOrWhiteSpace(fromEnv) && Directory.Exists(fromEnv)) return fromEnv;

            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, DataFolderName);
                if (Directory.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }

            Console.Write($"  Folder \"{DataFolderName}\" not found. Enter its full path: ");
            string typed = (Console.ReadLine() ?? "").Trim('"', ' ');
            if (Directory.Exists(typed)) return typed;
            throw new DirectoryNotFoundException($"5e.tools items folder not found (\"{DataFolderName}\").");
        }
    }
}
