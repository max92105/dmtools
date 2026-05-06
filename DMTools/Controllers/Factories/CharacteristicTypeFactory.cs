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
    public class CharacteristicTypeFactory
    {
        public List<CharacteristicType> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<CharacteristicType>("characteristicTypes");
                var characteristicTypes = collection.FindAll().ToList();
                foreach (var characteristicType in characteristicTypes)
                {
                    characteristicType.SetInternalState(InternalStates.UnModified, true);
                }
                return characteristicTypes;
            }
        }
    }
}
