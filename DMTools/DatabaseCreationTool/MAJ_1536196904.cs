using Data.Constants;
using Data.Objects;
using FluentMigrator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DatabaseCreationTool
{
    [Migration(1536196904)]
    public class MAJ_1536196904 : Migration
    {
        public override void Up()
        {
            using (StreamReader streamReader = new StreamReader("5e-SRD-Monsters.json"))
            {
                string json = streamReader.ReadToEnd();
                List<MonsterJsonImport> monsters = JsonConvert.DeserializeObject<List<MonsterJsonImport>>(json);

                Create.Table("Monsters").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("Name").AsString().NotNullable()
                    .WithColumn("Size").AsString().NotNullable()
                    .WithColumn("Type").AsString().NotNullable()
                    .WithColumn("Subtype").AsString().Nullable()
                    .WithColumn("Alignment").AsString().NotNullable()
                    .WithColumn("ArmorClass").AsInt16().NotNullable()
                    .WithColumn("HitPoints").AsInt16().NotNullable()
                    .WithColumn("HitDice").AsString().NotNullable()
                    .WithColumn("Speed").AsString().NotNullable()
                    .WithColumn("DamageVulnerabilities").AsString().Nullable()
                    .WithColumn("DamageResistances").AsString().Nullable()
                    .WithColumn("DamageImmunities").AsString().Nullable()
                    .WithColumn("ConditionImmunities").AsString().Nullable()
                    .WithColumn("Senses").AsString().NotNullable()
                    .WithColumn("Languages").AsString().NotNullable()
                    .WithColumn("ChallengeRating").AsDecimal(9,2).NotNullable();

                Create.Table("Characteristics").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("MonsterId").AsGuid().NotNullable()
                    .WithColumn("CharacteristicTypeId").AsGuid().NotNullable()
                    .WithColumn("Score").AsInt16().NotNullable()
                    .WithColumn("Save").AsInt16().NotNullable();

                Create.Table("CharacteristicTypes").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("Name").AsString().NotNullable();

                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.StrenghtId, Name = "Strength" });
                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.DexterityId, Name = "Dexterity" });
                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.ConstitutionId, Name = "Constitution" });
                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.IntelligenceId, Name = "Intelligence" });
                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.WisdomId, Name = "Wisdom" });
                Insert.IntoTable("CharacteristicTypes").Row(new { Id = Characteristics.CharismaId, Name = "Charisma" });

                Create.Table("Skills").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("MonsterId").AsGuid().NotNullable()
                    .WithColumn("SkillTypeId").AsGuid().NotNullable()
                    .WithColumn("Save").AsInt16().NotNullable();

                Create.Table("SkillTypes").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("Name").AsString().NotNullable();

                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.AcrobaticsId, Name = "Acrobatics" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.AnimalHandlingId, Name = "AnimalHandling" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.ArcanaId, Name = "Arcana" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.AthleticsId, Name = "Athletics" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.DeceptionId, Name = "Deception" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.HistoryId, Name = "History" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.InsightId, Name = "Insight" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.IntimidationId, Name = "Intimidation" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.InvestigationId, Name = "Investigation" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.MedicineId, Name = "Medicine" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.NatureId, Name = "Nature" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.PerceptionId, Name = "Perception" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.PerformanceId, Name = "Performance" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.PersuasionId, Name = "Persuasion" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.ReligionId, Name = "Religion" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.SleightOfHandId, Name = "SleightOfHand" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.StealthId, Name = "Stealth" });
                Insert.IntoTable("SkillTypes").Row(new { Id = SkillTypes.SurvivalId, Name = "Survival" });

                Create.Table("SpecialAbilities").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("MonsterId").AsGuid().NotNullable()
                    .WithColumn("Name").AsString().NotNullable()
                    .WithColumn("Description").AsString().NotNullable()
                    .WithColumn("AttackBonus").AsInt16().Nullable();

                Create.Table("Actions").WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
                    .WithColumn("MonsterId").AsGuid().NotNullable()
                    .WithColumn("Name").AsString().NotNullable()
                    .WithColumn("Description").AsString().NotNullable()
                    .WithColumn("AttackBonus").AsInt16().Nullable()
                    .WithColumn("DamageDice").AsString().Nullable()
                    .WithColumn("DamageBonus").AsInt16().Nullable()
                    .WithColumn("IsLegendary").AsBoolean().NotNullable();

                foreach (MonsterJsonImport monster in monsters)
                {
                    Guid newMonsterId = Guid.NewGuid();

                    Insert.IntoTable("Monsters").Row(new
                    {
                        Id = newMonsterId,
                        Name = monster.name,
                        Size = monster.size,
                        Type = monster.type,
                        Subtype = monster.subtype,
                        Alignment = monster.alignment,
                        ArmorClass = monster.armor_class,
                        HitPoints = monster.hit_points,
                        HitDice = monster.hit_dice,
                        Speed = monster.speed,
                        DamageVulnerabilities = monster.damage_vulnerabilities,
                        DamageResistances = monster.damage_resistances,
                        DamageImmunities = monster.damage_immunities,
                        ConditionImmunities = monster.condition_immunities,
                        Senses = monster.senses,
                        Languages = monster.languages,
                        ChallengeRating = monster.challenge_rating
                    });

                    //Insert Strength Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.StrenghtId, monster.strength, monster.strength_save);

                    //Insert Dexterity Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.DexterityId, monster.dexterity, monster.dexterity_save);

                    //Insert Constitution Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.ConstitutionId, monster.constitution, monster.constitution_save);

                    //Insert Intelligence Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.IntelligenceId, monster.intelligence, monster.intelligence_save);

                    //Insert Wisdom Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.WisdomId, monster.wisdom, monster.wisdom_save);

                    //Insert Charisma Characteristic
                    CreateCharacteristicEntry(newMonsterId, Characteristics.CharismaId, monster.charisma, monster.charisma_save);

                    //Skills
                    if (monster.acrobatics != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.AcrobaticsId, monster.acrobatics);

                    if (monster.arcana != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.ArcanaId, monster.arcana);

                    if (monster.athletics != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.AthleticsId, monster.athletics);

                    if (monster.deception != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.DeceptionId, monster.deception);

                    if (monster.history != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.HistoryId, monster.history);

                    if (monster.insight != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.InsightId, monster.insight);

                    if (monster.intimidation != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.IntimidationId, monster.intimidation);

                    if (monster.investigation != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.InvestigationId, monster.investigation);

                    if (monster.medicine != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.MedicineId, monster.medicine);

                    if (monster.nature != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.NatureId, monster.nature);

                    if (monster.perception != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.PerceptionId, monster.perception);

                    if (monster.performance != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.PerformanceId, monster.performance);

                    if (monster.persuasion != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.PersuasionId, monster.persuasion);

                    if (monster.religion != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.ReligionId, monster.religion);

                    if (monster.stealth != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.StealthId, monster.stealth);

                    if (monster.survival != 0)
                        CreateSkillEntry(newMonsterId, SkillTypes.SurvivalId, monster.survival);

                    //SpecialAbilities
                    if (monster.special_abilities != null)
                    {
                        foreach (SpecialAbilityImport specialAbility in monster.special_abilities)
                        {
                            if (specialAbility.attack_bonus != 0)
                            {
                                Insert.IntoTable("SpecialAbilities").Row(new
                                {
                                    Id = Guid.NewGuid(),
                                    MonsterId = newMonsterId,
                                    Name = specialAbility.name,
                                    Description = specialAbility.desc,
                                    AttackBonus = specialAbility.attack_bonus
                                });
                            }
                            else
                            {
                                Insert.IntoTable("SpecialAbilities").Row(new
                                {
                                    Id = Guid.NewGuid(),
                                    MonsterId = newMonsterId,
                                    Name = specialAbility.name,
                                    Description = specialAbility.desc
                                });
                            }
                        }
                    }

                    //Actions
                    if (monster.actions != null)
                    {
                        foreach (Data.Objects.ActionImport action in monster.actions)
                        {
                            Insert.IntoTable("Actions").Row(new
                            {
                                Id = Guid.NewGuid(),
                                MonsterId = newMonsterId,
                                Name = action.name,
                                Description = action.desc,
                                AttackBonus = action.attack_bonus,
                                DamageDice = action.damage_dice,
                                DamageBonus = action.damage_bonus,
                                IsLegendary = 0
                            });
                        }
                    }

                    //Legendary Actions
                    if (monster.legendary_actions != null)
                    {
                        foreach (LegendaryAction legendaryAction in monster.legendary_actions)
                        {
                            Insert.IntoTable("Actions").Row(new
                            {
                                Id = Guid.NewGuid(),
                                MonsterId = newMonsterId,
                                Name = legendaryAction.name,
                                Description = legendaryAction.desc,
                                AttackBonus = legendaryAction.attack_bonus,
                                IsLegendary = 1,
                            });
                        }
                    }
                }
            }
        }

        public void CreateCharacteristicEntry(Guid monsterId, 
            Guid characteristicTypeId, int score, int save)
        {
            Insert.IntoTable("Characteristics").Row(new
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                CharacteristicTypeId = characteristicTypeId,
                Score = score,
                Save = save
            });
        }

        public void CreateSkillEntry(Guid monsterId, Guid skillTypeId, int save)
        {
            Insert.IntoTable("Skills").Row(new
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                SkillTypeId = skillTypeId,
                Save = save
            });
        }

        public override void Down()
        {
            throw new NotImplementedException();
        }
    }
}