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
    public partial class ActionWindow : Window
    {
        private static readonly DependencyProperty _SelectedDisplayActionProperty =
        DependencyProperty.Register(
        "SelectedDisplayAction", typeof(DisplayAction),
        typeof(ActionWindow));

        public DisplayAction SelectedDisplayAction
        {
            get { return (DisplayAction)GetValue(_SelectedDisplayActionProperty); }
            set { SetValue(_SelectedDisplayActionProperty, value); }
        }

        private static readonly DependencyProperty _ActionProperty =
        DependencyProperty.Register(
        "Action", typeof(Data.Objects.Action),
        typeof(SpecialAbilitiesWindow));

        public Data.Objects.Action Action
        {
            get { return (Data.Objects.Action)GetValue(_ActionProperty); }
            set { SetValue(_ActionProperty, value); }
        }

        public ActionWindow(Data.Objects.Action action)
        {
            InitializeComponent();

            try
            {
                Action = action;
                cboActions.ItemsSource = MonsterEditionPageDataController.GetActions();

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
                if (SelectedDisplayAction != null)
                {
                    Data.Objects.Action fromAction = MonsterEditionPageDataController.GetAction(SelectedDisplayAction.Id);

                    Action.AttackBonus = fromAction.AttackBonus;
                    Action.DamageBonus = fromAction.DamageBonus;
                    Action.DamageDice = fromAction.DamageDice;
                    Action.Description = fromAction.Description;
                    Action.IsLegendary = fromAction.IsLegendary;
                    Action.Name = fromAction.Name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
