using Controllers.Controllers;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    public partial class ActionWindow : Window
    {
        private readonly string _currentMonsterName;
        private readonly int    _proficiencyBonus;
        private readonly Dictionary<string, int> _abilityModifiers;
        private List<DisplayAction> _allActions;

        private static readonly DependencyProperty _ActionProperty =
            DependencyProperty.Register("Action", typeof(Data.Objects.Action), typeof(ActionWindow));

        public Data.Objects.Action Action
        {
            get { return (Data.Objects.Action)GetValue(_ActionProperty); }
            set { SetValue(_ActionProperty, value); }
        }

        public ActionWindow(Data.Objects.Action action, string monsterName, int proficiencyBonus,
            Dictionary<string, int> abilityModifiers = null)
        {
            InitializeComponent();

            try
            {
                Action               = action;
                _currentMonsterName  = monsterName ?? "";
                _proficiencyBonus    = proficiencyBonus;
                _abilityModifiers    = abilityModifiers ?? new Dictionary<string, int>();

                _allActions = MonsterEditionPageDataController.GetActions();
                lbActions.ItemsSource = _allActions;

                cboName.ItemsSource        = ActionNames.All;
                cboRange.ItemsSource       = ActionRanges.All;
                cboDamageType.ItemsSource  = DamageTypes.All;

                var abilityOptions = new List<AttackAbilityOption> { new AttackAbilityOption() };
                foreach (var key in new[] { "STR", "DEX", "CON", "INT", "WIS", "CHA" })
                {
                    if (_abilityModifiers.TryGetValue(key, out int mod))
                        abilityOptions.Add(new AttackAbilityOption(key, mod));
                }
                cboAttackAbility.ItemsSource = abilityOptions;

                lblProficiency.Text = $"+{_proficiencyBonus}  PB";

                DataContext = this;

                // Restore previously selected attack ability
                if (!string.IsNullOrEmpty(action.AttackAbility))
                {
                    var match = abilityOptions.FirstOrDefault(o => o.Key == action.AttackAbility);
                    if (match != null) cboAttackAbility.SelectedItem = match;
                }

                UpdateBonusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Filter ───────────────────────────────────────────────────────

        private void OnFilterChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            string monsterFilter = txtFilterMonster.Text ?? "";
            string nameFilter    = txtFilterName.Text    ?? "";
            string descFilter    = txtFilterDesc.Text    ?? "";

            var filtered = _allActions.Where(a =>
                (string.IsNullOrEmpty(monsterFilter) || a.MonsterName.IndexOf(monsterFilter,  StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrEmpty(nameFilter)    || a.ActionName.IndexOf(nameFilter,      StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrEmpty(descFilter)    || a.Description.IndexOf(descFilter,     StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

            lbActions.ItemsSource = filtered;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtFilterMonster.Text = "";
            txtFilterName.Text    = "";
            txtFilterDesc.Text    = "";
        }

        // ── Apply from library ───────────────────────────────────────────

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = lbActions.SelectedItem as DisplayAction;
                if (selected == null)
                {
                    MessageBox.Show("Select an action from the list first.", "Nothing selected",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Data.Objects.Action source = MonsterEditionPageDataController.GetAction(selected.Id);

                // Copy fields
                Action.Range      = source.Range;
                Action.DamageType = source.DamageType;
                Action.DamageDice = source.DamageDice;
                Action.DamageBonus = source.DamageBonus;
                Action.AttackAbility = source.AttackAbility;
                Action.IsLegendary  = source.IsLegendary;
                Action.IsBonus      = source.IsBonus;
                Action.IsReaction   = source.IsReaction;

                string name = source.Name ?? "";
                string desc = source.Description ?? "";

                // Replace source monster name with current monster name
                if (!string.IsNullOrEmpty(selected.MonsterName) && !string.IsNullOrEmpty(_currentMonsterName))
                {
                    name = Regex.Replace(name, Regex.Escape(selected.MonsterName), _currentMonsterName, RegexOptions.IgnoreCase);
                    desc = Regex.Replace(desc, Regex.Escape(selected.MonsterName), _currentMonsterName, RegexOptions.IgnoreCase);
                }

                // Recalculate attack bonus and DC using current monster's stats
                var castingOption = cboAttackAbility.SelectedItem as AttackAbilityOption;
                if (castingOption != null && castingOption.Key != null)
                {
                    int mod = castingOption.Modifier;
                    int dc  = 8 + _proficiencyBonus + mod;
                    int atk = _proficiencyBonus + mod;

                    desc = Regex.Replace(desc, @"\bDC\s+\d+",      $"DC {dc}", RegexOptions.IgnoreCase);
                    desc = Regex.Replace(desc, @"\+\d+\s+to\s+hit", $"+{atk} to hit", RegexOptions.IgnoreCase);
                    Action.AttackBonus = (short)atk;
                }
                else
                {
                    Action.AttackBonus = source.AttackBonus;
                }

                Action.Name        = name;
                Action.Description = desc;

                // Sync attack ability dropdown
                if (!string.IsNullOrEmpty(source.AttackAbility))
                {
                    var options = cboAttackAbility.ItemsSource as List<AttackAbilityOption>;
                    var match = options?.FirstOrDefault(o => o.Key == source.AttackAbility);
                    if (match != null) cboAttackAbility.SelectedItem = match;
                }

                UpdateBonusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Attack ability combo ─────────────────────────────────────────

        private void cboAttackAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateBonusLabel();
        }

        private void UpdateBonusLabel()
        {
            var option = cboAttackAbility.SelectedItem as AttackAbilityOption;
            if (option != null && option.Key != null)
            {
                int atkBonus = _proficiencyBonus + option.Modifier;
                int dmgBonus = option.Modifier;
                string atkStr = atkBonus >= 0 ? $"+{atkBonus}" : $"{atkBonus}";
                string dmgStr = dmgBonus >= 0 ? $"+{dmgBonus}" : $"{dmgBonus}";
                lblCalculatedBonus.Text = $"{atkStr} to hit  /  {dmgStr} dmg  ({option.Key})";
            }
            else
            {
                lblCalculatedBonus.Text = "";
            }
        }

        // ── Generate ─────────────────────────────────────────────────────

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name       = (Action.Name       ?? "").Trim();
                string range      = (Action.Range      ?? "").Trim();
                string damageType = (Action.DamageType ?? "").Trim();
                string damageDice = (Action.DamageDice ?? "").Trim();
                int    damageBonus = Action.DamageBonus;
                string monster    = string.IsNullOrEmpty(_currentMonsterName) ? "the creature" : _currentMonsterName;

                var ability = cboAttackAbility.SelectedItem as AttackAbilityOption;
                int atkMod  = ability?.Modifier ?? 0;
                int atkBonus = _proficiencyBonus + atkMod;
                int dc       = 8 + _proficiencyBonus + atkMod;

                string nameLower = name.ToLowerInvariant();

                string generated;

                if (nameLower == "multiattack")
                {
                    generated = $"Multiattack. The {monster} makes two attacks.";
                }
                else if (nameLower.Contains("frightful presence"))
                {
                    string dcStr  = $"DC {dc}";
                    generated =
                        $"Frightful Presence. Each creature of {monster}'s choice that is within 120 feet of {monster} " +
                        $"and aware of it must succeed on a {dcStr} Wisdom saving throw or become frightened for 1 minute. " +
                        $"A creature can repeat the saving throw at the end of each of its turns, ending the effect on " +
                        $"itself on a success. If a creature's saving throw is successful or the effect ends for it, " +
                        $"the creature is immune to {monster}'s Frightful Presence for the next 24 hours.";
                }
                else if (nameLower.Contains("breath"))
                {
                    string diceExpr = BuildDiceExpression(damageDice, damageBonus);
                    string avg      = ComputeAverage(damageDice, damageBonus);
                    string typeStr  = string.IsNullOrEmpty(damageType) ? "damage" : $"{damageType.ToLower()} damage";
                    string dcStr    = $"DC {dc}";
                    string saveStat = nameLower.Contains("poison") || nameLower.Contains("acid") ? "Constitution" : "Dexterity";
                    string areaDesc = "a 30-foot cone";
                    generated =
                        $"{name}. {monster} exhales in {areaDesc}. Each creature in that area must make a {dcStr} {saveStat} " +
                        $"saving throw, taking {avg} ({diceExpr}) {typeStr} on a failed save, or half as much damage on a successful one.";
                }
                else if (!string.IsNullOrEmpty(range))
                {
                    bool isMelee   = range.StartsWith("Melee",   StringComparison.OrdinalIgnoreCase);
                    bool isRanged  = range.StartsWith("Ranged",  StringComparison.OrdinalIgnoreCase);
                    bool isBoth    = range.StartsWith("Melee or", StringComparison.OrdinalIgnoreCase);

                    string attackType = isBoth   ? "Melee or Ranged Weapon Attack"
                                      : isRanged ? "Ranged Weapon Attack"
                                                 : "Melee Weapon Attack";

                    string reachRange = ParseReachRangePart(range);
                    string atkStr     = atkBonus >= 0 ? $"+{atkBonus}" : $"{atkBonus}";
                    string diceExpr   = BuildDiceExpression(damageDice, damageBonus);
                    string avg        = ComputeAverage(damageDice, damageBonus);
                    string typeStr    = string.IsNullOrEmpty(damageType) ? "damage" : $"{damageType.ToLower()} damage";

                    generated =
                        $"{name}. {attackType}: {atkStr} to hit, {reachRange}, one target. " +
                        $"Hit: {avg} ({diceExpr}) {typeStr}.";
                }
                else
                {
                    // Generic fallback: just name + placeholder
                    generated = $"{name}. [Description]";
                }

                Action.Description = generated;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private static string ParseReachRangePart(string range)
        {
            // Extract the parenthesised part if present, else use a sensible default
            var m = Regex.Match(range, @"\(([^)]+)\)");
            if (!m.Success) return "reach 5 ft.";

            string inner = m.Value; // e.g. "(5 ft.)" or "(30/120 ft.)"
            bool isRanged = range.StartsWith("Ranged", StringComparison.OrdinalIgnoreCase);
            bool isBoth   = range.StartsWith("Melee or", StringComparison.OrdinalIgnoreCase);

            if (isBoth)
            {
                // "Melee or Ranged (5 ft. or 20/60 ft.)" → two parts split by " or "
                var parts = inner.Trim('(', ')').Split(new[] { " or " }, StringSplitOptions.None);
                if (parts.Length == 2)
                    return $"reach {parts[0].Trim()} or range {parts[1].Trim()}";
                return $"reach/range {inner.Trim('(', ')')}";
            }

            if (isRanged)
                return $"range {inner.Trim('(', ')')}";

            return $"reach {inner.Trim('(', ')')}";
        }

        private static string BuildDiceExpression(string damageDice, int damageBonus)
        {
            if (string.IsNullOrWhiteSpace(damageDice) && damageBonus == 0) return "1d6";
            if (string.IsNullOrWhiteSpace(damageDice)) return damageBonus >= 0 ? $"+{damageBonus}" : $"{damageBonus}";
            if (damageBonus == 0)  return damageDice;
            if (damageBonus > 0)   return $"{damageDice} + {damageBonus}";
            return $"{damageDice} - {Math.Abs(damageBonus)}";
        }

        private static string ComputeAverage(string damageDice, int damageBonus)
        {
            // Parses NdX, returns "(N*(X+1)/2 + bonus)" as integer string
            if (string.IsNullOrWhiteSpace(damageDice))
                return damageBonus.ToString();

            var m = Regex.Match(damageDice.Trim(), @"^(\d+)d(\d+)$", RegexOptions.IgnoreCase);
            if (!m.Success) return damageBonus.ToString();

            int n    = int.Parse(m.Groups[1].Value);
            int x    = int.Parse(m.Groups[2].Value);
            int avg  = (int)Math.Floor(n * (x + 1) / 2.0) + damageBonus;
            return avg.ToString();
        }

        // ── Footer ───────────────────────────────────────────────────────

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try { DialogResult = true; Close(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ── Inner type ───────────────────────────────────────────────────

        private class AttackAbilityOption
        {
            public string Key      { get; }
            public int    Modifier { get; }
            public string Display  { get; }

            public AttackAbilityOption()
            {
                Key     = null;
                Display = "— none —";
            }

            public AttackAbilityOption(string key, int modifier)
            {
                Key      = key;
                Modifier = modifier;
                Display  = $"{key}  ({(modifier >= 0 ? "+" : "")}{modifier})";
            }

            public override string ToString() => Display;
        }
    }
}
