using Controllers.Controllers;
using Data.Constants;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    public partial class ActionWindow : Window
    {
        private Dictionary<string, int> _AbilityModifiers;

        private static readonly DependencyProperty _SelectedDisplayActionProperty =
            DependencyProperty.Register("SelectedDisplayAction", typeof(DisplayAction), typeof(ActionWindow));

        public DisplayAction SelectedDisplayAction
        {
            get { return (DisplayAction)GetValue(_SelectedDisplayActionProperty); }
            set { SetValue(_SelectedDisplayActionProperty, value); }
        }

        private static readonly DependencyProperty _ActionProperty =
            DependencyProperty.Register("Action", typeof(Data.Objects.Action), typeof(ActionWindow));

        public Data.Objects.Action Action
        {
            get { return (Data.Objects.Action)GetValue(_ActionProperty); }
            set { SetValue(_ActionProperty, value); }
        }

        public ActionWindow(Data.Objects.Action action, Dictionary<string, int> abilityModifiers = null)
        {
            InitializeComponent();

            try
            {
                _AbilityModifiers = abilityModifiers ?? new Dictionary<string, int>();

                Action = action;
                cboActions.ItemsSource = MonsterEditionPageDataController.GetActions();
                cboName.ItemsSource = ActionNames.All;
                cboRange.ItemsSource = ActionRanges.All;
                cboDamageType.ItemsSource = DamageTypes.All;
                cboAttackAbility.ItemsSource = new[] { "", "STR", "DEX", "CON", "INT", "WIS", "CHA" };

                DataContext = this;

                // If editing an existing action, show the calculated bonus label
                UpdateCalculatedBonus(action.AttackAbility);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cboAttackAbility_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string ability = cboAttackAbility.SelectedItem as string;
            UpdateCalculatedBonus(ability);

            if (!Action.OverrideAttackBonus)
                SetAttackBonusFromAbility(ability);
        }

        private void chkOverride_Changed(object sender, RoutedEventArgs e)
        {
            if (!Action.OverrideAttackBonus)
                SetAttackBonusFromAbility(cboAttackAbility.SelectedItem as string);
        }

        private void UpdateCalculatedBonus(string ability)
        {
            if (!string.IsNullOrEmpty(ability) && _AbilityModifiers.TryGetValue(ability, out int mod))
                lblCalculatedBonus.Text = string.Format("({0}{1} from {2})", mod >= 0 ? "+" : "", mod, ability);
            else
                lblCalculatedBonus.Text = "";
        }

        private void SetAttackBonusFromAbility(string ability)
        {
            if (!string.IsNullOrEmpty(ability) && _AbilityModifiers.TryGetValue(ability, out int mod))
                Action.AttackBonus = (short)mod;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDisplayAction == null) return;

                Data.Objects.Action from = MonsterEditionPageDataController.GetAction(SelectedDisplayAction.Id);
                Action.Name = from.Name;
                Action.Description = from.Description;
                Action.Range = from.Range;
                Action.DamageType = from.DamageType;
                Action.DamageDice = from.DamageDice;
                Action.DamageBonus = from.DamageBonus;
                Action.AttackAbility = from.AttackAbility;
                Action.AttackBonus = from.AttackBonus;
                Action.OverrideAttackBonus = from.OverrideAttackBonus;
                Action.IsLegendary = from.IsLegendary;
                Action.IsBonus = from.IsBonus;
                Action.IsReaction = from.IsReaction;

                UpdateCalculatedBonus(from.AttackAbility);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
