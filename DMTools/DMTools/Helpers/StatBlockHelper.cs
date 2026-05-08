using Controllers.Controllers;
using Data.Constants;
using Data.DataModels.StatBlockHelper;
using Data.Objects;
using DMTools.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private static string FormatCR(decimal cr)
            => cr == Math.Floor(cr) ? ((int)cr).ToString() : cr.ToString("G");

        private static string FormatText(string text)
        {
            if (String.IsNullOrEmpty(text)) return text ?? "";
            return text.Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
        }

        private static string FormatModifier(int score)
        {
            int mod = (int)Math.Floor((score - 10) / 2.0);
            return mod >= 0 ? "+" + mod : "\u2013" + Math.Abs(mod);
        }

        private static string AbilityText(int score)
        {
            return score + " (" + FormatModifier(score) + ")";
        }

        private static int GetProficiencyBonus(decimal cr)
        {
            if (cr <= 4) return 2;
            if (cr <= 8) return 3;
            if (cr <= 12) return 4;
            if (cr <= 16) return 5;
            if (cr <= 20) return 6;
            if (cr <= 24) return 7;
            if (cr <= 28) return 8;
            return 9;
        }

        private static string FormatSpeedList(IEnumerable<Speed> speeds)
        {
            var parts = new List<string>();
            foreach (var s in speeds)
            {
                if (string.IsNullOrEmpty(s.SpeedType) || s.SpeedType == "Walk")
                    parts.Add(s.Value + " ft.");
                else
                    parts.Add(s.SpeedType.ToLower() + " " + s.Value + " ft.");
            }
            return parts.Count > 0 ? String.Join(", ", parts) : "\u2014";
        }

        private static string FormatSenseList(IEnumerable<Sense> senses)
        {
            var parts = senses.Select(s => s.SenseType + " " + s.Value + " ft.").ToList();
            return parts.Count > 0 ? String.Join(", ", parts) : "\u2014";
        }

        private static string FormatAcList(IEnumerable<ArmorClassEntry> entries)
        {
            var list = entries.ToList();
            if (list.Count == 0) return "\u2014";
            if (list.Count == 1 && (String.IsNullOrEmpty(list[0].Label) || list[0].Label == "Default"))
                return list[0].Value.ToString();
            return String.Join(", ", list.Select(e => e.Display));
        }

        private static string FormatDamageModifiers(IEnumerable<DamageModifier> modifiers, string modifierType)
        {
            var group = modifiers.Where(m => m.ModifierType == modifierType).ToList();
            if (group.Count == 0) return null;
            var parts = group.Select(m =>
                modifierType == "Immunity"
                    ? m.DamageType
                    : m.DamageType + " (" + m.DiceCount + "d" + m.DiceSize + ")"
            );
            return String.Join(", ", parts);
        }

        private static string GenerateActionDescription(Data.Objects.Action action, Dictionary<string, int> abilityModifiers, int proficiencyBonus)
        {
            if (String.IsNullOrEmpty(action.DamageDice) || !String.IsNullOrEmpty(action.Description))
                return FormatText(action.Description);

            int attackBonus;
            if (action.OverrideAttackBonus)
            {
                attackBonus = action.AttackBonus;
            }
            else if (!String.IsNullOrEmpty(action.AttackAbility) && abilityModifiers.TryGetValue(action.AttackAbility, out int mod))
            {
                attackBonus = mod + proficiencyBonus;
            }
            else
            {
                attackBonus = action.AttackBonus;
            }

            var sb = new StringBuilder();

            if (!String.IsNullOrEmpty(action.Range))
                sb.Append(action.Range + " Attack: ");

            sb.Append((attackBonus >= 0 ? "+" : "") + attackBonus + " to hit. ");
            sb.Append("Hit: " + action.DamageDice);

            if (action.DamageBonus > 0)
                sb.Append(" + " + action.DamageBonus);
            else if (action.DamageBonus < 0)
                sb.Append(" - " + Math.Abs(action.DamageBonus));

            if (!String.IsNullOrEmpty(action.DamageType))
                sb.Append(" " + action.DamageType.ToLower() + " damage.");
            else
                sb.Append(" damage.");

            if (!String.IsNullOrEmpty(action.Description))
                sb.Append(" " + FormatText(action.Description));

            return sb.ToString();
        }

        private static void AppendActionSection(StringBuilder sb, string header, IEnumerable<Data.Objects.Action> actions,
            Dictionary<string, int> abilityModifiers, int proficiencyBonus)
        {
            var list = actions.ToList();
            if (list.Count == 0) return;
            sb.Append("<h3 class=\"section-header\">" + header + "</h3>");
            foreach (var action in list)
            {
                string desc = GenerateActionDescription(action, abilityModifiers, proficiencyBonus);
                sb.Append("<div class=\"property-block\"><h4>" + action.Name + ". </h4><p>" + desc + "</p></div>");
            }
        }

        public static String GenerateMonster(Monster monster)
        {
            StatBlockHelperDataModel stats = StatBlockHelperDataController.LoadMonsterStats(monster.Id);
            _StatBlockDataModel.Actions = stats.Actions;
            _StatBlockDataModel.Characteristics = stats.Characteristics;
            _StatBlockDataModel.Skills = stats.Skills;
            _StatBlockDataModel.SpecialAbilities = stats.SpecialAbilities;

            int profBonus = GetProficiencyBonus(monster.ChallengeRating);

            // Build ability modifier lookup for action descriptions
            var abilityModifiers = new Dictionary<string, int>
            {
                ["STR"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.StrenghtId)?.Score ?? 10) - 10) / 2.0),
                ["DEX"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.DexterityId)?.Score ?? 10) - 10) / 2.0),
                ["CON"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.ConstitutionId)?.Score ?? 10) - 10) / 2.0),
                ["INT"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.IntelligenceId)?.Score ?? 10) - 10) / 2.0),
                ["WIS"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.WisdomId)?.Score ?? 10) - 10) / 2.0),
                ["CHA"] = (int)Math.Floor(((_StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.CharismaId)?.Score ?? 10) - 10) / 2.0),
            };

            var sb = new StringBuilder(_StatHeader);

            sb.Append("<div class=\"stat-block\">");
            sb.Append("<div class=\"bar\"></div>");
            sb.Append("<div class=\"content-wrap\">");

            // Creature Heading
            sb.Append("<div class=\"creature-heading\">");
            sb.Append("<h1>" + monster.Name + "</h1>");
            sb.Append("<h2>" + monster.Size + " " + monster.Type);
            if (!String.IsNullOrEmpty(monster.Subtype))
                sb.Append(" (" + monster.Subtype + ")");
            sb.Append(", " + monster.Alignment + "</h2>");
            sb.Append("</div>");

            // Top Stats
            sb.Append("<div class=\"top-stats\">");
            sb.Append("<hr class=\"tapered-rule\" />");

            sb.Append("<div class=\"property-line\"><h4>Armor Class </h4><p>" + FormatAcList(stats.ArmorClassEntries) + "</p></div>");
            sb.Append("<div class=\"property-line\"><h4>Hit Points </h4><p>" + monster.HitPoints + "</p></div>");
            sb.Append("<div class=\"property-line\"><h4>Speed </h4><p>" + FormatSpeedList(stats.Speeds) + "</p></div>");

            sb.Append("<hr class=\"tapered-rule\" />");

            // Abilities Table
            var str = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.StrenghtId);
            var dex = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.DexterityId);
            var con = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.ConstitutionId);
            var intl = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.IntelligenceId);
            var wis = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.WisdomId);
            var cha = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == Characteristics.CharismaId);

            sb.Append("<table class=\"abilities-table\"><tbody>");
            sb.Append("<tr><th>STR</th><th>DEX</th><th>CON</th><th>INT</th><th>WIS</th><th>CHA</th></tr>");
            sb.Append("<tr>");
            sb.Append("<td>" + (str != null ? AbilityText(str.Score) : "-") + "</td>");
            sb.Append("<td>" + (dex != null ? AbilityText(dex.Score) : "-") + "</td>");
            sb.Append("<td>" + (con != null ? AbilityText(con.Score) : "-") + "</td>");
            sb.Append("<td>" + (intl != null ? AbilityText(intl.Score) : "-") + "</td>");
            sb.Append("<td>" + (wis != null ? AbilityText(wis.Score) : "-") + "</td>");
            sb.Append("<td>" + (cha != null ? AbilityText(cha.Score) : "-") + "</td>");
            sb.Append("</tr></tbody></table>");

            sb.Append("<hr class=\"tapered-rule\" />");

            // Saving Throws
            var saveParts = new List<string>();
            foreach (CharacteristicType ct in _StatBlockDataModel.CharacteristicTypes)
            {
                var ch = _StatBlockDataModel.Characteristics.FirstOrDefault(c => c.CharacteristicTypeId == ct.Id);
                if (ch != null && ch.Save != 0)
                    saveParts.Add(ct.Name + " " + (ch.Save >= 0 ? "+" : "") + ch.Save);
            }
            if (saveParts.Count > 0)
                sb.Append("<div class=\"property-line\"><h4>Saving Throws </h4><p>" + String.Join(", ", saveParts) + "</p></div>");

            // Skills
            if (_StatBlockDataModel.Skills.Count > 0)
            {
                var skillParts = new List<string>();
                foreach (Skill skill in _StatBlockDataModel.Skills)
                {
                    var skillType = _StatBlockDataModel.SkillTypes.FirstOrDefault(st => st.Id == skill.SkillTypeId);
                    if (skillType != null)
                        skillParts.Add(skillType.Name + " " + (skill.Save >= 0 ? "+" : "") + skill.Save);
                }
                if (skillParts.Count > 0)
                    sb.Append("<div class=\"property-line\"><h4>Skills </h4><p>" + String.Join(", ", skillParts) + "</p></div>");
            }

            // Damage Modifiers (homebrew dice pools)
            string vulnText = FormatDamageModifiers(stats.DamageModifiers, "Vulnerability");
            string resText = FormatDamageModifiers(stats.DamageModifiers, "Resistance");
            string immText = FormatDamageModifiers(stats.DamageModifiers, "Immunity");

            if (vulnText != null)
                sb.Append("<div class=\"property-line\"><h4>Damage Vulnerabilities </h4><p>" + vulnText + "</p></div>");
            if (resText != null)
                sb.Append("<div class=\"property-line\"><h4>Damage Resistances </h4><p>" + resText + "</p></div>");
            if (immText != null)
                sb.Append("<div class=\"property-line\"><h4>Damage Immunities </h4><p>" + immText + "</p></div>");

            // Condition Immunities (still stored as string on Monster)
            if (!String.IsNullOrEmpty(monster.ConditionImmunities))
                sb.Append("<div class=\"property-line\"><h4>Condition Immunities </h4><p>" + monster.ConditionImmunities + "</p></div>");

            sb.Append("<div class=\"property-line\"><h4>Senses </h4><p>" + FormatSenseList(stats.Senses) + "</p></div>");
            sb.Append("<div class=\"property-line\"><h4>Languages </h4><p>" + (String.IsNullOrEmpty(monster.Languages) ? "\u2014" : monster.Languages) + "</p></div>");
            sb.Append("<div class=\"property-line\"><h4>Challenge </h4><p>" + FormatCR(monster.ChallengeRating) + " (proficiency +" + profBonus + ")</p></div>");

            sb.Append("</div>"); // end top-stats

            // Special Abilities
            foreach (SpecialAbility sa in _StatBlockDataModel.SpecialAbilities)
                sb.Append("<div class=\"property-block\"><h4>" + sa.Name + ". </h4><p>" + FormatText(sa.Description) + "</p></div>");

            // Action sections
            AppendActionSection(sb, "Actions",
                _StatBlockDataModel.Actions.Where(a => !a.IsLegendary && !a.IsBonus && !a.IsReaction),
                abilityModifiers, profBonus);

            AppendActionSection(sb, "Bonus Actions",
                _StatBlockDataModel.Actions.Where(a => a.IsBonus),
                abilityModifiers, profBonus);

            AppendActionSection(sb, "Reactions",
                _StatBlockDataModel.Actions.Where(a => a.IsReaction),
                abilityModifiers, profBonus);

            AppendActionSection(sb, "Legendary Actions",
                _StatBlockDataModel.Actions.Where(a => a.IsLegendary),
                abilityModifiers, profBonus);

            sb.Append("</div>"); // end content-wrap
            sb.Append("<div class=\"bar\"></div>");
            sb.Append("</div>"); // end stat-block
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }
}
