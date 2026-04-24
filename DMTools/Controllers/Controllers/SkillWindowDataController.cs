using Controllers.Factories;
using Data.DataModels.SkillWindow;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Controllers.Controllers
{
    public static class SkillWindowDataController
    {
        public static SkillWindowDataModel Load(Guid monsterId)
        {
            SkillWindowDataModel skillWindowDataModel = new SkillWindowDataModel();
            
            skillWindowDataModel.SkillTypes = new ObservableCollection<SkillType>(new SkillTypeFactory().GetObjects());
            skillWindowDataModel.DisplaySkills = new ObservableCollection<DisplaySkill>(new DisplaySkillFactory().GetObjectsByMonsterId(monsterId));

            foreach (SkillType type in skillWindowDataModel.SkillTypes)
            {
                if (!skillWindowDataModel.DisplaySkills.Any(obj => obj.SkillTypeId == type.Id))
                {
                    skillWindowDataModel.DisplaySkills.Add(new DisplaySkill()
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = monsterId,
                        Name = type.Name,
                        SkillTypeId = type.Id
                    });
                }
            }

            return skillWindowDataModel;
        }

        public static void SaveSkill(SaveSkillDataQuery saveSkillDataQuery)
        {
            SkillFactory skillFactory = new SkillFactory();

            foreach (Skill skill in saveSkillDataQuery.DisplaySkills)
                skillFactory.SaveObject(skill);
        }
    }
}
   