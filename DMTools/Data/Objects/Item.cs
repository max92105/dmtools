using System;

namespace Data.Objects
{
    /// <summary>An item from the item library (magic items, gear, gems, art objects...).</summary>
    public class Item
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        /// <summary>Book the item comes from (e.g. "DMG", "XGE").</summary>
        public String Source { get; set; }

        /// <summary>Wondrous Item, Weapon, Armor, Potion, Ring, Scroll, Treasure...</summary>
        public String Category { get; set; }

        /// <summary>Common, Uncommon, Rare, Very Rare, Legendary, Artifact, Varies, Mundane...</summary>
        public String Rarity { get; set; }

        public Boolean IsMagic { get; set; }
        public Boolean RequiresAttunement { get; set; }

        /// <summary>Attunement restriction, e.g. "by a wizard". Empty when any creature can attune.</summary>
        public String AttunementNote { get; set; }

        /// <summary>Price in gold pieces, when known.</summary>
        public Decimal? ValueGp { get; set; }

        /// <summary>Weight in pounds, when known.</summary>
        public Decimal? Weight { get; set; }

        /// <summary>Short mechanical summary: damage, AC, charges...</summary>
        public String Properties { get; set; }

        public String Description { get; set; }
    }
}
