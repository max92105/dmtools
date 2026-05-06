using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class ArmorClassEntryFactory
    {
        public List<ArmorClassEntry> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<ArmorClassEntry>("armorClassEntries");
                var entries = collection.Find(a => a.MonsterId == monsterId).ToList();
                foreach (var entry in entries)
                {
                    entry.SetInternalState(InternalStates.UnModified, true);
                }
                return entries;
            }
        }

        public void SaveObject(ArmorClassEntry armorClassEntry)
        {
            if (armorClassEntry.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<ArmorClassEntry>("armorClassEntries");

                    switch (armorClassEntry.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(armorClassEntry);
                            break;

                        case InternalStates.Modified:
                            collection.Update(armorClassEntry);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(armorClassEntry.Id);
                            break;
                    }

                    armorClassEntry.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
