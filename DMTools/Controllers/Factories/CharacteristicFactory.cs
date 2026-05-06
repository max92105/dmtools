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
    public class CharacteristicFactory
    {
        public List<Characteristic> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Characteristic>("characteristics");
                var characteristics = collection.FindAll().ToList();
                foreach (var characteristic in characteristics)
                {
                    characteristic.SetInternalState(InternalStates.UnModified, true);
                }
                return characteristics;
            }
        }

        public List<Characteristic> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Characteristic>("characteristics");
                var characteristics = collection.Find(c => c.MonsterId == monsterId).ToList();
                foreach (var characteristic in characteristics)
                {
                    characteristic.SetInternalState(InternalStates.UnModified, true);
                }
                return characteristics;
            }
        }

        public Characteristic GetObjectByCharacterisitcTypeIdAndMonsterId(Guid characteristicTypeId, Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Characteristic>("characteristics");
                var characteristic = collection.FindOne(c => c.CharacteristicTypeId == characteristicTypeId && c.MonsterId == monsterId);
                if (characteristic != null)
                {
                    characteristic.SetInternalState(InternalStates.UnModified, true);
                }
                return characteristic ?? new Characteristic();
            }
        }

        public void SaveObject(Characteristic characteristic)
        {
            if (characteristic.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Characteristic>("characteristics");

                    switch (characteristic.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(characteristic);
                            break;

                        case InternalStates.Modified:
                            collection.Update(characteristic);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(characteristic.Id);
                            break;
                    }

                    characteristic.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
