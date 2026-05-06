using Controllers.Helpers;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class DisplaySpecialAbilityFactory
    {
        public List<DisplaySpecialAbility> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var specialAbilitiesCollection = db.GetCollection<SpecialAbility>("specialAbilities");
                var monstersCollection = db.GetCollection<Monster>("monsters");

                var specialAbilities = specialAbilitiesCollection.FindAll().ToList();
                var monsters = monstersCollection.FindAll().ToDictionary(m => m.Id, m => m.Name);

                return specialAbilities.Select(s => new DisplaySpecialAbility
                {
                    Id = s.Id,
                    DisplayName = (monsters.ContainsKey(s.MonsterId) ? monsters[s.MonsterId] : "Unknown") + " - " + s.Name
                }).ToList();
            }
        }
    }
}
