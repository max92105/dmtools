using Controllers.Factories;
using Data.DataModels.StatBlockHelper;
using Data.Objects;
using System;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public static class StatBlockHelperDataController
    {
        public static StatBlockHelperDataModel LoadTypes()
        {
            StatBlockHelperDataModel statBlockDataModel = new StatBlockHelperDataModel();

            statBlockDataModel.CharacteristicTypes = new ObservableCollection<CharacteristicType>(new CharacteristicTypeFactory().GetObjects());
            statBlockDataModel.SkillTypes = new ObservableCollection<SkillType>(new SkillTypeFactory().GetObjects());

            return statBlockDataModel;
        }

        public static StatBlockHelperDataModel LoadMonsterStats(Guid monsterId)
        {
            StatBlockHelperDataModel statBlockDataModel = new StatBlockHelperDataModel();

            statBlockDataModel.Characteristics = new ObservableCollection<Characteristic>(new CharacteristicFactory().GetObjectsByMonsterId(monsterId));         
            statBlockDataModel.Skills = new ObservableCollection<Skill>(new SkillFactory().GetObjectsByMonsterId(monsterId));
            statBlockDataModel.SpecialAbilities = new ObservableCollection<SpecialAbility>(new SpecialAbilityFactory().GetObjectsByMonsterId(monsterId));
            statBlockDataModel.Actions = new ObservableCollection<Data.Objects.Action>(new ActionFactory().GetObjectsMonsterId(monsterId));

            return statBlockDataModel;
        }

        public static StatBlockHelperDataModel LoadMonster(Guid monsterId)
        {
            StatBlockHelperDataModel statBlockDataModel = new StatBlockHelperDataModel();

            statBlockDataModel.Monster = new MonsterFactory().GetObject(monsterId);

            return statBlockDataModel;
        }
    }
}
