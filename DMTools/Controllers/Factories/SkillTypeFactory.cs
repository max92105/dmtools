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
    public class SkillTypeFactory
    {
        public List<SkillType> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<SkillType>("skillTypes");
                var skillTypes = collection.FindAll().OrderBy(s => s.Name).ToList();
                foreach (var skillType in skillTypes)
                {
                    skillType.SetInternalState(InternalStates.UnModified, true);
                }
                return skillTypes;
            }
        }
    }
}
