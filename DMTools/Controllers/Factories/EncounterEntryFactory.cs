using Controllers.Helpers;
using Data.Constant;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class EncounterEntryFactory
    {
        public List<EncounterEntry> GetObjectsByEncounterId(Guid encounterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<EncounterEntry>("encounterEntries");
                var entries = collection.Find(e => e.EncounterId == encounterId).ToList();
                foreach (var entry in entries)
                    entry.SetInternalState(InternalStates.UnModified, true);
                return entries;
            }
        }

        public void DeleteAllByEncounterId(Guid encounterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<EncounterEntry>("encounterEntries");
                collection.DeleteMany(e => e.EncounterId == encounterId);
            }
        }

        public void SaveObject(EncounterEntry entry)
        {
            if (entry.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<EncounterEntry>("encounterEntries");
                    switch (entry.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(entry);
                            break;
                        case InternalStates.Modified:
                            collection.Update(entry);
                            break;
                        case InternalStates.Deleted:
                            collection.Delete(entry.Id);
                            break;
                    }
                    entry.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
