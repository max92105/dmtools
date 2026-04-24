using Controllers.Controllers;
using Data.Constants;
using Data.DataModels.StatBlockHelper;
using Data.Objects;
using DMTools.Pages;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace DMTools.Helpers
{
    public static class StatBlockHelper
    {
        private static String _StatHeader;
        private static StatBlockHelperDataModel _StatBlockDataModel;

        public static String StatHeader
        {
            get { return _StatHeader; }
        }

        public static void Initialize()
        {
            _StatHeader = File.ReadAllText(@"Html\StatBlockTemplate.html");
            _StatBlockDataModel = StatBlockHelperDataController.LoadTypes();
        }

        public static void ShowStatBlockWithMonsterId(Guid monsterId)
        {
            StatBlockHelperDataModel statBlockHelperDataModel = StatBlockHelperDataController.LoadMonster(monsterId);

            ShowStatBlock(statBlockHelperDataModel.Monster);
        }

        public static void ShowStatBlock(Monster monster)
        {
            try
            {
                if (monster != null)
                {
                    String html = GenerateMonster(monster);

                    StatBlockWindow statBlockWindow = new StatBlockWindow(html);
                    statBlockWindow.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static String GenerateMonsterWithId(Guid monsterId)
        {
            StatBlockHelperDataModel statBlockHelperDataModel = StatBlockHelperDataController.LoadMonster(monsterId);

            return GenerateMonster(statBlockHelperDataModel.Monster);
        }

        public static String GenerateMonster(Monster monster)
        {
            StatBlockHelperDataModel monsterStats = StatBlockHelperDataController.LoadMonsterStats(monster.Id);
            _StatBlockDataModel.Actions = monsterStats.Actions;
            _StatBlockDataModel.Characteristics = monsterStats.Characteristics;
            _StatBlockDataModel.Skills = monsterStats.Skills;
            _StatBlockDataModel.SpecialAbilities = monsterStats.SpecialAbilities;

            String statBlock = _StatHeader;

            statBlock += @"<stat-block>";
            statBlock += @"<creature-heading>";
            statBlock += @"<h1>" + monster.Name + "</h1>";
            statBlock += @"<h2>" + monster.Size + " " + monster.Type + " ";

            if (!String.IsNullOrEmpty(monster.Subtype))
                statBlock += @"(" + monster.Subtype + "), ";

            statBlock += monster.Alignment + "</h2>";
            statBlock += @"</creature-heading>";
            statBlock += @"<top-stats>";
            statBlock += @"<property-line>";
            statBlock += @"<h4>Armor Class </h4>";
            statBlock += @"<p>";
            statBlock += monster.ArmorClass;
            statBlock += @"</p>";
            statBlock += @"</property-line>";
            statBlock += @"<property-line>";
            statBlock += @"<h4>Hit Points </h4>";
            statBlock += @"<p>" + monster.HitPoints + " (" + monster.HitDice + ")</p>";
            statBlock += @"</property-line>";
            statBlock += @"<property-line>";
            statBlock += @"<h4>Speed </h4>";
            statBlock += @"<p>" + monster.Speed + "</p>";
            statBlock += @"<abilities-block data-cha=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.CharismaId).Score
                + " data-con=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.ConstitutionId).Score
                + " data-dex=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.DexterityId).Score
                + " data-int=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.IntelligenceId).Score
                + " data-str=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.StrenghtId).Score
                + " data-wis=" + _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == Characteristics.WisdomId).Score
                + " ></abilities-block>";

            statBlock += @"<property-line>";
            statBlock += @"<h4>Saving Throws </h4>";
            statBlock += @"<p>";

            foreach (CharacteristicType characteristicType in _StatBlockDataModel.CharacteristicTypes)
            {
                Characteristic characteristic = _StatBlockDataModel.Characteristics.First(obj => obj.CharacteristicTypeId == characteristicType.Id);

                if (characteristic.Save != 0)
                {
                    statBlock += characteristicType.Name + " " + characteristic.Save + " ";
                }
            }

            statBlock += @" </p>";
            statBlock += @"</property-line>";

            statBlock += @"<property-line>";
            statBlock += @"<h4>Skills </h4>";
            statBlock += @"<p>";

            foreach (Skill skill in _StatBlockDataModel.Skills)
            {
                statBlock += _StatBlockDataModel.SkillTypes.First(obj => obj.Id == skill.SkillTypeId).Name + " " + skill.Save + " ";
            }

            statBlock += @" </p>";
            statBlock += @"</property-line>";

            if (!String.IsNullOrEmpty(monster.ConditionImmunities))
            {
                statBlock += @"<property-line>";
                statBlock += @"<h4>Condition Immunities </h4>";
                statBlock += @"<p>" + monster.ConditionImmunities + "</p>";
                statBlock += @"</property-line>";
            }

            if (!String.IsNullOrEmpty(monster.DamageVulnerabilities))
            {
                statBlock += @"<property-line>";
                statBlock += @"<h4>Damage Vulnerabilities </h4>";
                statBlock += @"<p>" + monster.DamageVulnerabilities + "</p>";
                statBlock += @"</property-line>";
            }

            if (!String.IsNullOrEmpty(monster.DamageResistances))
            {
                statBlock += @"<property-line>";
                statBlock += @"<h4>Damage Resistances </h4>";
                statBlock += @"<p>" + monster.DamageResistances + "</p>";
                statBlock += @"</property-line>";
            }

            if (!String.IsNullOrEmpty(monster.DamageImmunities))
            {
                statBlock += @"<property-line>";
                statBlock += @"<h4>Damage Immunities </h4>";
                statBlock += @"<p>" + monster.DamageImmunities + "</p>";
                statBlock += @"</property-line>";
            }

            statBlock += @"<property-line>";
            statBlock += @"<h4>Senses </h4>";
            statBlock += @"<p>" + monster.Senses + "</p>";
            statBlock += @"</property-line>";

            statBlock += @"<property-line>";
            statBlock += @"<h4>Languages </h4>";
            statBlock += @"<p>" + monster.Languages + "</p>";
            statBlock += @"</property-line>";

            statBlock += @"<property-line>";
            statBlock += @"<h4>Challenge </h4>";
            statBlock += @"<p>" + monster.ChallengeRating + "</p>";
            statBlock += @"</property-line>";

            statBlock += @"</top-stats>";

            foreach (SpecialAbility specialAbility in _StatBlockDataModel.SpecialAbilities)
            {
                statBlock += @"<property-block>";
                statBlock += @"<h4>" + specialAbility.Name + " </h4>";
                statBlock += @"<p>";
                statBlock += specialAbility.Description;
                statBlock += @"</p>";
                statBlock += @"</property-block>";
            }

            if (_StatBlockDataModel.Actions.Where(obj => !obj.IsLegendary && !obj.IsBonus && !obj.IsReaction).Count() > 0)
                statBlock += @"<h3>Actions</h3>";

            foreach (Data.Objects.Action action in _StatBlockDataModel.Actions.Where(obj => !obj.IsLegendary && !obj.IsBonus && !obj.IsReaction))
            {
                statBlock += @"<property-block>";
                statBlock += @"<h4>" + action.Name + " </h4>";
                statBlock += @"<p>";
                statBlock += action.Description;
                statBlock += @"</p>";
                statBlock += @"</property-block>";
            }

            if (_StatBlockDataModel.Actions.Where(obj => obj.IsBonus).Count() > 0)
                statBlock += @"<h3>Bonus Actions</h3>";

            foreach (Data.Objects.Action action in _StatBlockDataModel.Actions.Where(obj => obj.IsBonus))
            {
                statBlock += @"<property-block>";
                statBlock += @"<h4>" + action.Name + " </h4>";
                statBlock += @"<p>";
                statBlock += action.Description;
                statBlock += @"</p>";
                statBlock += @"</property-block>";
            }

            if (_StatBlockDataModel.Actions.Where(obj => obj.IsReaction).Count() > 0)
                statBlock += @"<h3>Reactions</h3>";

            foreach (Data.Objects.Action action in _StatBlockDataModel.Actions.Where(obj => obj.IsReaction))
            {
                statBlock += @"<property-block>";
                statBlock += @"<h4>" + action.Name + " </h4>";
                statBlock += @"<p>";
                statBlock += action.Description;
                statBlock += @"</p>";
                statBlock += @"</property-block>";
            }

            if (_StatBlockDataModel.Actions.Where(obj => obj.IsLegendary).Count() > 0)
                statBlock += @"<h3>Legendary Actions</h3>";

            foreach (Data.Objects.Action action in _StatBlockDataModel.Actions.Where(obj => obj.IsLegendary))
            {
                statBlock += @"<property-block>";
                statBlock += @"<h4>" + action.Name + " </h4>";
                statBlock += @"<p>";
                statBlock += action.Description;
                statBlock += @"</p>";
                statBlock += @"</property-block>";
            }

            statBlock += @" </stat-block> </body> </html>";

            return statBlock;
        }
    }
}
