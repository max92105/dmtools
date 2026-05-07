using Controllers.Controllers;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    public partial class SpecialAbilitiesWindow : Window
    {
        private static readonly DependencyProperty _SpecialAbilityProperty =
            DependencyProperty.Register("SpecialAbility", typeof(SpecialAbility), typeof(SpecialAbilitiesWindow));

        public SpecialAbility SpecialAbility
        {
            get { return (SpecialAbility)GetValue(_SpecialAbilityProperty); }
            set { SetValue(_SpecialAbilityProperty, value); }
        }

        private readonly string _currentMonsterName;
        private readonly decimal _challengeRating;
        private readonly int _proficiencyBonus;
        private List<DisplaySpecialAbility> _allAbilities;

        private class CastingAbilityOption
        {
            public string Key      { get; }
            public int    Modifier { get; }
            public string Display  { get; }

            // Sentinel: no recalculation
            public CastingAbilityOption()
            {
                Key     = null;
                Display = "— no recalculation —";
            }

            public CastingAbilityOption(string key, int modifier)
            {
                Key      = key;
                Modifier = modifier;
                Display  = $"{key}  ({(modifier >= 0 ? "+" : "")}{modifier})";
            }

            // Required so the non-editable DarkCombo ContentPresenter shows Display instead of type name.
            public override string ToString() => Display;
        }

        public SpecialAbilitiesWindow(SpecialAbility specialAbility, string monsterName,
            decimal challengeRating, Dictionary<string, int> abilityModifiers)
        {
            InitializeComponent();

            try
            {
                SpecialAbility       = specialAbility;
                _currentMonsterName  = monsterName ?? "";
                _challengeRating     = challengeRating;
                _proficiencyBonus    = ProficiencyBonus(challengeRating);

                // Load full ability library
                _allAbilities = MonsterEditionPageDataController.GetSpecialAbilities();
                lbAbilities.ItemsSource = _allAbilities;

                // Build casting ability options in standard order
                var options = new List<CastingAbilityOption> { new CastingAbilityOption() };
                foreach (var key in new[] { "STR", "DEX", "CON", "INT", "WIS", "CHA" })
                {
                    if (abilityModifiers != null && abilityModifiers.TryGetValue(key, out int mod))
                        options.Add(new CastingAbilityOption(key, mod));
                }
                cboCastingAbility.ItemsSource  = options;
                cboCastingAbility.SelectedIndex = 0;

                lblProficiency.Text = $"+{_proficiencyBonus}  PB";

                DataContext = this;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static int ProficiencyBonus(decimal cr)
        {
            if (cr <= 4)  return 2;
            if (cr <= 8)  return 3;
            if (cr <= 12) return 4;
            if (cr <= 16) return 5;
            if (cr <= 20) return 6;
            if (cr <= 24) return 7;
            if (cr <= 28) return 8;
            return 9;
        }

        private static string CrDisplay(decimal cr)
        {
            if (cr == 0.125m) return "1/8";
            if (cr == 0.25m)  return "1/4";
            if (cr == 0.5m)   return "1/2";
            return cr.ToString("0.##");
        }

        private void OnFilterChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        private void ApplyFilter()
        {
            string monsterFilter = txtFilterMonster.Text ?? "";
            string nameFilter    = txtFilterName.Text    ?? "";
            string descFilter    = txtFilterDesc.Text    ?? "";

            var filtered = _allAbilities.Where(a =>
                (string.IsNullOrEmpty(monsterFilter) || a.MonsterName.IndexOf(monsterFilter,  StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrEmpty(nameFilter)    || a.AbilityName.IndexOf(nameFilter,     StringComparison.OrdinalIgnoreCase) >= 0) &&
                (string.IsNullOrEmpty(descFilter)    || a.Description.IndexOf(descFilter,     StringComparison.OrdinalIgnoreCase) >= 0)
            ).ToList();

            lbAbilities.ItemsSource = filtered;
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            txtFilterMonster.Text = "";
            txtFilterName.Text    = "";
            txtFilterDesc.Text    = "";
            // OnFilterChanged fires automatically from the TextChanged events above
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = lbAbilities.SelectedItem as DisplaySpecialAbility;
                if (selected == null)
                {
                    MessageBox.Show("Select an ability from the list first.", "Nothing selected",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                SpecialAbility source = MonsterEditionPageDataController.GetSpecialAbility(selected.Id);
                string desc = source.Description ?? "";

                bool sourceHadAttackBonus = source.AttackBonus != 0;
                bool descHasAttackRolls   = Regex.IsMatch(desc, @"\+\d+\s+to\s+hit", RegexOptions.IgnoreCase);

                // Replace source monster name with current monster name (case-insensitive)
                if (!string.IsNullOrEmpty(selected.MonsterName) && !string.IsNullOrEmpty(_currentMonsterName))
                    desc = Regex.Replace(desc, Regex.Escape(selected.MonsterName),
                        _currentMonsterName, RegexOptions.IgnoreCase);

                var castingOption = cboCastingAbility.SelectedItem as CastingAbilityOption;
                if (castingOption != null && castingOption.Key != null)
                {
                    int mod = castingOption.Modifier;
                    int dc  = 8 + _proficiencyBonus + mod;
                    int atk = _proficiencyBonus + mod;

                    desc = Regex.Replace(desc, @"\bDC\s+\d+",      $"DC {dc}", RegexOptions.IgnoreCase);
                    desc = Regex.Replace(desc, @"\+\d+\s+to\s+hit", $"+{atk} to hit", RegexOptions.IgnoreCase);

                    SpecialAbility.AttackBonus = (sourceHadAttackBonus || descHasAttackRolls)
                        ? (short)atk : (short)0;
                }
                else
                {
                    SpecialAbility.AttackBonus = source.AttackBonus;
                }

                SpecialAbility.Name        = source.Name;
                SpecialAbility.Description = desc;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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
    }
}
