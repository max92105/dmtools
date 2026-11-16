using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DMTools.Helpers
{
    /// <summary>Dice and weighted random picks for the loot generator.</summary>
    public static class LootRoller
    {
        private static readonly Random _random = new Random();

        /// <summary>Rolls "3", "1d4", "2d6+1", "1d6 - 1"... Invalid or empty text counts as 1.</summary>
        public static int RollQuantity(string expression)
        {
            string e = (expression ?? "").Replace(" ", "").ToLowerInvariant();
            if (e.Length == 0) return 1;
            if (int.TryParse(e, out int fixedValue)) return Math.Max(0, fixedValue);

            var m = Regex.Match(e, @"^(\d*)d(\d+)([+-]\d+)?$");
            if (!m.Success) return 1;

            int count = m.Groups[1].Value.Length > 0 ? int.Parse(m.Groups[1].Value) : 1;
            int faces = int.Parse(m.Groups[2].Value);
            int total = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0;
            for (int i = 0; i < count; i++) total += _random.Next(1, faces + 1);
            return Math.Max(0, total);
        }

        /// <summary>True when the text is a valid quantity ("2", "1d4", "2d6+1").</summary>
        public static bool IsValidQuantity(string expression)
        {
            string e = (expression ?? "").Replace(" ", "");
            return e.Length > 0 && (int.TryParse(e, out _) || Regex.IsMatch(e, @"^\d*d\d+([+-]\d+)?$", RegexOptions.IgnoreCase));
        }

        /// <summary>Picks one element, each with a chance proportional to its weight.</summary>
        public static T PickWeighted<T>(IList<T> items, Func<T, int> weight)
        {
            int total = items.Sum(i => Math.Max(0, weight(i)));
            if (total <= 0) return default(T);
            int roll = _random.Next(total);
            foreach (var item in items)
            {
                roll -= Math.Max(0, weight(item));
                if (roll < 0) return item;
            }
            return items.Last();
        }

        /// <summary>Picks up to <paramref name="count"/> different elements at random.</summary>
        public static List<T> PickDistinct<T>(IList<T> items, int count)
            => items.OrderBy(_ => _random.Next()).Take(Math.Max(0, count)).ToList();
    }
}
