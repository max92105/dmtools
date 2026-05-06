using Controllers.Helpers;
using Data.Constant;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class EncounterFactory
    {
        public Encounter GetObject(Guid encounterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Encounter>("encounters");
                var encounter = collection.FindById(encounterId);
                if (encounter != null)
                    encounter.SetInternalState(InternalStates.UnModified, true);
                return encounter ?? new Encounter();
            }
        }

        public List<Encounter> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Encounter>("encounters");
                var encounters = collection.FindAll().OrderBy(e => e.Name).ToList();
                foreach (var encounter in encounters)
                    encounter.SetInternalState(InternalStates.UnModified, true);
                return encounters;
            }
        }

        public void SaveObject(Encounter encounter)
        {
            if (encounter.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Encounter>("encounters");
                    switch (encounter.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(encounter);
                            break;
                        case InternalStates.Modified:
                            collection.Update(encounter);
                            break;
                        case InternalStates.Deleted:
                            collection.Delete(encounter.Id);
                            break;
                    }
                    encounter.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
