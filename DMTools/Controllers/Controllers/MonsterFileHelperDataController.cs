using Controllers.Factories;
using Data.DataModels.MonsterFileHelper;
using Data.Objects;
using System;
using System.Collections.Generic;

namespace Controllers.Controllers
{
    public static class MonsterFileHelperDataController
    {
        public static MonsterFileHelperDataModel LoadMonsters(List<Guid> monsterIds)
        {
            MonsterFileHelperDataModel monsterFileHelperDataModel = new MonsterFileHelperDataModel();

            foreach (Guid monsterId in monsterIds)
            {
                monsterFileHelperDataModel.Monsters.Add(new MonsterFactory().GetObject(monsterId));

                CharacteristicFactory characteristicFactory = new CharacteristicFactory();

                monsterFileHelperDataModel.Characteristics.AddRange( characteristicFactory.GetObjectsByMonsterId(monsterId) );
            
                monsterFileHelperDataModel.Skills.AddRange( new SkillFactory().GetObjectsByMonsterId(monsterId) );
                monsterFileHelperDataModel.SpecialAbilities.AddRange( new SpecialAbilityFactory().GetObjectsByMonsterId(monsterId) );
                monsterFileHelperDataModel.Actions.AddRange( new ActionFactory().GetObjectsMonsterId(monsterId) );
            }

            return monsterFileHelperDataModel;
        }

        public static void ImportMonster(ImportMonsterDataQuery importMonsterDataQuery)
        {
            MonsterFactory monsterFactory = new MonsterFactory();

            foreach (Monster monster in importMonsterDataQuery.Monsters)
                monsterFactory.SaveObject(monster);
        }

        public static void ImportCharacteristic(ImportCharacteristicDataQuery importCharacteristicDataQuery)
        {
            CharacteristicFactory characteristicFactory = new CharacteristicFactory();

            foreach (Characteristic characteristic in importCharacteristicDataQuery.Characteristics)
                characteristicFactory.SaveObject(characteristic);
        }

        public static void ImportSkill(ImportSkillDataQuery importSkillDataQuery)
        {
            SkillFactory skillFactory = new SkillFactory();

            foreach (Skill skill in importSkillDataQuery.Skills)
                skillFactory.SaveObject(skill);
        }

        public static void ImportSpecialAbility(ImportSpecialAbilityDataQuery importSpecialAbilityDataQuery)
        {
            SpecialAbilityFactory specialAbilityFactory = new SpecialAbilityFactory();

            foreach (SpecialAbility specialAbility in importSpecialAbilityDataQuery.SpecialAbilities)
                specialAbilityFactory.SaveObject(specialAbility);
        }

        public static void ImportAction(ImportActionDataQuery importActionDataQuery)
        {
            ActionFactory actionFactory = new ActionFactory();

            foreach (Data.Objects.Action action in importActionDataQuery.Actions)
                actionFactory.SaveObject(action);
        }
    }
}
