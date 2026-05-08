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
    public class SenseFactory
    {
        public List<Sense> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Sense>("senses");
                var senses = collection.Find(s => s.MonsterId == monsterId).OrderBy(s => s.SortOrder).ToList();
                foreach (var sense in senses)
                {
                    sense.SetInternalState(InternalStates.UnModified, true);
                }
                return senses;
            }
        }

        public void SaveObject(Sense sense)
        {
            if (sense.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Sense>("senses");

                    switch (sense.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(sense);
                            break;

                        case InternalStates.Modified:
                            collection.Update(sense);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(sense.Id);
                            break;
                    }

                    sense.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
