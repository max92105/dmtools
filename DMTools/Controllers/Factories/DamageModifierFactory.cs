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
    public class DamageModifierFactory
    {
        public List<DamageModifier> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<DamageModifier>("damageModifiers");
                var modifiers = collection.Find(d => d.MonsterId == monsterId).OrderBy(d => d.SortOrder).ToList();
                foreach (var modifier in modifiers)
                {
                    modifier.SetInternalState(InternalStates.UnModified, true);
                }
                return modifiers;
            }
        }

        public void SaveObject(DamageModifier damageModifier)
        {
            if (damageModifier.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<DamageModifier>("damageModifiers");

                    switch (damageModifier.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(damageModifier);
                            break;

                        case InternalStates.Modified:
                            collection.Update(damageModifier);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(damageModifier.Id);
                            break;
                    }

                    damageModifier.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
