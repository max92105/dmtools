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
    public class SpecialAbilityFactory
    {
        public SpecialAbility GetObject(Guid id)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<SpecialAbility>("specialAbilities");
                var specialAbility = collection.FindById(id);
                if (specialAbility != null)
                {
                    specialAbility.SetInternalState(InternalStates.UnModified, true);
                }
                return specialAbility ?? new SpecialAbility();
            }
        }

        public List<SpecialAbility> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<SpecialAbility>("specialAbilities");
                var specialAbilities = collection.FindAll().ToList();
                foreach (var specialAbility in specialAbilities)
                {
                    specialAbility.SetInternalState(InternalStates.UnModified, true);
                }
                return specialAbilities;
            }
        }

        public List<SpecialAbility> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<SpecialAbility>("specialAbilities");
                var specialAbilities = collection.Find(s => s.MonsterId == monsterId).ToList();
                foreach (var specialAbility in specialAbilities)
                {
                    specialAbility.SetInternalState(InternalStates.UnModified, true);
                }
                return specialAbilities;
            }
        }

        public void SaveObject(SpecialAbility specialAbility)
        {
            if (specialAbility.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<SpecialAbility>("specialAbilities");

                    switch (specialAbility.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(specialAbility);
                            break;

                        case InternalStates.Modified:
                            collection.Update(specialAbility);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(specialAbility.Id);
                            break;
                    }

                    specialAbility.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
