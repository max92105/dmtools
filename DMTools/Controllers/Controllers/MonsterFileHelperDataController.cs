using Controllers.Factories;
using Data.DataModels.MonsterFileHelper;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Controllers.Controllers
{
    public static class MonsterFileHelperDataController
    {
        public static MonsterFileHelperDataModel LoadMonsters(List<Guid> monsterIds)
        {
            MonsterFileHelperDataModel model = new MonsterFileHelperDataModel();

            foreach (Guid monsterId in monsterIds)
            {
                model.Monsters.Add(new MonsterFactory().GetObject(monsterId));
                model.Characteristics.AddRange(new CharacteristicFactory().GetObjectsByMonsterId(monsterId));
                model.Skills.AddRange(new SkillFactory().GetObjectsByMonsterId(monsterId));
                model.SpecialAbilities.AddRange(new SpecialAbilityFactory().GetObjectsByMonsterId(monsterId));
                model.Actions.AddRange(new ActionFactory().GetObjectsMonsterId(monsterId));
                model.ArmorClassEntries.AddRange(new ArmorClassEntryFactory().GetObjectsByMonsterId(monsterId));
                model.Speeds.AddRange(new SpeedFactory().GetObjectsByMonsterId(monsterId));
                model.Senses.AddRange(new SenseFactory().GetObjectsByMonsterId(monsterId));
                model.DamageModifiers.AddRange(new DamageModifierFactory().GetObjectsByMonsterId(monsterId));
            }

            return model;
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

        public static void ImportArmorClassEntry(ImportArmorClassEntryDataQuery query)
        {
            ArmorClassEntryFactory factory = new ArmorClassEntryFactory();
            foreach (ArmorClassEntry entry in query.ArmorClassEntries)
                factory.SaveObject(entry);
        }

        public static void ImportSpeed(ImportSpeedDataQuery query)
        {
            SpeedFactory factory = new SpeedFactory();
            foreach (Speed speed in query.Speeds)
                factory.SaveObject(speed);
        }

        public static void ImportSense(ImportSenseDataQuery query)
        {
            SenseFactory factory = new SenseFactory();
            foreach (Sense sense in query.Senses)
                factory.SaveObject(sense);
        }

        public static void ImportDamageModifier(ImportDamageModifierDataQuery query)
        {
            DamageModifierFactory factory = new DamageModifierFactory();
            foreach (DamageModifier modifier in query.DamageModifiers)
                factory.SaveObject(modifier);
        }
    }
}
