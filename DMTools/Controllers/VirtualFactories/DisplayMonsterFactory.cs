using Controllers.Helpers;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.VirtualFactories
{
    public class DisplayMonsterFactory
    {
        public List<DisplayMonster> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                return collection.FindAll()
                    .OrderBy(m => m.Name)
                    .Select(Map)
                    .ToList();
            }
        }

        public List<DisplayMonster> GetObjectsBySearchCriteria(String name)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Monster>("monsters");
                return collection.Find(m => m.Name.Contains(name))
                    .OrderBy(m => m.Name)
                    .Select(Map)
                    .ToList();
            }
        }

        private static DisplayMonster Map(Monster m) => new DisplayMonster
        {
            Id             = m.Id,
            Name           = m.Name,
            Type           = m.Type,
            Subtype        = m.Subtype,
            Size           = m.Size,
            Alignment      = m.Alignment,
            ArmorClass     = m.ArmorClass,
            HitPoints      = m.HitPoints,
            ChallengeRating = m.ChallengeRating
        };
    }
}
