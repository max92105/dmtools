using Controllers.Helpers;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Controllers
{
    /// <summary>Item library and loot tables (collections "items", "lootTables", "lootTableEntries").</summary>
    public static class LootGeneratorDataController
    {
        private const string Items = "items";
        private const string Tables = "lootTables";
        private const string Entries = "lootTableEntries";

        // ── Items ────────────────────────────────────────────────────────────

        public static List<Item> LoadItems()
        {
            using (var db = DatabaseHelper.GetDatabase())
                return db.GetCollection<Item>(Items).FindAll().OrderBy(i => i.Name).ToList();
        }

        // ── Loot tables ──────────────────────────────────────────────────────

        public static List<LootTable> LoadTables()
        {
            using (var db = DatabaseHelper.GetDatabase())
                return db.GetCollection<LootTable>(Tables).FindAll()
                    .OrderBy(t => string.IsNullOrEmpty(t.Source) ? 0 : 1) // your own tables first
                    .ThenBy(t => t.Name)
                    .ToList();
        }

        public static void SaveTable(LootTable table)
        {
            using (var db = DatabaseHelper.GetDatabase())
                db.GetCollection<LootTable>(Tables).Upsert(table);
        }

        /// <summary>Deletes the table and all its entries.</summary>
        public static void DeleteTable(Guid tableId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                db.GetCollection<LootTableEntry>(Entries).DeleteMany(e => e.LootTableId == tableId);
                db.GetCollection<LootTable>(Tables).Delete(tableId);
            }
        }

        // ── Entries ──────────────────────────────────────────────────────────

        public static List<LootTableEntry> LoadEntries(Guid tableId)
        {
            using (var db = DatabaseHelper.GetDatabase())
                return db.GetCollection<LootTableEntry>(Entries)
                    .Find(e => e.LootTableId == tableId)
                    .OrderBy(e => e.SortOrder)
                    .ToList();
        }

        public static void SaveEntry(LootTableEntry entry)
        {
            using (var db = DatabaseHelper.GetDatabase())
                db.GetCollection<LootTableEntry>(Entries).Upsert(entry);
        }

        public static void DeleteEntry(Guid entryId)
        {
            using (var db = DatabaseHelper.GetDatabase())
                db.GetCollection<LootTableEntry>(Entries).Delete(entryId);
        }
    }
}
