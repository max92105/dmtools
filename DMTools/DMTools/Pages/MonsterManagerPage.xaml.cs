using Controllers.Controllers;
using Data.VirtualObject;
using DMTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace DMTools.Pages
{
    public partial class MonsterManagerPage : Page
    {
        private List<DisplayMonster> _allMonsters = new List<DisplayMonster>();
        private ListCollectionView _view;
        private bool _suppressFilter = false;
        private DispatcherTimer _filterTimer;

        public MonsterManagerPage()
        {
            InitializeComponent();
            _filterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _filterTimer.Tick += (s, e) => { _filterTimer.Stop(); ApplyFilter(); };
        }

        // ── Load ─────────────────────────────────────────────────────────────

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _allMonsters = MonsterManagerPageDataController.Load().Monsters.ToList();
                _view = new ListCollectionView(_allMonsters) { Filter = FilterPredicate };
                icMonsters.ItemsSource = _view;

                PopulateFilterDropdowns();
                UpdateCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateFilterDropdowns()
        {
            _suppressFilter = true;

            var types = new List<string> { "" };
            types.AddRange(_allMonsters
                .Where(m => !string.IsNullOrWhiteSpace(m.Type))
                .Select(m => m.Type).Distinct().OrderBy(t => t));
            cmbType.ItemsSource = types;
            cmbType.SelectedIndex = 0;

            var sizes = new List<string> { "" };
            sizes.AddRange(_allMonsters
                .Where(m => !string.IsNullOrWhiteSpace(m.Size))
                .Select(m => m.Size).Distinct()
                .OrderBy(s => SizeOrder(s)));
            cmbSize.ItemsSource = sizes;
            cmbSize.SelectedIndex = 0;

            var alignments = new List<string> { "" };
            alignments.AddRange(_allMonsters
                .Where(m => !string.IsNullOrWhiteSpace(m.Alignment))
                .Select(m => m.Alignment).Distinct().OrderBy(a => a));
            cmbAlignment.ItemsSource = alignments;
            cmbAlignment.SelectedIndex = 0;

            _suppressFilter = false;
        }

        private static int SizeOrder(string size)
        {
            switch (size?.ToLower())
            {
                case "tiny":       return 0;
                case "small":      return 1;
                case "medium":     return 2;
                case "large":      return 3;
                case "huge":       return 4;
                case "gargantuan": return 5;
                default:           return 6;
            }
        }

        // ── Filtering ─────────────────────────────────────────────────────────

        private void txtSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _filterTimer.Stop();
            _filterTimer.Start();
        }

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_suppressFilter) ApplyFilter();
        }

        private bool FilterPredicate(object obj)
        {
            var m = obj as DisplayMonster;
            if (m == null) return false;

            string name = txtSearchName.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name) &&
                (m.Name == null || m.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) < 0))
                return false;

            string type = cmbType.SelectedItem as string;
            if (!string.IsNullOrEmpty(type) &&
                !string.Equals(m.Type, type, StringComparison.OrdinalIgnoreCase))
                return false;

            string subtype = txtSubtype.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(subtype) &&
                (m.Subtype == null || m.Subtype.IndexOf(subtype, StringComparison.OrdinalIgnoreCase) < 0))
                return false;

            string size = cmbSize.SelectedItem as string;
            if (!string.IsNullOrEmpty(size) &&
                !string.Equals(m.Size, size, StringComparison.OrdinalIgnoreCase))
                return false;

            string alignment = cmbAlignment.SelectedItem as string;
            if (!string.IsNullOrEmpty(alignment) &&
                (m.Alignment == null || m.Alignment.IndexOf(alignment, StringComparison.OrdinalIgnoreCase) < 0))
                return false;

            if (decimal.TryParse(txtCrMin.Text, out decimal crMin) && m.ChallengeRating < crMin) return false;
            if (decimal.TryParse(txtCrMax.Text, out decimal crMax) && m.ChallengeRating > crMax) return false;
            if (int.TryParse(txtHpMin.Text, out int hpMin) && m.HitPoints < hpMin) return false;
            if (int.TryParse(txtHpMax.Text, out int hpMax) && m.HitPoints > hpMax) return false;
            if (int.TryParse(txtAcMin.Text, out int acMin) && m.ArmorClass < acMin) return false;
            if (int.TryParse(txtAcMax.Text, out int acMax) && m.ArmorClass > acMax) return false;

            return true;
        }

        private void ApplyFilter()
        {
            if (_suppressFilter || _view == null) return;
            _view.Refresh();
            UpdateCount();
        }

        private void UpdateCount()
        {
            int count = _view?.Count ?? _allMonsters.Count;
            tbCount.Text = $"{count} monster{(count != 1 ? "s" : "")}";
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            _filterTimer.Stop();
            _suppressFilter = true;
            txtSearchName.Text = string.Empty;
            txtSubtype.Text = string.Empty;
            txtCrMin.Text = string.Empty;
            txtCrMax.Text = string.Empty;
            txtHpMin.Text = string.Empty;
            txtHpMax.Text = string.Empty;
            txtAcMin.Text = string.Empty;
            txtAcMax.Text = string.Empty;
            cmbType.SelectedIndex = 0;
            cmbSize.SelectedIndex = 0;
            cmbAlignment.SelectedIndex = 0;
            _suppressFilter = false;
            ApplyFilter();
        }

        // ── Row interaction ───────────────────────────────────────────────────

        private void CheckboxColumn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void chkSelectAll_Click(object sender, RoutedEventArgs e)
        {
            bool select = chkSelectAll.IsChecked == true;
            foreach (DisplayMonster m in _view)
                m.IsSelected = select;
        }

        private void MonsterRow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            var dm = (sender as FrameworkElement)?.DataContext as DisplayMonster;
            if (dm != null)
                NavigationService.Navigate(new MonsterEditionPage(dm.Id));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var dm = (sender as Button)?.Tag as DisplayMonster;
            if (dm != null)
                NavigationService.Navigate(new MonsterEditionPage(dm.Id));
        }

        private void btnStatBlock_Click(object sender, RoutedEventArgs e)
        {
            var dm = (sender as Button)?.Tag as DisplayMonster;
            if (dm != null)
                StatBlockHelper.ShowStatBlockWithMonsterId(dm.Id);
        }

        // ── Action bar ────────────────────────────────────────────────────────

        private void btnNewMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new MonsterEditionPage(Guid.Empty));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = _allMonsters.Where(m => m.IsSelected).Select(m => m.Id).ToList();
                if (selected.Any())
                    MonsterFileHelper.ExportMonsters(selected);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonsterFileHelper.ImportMonsters();
                _allMonsters = MonsterManagerPageDataController.Load().Monsters.ToList();
                _view = new ListCollectionView(_allMonsters) { Filter = FilterPredicate };
                icMonsters.ItemsSource = _view;
                PopulateFilterDropdowns();
                UpdateCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
