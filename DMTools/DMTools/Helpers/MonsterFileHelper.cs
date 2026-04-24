using Controllers.Controllers;
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

        private const String _MonsterFileName = "Monsters.json";
        private const String _SpecialAbilityFileName = "SpecialAbilities.json";
        private const String _CharacteristicFileName = "Characteritics.json";
        private const String _SkillFileName = "Skills.json";
        private const String _ActionFileName = "Actions.json";

        public static void ExportMonsters(List<Guid> monsterIds)
        {
            _MonsterFileHelperDataModel = MonsterFileHelperDataController.LoadMonsters(monsterIds);

            String zipFolderPath = String.Empty;

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                zipFolderPath = folderBrowserDialog.SelectedPath;

                String sMonsterJson = GenerateMonsterJson();
                String sSpecialAbilityJson = GenerateSpecialAbilityJson();
                String sCharacteristicJson = GenerateCharacteristicJson();
                String sSkillJson = GenerateSkillJson();
                String sActionJson = GenerateActionJson();

                CreateArchive(zipFolderPath, sMonsterJson, sSpecialAbilityJson, sCharacteristicJson, sSkillJson, sActionJson);

                NotificationHelper.NewNotification("Exportation", "Exportation Succeeded", Notifications.Wpf.NotificationType.Success);
            }
        }

        #region Json Generation

        private static String GenerateMonsterJson()
        {
            String sMonsterJson = "[";

            foreach (Monster monster in _MonsterFileHelperDataModel.Monsters)
            {
                sMonsterJson += JsonConvert.SerializeObject(monster);

                if (_MonsterFileHelperDataModel.Monsters.Last() != monster)
                    sMonsterJson += ",";
            }

            sMonsterJson += "]";

            return sMonsterJson;
        }

        private static String GenerateSpecialAbilityJson()
        {
            String sSpecialAbilityJson = "[";

            foreach (SpecialAbility specialAbility in _MonsterFileHelperDataModel.SpecialAbilities)
            {
                sSpecialAbilityJson += JsonConvert.SerializeObject(specialAbility);

                if (_MonsterFileHelperDataModel.SpecialAbilities.Last() != specialAbility)
                    sSpecialAbilityJson += ",";
            }

            sSpecialAbilityJson += "]";

            return sSpecialAbilityJson;
        }

        private static String GenerateCharacteristicJson()
        {
            String sCharacteristicJson = "[";

            foreach (Characteristic characteristic in _MonsterFileHelperDataModel.Characteristics)
            {
                sCharacteristicJson += JsonConvert.SerializeObject(characteristic);

                if (_MonsterFileHelperDataModel.Characteristics.Last() != characteristic)
                    sCharacteristicJson += ",";
            }

            sCharacteristicJson += "]";

            return sCharacteristicJson;
        }

        private static String GenerateSkillJson()
        {
            String sSkillJson = "[";

            foreach (Skill skill in _MonsterFileHelperDataModel.Skills)
            {
                sSkillJson += JsonConvert.SerializeObject(skill);

                if (_MonsterFileHelperDataModel.Skills.Last() != skill)
                    sSkillJson += ",";
            }

            sSkillJson += "]";

            return sSkillJson;
        }

        private static String GenerateActionJson()
        {
            String sActionJson = "[";

            foreach (Data.Objects.Action action in _MonsterFileHelperDataModel.Actions)
            {
                sActionJson += JsonConvert.SerializeObject(action);

                if (_MonsterFileHelperDataModel.Actions.Last() != action)
                    sActionJson += ",";
            }

            sActionJson += "]";

            return sActionJson;
        }

        #endregion

        private static void CreateArchive(String zipFolderPath,
            String sMonsterJson,
            String sSpecialAbilityJson,
            String sCharacteristicJson,
            String sSkillJson,
            String sActionJson)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var monstersFile = archive.CreateEntry(_MonsterFileName);

                    using (var entryStream = monstersFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(sMonsterJson);
                    }

                    var specialAbilitiesFile = archive.CreateEntry(_SpecialAbilityFileName);

                    using (var entryStream = specialAbilitiesFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(sSpecialAbilityJson);
                    }

                    var skillsFile = archive.CreateEntry(_SkillFileName);

                    using (var entryStream = skillsFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(sSkillJson);
                    }

                    var characteristicsFile = archive.CreateEntry(_CharacteristicFileName);

                    using (var entryStream = characteristicsFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(sCharacteristicJson);
                    }

                    var actionsFile = archive.CreateEntry(_ActionFileName);

                    using (var entryStream = actionsFile.Open())
                    using (var streamWriter = new StreamWriter(entryStream))
                    {
                        streamWriter.Write(sActionJson);
                    }
                }

                using (var fileStream = new FileStream(zipFolderPath + _ArchiveFileName, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        public static void ImportMonsters()
        {
            String zipFolderPath = String.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All Files (*.dmtm)|*.dmtm";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK && !String.IsNullOrEmpty(openFileDialog.FileName))
            {
                zipFolderPath = openFileDialog.FileName;    

                using (ZipArchive archive = ZipFile.OpenRead(zipFolderPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        using (var stream = entry.Open())
                        using (var reader = new StreamReader(stream))
                        {
                            String fileNameKey = entry.Name;
                            String json = reader.ReadToEnd();

                            Import(fileNameKey, json);
                        }
                    }
                }

                NotificationHelper.NewNotification("Importation", "Importation Succeeded", Notifications.Wpf.NotificationType.Success);
            }
        }

        private static void Import(String fileNameKey, String json)
        {
            switch(fileNameKey)
            {
                case _MonsterFileName:
                    ImportMonsterDataQuery importMonsterDataQuery = new ImportMonsterDataQuery();
                    importMonsterDataQuery.Monsters = JsonConvert.DeserializeObject<Monster[]>(json).ToList();

                    foreach (Monster monster in importMonsterDataQuery.Monsters)
                    {
                        Guid newMonsterId = Guid.NewGuid();

                        _MonsterIds.Add(monster.Id, newMonsterId);

                        monster.Id = newMonsterId;
                    }

                    MonsterFileHelperDataController.ImportMonster(importMonsterDataQuery);
                    break;

                case _SpecialAbilityFileName:
                    ImportSpecialAbilityDataQuery importSpecialAbilityDataQuery = new ImportSpecialAbilityDataQuery();
                    importSpecialAbilityDataQuery.SpecialAbilities = JsonConvert.DeserializeObject<SpecialAbility[]>(json).ToList();

                    foreach (SpecialAbility specialAbility in importSpecialAbilityDataQuery.SpecialAbilities)
                    {
                        Guid newMonsterId = _MonsterIds[specialAbility.MonsterId];

                        specialAbility.Id = Guid.NewGuid();
                        specialAbility.MonsterId = newMonsterId;
                    }

                    MonsterFileHelperDataController.ImportSpecialAbility(importSpecialAbilityDataQuery);
                    break;

                case _CharacteristicFileName:
                    ImportCharacteristicDataQuery importCharacteristicDataQuery = new ImportCharacteristicDataQuery();
                    importCharacteristicDataQuery.Characteristics = JsonConvert.DeserializeObject<Characteristic[]>(json).ToList();

                    foreach (Characteristic characteristic in importCharacteristicDataQuery.Characteristics)
                    {
                        Guid newMonsterId = _MonsterIds[characteristic.MonsterId];

                        characteristic.Id = Guid.NewGuid();
                        characteristic.MonsterId = newMonsterId;
                    }

                    MonsterFileHelperDataController.ImportCharacteristic(importCharacteristicDataQuery);
                    break;

                case _SkillFileName:
                    ImportSkillDataQuery importSkillDataQuery = new ImportSkillDataQuery();
                    importSkillDataQuery.Skills = JsonConvert.DeserializeObject<Skill[]>(json).ToList();

                    foreach (Skill skill in importSkillDataQuery.Skills)
                    {
                        Guid newMonsterId = _MonsterIds[skill.MonsterId];

                        skill.Id = Guid.NewGuid();
                        skill.MonsterId = newMonsterId;
                    }

                    MonsterFileHelperDataController.ImportSkill(importSkillDataQuery);
                    break;

                case _ActionFileName:
                    ImportActionDataQuery importActionDataQuery = new ImportActionDataQuery();
                    importActionDataQuery.Actions = JsonConvert.DeserializeObject<Data.Objects.Action[]>(json).ToList();

                    foreach (Data.Objects.Action action in importActionDataQuery.Actions)
                    {
                        Guid newMonsterId = _MonsterIds[action.MonsterId];

                        action.Id = Guid.NewGuid();
                        action.MonsterId = newMonsterId;
                    }

                    MonsterFileHelperDataController.ImportAction(importActionDataQuery);
                    break;
            }
        }
    }
}
