using Data.Constants;
using Data.Objects;
using DatabaseCreationTool.FiveETools;
using LiteDB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DatabaseCreationTool.Migrations
{
    /// <summary>
    /// Imports every monster of the 5e.tools bestiary data ("5eTools - Bestiary" folder):
    /// resolves "_copy" monsters and templates, converts them to DM Tools objects and inserts them.
    /// Monsters whose name already exists in the database are skipped (your SRD / homebrew monsters are untouched).
    /// </summary>
    public class Migration_1794787200 : LiteDbMigration
    {
        public override long Version => 1794787200;
        public override string Description => "Import the 5e.tools bestiary (all sources, with lair actions and copies resolved)";

        private const string DataFolderName = "5eTools - Bestiary";
        private const string LogFileName = "FiveEToolsImport.log";

        public override void Up(LiteDatabase db)
        {
            var timer = Stopwatch.StartNew();
            string folder = FindDataFolder();
            Console.WriteLine("  Data folder: " + folder);

            // ── Load raw data ──
            var raw = new List<JObject>();
            foreach (string file in Directory.GetFiles(folder, "bestiary-*.json").OrderBy(f => f))
                raw.AddRange(((JObject.Parse(File.ReadAllText(file))["monster"] as JArray) ?? new JArray()).OfType<JObject>());

            var templates = ReadArray(Path.Combine(folder, "traits.json"), "trait");
            var legendaryGroupsRaw = ReadArray(Path.Combine(folder, "meta.json"), "legendaryGroup");
            Console.WriteLine($"  {raw.Count} monster entries, {legendaryGroupsRaw.Count} legendary groups, {templates.Count} templates.");

            var resolver = new FiveEToolsCopyResolver(raw, templates);
            var groupResolver = new FiveEToolsCopyResolver(legendaryGroupsRaw, null);
            var legendaryGroups = new Dictionary<string, JObject>(StringComparer.OrdinalIgnoreCase);
            foreach (var g in legendaryGroupsRaw)
                legendaryGroups[FiveEToolsCopyResolver.Key((string)g["name"], (string)g["source"])] = groupResolver.Resolve(g);

            // ── Existing monsters are kept as they are ──
            var monstersCol = db.GetCollection<Monster>("monsters");
            var existingNames = new HashSet<string>(monstersCol.FindAll().Select(m => (m.Name ?? "").Trim()), StringComparer.OrdinalIgnoreCase);

            var log = new List<string>();
            var mapped = new List<MappedMonster>();
            int skipped = 0, failed = 0;

            foreach (var entry in raw)
            {
                string label = $"{entry["name"]} ({entry["source"]})";
                if (existingNames.Contains(((string)entry["name"] ?? "").Trim())) { skipped++; continue; }

                try
                {
                    JObject mon = resolver.Resolve(entry);
                    JObject group = null;
                    if (mon["legendaryGroup"] is JObject lg)
                    {
                        legendaryGroups.TryGetValue(FiveEToolsCopyResolver.Key((string)lg["name"], (string)lg["source"]), out group);
                        if (group == null) log.Add($"{label}: legendary group \"{lg["name"]}\" not found.");
                    }

                    var m = FiveEToolsMonsterMapper.Map(mon, group);
                    mapped.Add(m);
                    log.AddRange(m.Warnings.Select(w => label + ": " + w));
                }
                catch (Exception ex)
                {
                    failed++;
                    log.Add($"{label}: NOT IMPORTED — {ex.Message}");
                }
            }
            log.AddRange(resolver.Warnings);
            log.AddRange(groupResolver.Warnings);

            // ── Configuration lists (types / subtypes) ──
            var typeNames = EnsureConfigList(db, "monster_types", MonsterTypes.All, mapped.Select(m => m.Monster.Type));
            var subtypeNames = EnsureConfigList(db, "monster_subtypes", MonsterSubtypes.All, mapped.Select(m => m.Monster.Subtype));
            foreach (var m in mapped)
            {
                if (m.Monster.Type != null && typeNames.TryGetValue(m.Monster.Type, out string t)) m.Monster.Type = t;
                if (m.Monster.Subtype != null && subtypeNames.TryGetValue(m.Monster.Subtype, out string s)) m.Monster.Subtype = s;
            }

            // ── Insert everything in bulk ──
            monstersCol.InsertBulk(mapped.Select(m => m.Monster));
            db.GetCollection<Characteristic>("characteristics").InsertBulk(mapped.SelectMany(m => m.Characteristics));
            db.GetCollection<Skill>("skills").InsertBulk(mapped.SelectMany(m => m.Skills));
            db.GetCollection<SpecialAbility>("specialAbilities").InsertBulk(mapped.SelectMany(m => m.SpecialAbilities));
            db.GetCollection<Data.Objects.Action>("actions").InsertBulk(mapped.SelectMany(m => m.Actions));
            db.GetCollection<ArmorClassEntry>("armorClassEntries").InsertBulk(mapped.SelectMany(m => m.ArmorClassEntries));
            db.GetCollection<Speed>("speeds").InsertBulk(mapped.SelectMany(m => m.Speeds));
            db.GetCollection<Sense>("senses").InsertBulk(mapped.SelectMany(m => m.Senses));
            db.GetCollection<DamageModifier>("damageModifiers").InsertBulk(mapped.SelectMany(m => m.DamageModifiers));

            EnsureIndexes(db);

            // ── Report (never fails the migration: the data is already inserted) ──
            var report = new[]
            {
                $"5e.tools import — {DateTime.Now}",
                $"Imported: {mapped.Count}   Skipped (name already in database): {skipped}   Failed: {failed}",
                ""
            }.Concat(log).ToList();
            string logPath = WriteLog(report, folder);

            Console.WriteLine($"  Imported {mapped.Count} monsters ({mapped.Sum(m => m.Actions.Count)} actions, {mapped.Sum(m => m.SpecialAbilities.Count)} traits).");
            Console.WriteLine($"  Skipped {skipped} (already in database), failed {failed}, {log.Count} notes.");
            Console.WriteLine($"  Details: {logPath}");
            Console.WriteLine($"  Done in {timer.Elapsed.TotalSeconds:0.0}s.");
        }

        /// <summary>Removes the monsters imported from 5e.tools (they have a Source) and all their data.</summary>
        public override void Down(LiteDatabase db)
        {
            var monsters = db.GetCollection<Monster>("monsters");
            var ids = new HashSet<Guid>(monsters.FindAll().Where(m => !string.IsNullOrEmpty(m.Source)).Select(m => m.Id));

            foreach (string col in new[] { "characteristics", "skills", "specialAbilities", "actions", "armorClassEntries", "speeds", "senses", "damageModifiers" })
            {
                var c = db.GetCollection(col);
                foreach (var doc in c.FindAll().Where(d => d["MonsterId"].IsGuid && ids.Contains(d["MonsterId"].AsGuid)).ToList())
                    c.Delete(doc["_id"]);
            }
            foreach (Guid id in ids) monsters.Delete(id);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        /// <summary>
        /// Data folder: DMTOOLS_BESTIARY_PATH environment variable if set, else next to the tool or in a parent
        /// folder (repository root), else asked on the console.
        /// </summary>
        private static string FindDataFolder()
        {
            string fromEnv = Environment.GetEnvironmentVariable("DMTOOLS_BESTIARY_PATH");
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

            // Throwing keeps the migration "not applied", so it runs again next time.
            throw new DirectoryNotFoundException($"5e.tools bestiary folder not found (\"{DataFolderName}\").");
        }

        private static string WriteLog(List<string> lines, string dataFolder)
        {
            foreach (string dir in new[] { dataFolder, Path.GetTempPath() })
            {
                try
                {
                    string path = Path.Combine(dir, LogFileName);
                    File.WriteAllLines(path, lines);
                    return path;
                }
                catch { /* try the next location */ }
            }
            return "(log could not be written)";
        }

        private static List<JObject> ReadArray(string path, string prop)
        {
            if (!File.Exists(path)) return new List<JObject>();
            return ((JObject.Parse(File.ReadAllText(path))[prop] as JArray) ?? new JArray()).OfType<JObject>().ToList();
        }

        /// <summary>
        /// Makes sure every used type / subtype exists in the configuration list (seeding the defaults first if the list
        /// was never created, like the app does). Returns name → stored casing.
        /// </summary>
        private static Dictionary<string, string> EnsureConfigList(LiteDatabase db, string collectionName, IEnumerable<string> defaults, IEnumerable<string> used)
        {
            var col = db.GetCollection<Data.Objects.Type>(collectionName);
            if (col.Count() == 0)
                col.InsertBulk(defaults.Where(n => !string.IsNullOrEmpty(n)).Select(n => new Data.Objects.Type { Id = Guid.NewGuid(), Name = n }));

            var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var t in col.FindAll()) if (!string.IsNullOrEmpty(t.Name)) names[t.Name] = t.Name;

            var toAdd = used.Where(n => !string.IsNullOrWhiteSpace(n) && !names.ContainsKey(n))
                            .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            col.InsertBulk(toAdd.Select(n => new Data.Objects.Type { Id = Guid.NewGuid(), Name = n }));
            foreach (string n in toAdd) names[n] = n;
            return names;
        }

        private static void EnsureIndexes(LiteDatabase db)
        {
            db.GetCollection<Monster>("monsters").EnsureIndex(m => m.Name);
            db.GetCollection<Characteristic>("characteristics").EnsureIndex(x => x.MonsterId);
            db.GetCollection<Skill>("skills").EnsureIndex(x => x.MonsterId);
            db.GetCollection<SpecialAbility>("specialAbilities").EnsureIndex(x => x.MonsterId);
            db.GetCollection<Data.Objects.Action>("actions").EnsureIndex(x => x.MonsterId);
            db.GetCollection<ArmorClassEntry>("armorClassEntries").EnsureIndex(x => x.MonsterId);
            db.GetCollection<Speed>("speeds").EnsureIndex(x => x.MonsterId);
            db.GetCollection<Sense>("senses").EnsureIndex(x => x.MonsterId);
            db.GetCollection<DamageModifier>("damageModifiers").EnsureIndex(x => x.MonsterId);
        }
    }
}
