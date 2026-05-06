using Data.Constants;
using Data.Objects;
using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseCreationTool.Migrations
{
    public class Migration_1536196904 : LiteDbMigration
    {
        public override long Version => 1536196904;
        public override string Description => "Initial import of SRD monsters, characteristics, skills, special abilities and actions";

        public override void Up(LiteDatabase db)
        {
            var monstersCollection = db.GetCollection<Monster>("monsters");
            var characteristicsCollection = db.GetCollection<Characteristic>("characteristics");
            var characteristicTypesCollection = db.GetCollection<CharacteristicType>("characteristicTypes");
            var skillsCollection = db.GetCollection<Skill>("skills");
            var skillTypesCollection = db.GetCollection<SkillType>("skillTypes");
            var specialAbilitiesCollection = db.GetCollection<SpecialAbility>("specialAbilities");
            var actionsCollection = db.GetCollection<Data.Objects.Action>("actions");

            // Upsert CharacteristicTypes (fixed GUIDs — safe to re-run)
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.StrenghtId, "Strength");
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.DexterityId, "Dexterity");
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.ConstitutionId, "Constitution");
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.IntelligenceId, "Intelligence");
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.WisdomId, "Wisdom");
            InsertCharacteristicType(characteristicTypesCollection, Characteristics.CharismaId, "Charisma");

            // Upsert SkillTypes (fixed GUIDs — safe to re-run)
            InsertSkillType(skillTypesCollection, SkillTypes.AcrobaticsId, "Acrobatics");
            InsertSkillType(skillTypesCollection, SkillTypes.AnimalHandlingId, "AnimalHandling");
            InsertSkillType(skillTypesCollection, SkillTypes.ArcanaId, "Arcana");
            InsertSkillType(skillTypesCollection, SkillTypes.AthleticsId, "Athletics");
            InsertSkillType(skillTypesCollection, SkillTypes.DeceptionId, "Deception");
            InsertSkillType(skillTypesCollection, SkillTypes.HistoryId, "History");
            InsertSkillType(skillTypesCollection, SkillTypes.InsightId, "Insight");
            InsertSkillType(skillTypesCollection, SkillTypes.IntimidationId, "Intimidation");
            InsertSkillType(skillTypesCollection, SkillTypes.InvestigationId, "Investigation");
            InsertSkillType(skillTypesCollection, SkillTypes.MedicineId, "Medicine");
            InsertSkillType(skillTypesCollection, SkillTypes.NatureId, "Nature");
            InsertSkillType(skillTypesCollection, SkillTypes.PerceptionId, "Perception");
            InsertSkillType(skillTypesCollection, SkillTypes.PerformanceId, "Performance");
            InsertSkillType(skillTypesCollection, SkillTypes.PersuasionId, "Persuasion");
            InsertSkillType(skillTypesCollection, SkillTypes.ReligionId, "Religion");
            InsertSkillType(skillTypesCollection, SkillTypes.SleightOfHandId, "SleightOfHand");
            InsertSkillType(skillTypesCollection, SkillTypes.StealthId, "Stealth");
            InsertSkillType(skillTypesCollection, SkillTypes.SurvivalId, "Survival");

            // Skip monster JSON import if monsters already exist in the database
            if (monstersCollection.Count() > 0)
            {
                Console.WriteLine("  Monsters already present — skipping JSON import.");
                monstersCollection.EnsureIndex(m => m.Name);
                characteristicsCollection.EnsureIndex(c => c.MonsterId);
                skillsCollection.EnsureIndex(s => s.MonsterId);
                specialAbilitiesCollection.EnsureIndex(sa => sa.MonsterId);
                actionsCollection.EnsureIndex(a => a.MonsterId);
                return;
            }

            // Import monsters from JSON
            using (StreamReader streamReader = new StreamReader("5e-SRD-Monsters.json"))
            {
                string json = streamReader.ReadToEnd();
                List<MonsterJsonImport> monsters = JsonConvert.DeserializeObject<List<MonsterJsonImport>>(json);

                foreach (MonsterJsonImport monsterImport in monsters)
                {
                    Guid monsterId = Guid.NewGuid();

                    var monster = new Monster
                    {
                        Id = monsterId,
                        Name = monsterImport.name,
                        Size = monsterImport.size,
                        Type = monsterImport.type,
                        Subtype = monsterImport.subtype,
                        Alignment = monsterImport.alignment,
                        ArmorClass = monsterImport.armor_class,
                        HitPoints = monsterImport.hit_points,
                        HitDice = monsterImport.hit_dice,
                        Speed = monsterImport.speed,
                        DamageVulnerabilities = monsterImport.damage_vulnerabilities,
                        DamageResistances = monsterImport.damage_resistances,
                        DamageImmunities = monsterImport.damage_immunities,
                        ConditionImmunities = monsterImport.condition_immunities,
                        Senses = monsterImport.senses,
                        Languages = monsterImport.languages,
                        ChallengeRating = monsterImport.challenge_rating
                    };
                    monstersCollection.Insert(monster);

                    // Characteristics
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.StrenghtId, monsterImport.strength, monsterImport.strength_save);
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.DexterityId, monsterImport.dexterity, monsterImport.dexterity_save);
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.ConstitutionId, monsterImport.constitution, monsterImport.constitution_save);
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.IntelligenceId, monsterImport.intelligence, monsterImport.intelligence_save);
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.WisdomId, monsterImport.wisdom, monsterImport.wisdom_save);
                    InsertCharacteristic(characteristicsCollection, monsterId, Characteristics.CharismaId, monsterImport.charisma, monsterImport.charisma_save);

                    // Skills
                    if (monsterImport.acrobatics != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.AcrobaticsId, monsterImport.acrobatics);
                    if (monsterImport.arcana != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.ArcanaId, monsterImport.arcana);
                    if (monsterImport.athletics != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.AthleticsId, monsterImport.athletics);
                    if (monsterImport.deception != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.DeceptionId, monsterImport.deception);
                    if (monsterImport.history != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.HistoryId, monsterImport.history);
                    if (monsterImport.insight != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.InsightId, monsterImport.insight);
                    if (monsterImport.intimidation != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.IntimidationId, monsterImport.intimidation);
                    if (monsterImport.investigation != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.InvestigationId, monsterImport.investigation);
                    if (monsterImport.medicine != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.MedicineId, monsterImport.medicine);
                    if (monsterImport.nature != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.NatureId, monsterImport.nature);
                    if (monsterImport.perception != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.PerceptionId, monsterImport.perception);
                    if (monsterImport.performance != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.PerformanceId, monsterImport.performance);
                    if (monsterImport.persuasion != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.PersuasionId, monsterImport.persuasion);
                    if (monsterImport.religion != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.ReligionId, monsterImport.religion);
                    if (monsterImport.stealth != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.StealthId, monsterImport.stealth);
                    if (monsterImport.survival != 0)
                        InsertSkill(skillsCollection, monsterId, SkillTypes.SurvivalId, monsterImport.survival);

                    // Special Abilities
                    if (monsterImport.special_abilities != null)
                    {
                        foreach (SpecialAbilityImport sa in monsterImport.special_abilities)
                        {
                            var specialAbility = new SpecialAbility
                            {
                                Id = Guid.NewGuid(),
                                MonsterId = monsterId,
                                Name = sa.name,
                                Description = sa.desc,
                                AttackBonus = sa.attack_bonus
                            };
                            specialAbilitiesCollection.Insert(specialAbility);
                        }
                    }

                    // Actions
                    if (monsterImport.actions != null)
                    {
                        foreach (ActionImport action in monsterImport.actions)
                        {
                            var newAction = new Data.Objects.Action
                            {
                                Id = Guid.NewGuid(),
                                MonsterId = monsterId,
                                Name = action.name,
                                Description = action.desc,
                                AttackBonus = action.attack_bonus,
                                DamageDice = action.damage_dice,
                                DamageBonus = action.damage_bonus,
                                IsLegendary = false
                            };
                            actionsCollection.Insert(newAction);
                        }
                    }

                    // Legendary Actions
                    if (monsterImport.legendary_actions != null)
                    {
                        foreach (LegendaryAction legendaryAction in monsterImport.legendary_actions)
                        {
                            var newAction = new Data.Objects.Action
                            {
                                Id = Guid.NewGuid(),
                                MonsterId = monsterId,
                                Name = legendaryAction.name,
                                Description = legendaryAction.desc,
                                AttackBonus = legendaryAction.attack_bonus,
                                IsLegendary = true
                            };
                            actionsCollection.Insert(newAction);
                        }
                    }
                }
            }

            // Create indexes
            monstersCollection.EnsureIndex(m => m.Name);
            characteristicsCollection.EnsureIndex(c => c.MonsterId);
            skillsCollection.EnsureIndex(s => s.MonsterId);
            specialAbilitiesCollection.EnsureIndex(sa => sa.MonsterId);
            actionsCollection.EnsureIndex(a => a.MonsterId);
        }

        public override void Down(LiteDatabase db)
        {
            db.DropCollection("monsters");
            db.DropCollection("characteristics");
            db.DropCollection("characteristicTypes");
            db.DropCollection("skills");
            db.DropCollection("skillTypes");
            db.DropCollection("specialAbilities");
            db.DropCollection("actions");
        }

        private void InsertCharacteristicType(ILiteCollection<CharacteristicType> collection, Guid id, string name)
        {
            collection.Upsert(new CharacteristicType { Id = id, Name = name });
        }

        private void InsertSkillType(ILiteCollection<SkillType> collection, Guid id, string name)
        {
            collection.Upsert(new SkillType { Id = id, Name = name });
        }

        private void InsertCharacteristic(ILiteCollection<Characteristic> collection, Guid monsterId, Guid characteristicTypeId, short score, short save)
        {
            var c = new Characteristic
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = characteristicTypeId,
                Score = score,
                Save = save
            };
            collection.Insert(c);
        }

        private void InsertSkill(ILiteCollection<Skill> collection, Guid monsterId, Guid skillTypeId, short save)
        {
            var s = new Skill
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                SkillTypeId = skillTypeId,
                Save = save
            };
            collection.Insert(s);
        }
    }
}
