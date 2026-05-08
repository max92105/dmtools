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
    public class SpeedFactory
    {
        public List<Speed> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Speed>("speeds");
                var speeds = collection.Find(s => s.MonsterId == monsterId).OrderBy(s => s.SortOrder).ToList();
                foreach (var speed in speeds)
                {
                    speed.SetInternalState(InternalStates.UnModified, true);
                }
                return speeds;
            }
        }

        public void SaveObject(Speed speed)
        {
            if (speed.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Speed>("speeds");

                    switch (speed.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(speed);
                            break;

                        case InternalStates.Modified:
                            collection.Update(speed);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(speed.Id);
                            break;
                    }

                    speed.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
