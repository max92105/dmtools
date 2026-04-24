using Controllers.Factories;
using Data.Constants;
using Data.DataModels.MonsterEditionPage;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Controllers.Controllers
{
    public static class MonsterEditionPageDataController
    {
        public static MonsterEditionPageDataModel LoadNewMonster()
        {
            MonsterEditionPageDataModel monsterEditionPageDataModel = new MonsterEditionPageDataModel();

            NewMonster(monsterEditionPageDataModel);

            return monsterEditionPageDataModel;
        }

        public static MonsterEditionPageDataModel LoadMonster(Guid monsterId)
        {
            MonsterEditionPageDataModel monsterEditionPageDataModel = new MonsterEditionPageDataModel();

            monsterEditionPageDataModel.Monster = new MonsterFactory().GetObject(monsterId);

            CharacteristicFactory characteristicFactory = new CharacteristicFactory();

            monsterEditionPageDataModel.Strength = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.StrenghtId, monsterId);
            monsterEditionPageDataModel.Dexterity = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.DexterityId, monsterId);
            monsterEditionPageDataModel.Constitution = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.ConstitutionId, monsterId);
            monsterEditionPageDataModel.Intelligence = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.IntelligenceId, monsterId);
            monsterEditionPageDataModel.Wisdom = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.WisdomId, monsterId);
            monsterEditionPageDataModel.Charisma = characteristicFactory.GetObjectByCharacterisitcTypeIdAndMonsterId(Characteristics.CharismaId, monsterId);

            monsterEditionPageDataModel.DisplaySkills = new ObservableCollection<DisplaySkill>(new DisplaySkillFactory().GetObjectsByMonsterId(monsterId));
            monsterEditionPageDataModel.SpecialAbilities = new ObservableCollection<SpecialAbility>(new SpecialAbilityFactory().GetObjectsByMonsterId(monsterId));
            monsterEditionPageDataModel.Actions = new ObservableCollection<Data.Objects.Action>(new ActionFactory().GetObjectsMonsterId(monsterId));

            return monsterEditionPageDataModel;
        }

        public static MonsterEditionPageDataModel NewMonster(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            monsterEditionPageDataModel.Monster = new Monster() { Id = Guid.NewGuid(), Name = "New Monster" };

            monsterEditionPageDataModel.Strength = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.StrenghtId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Dexterity = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.DexterityId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Constitution = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.ConstitutionId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Intelligence = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.IntelligenceId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Wisdom = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.WisdomId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Charisma = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterEditionPageDataModel.Monster.Id,
                CharacteristicTypeId = Characteristics.CharismaId,
                Score = 10,
                Save = 0
            };

            return monsterEditionPageDataModel;
        }

        public static void Save(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            CharacteristicFactory characteristicFactory = new CharacteristicFactory();
            SkillFactory skillFactory = new SkillFactory();
            SpecialAbilityFactory specialAbilityFactory = new SpecialAbilityFactory();
            ActionFactory actionFactory = new ActionFactory();

            new MonsterFactory().SaveObject(monsterEditionPageDataModel.Monster);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Strength);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Dexterity);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Constitution);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Intelligence);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Wisdom);
            characteristicFactory.SaveObject(monsterEditionPageDataModel.Charisma);

            foreach (Skill skill in monsterEditionPageDataModel.DisplaySkills)
                skillFactory.SaveObject(skill);

            foreach (SpecialAbility specialAbility in monsterEditionPageDataModel.SpecialAbilities)
                specialAbilityFactory.SaveObject(specialAbility);

            foreach (Data.Objects.Action action in monsterEditionPageDataModel.Actions)
                actionFactory.SaveObject(action); 
        }

        public static void Copy(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            Guid monsterId = Guid.NewGuid();
            monsterEditionPageDataModel.Monster.Name = String.Empty;
            monsterEditionPageDataModel.Monster.Id = monsterId;
            monsterEditionPageDataModel.Monster.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Strength.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Strength.MonsterId = monsterId;
            monsterEditionPageDataModel.Strength.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Dexterity.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Dexterity.MonsterId = monsterId;
            monsterEditionPageDataModel.Dexterity.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Constitution.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Constitution.MonsterId = monsterId;
            monsterEditionPageDataModel.Constitution.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Intelligence.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Intelligence.MonsterId = monsterId;
            monsterEditionPageDataModel.Intelligence.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Wisdom.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Wisdom.MonsterId = monsterId;
            monsterEditionPageDataModel.Wisdom.SetInternalState(Data.Constant.InternalStates.New, true);

            monsterEditionPageDataModel.Charisma.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Charisma.MonsterId = monsterId;
            monsterEditionPageDataModel.Charisma.SetInternalState(Data.Constant.InternalStates.New, true);


            foreach (Skill skill in monsterEditionPageDataModel.DisplaySkills)
            {
                skill.Id = Guid.NewGuid();
                skill.MonsterId = monsterId;
                skill.SetInternalState(Data.Constant.InternalStates.New, true);
            }

            foreach (SpecialAbility specialAbility in monsterEditionPageDataModel.SpecialAbilities)
            {
                specialAbility.Id = Guid.NewGuid();
                specialAbility.MonsterId = monsterId;
                specialAbility.SetInternalState(Data.Constant.InternalStates.New, true);
            }

            foreach (Data.Objects.Action action in monsterEditionPageDataModel.Actions)
            {
                action.Id = Guid.NewGuid();
                action.MonsterId = monsterId;
                action.SetInternalState(Data.Constant.InternalStates.New, true);
            }
        }

        public static void DeleteSkill(Skill skill)
        {
            SkillFactory skillFactory = new SkillFactory();
            skillFactory.SaveObject(skill);
        }

        public static SpecialAbility GetSpecialAbility(Guid specialAbilityId)
        {
            SpecialAbilityFactory specialAbilityFactory = new SpecialAbilityFactory();

            return specialAbilityFactory.GetObject(specialAbilityId);
        }

        public static List<DisplaySpecialAbility> GetSpecialAbilities()
        {
            DisplaySpecialAbilityFactory displaySpecialAbilityFactory = new DisplaySpecialAbilityFactory();

            return displaySpecialAbilityFactory.GetObjects();
        }

        public static void DeleteSpecialAbility(SpecialAbility specialAbility)
        {
            SpecialAbilityFactory specialAbilityFactory = new SpecialAbilityFactory();
            specialAbilityFactory.SaveObject(specialAbility);
        }

        public static Data.Objects.Action GetAction(Guid actionId)
        {
            ActionFactory actionFactory = new ActionFactory();

            return actionFactory.GetObject(actionId);
        }

        public static List<DisplayAction> GetActions()
        {
            DisplayActionFactory displayActionFactory = new DisplayActionFactory();

            return displayActionFactory.GetObjects();
        }

        public static void DeleteAction(Data.Objects.Action action)
        {
            ActionFactory actionFactory = new ActionFactory();
            actionFactory.SaveObject(action);
        }
    }
}
