using Controllers.Controllers;
using Data.DataModels.PcEditionPage;
using System;
using System.Windows;
using System.Windows.Input;

namespace DMTools.Pages
{
    public partial class PcEditionWindow : Window
    {
        private PcEditionPageDataModel _model;
        private readonly bool _isNew;

        public PcEditionWindow(Guid playerCharacterId)
        {
            InitializeComponent();

            _isNew = playerCharacterId == Guid.Empty;

            if (_isNew)
            {
                tbTitle.Text = "New Player Character";
                _model = PcEditionPageDataController.LoadNewPlayerCharacter();
            }
            else
            {
                tbTitle.Text = "Edit Player Character";
                _model = PcEditionPageDataController.LoadPlayerCharacter(playerCharacterId);
                btnDelete.Visibility = Visibility.Visible;
            }

            PopulateFields();
        }

        private void PopulateFields()
        {
            var pc = _model.PlayerCharacter;
            txtName.Text = pc.Name;
            txtLevel.Text = pc.Level.ToString();
            txtArmorClass.Text = pc.ArmorClass.ToString();
            txtInitiativeBonus.Text = pc.InitiativeBonus.ToString();
            txtPassivePerception.Text = pc.PassivePerception.ToString();
            txtMaxHp.Text = pc.MaxHp.ToString();
        }

        private bool ApplyFields()
        {
            var pc = _model.PlayerCharacter;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            pc.Name = txtName.Text.Trim();

            if (!short.TryParse(txtLevel.Text, out short level) || level < 1 || level > 20)
            {
                MessageBox.Show("Level must be a number between 1 and 20.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            pc.Level = level;

            if (!short.TryParse(txtArmorClass.Text, out short ac))
            {
                MessageBox.Show("Armor Class must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            pc.ArmorClass = ac;

            if (!short.TryParse(txtInitiativeBonus.Text, out short init))
            {
                MessageBox.Show("Initiative Bonus must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            pc.InitiativeBonus = init;

            if (!short.TryParse(txtPassivePerception.Text, out short pp))
            {
                MessageBox.Show("Passive Perception must be a number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            pc.PassivePerception = pp;

            if (!short.TryParse(txtMaxHp.Text, out short maxHp) || maxHp < 0)
            {
                MessageBox.Show("Max HP must be a non-negative number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            pc.MaxHp = maxHp;

            return true;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ApplyFields()) return;
                PcEditionPageDataController.Save(_model);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    $"Delete {_model.PlayerCharacter.Name}?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes) return;

                _model.PlayerCharacter.Delete();
                PcEditionPageDataController.Save(_model);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;

        private void Close_Click(object sender, RoutedEventArgs e)
            => DialogResult = false;
    }
}
