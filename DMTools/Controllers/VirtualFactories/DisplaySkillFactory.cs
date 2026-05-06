using Controllers.Helpers;
using Data.Constant;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Factories
{
    public class DisplaySkillFactory
    {
        public List<DisplaySkill> GetObjectsByMonsterId(Guid monsterId)
        {
            using (var db = DatabaseHelper.GetDatabase())
            {
                var skillsCollection = db.GetCollection<Skill>("skills");
                var skillTypesCollection = db.GetCollection<SkillType>("skillTypes");

                var skills = skillsCollection.Find(s => s.MonsterId == monsterId).ToList();
                var skillTypes = skillTypesCollection.FindAll().ToDictionary(st => st.Id, st => st.Name);

                var displaySkills = skills.Select(s => new DisplaySkill
                {
                    Id = s.Id,
                    MonsterId = s.MonsterId,
                    SkillTypeId = s.SkillTypeId,
                    Name = skillTypes.ContainsKey(s.SkillTypeId) ? skillTypes[s.SkillTypeId] : "Unknown",
                    Save = s.Save
                }).OrderBy(ds => ds.Name).ToList();

                foreach (var ds in displaySkills)
                {
                    ds.SetInternalState(InternalStates.UnModified, true);
                }

                return displaySkills;
            }
        }
    }
}
