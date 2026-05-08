using Controllers.Controllers;
using Controllers.Factories;
using Data.DataModels.MonsterFileHelper;
using Data.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows.Forms;

namespace DMTools.Helpers
{
    public class MonsterFileHelper
    {
        private static MonsterFileHelperDataModel _MonsterFileHelperDataModel;
        private static Dictionary<Guid, Guid> _MonsterIds = new Dictionary<Guid, Guid>();

        private static String _ArchiveFileName = @"\MonsterExport.dmtm";

        private const String _MonsterFileName          = "Monsters.json";
        private const String _SpecialAbilityFileName   = "SpecialAbilities.json";
        private const String _CharacteristicFileName   = "Characteritics.json";
        private const String _SkillFileName            = "Skills.json";
        private const String _ActionFileName           = "Actions.json";
        private const String _ArmorClassEntryFileName  = "ArmorClassEntries.json";
        private const String _SpeedFileName            = "Speeds.json";
        private const String _SenseFileName            = "Senses.json";
        private const String _DamageModifierFileName   = "DamageModifiers.json";

        public static void ExportMonsters(List<Guid> monsterIds)
        {
            _MonsterFileHelperDataModel = MonsterFileHelperDataController.LoadMonsters(monsterIds);

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                string zipFolderPath = folderBrowserDialog.SelectedPath;

                CreateArchive(zipFolderPath,
                    GenerateMonsterJson(),
                    GenerateSpecialAbilityJson(),
                    GenerateCharacteristicJson(),
                    GenerateSkillJson(),
                    GenerateActionJson(),
                    GenerateArmorClassEntryJson(),
                    GenerateSpeedJson(),
                    GenerateSenseJson(),
                    GenerateDamageModifierJson());

                NotificationHelper.NewNotification("Exportation", "Exportation Succeeded", Notifications.Wpf.NotificationType.Success);
            }
        }

        #region Json Generation

        private static String SerializeList<T>(List<T> items)
        {
            if (items == null || items.Count == 0) return "[]";
            return "[" + string.Join(",", items.Select(i => JsonConvert.SerializeObject(i))) + "]";
        }

        private static String GenerateMonsterJson()
            => SerializeList(_MonsterFileHelperDataModel.Monsters);

        private static String GenerateSpecialAbilityJson()
            => SerializeList(_MonsterFileHelperDataModel.SpecialAbilities);

        private static String GenerateCharacteristicJson()
            => SerializeList(_MonsterFileHelperDataModel.Characteristics);

        private static String GenerateSkillJson()
        {
            var skillTypes = new SkillTypeFactory().GetObjects();
            var entries = _MonsterFileHelperDataModel.Skills.Select(s => new SkillExportEntry
            {
                Id = s.Id,
                MonsterId = s.MonsterId,
                SkillTypeId = s.SkillTypeId,
                SkillTypeName = skillTypes.FirstOrDefault(t => t.Id == s.SkillTypeId)?.Name ?? "",
                Save = s.Save
            }).ToList();
            return SerializeList(entries);
        }

        private static String GenerateActionJson()
            => SerializeList(_MonsterFileHelperDataModel.Actions);

        private static String GenerateArmorClassEntryJson()
            => SerializeList(_MonsterFileHelperDataModel.ArmorClassEntries);

        private static String GenerateSpeedJson()
            => SerializeList(_MonsterFileHelperDataModel.Speeds);

        private static String GenerateSenseJson()
            => SerializeList(_MonsterFileHelperDataModel.Senses);

        private static String GenerateDamageModifierJson()
            => SerializeList(_MonsterFileHelperDataModel.DamageModifiers);

        #endregion

        private static void CreateArchive(String zipFolderPath,
            String sMonsterJson,
            String sSpecialAbilityJson,
            String sCharacteristicJson,
            String sSkillJson,
            String sActionJson,
            String sArmorClassEntryJson,
            String sSpeedJson,
            String sSenseJson,
            String sDamageModifierJson)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    WriteEntry(archive, _MonsterFileName,         sMonsterJson);
                    WriteEntry(archive, _SpecialAbilityFileName,  sSpecialAbilityJson);
                    WriteEntry(archive, _SkillFileName,           sSkillJson);
                    WriteEntry(archive, _CharacteristicFileName,  sCharacteristicJson);
                    WriteEntry(archive, _ActionFileName,          sActionJson);
                    WriteEntry(archive, _ArmorClassEntryFileName, sArmorClassEntryJson);
                    WriteEntry(archive, _SpeedFileName,           sSpeedJson);
                    WriteEntry(archive, _SenseFileName,           sSenseJson);
                    WriteEntry(archive, _DamageModifierFileName,  sDamageModifierJson);
                }

                using (var fileStream = new FileStream(zipFolderPath + _ArchiveFileName, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        private static void WriteEntry(ZipArchive archive, string name, string content)
        {
            var entry = archive.CreateEntry(name);
            using (var stream = entry.Open())
            using (var writer = new StreamWriter(stream))
                writer.Write(content);
        }

        public static void ImportMonsters()
        {
            _MonsterIds = new Dictionary<Guid, Guid>();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.dmtm)|*.dmtm";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(openFileDialog.FileName))
            {
                // Monsters must be processed first so _MonsterIds is populated before dependents
                var entries = new Dictionary<string, string>();
                using (ZipArchive archive = ZipFile.OpenRead(openFileDialog.FileName))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        using (var stream = entry.Open())
                        using (var reader = new StreamReader(stream))
                            entries[entry.Name] = reader.ReadToEnd();
                    }
                }

                // Process monsters first
                if (entries.ContainsKey(_MonsterFileName))
                    Import(_MonsterFileName, entries[_MonsterFileName]);

                // Then all dependents
                foreach (var kv in entries)
                {
                    if (kv.Key != _MonsterFileName)
                        Import(kv.Key, kv.Value);
                }

                NotificationHelper.NewNotification("Importation", "Importation Succeeded", Notifications.Wpf.NotificationType.Success);
            }
        }

        private static void Import(String fileNameKey, String json)
        {
            switch (fileNameKey)
            {
                case _MonsterFileName:
                {
                    var query = new ImportMonsterDataQuery();
                    query.Monsters = JsonConvert.DeserializeObject<Monster[]>(json).ToList();
                    foreach (Monster monster in query.Monsters)
                    {
                        Guid newId = Guid.NewGuid();
                        _MonsterIds.Add(monster.Id, newId);
                        monster.Id = newId;
                    }
                    MonsterFileHelperDataController.ImportMonster(query);
                    break;
                }

                case _SpecialAbilityFileName:
                {
                    var query = new ImportSpecialAbilityDataQuery();
                    query.SpecialAbilities = JsonConvert.DeserializeObject<SpecialAbility[]>(json).ToList();
                    foreach (SpecialAbility sa in query.SpecialAbilities)
                    {
                        sa.Id = Guid.NewGuid();
                        sa.MonsterId = _MonsterIds[sa.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportSpecialAbility(query);
                    break;
                }

                case _CharacteristicFileName:
                {
                    var query = new ImportCharacteristicDataQuery();
                    query.Characteristics = JsonConvert.DeserializeObject<Characteristic[]>(json).ToList();
                    foreach (Characteristic c in query.Characteristics)
                    {
                        c.Id = Guid.NewGuid();
                        c.MonsterId = _MonsterIds[c.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportCharacteristic(query);
                    break;
                }

                case _SkillFileName:
                {
                    var skillTypes = new SkillTypeFactory().GetObjects();
                    var entries = JsonConvert.DeserializeObject<SkillExportEntry[]>(json).ToList();
                    var query = new ImportSkillDataQuery();
                    query.Skills = new List<Skill>();
                    foreach (SkillExportEntry entry in entries)
                    {
                        // Resolve SkillTypeId by name — required for cross-DB compatibility
                        SkillType localType = skillTypes.FirstOrDefault(t =>
                            string.Equals(t.Name, entry.SkillTypeName, StringComparison.OrdinalIgnoreCase));
                        if (localType == null) continue;

                        var skill = new Skill();
                        skill.Id = Guid.NewGuid();
                        skill.MonsterId = _MonsterIds[entry.MonsterId];
                        skill.SkillTypeId = localType.Id;
                        skill.Save = entry.Save;
                        query.Skills.Add(skill);
                    }
                    MonsterFileHelperDataController.ImportSkill(query);
                    break;
                }

                case _ActionFileName:
                {
                    var query = new ImportActionDataQuery();
                    query.Actions = JsonConvert.DeserializeObject<Data.Objects.Action[]>(json).ToList();
                    foreach (Data.Objects.Action action in query.Actions)
                    {
                        action.Id = Guid.NewGuid();
                        action.MonsterId = _MonsterIds[action.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportAction(query);
                    break;
                }

                case _ArmorClassEntryFileName:
                {
                    var query = new ImportArmorClassEntryDataQuery();
                    query.ArmorClassEntries = JsonConvert.DeserializeObject<ArmorClassEntry[]>(json).ToList();
                    foreach (ArmorClassEntry entry in query.ArmorClassEntries)
                    {
                        entry.Id = Guid.NewGuid();
                        entry.MonsterId = _MonsterIds[entry.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportArmorClassEntry(query);
                    break;
                }

                case _SpeedFileName:
                {
                    var query = new ImportSpeedDataQuery();
                    query.Speeds = JsonConvert.DeserializeObject<Speed[]>(json).ToList();
                    foreach (Speed speed in query.Speeds)
                    {
                        speed.Id = Guid.NewGuid();
                        speed.MonsterId = _MonsterIds[speed.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportSpeed(query);
                    break;
                }

                case _SenseFileName:
                {
                    var query = new ImportSenseDataQuery();
                    query.Senses = JsonConvert.DeserializeObject<Sense[]>(json).ToList();
                    foreach (Sense sense in query.Senses)
                    {
                        sense.Id = Guid.NewGuid();
                        sense.MonsterId = _MonsterIds[sense.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportSense(query);
                    break;
                }

                case _DamageModifierFileName:
                {
                    var query = new ImportDamageModifierDataQuery();
                    query.DamageModifiers = JsonConvert.DeserializeObject<DamageModifier[]>(json).ToList();
                    foreach (DamageModifier modifier in query.DamageModifiers)
                    {
                        modifier.Id = Guid.NewGuid();
                        modifier.MonsterId = _MonsterIds[modifier.MonsterId];
                    }
                    MonsterFileHelperDataController.ImportDamageModifier(query);
                    break;
                }
            }
        }
    }
}
