using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class ActionFactory
    {
        public Data.Objects.Action GetObject(Guid id)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Data.Objects.Action>("actions");
                var action = collection.FindById(id);
                if (action != null)
                {
                    action.SetInternalState(InternalStates.UnModified, true);
                }
                return action ?? new Data.Objects.Action();
            }
        }

        public List<Data.Objects.Action> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Data.Objects.Action>("actions");
                var actions = collection.FindAll().ToList();
                foreach (var action in actions)
                {
                    action.SetInternalState(InternalStates.UnModified, true);
                }
                return actions;
            }
        }

        public List<Data.Objects.Action> GetObjectsMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Data.Objects.Action>("actions");
                var actions = collection.Find(a => a.MonsterId == monsterId).OrderBy(a => a.SortOrder).ToList();
                foreach (var action in actions)
                {
                    action.SetInternalState(InternalStates.UnModified, true);
                }
                return actions;
            }
        }

        public void SaveObject(Data.Objects.Action action)
        {
            if (action.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Data.Objects.Action>("actions");

                    switch (action.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(action);
                            break;

                        case InternalStates.Modified:
                            collection.Update(action);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(action.Id);
                            break;
                    }

                    action.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
