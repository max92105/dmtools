using Controllers.Controllers;
using Data.Constants;
using Data.Objects;
using DMTools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DMTools.Pages
{
    public partial class EncounterBuilderPage : Page
    {
        private List<Monster> _allMonsters = new List<Monster>();
        private ObservableCollection<EncounterEntry> _currentEntries = new ObservableCollection<EncounterEntry>();
        private Monster _selectedMonster;
        private Guid _editingEncounterId = Guid.Empty;

        public EncounterBuilderPage()
        {
            InitializeComponent();
            lbCurrentEntries.ItemsSource = _currentEntries;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try { Refresh(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh()
        {
            _allMonsters = EncounterBuilderPageDataController.LoadMonsters();

            var types = new List<string> { "(all types)" };
            types.AddRange(MonsterTypes.All);
            cmbTypeFilter.ItemsSource = types;
            cmbTypeFilter.SelectedIndex = 0;

            ApplyMonsterFilter();
            RefreshSavedEncounters();
        }

        private void RefreshSavedEncounters()
        {
            lbSavedEncounters.ItemsSource = new ObservableCollection<Encounter>(
                EncounterBuilderPageDataController.LoadEncounters());
            btnLoadEncounter.IsEnabled = false;
            btnDeleteEncounter.IsEnabled = false;
        }

        private void txtMonsterSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyMonsterFilter();
        private void txtCrFilter_TextChanged(object sender, TextChangedEventArgs e) => ApplyMonsterFilter();
        private void cmbTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyMonsterFilter();

        private void ApplyMonsterFilter()
        {
            try
            {
                string name = txtMonsterSearch.Text?.Trim() ?? string.Empty;
                string typeFilter = cmbTypeFilter.SelectedItem as string;
                bool filterByType = !string.IsNullOrEmpty(typeFilter) && typeFilter != "(all types)";

                decimal? crMin = null, crMax = null;
                if (decimal.TryParse(txtCrMin.Text?.Trim(), out decimal parsedMin)) crMin = parsedMin;
                if (decimal.TryParse(txtCrMax.Text?.Trim(), out decimal parsedMax)) crMax = parsedMax;

                lbMonsters.ItemsSource = _allMonsters.Where(m =>
                    (string.IsNullOrEmpty(name) || (m.Name != null && m.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)) &&
                    (!filterByType || string.Equals(m.Type, typeFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (crMin == null || m.ChallengeRating >= crMin) &&
                    (crMax == null || m.ChallengeRating <= crMax)
                ).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static readonly System.Windows.Media.SolidColorBrush _parchment =
            new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#F0E6D3"));
        private static readonly System.Windows.Media.SolidColorBrush _dimmed =
            new System.Windows.Media.SolidColorBrush(
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#55E6D3B8"));

        private void lbMonsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMonster = lbMonsters.SelectedItem as Monster;
            if (_selectedMonster != null)
            {
                tbSelectedMonster.Text = _selectedMonster.Name;
                tbSelectedMonster.Foreground = _parchment;
                tbSelectedMonster.FontStyle = FontStyles.Normal;
            }
            else
            {
                tbSelectedMonster.Text = "— select a monster —";
                tbSelectedMonster.Foreground = _dimmed;
                tbSelectedMonster.FontStyle = FontStyles.Italic;
            }
            btnAddToEncounter.IsEnabled = _selectedMonster != null;
        }

        private void btnAddToEncounter_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMonster == null) return;

            if (!int.TryParse(txtAddQty.Text, out int qty) || qty < 1)
                qty = 1;

            var existing = _currentEntries.FirstOrDefault(en => en.MonsterId == _selectedMonster.Id);
            if (existing != null)
            {
                existing.Quantity += qty;
            }
            else
            {
                _currentEntries.Add(new EncounterEntry
                {
                    Id = Guid.NewGuid(),
                    MonsterId = _selectedMonster.Id,
                    MonsterName = _selectedMonster.Name,
                    Quantity = qty
                });
            }

            txtAddQty.Text = "1";
        }

        private void btnRemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is EncounterEntry entry)
                _currentEntries.Remove(entry);
        }

        private void btnClearEncounter_Click(object sender, RoutedEventArgs e)
        {
            _currentEntries.Clear();
            txtEncounterName.Text = string.Empty;
            _editingEncounterId = Guid.Empty;
        }

        private void btnSaveEncounter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtEncounterName.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Please enter an encounter name.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (_currentEntries.Count == 0)
                {
                    MessageBox.Show("Add at least one monster to the encounter.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _editingEncounterId = EncounterBuilderPageDataController.SaveEncounter(
                    _editingEncounterId, name, _currentEntries.ToList());

                RefreshSavedEncounters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRunEncounter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_currentEntries.Count == 0)
                {
                    MessageBox.Show("Add at least one monster to the encounter.", "Validation",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                EncounterService.PendingEntries = _currentEntries
                    .Select(en => new EncounterPendingEntry { MonsterId = en.MonsterId, Quantity = en.Quantity })
                    .ToList();

                NavigationService.Navigate(new InitiativeManagerPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lbSavedEncounters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool hasSelection = lbSavedEncounters.SelectedItem != null;
            btnLoadEncounter.IsEnabled = hasSelection;
            btnDeleteEncounter.IsEnabled = hasSelection;
        }

        private void btnLoadEncounter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(lbSavedEncounters.SelectedItem is Encounter encounter)) return;

                _editingEncounterId = encounter.Id;
                txtEncounterName.Text = encounter.Name;

                _currentEntries.Clear();
                foreach (var entry in EncounterBuilderPageDataController.LoadEncounterEntries(encounter.Id))
                    _currentEntries.Add(entry);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteEncounter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(lbSavedEncounters.SelectedItem is Encounter encounter)) return;

                var result = MessageBox.Show($"Delete encounter '{encounter.Name}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes) return;

                EncounterBuilderPageDataController.DeleteEncounter(encounter.Id);

                if (_editingEncounterId == encounter.Id)
                {
                    _editingEncounterId = Guid.Empty;
                    txtEncounterName.Text = string.Empty;
                    _currentEntries.Clear();
                }

                RefreshSavedEncounters();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void lbMonsters_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            monsterScroll.ScrollToVerticalOffset(monsterScroll.VerticalOffset - e.Delta * 0.5);
            e.Handled = true;
        }

        private void lbCurrentEntries_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            entriesScroll.ScrollToVerticalOffset(entriesScroll.VerticalOffset - e.Delta * 0.5);
            e.Handled = true;
        }

        private void lbSavedEncounters_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            savedScroll.ScrollToVerticalOffset(savedScroll.VerticalOffset - e.Delta * 0.5);
            e.Handled = true;
        }
    }
}
