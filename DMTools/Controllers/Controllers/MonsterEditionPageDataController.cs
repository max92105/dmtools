using Controllers.Factories;
using Data.Constant;
using Data.Constants;
using Data.DataModels.MonsterEditionPage;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

            // Load new structured data
            monsterEditionPageDataModel.Speeds = new ObservableCollection<Speed>(new SpeedFactory().GetObjectsByMonsterId(monsterId));
            monsterEditionPageDataModel.Senses = new ObservableCollection<Sense>(new SenseFactory().GetObjectsByMonsterId(monsterId));
            monsterEditionPageDataModel.DamageModifiers = new ObservableCollection<DamageModifier>(new DamageModifierFactory().GetObjectsByMonsterId(monsterId));
            monsterEditionPageDataModel.ArmorClassEntries = new ObservableCollection<ArmorClassEntry>(new ArmorClassEntryFactory().GetObjectsByMonsterId(monsterId));

            // Parse languages from the monster's Languages string into the collection
            monsterEditionPageDataModel.Languages = ParseLanguages(monsterEditionPageDataModel.Monster.Languages);

            return monsterEditionPageDataModel;
        }

        public static MonsterEditionPageDataModel NewMonster(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            Guid monsterId = Guid.NewGuid();
            monsterEditionPageDataModel.Monster = new Monster() { Id = monsterId, Name = "New Monster", Size = "Medium", Type = "Humanoid", Alignment = "Neutral" };

            monsterEditionPageDataModel.Strength = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.StrenghtId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Dexterity = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.DexterityId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Constitution = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.ConstitutionId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Intelligence = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.IntelligenceId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Wisdom = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.WisdomId,
                Score = 10,
                Save = 0
            };

            monsterEditionPageDataModel.Charisma = new Characteristic()
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = Characteristics.CharismaId,
                Score = 10,
                Save = 0
            };

            // Add a default walking speed
            var defaultSpeed = new Speed() { Id = Guid.NewGuid(), MonsterId = monsterId, SpeedType = "Walk", Value = 30 };
            monsterEditionPageDataModel.Speeds = new ObservableCollection<Speed> { defaultSpeed };

            // Add a default AC entry
            var defaultAc = new ArmorClassEntry() { Id = Guid.NewGuid(), MonsterId = monsterId, Label = "Default", Value = 10 };
            monsterEditionPageDataModel.ArmorClassEntries = new ObservableCollection<ArmorClassEntry> { defaultAc };

            return monsterEditionPageDataModel;
        }

        public static void Save(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            CharacteristicFactory characteristicFactory = new CharacteristicFactory();
            SkillFactory skillFactory = new SkillFactory();
            SpecialAbilityFactory specialAbilityFactory = new SpecialAbilityFactory();
            ActionFactory actionFactory = new ActionFactory();
            SpeedFactory speedFactory = new SpeedFactory();
            SenseFactory senseFactory = new SenseFactory();
            DamageModifierFactory damageModifierFactory = new DamageModifierFactory();
            ArmorClassEntryFactory armorClassEntryFactory = new ArmorClassEntryFactory();

            // Build Languages string from collection before saving
            monsterEditionPageDataModel.Monster.Languages = String.Join(", ", monsterEditionPageDataModel.Languages);

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

            foreach (Speed speed in monsterEditionPageDataModel.Speeds)
                speedFactory.SaveObject(speed);

            foreach (Sense sense in monsterEditionPageDataModel.Senses)
                senseFactory.SaveObject(sense);

            foreach (DamageModifier damageModifier in monsterEditionPageDataModel.DamageModifiers)
                damageModifierFactory.SaveObject(damageModifier);

            foreach (ArmorClassEntry armorClassEntry in monsterEditionPageDataModel.ArmorClassEntries)
                armorClassEntryFactory.SaveObject(armorClassEntry);
        }

        public static void Copy(MonsterEditionPageDataModel monsterEditionPageDataModel)
        {
            Guid monsterId = Guid.NewGuid();
            monsterEditionPageDataModel.Monster.Name = String.Empty;
            monsterEditionPageDataModel.Monster.Id = monsterId;
            monsterEditionPageDataModel.Monster.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Strength.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Strength.MonsterId = monsterId;
            monsterEditionPageDataModel.Strength.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Dexterity.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Dexterity.MonsterId = monsterId;
            monsterEditionPageDataModel.Dexterity.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Constitution.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Constitution.MonsterId = monsterId;
            monsterEditionPageDataModel.Constitution.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Intelligence.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Intelligence.MonsterId = monsterId;
            monsterEditionPageDataModel.Intelligence.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Wisdom.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Wisdom.MonsterId = monsterId;
            monsterEditionPageDataModel.Wisdom.SetInternalState(InternalStates.New, true);

            monsterEditionPageDataModel.Charisma.Id = Guid.NewGuid();
            monsterEditionPageDataModel.Charisma.MonsterId = monsterId;
            monsterEditionPageDataModel.Charisma.SetInternalState(InternalStates.New, true);

            foreach (Skill skill in monsterEditionPageDataModel.DisplaySkills)
            {
                skill.Id = Guid.NewGuid();
                skill.MonsterId = monsterId;
                skill.SetInternalState(InternalStates.New, true);
            }

            foreach (SpecialAbility specialAbility in monsterEditionPageDataModel.SpecialAbilities)
            {
                specialAbility.Id = Guid.NewGuid();
                specialAbility.MonsterId = monsterId;
                specialAbility.SetInternalState(InternalStates.New, true);
            }

            foreach (Data.Objects.Action action in monsterEditionPageDataModel.Actions)
            {
                action.Id = Guid.NewGuid();
                action.MonsterId = monsterId;
                action.SetInternalState(InternalStates.New, true);
            }

            foreach (Speed speed in monsterEditionPageDataModel.Speeds)
            {
                speed.Id = Guid.NewGuid();
                speed.MonsterId = monsterId;
                speed.SetInternalState(InternalStates.New, true);
            }

            foreach (Sense sense in monsterEditionPageDataModel.Senses)
            {
                sense.Id = Guid.NewGuid();
                sense.MonsterId = monsterId;
                sense.SetInternalState(InternalStates.New, true);
            }

            foreach (DamageModifier damageModifier in monsterEditionPageDataModel.DamageModifiers)
            {
                damageModifier.Id = Guid.NewGuid();
                damageModifier.MonsterId = monsterId;
                damageModifier.SetInternalState(InternalStates.New, true);
            }

            foreach (ArmorClassEntry armorClassEntry in monsterEditionPageDataModel.ArmorClassEntries)
            {
                armorClassEntry.Id = Guid.NewGuid();
                armorClassEntry.MonsterId = monsterId;
                armorClassEntry.SetInternalState(InternalStates.New, true);
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

        public static void DeleteSpeed(Speed speed)
        {
            speed.Delete();
            new SpeedFactory().SaveObject(speed);
        }

        public static void DeleteSense(Sense sense)
        {
            sense.Delete();
            new SenseFactory().SaveObject(sense);
        }

        public static void DeleteDamageModifier(DamageModifier damageModifier)
        {
            damageModifier.Delete();
            new DamageModifierFactory().SaveObject(damageModifier);
        }

        public static void DeleteArmorClassEntry(ArmorClassEntry armorClassEntry)
        {
            armorClassEntry.Delete();
            new ArmorClassEntryFactory().SaveObject(armorClassEntry);
        }

        /// <summary>
        /// Parses a comma-separated languages string into an ObservableCollection.
        /// </summary>
        private static ObservableCollection<string> ParseLanguages(string languages)
        {
            var result = new ObservableCollection<string>();
            if (!String.IsNullOrWhiteSpace(languages))
            {
                var parts = languages.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    string trimmed = part.Trim();
                    if (!String.IsNullOrEmpty(trimmed))
                        result.Add(trimmed);
                }
            }
            return result;
        }
    }
}
