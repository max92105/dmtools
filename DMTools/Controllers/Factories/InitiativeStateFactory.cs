using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class InitiativeStateFactory
    {
        private const string Collection = "activeInitiative";

        public void SaveState(IEnumerable<InitiativeEntry> entries)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var col = db.GetCollection<InitiativeEntry>(Collection);
                col.DeleteAll();
                foreach (var entry in entries)
                {
                    if (entry.Id == Guid.Empty)
                        entry.Id = Guid.NewGuid();
                    col.Insert(entry);
                }
            }
        }

        public List<InitiativeEntry> LoadState()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var col = db.GetCollection<InitiativeEntry>(Collection);
                var list = col.FindAll()
                              .OrderByDescending(e => e.Initiative)
                              .ThenBy(e => e.TieBreaker)
                              .ToList();
                foreach (var entry in list)
                    entry.SetInternalState(InternalStates.UnModified, true);
                return list;
            }
        }

        public void ClearState()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                db.GetCollection<InitiativeEntry>(Collection).DeleteAll();
            }
        }
    }
}
