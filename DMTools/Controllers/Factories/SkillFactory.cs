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
    public class SkillFactory
    {
        public List<Skill> GetObjects()
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Skill>("skills");
                var skills = collection.FindAll().ToList();
                foreach (var skill in skills)
                {
                    skill.SetInternalState(InternalStates.UnModified, true);
                }
                return skills;
            }
        }

        public List<Skill> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var collection = db.GetCollection<Skill>("skills");
                var skills = collection.Find(s => s.MonsterId == monsterId).ToList();
                foreach (var skill in skills)
                {
                    skill.SetInternalState(InternalStates.UnModified, true);
                }
                return skills;
            }
        }

        public void SaveObject(Skill skill)
        {
            if (skill.InternalState != InternalStates.UnModified)
            {
                using (var db = DatabaseHelper.GetDatabase())
                {
                    var collection = db.GetCollection<Skill>("skills");

                    switch (skill.InternalState)
                    {
                        case InternalStates.New:
                            collection.Insert(skill);
                            break;

                        case InternalStates.Modified:
                            collection.Update(skill);
                            break;

                        case InternalStates.Deleted:
                            collection.Delete(skill.Id);
                            break;
                    }

                    skill.SetInternalState(InternalStates.UnModified, true);
                }
            }
        }
    }
}
