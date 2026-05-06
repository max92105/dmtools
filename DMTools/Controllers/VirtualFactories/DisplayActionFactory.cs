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
    public class DisplayActionFactory
    {
        public List<DisplayAction> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var actionsCollection = db.GetCollection<Data.Objects.Action>("actions");
                var monstersCollection = db.GetCollection<Monster>("monsters");

                var actions = actionsCollection.FindAll().ToList();
                var monsters = monstersCollection.FindAll().ToDictionary(m => m.Id, m => m.Name);

                return actions.Select(a => new DisplayAction
                {
                    Id = a.Id,
                    DisplayName = (monsters.ContainsKey(a.MonsterId) ? monsters[a.MonsterId] : "Unknown") + " - " + a.Name
                }).ToList();
            }
        }
    }
}
