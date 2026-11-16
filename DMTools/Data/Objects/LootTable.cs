using System;

namespace Data.Objects
{
    /// <summary>A loot table: a list of weighted entries you can roll on.</summary>
    public class LootTable
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        /// <summary>"DMG" for the imported Dungeon Master's Guide tables, empty for your own.</summary>
        public String Source { get; set; }

        public override string ToString() => Name;
    }

    /// <summary>One line of a loot table: an item from the library, or just a name (e.g. "Weapon +1").</summary>
    public class LootTableEntry
    {
        public Guid Id { get; set; }
        public Guid LootTableId { get; set; }

        /// <summary>Linked item, or Guid.Empty for a name-only entry.</summary>
        public Guid ItemId { get; set; }

        public String Name { get; set; }

        /// <summary>Relative chance of being rolled (a d100 range of 51-60 is a weight of 10).</summary>
        public Int32 Weight { get; set; }

        /// <summary>How many: "1", "2", "1d4", "2d6+1"...</summary>
        public String Quantity { get; set; }

        public Int32 SortOrder { get; set; }
    }
}
