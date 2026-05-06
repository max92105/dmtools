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

        private static string FormatModifier(int score)
        {
            int mod = (score - 10) / 2;
            return mod >= 0 ? "+" + mod : "\u2013" + Math.Abs(mod);
        }

        private static string AbilityText(int score)
        {
            return score + " (" + FormatModifier(score) + ")";
        }

        public static String GenerateMonster(Monster monster)
        {
            StatBlockHelperDataModel monsterStats = StatBlockHelperDataController.LoadMonsterStats(monster.Id);
            _StatBlockDataModel.Actions = monsterStats.Actions;
            _StatBlockDataModel.Characteristics = monsterStats.Characteristics;
            _StatBlockDataModel.Skills = monsterStats.Skills;
            _StatBlockDataModel.SpecialAbilities = monsterStats.SpecialAbilities;

            String statBlock = _StatHeader;

            statBlock += "<div class=\"stat-block\">";
            statBlock += "<div class=\"bar\"></div>";
            statBlock += "<div class=\"content-wrap\">";

            // Creature Heading
            statBlock += "<div class=\"creature-heading\">";
            statBlock += "<h1>" + monster.Name + "</h1>";
            statBlock += "<h2>" + monster.Size + " " + monster.Type + " ";

            if (!String.IsNullOrEmpty(monster.Subtype))
                statBlock += "(" + monster.Subtype + "), ";

            statBlock += monster.Alignment + "</h2>";
            statBlock += "</div>";

            // Top Stats
            statBlock += "<div class=\"top-stats\">";
            statBlock += "<hr class=\"tapered-rule\" />";

            statBlock += "<div class=\"property-line\"><h4>Armor Class </h4><p>" + monster.ArmorClass + "</p></div>";
            statBlock += "<div class=\"property-line\"><h4>Hit Points </h4><p>" + monster.HitPoints + " (" + monster.HitDice + ")</p></div>";
            statBlock += "<div class=\"property-line\"><h4>Speed </h4><p>" + monster.Speed + "</p></div>";

            statBlock += "<hr class=\"tapered-rule\" />";

            // Abilities Table
            var str = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.StrenghtId);
            var dex = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.DexterityId);
            var con = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.ConstitutionId);
            var intl = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.IntelligenceId);
            var wis = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.WisdomId);
            var cha = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == Characteristics.CharismaId);

            statBlock += "<table class=\"abilities-table\"><tbody>";
            statBlock += "<tr><th>STR</th><th>DEX</th><th>CON</th><th>INT</th><th>WIS</th><th>CHA</th></tr>";
            statBlock += "<tr>";
            statBlock += "<td>" + (str != null ? AbilityText(str.Score) : "-") + "</td>";
            statBlock += "<td>" + (dex != null ? AbilityText(dex.Score) : "-") + "</td>";
            statBlock += "<td>" + (con != null ? AbilityText(con.Score) : "-") + "</td>";
            statBlock += "<td>" + (intl != null ? AbilityText(intl.Score) : "-") + "</td>";
            statBlock += "<td>" + (wis != null ? AbilityText(wis.Score) : "-") + "</td>";
            statBlock += "<td>" + (cha != null ? AbilityText(cha.Score) : "-") + "</td>";
            statBlock += "</tr></tbody></table>";

            statBlock += "<hr class=\"tapered-rule\" />";

            // Saving Throws
            String savesText = "";
            foreach (CharacteristicType characteristicType in _StatBlockDataModel.CharacteristicTypes)
            {
                Characteristic characteristic = _StatBlockDataModel.Characteristics.FirstOrDefault(obj => obj.CharacteristicTypeId == characteristicType.Id);

                if (characteristic != null && characteristic.Save != 0)
                {
                    if (savesText.Length > 0) savesText += ", ";
                    savesText += characteristicType.Name + " " + (characteristic.Save >= 0 ? "+" : "") + characteristic.Save;
                }
            }

            if (!String.IsNullOrEmpty(savesText))
            {
                statBlock += "<div class=\"property-line\"><h4>Saving Throws </h4><p>" + savesText + "</p></div>";
            }

            // Skills
            if (_StatBlockDataModel.Skills.Count > 0)
            {
                String skillsText = "";
                foreach (Skill skill in _StatBlockDataModel.Skills)
                {
                    var skillType = _StatBlockDataModel.SkillTypes.FirstOrDefault(obj => obj.Id == skill.SkillTypeId);
                    if (skillType != null)
                    {
                        if (skillsText.Length > 0) skillsText += ", ";
                        skillsText += skillType.Name + " " + (skill.Save >= 0 ? "+" : "") + skill.Save;
                    }
                }
                statBlock += "<div class=\"property-line\"><h4>Skills </h4><p>" + skillsText + "</p></div>";
            }

            if (!String.IsNullOrEmpty(monster.DamageVulnerabilities))
            {
                statBlock += "<div class=\"property-line\"><h4>Damage Vulnerabilities </h4><p>" + monster.DamageVulnerabilities + "</p></div>";
            }

            if (!String.IsNullOrEmpty(monster.DamageResistances))
            {
                statBlock += "<div class=\"property-line\"><h4>Damage Resistances </h4><p>" + monster.DamageResistances + "</p></div>";
            }

            if (!String.IsNullOrEmpty(monster.DamageImmunities))
            {
                statBlock += "<div class=\"property-line\"><h4>Damage Immunities </h4><p>" + monster.DamageImmunities + "</p></div>";
            }

            if (!String.IsNullOrEmpty(monster.ConditionImmunities))
            {
                statBlock += "<div class=\"property-line\"><h4>Condition Immunities </h4><p>" + monster.ConditionImmunities + "</p></div>";
            }

            statBlock += "<div class=\"property-line\"><h4>Senses </h4><p>" + monster.Senses + "</p></div>";
            statBlock += "<div class=\"property-line\"><h4>Languages </h4><p>" + monster.Languages + "</p></div>";
            statBlock += "<div class=\"property-line\"><h4>Challenge </h4><p>" + monster.ChallengeRating + "</p></div>";

            statBlock += "</div>"; // end top-stats

            // Special Abilities
            foreach (SpecialAbility specialAbility in _StatBlockDataModel.SpecialAbilities)
            {
                statBlock += "<div class=\"property-block\"><h4>" + specialAbility.Name + ". </h4><p>" + specialAbility.Description + "</p></div>";
            }

            // Actions
            var normalActions = _StatBlockDataModel.Actions.Where(obj => !obj.IsLegendary && !obj.IsBonus && !obj.IsReaction).ToList();
            if (normalActions.Count > 0)
            {
                statBlock += "<h3 class=\"section-header\">Actions</h3>";
                foreach (Data.Objects.Action action in normalActions)
                {
                    statBlock += "<div class=\"property-block\"><h4>" + action.Name + ". </h4><p>" + action.Description + "</p></div>";
                }
            }

            // Bonus Actions
            var bonusActions = _StatBlockDataModel.Actions.Where(obj => obj.IsBonus).ToList();
            if (bonusActions.Count > 0)
            {
                statBlock += "<h3 class=\"section-header\">Bonus Actions</h3>";
                foreach (Data.Objects.Action action in bonusActions)
                {
                    statBlock += "<div class=\"property-block\"><h4>" + action.Name + ". </h4><p>" + action.Description + "</p></div>";
                }
            }

            // Reactions
            var reactions = _StatBlockDataModel.Actions.Where(obj => obj.IsReaction).ToList();
            if (reactions.Count > 0)
            {
                statBlock += "<h3 class=\"section-header\">Reactions</h3>";
                foreach (Data.Objects.Action action in reactions)
                {
                    statBlock += "<div class=\"property-block\"><h4>" + action.Name + ". </h4><p>" + action.Description + "</p></div>";
                }
            }

            // Legendary Actions
            var legendaryActions = _StatBlockDataModel.Actions.Where(obj => obj.IsLegendary).ToList();
            if (legendaryActions.Count > 0)
            {
                statBlock += "<h3 class=\"section-header\">Legendary Actions</h3>";
                foreach (Data.Objects.Action action in legendaryActions)
                {
                    statBlock += "<div class=\"property-block\"><h4>" + action.Name + ". </h4><p>" + action.Description + "</p></div>";
                }
            }

            statBlock += "</div>"; // end content-wrap
            statBlock += "<div class=\"bar\"></div>";
            statBlock += "</div>"; // end stat-block
            statBlock += "</body></html>";

            return statBlock;
        }
    }
}
