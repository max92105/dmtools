using Controllers.Controllers;
using Data.Objects;
using Data.VirtualObject;
using System;
using System.Windows;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for SpecialAbilitiesWindow.xaml
    /// </summary>
    public partial class SpecialAbilitiesWindow : Window
    {
        private static readonly DependencyProperty _SelectedDisplaySpecialAbilityProperty =
        DependencyProperty.Register(
        "SelectedDisplaySpecialAbility", typeof(DisplaySpecialAbility),
        typeof(SpecialAbilitiesWindow));

        public DisplaySpecialAbility SelectedDisplaySpecialAbility
        {
            get { return (DisplaySpecialAbility)GetValue(_SelectedDisplaySpecialAbilityProperty); }
            set { SetValue(_SelectedDisplaySpecialAbilityProperty, value); }
        }

        private static readonly DependencyProperty _SpecialAbilityProperty =
        DependencyProperty.Register(
        "SpecialAbility", typeof(SpecialAbility),
        typeof(SpecialAbilitiesWindow));

        public SpecialAbility SpecialAbility
        {
            get { return (SpecialAbility)GetValue(_SpecialAbilityProperty); }
            set { SetValue(_SpecialAbilityProperty, value); }
        }

        public SpecialAbilitiesWindow(SpecialAbility specialAbility)
        {
            InitializeComponent();

            try
            {
                SpecialAbility = specialAbility;
                cboSpecialAbilities.ItemsSource = MonsterEditionPageDataController.GetSpecialAbilities();

                DataContext = this;
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
                this.DialogResult = true;
                Close();    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedDisplaySpecialAbility != null)
                {
                    SpecialAbility fromSpecialAbility = MonsterEditionPageDataController.GetSpecialAbility(SelectedDisplaySpecialAbility.Id);

                    SpecialAbility.AttackBonus = fromSpecialAbility.AttackBonus;
                    SpecialAbility.Description = fromSpecialAbility.Description;
                    SpecialAbility.Name = fromSpecialAbility.Name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
