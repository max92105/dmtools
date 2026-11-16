using Controllers.Controllers;
using Data.Objects;
using DMTools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DMTools.Pages
{
    /// <summary>
    /// Loot generator: browse / filter the item library, build loot tables and roll on them.
    /// </summary>
    public partial class LootGeneratorPage : Page
    {
        private static readonly string[] RarityOrder =
            { "Common", "Uncommon", "Rare", "Very Rare", "Legendary", "Artifact", "Varies", "Mundane" };

        private const string AllOption = "All";
        private const string AnyAttunement = "Any";
        private const string RequiresAttunement = "Requires attunement";
        private const string NoAttunement = "No attunement";

        private List<Item> _allItems = new List<Item>();
        private Dictionary<Guid, Item> _itemsById = new Dictionary<Guid, Item>();
        private List<Item> _filteredItems = new List<Item>();
        private List<LootTable> _tables = new List<LootTable>();
        private LootTable _currentTable;
        private readonly ObservableCollection<EntryView> _entries = new ObservableCollection<EntryView>();
        private readonly ObservableCollection<ResultView> _results = new ObservableCollection<ResultView>();
        private readonly List<ToggleButton> _rarityChips = new List<ToggleButton>();
        private readonly DispatcherTimer _filterTimer;
        private bool _suppressFilter;

        public LootGeneratorPage()
        {
            InitializeComponent();
            _filterTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            _filterTimer.Tick += (s, e) => { _filterTimer.Stop(); ApplyFilter(); };
            lbEntries.ItemsSource = _entries;
            lbResults.ItemsSource = _results;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_allItems.Count > 0) return; // coming back to the page: keep the state

                _allItems = LootGeneratorDataController.LoadItems();
                _itemsById = _allItems.ToDictionary(i => i.Id);
                BuildFilters();
                ApplyFilter();
                LoadTables();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Mode ─────────────────────────────────────────────────────────────

        private void tbItemsMode_Click(object sender, RoutedEventArgs e) => SetMode(tables: false);
        private void tbTablesMode_Click(object sender, RoutedEventArgs e) => SetMode(tables: true);

        private void SetMode(bool tables)
        {
            tbItemsMode.IsChecked = !tables;
            tbTablesMode.IsChecked = tables;
            panelFilters.Visibility = tables ? Visibility.Collapsed : Visibility.Visible;
            panelItems.Visibility = tables ? Visibility.Collapsed : Visibility.Visible;
            panelTables.Visibility = tables ? Visibility.Visible : Visibility.Collapsed;
            panelEntries.Visibility = tables ? Visibility.Visible : Visibility.Collapsed;
            if (tables && lbTables.SelectedItem == null && _tables.Count > 0) lbTables.SelectedIndex = 0;
        }

        // ── Filters ──────────────────────────────────────────────────────────

        private void BuildFilters()
        {
            _suppressFilter = true;

            wpRarity.Children.Clear();
            _rarityChips.Clear();
            foreach (string rarity in RarityOrder)
            {
                var chip = new ToggleButton { Content = rarity, Tag = rarity, Style = (Style)FindResource("Chip") };
                chip.Click += Filter_Changed;
                wpRarity.Children.Add(chip);
                _rarityChips.Add(chip);
            }

            var categories = new List<string> { AllOption };
            categories.AddRange(_allItems.Select(i => i.Category).Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c));
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedIndex = 0;

            cmbAttunement.ItemsSource = new[] { AnyAttunement, RequiresAttunement, NoAttunement };
            cmbAttunement.SelectedIndex = 0;

            var sources = new List<string> { AllOption };
            sources.AddRange(_allItems.Select(i => i.Source).Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s));
            cmbSource.ItemsSource = sources;
            cmbSource.SelectedIndex = 0;

            _suppressFilter = false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            if (_suppressFilter) return;
            _filterTimer.Stop();
            _filterTimer.Start();
        }

        private void ApplyFilter()
        {
            string search = (txtSearch.Text ?? "").Trim();
            var rarities = new HashSet<string>(_rarityChips.Where(c => c.IsChecked == true).Select(c => (string)c.Tag));
            string category = cmbCategory.SelectedItem as string;
            string attunement = cmbAttunement.SelectedItem as string;
            string source = cmbSource.SelectedItem as string;
            bool magicOnly = chkMagicOnly.IsChecked == true;

            _filteredItems = _allItems.Where(i =>
                (!magicOnly || i.IsMagic) &&
                (rarities.Count == 0 || rarities.Contains(i.Rarity)) &&
                (string.IsNullOrEmpty(category) || category == AllOption || i.Category == category) &&
                (string.IsNullOrEmpty(source) || source == AllOption || i.Source == source) &&
                (attunement != RequiresAttunement || i.RequiresAttunement) &&
                (attunement != NoAttunement || !i.RequiresAttunement) &&
                (search.Length == 0 ||
                 (i.Name ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                 (search.Length >= 3 && (i.Description ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0))
            ).ToList();

            lbItems.ItemsSource = _filteredItems;
            tbItemCount.Text = _allItems.Count == 0
                ? "No items yet — run the database tool (option 1) to import them."
                : $"{_filteredItems.Count} item{(_filteredItems.Count == 1 ? "" : "s")}";
        }

        private void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            _suppressFilter = true;
            txtSearch.Text = "";
            foreach (var chip in _rarityChips) chip.IsChecked = false;
            cmbCategory.SelectedIndex = 0;
            cmbAttunement.SelectedIndex = 0;
            cmbSource.SelectedIndex = 0;
            chkMagicOnly.IsChecked = true;
            _suppressFilter = false;
            ApplyFilter();
        }

        // ── Item details ─────────────────────────────────────────────────────

        private void lbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ShowDetails(lbItems.SelectedItem as Item, allowAddToTable: true);

        private void lbItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbItems.SelectedItem is Item) AddSelectedItemToTable();
        }

        private void ShowDetails(Item item, bool allowAddToTable)
        {
            tbDetailsEmpty.Text = "Select an item to see its details.";
            pnlDetails.Visibility = item != null ? Visibility.Visible : Visibility.Collapsed;
            tbDetailsEmpty.Visibility = item != null ? Visibility.Collapsed : Visibility.Visible;
            pnlAddToTable.Visibility = item != null && allowAddToTable ? Visibility.Visible : Visibility.Collapsed;
            if (item == null) return;

            pnlDetails.DataContext = item; // rarity colour
            tbDetailName.Text = item.Name;
            tbDetailRarity.Text = item.Rarity + (item.RequiresAttunement
                ? " (requires attunement" + (string.IsNullOrEmpty(item.AttunementNote) ? "" : " " + item.AttunementNote) + ")"
                : "");

            var meta = new List<string> { item.Category, item.Source };
            if (item.ValueGp.HasValue) meta.Add(FormatGp(item.ValueGp.Value));
            if (item.Weight.HasValue) meta.Add(item.Weight.Value.ToString("0.##", CultureInfo.InvariantCulture) + " lb.");
            tbDetailMeta.Text = string.Join(" · ", meta.Where(m => !string.IsNullOrEmpty(m)));

            tbDetailProperties.Text = item.Properties ?? "";
            tbDetailProperties.Visibility = string.IsNullOrEmpty(item.Properties) ? Visibility.Collapsed : Visibility.Visible;
            tbDetailDescription.Text = string.IsNullOrEmpty(item.Description) ? "No description." : item.Description;
        }

        private void ShowNameOnly(string name)
        {
            ShowDetails(null, false);
            tbDetailsEmpty.Text = name + " — not in the item list (no details).";
        }

        // ── Add to table ─────────────────────────────────────────────────────

        private void btnAddToTable_Click(object sender, RoutedEventArgs e) => AddSelectedItemToTable();

        private void AddSelectedItemToTable()
        {
            try
            {
                var item = lbItems.SelectedItem as Item;
                var table = cmbAddToTable.SelectedItem as LootTable;
                if (item == null) return;
                if (table == null)
                {
                    MessageBox.Show("Create a loot table first (LOOT TABLES tab → + New Table).", "No loot table",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var existing = LootGeneratorDataController.LoadEntries(table.Id);
                var entry = new LootTableEntry
                {
                    Id = Guid.NewGuid(),
                    LootTableId = table.Id,
                    ItemId = item.Id,
                    Name = item.Name,
                    Weight = 1,
                    Quantity = "1",
                    SortOrder = existing.Count == 0 ? 0 : existing.Max(x => x.SortOrder) + 1
                };
                LootGeneratorDataController.SaveEntry(entry);
                if (_currentTable?.Id == table.Id) LoadEntries();

                NotificationHelper.NewNotification("Loot table", $"{item.Name} added to {table.Name}", Notifications.Wpf.NotificationType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Loot tables ──────────────────────────────────────────────────────

        private void LoadTables(Guid? select = null)
        {
            _tables = LootGeneratorDataController.LoadTables();
            lbTables.ItemsSource = _tables;
            cmbAddToTable.ItemsSource = _tables;

            var target = select.HasValue ? _tables.FirstOrDefault(t => t.Id == select.Value) : null;
            target = target ?? (_currentTable != null ? _tables.FirstOrDefault(t => t.Id == _currentTable.Id) : null);
            lbTables.SelectedItem = target;
            cmbAddToTable.SelectedItem = target ?? _tables.FirstOrDefault(t => string.IsNullOrEmpty(t.Source)) ?? _tables.FirstOrDefault();
        }

        private void lbTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentTable = lbTables.SelectedItem as LootTable;
            txtTableName.Text = _currentTable?.Name ?? "";
            tbTableTitle.Text = _currentTable?.Name ?? "Select or create a loot table";
            tbTableDescription.Text = _currentTable?.Description ?? "";
            if (_currentTable != null) cmbAddToTable.SelectedItem = _currentTable;
            LoadEntries();
        }

        private void LoadEntries()
        {
            _entries.Clear();
            if (_currentTable == null) return;
            foreach (var entry in LootGeneratorDataController.LoadEntries(_currentTable.Id))
            {
                _itemsById.TryGetValue(entry.ItemId, out Item item);
                _entries.Add(new EntryView(entry, item));
            }
            UpdateChances();
        }

        private void UpdateChances()
        {
            int total = _entries.Sum(e => Math.Max(0, e.Weight));
            foreach (var e in _entries)
                e.ChanceText = total > 0 ? (100.0 * Math.Max(0, e.Weight) / total).ToString("0.#", CultureInfo.InvariantCulture) + " %" : "—";
        }

        private void btnNewTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var table = new LootTable { Id = Guid.NewGuid(), Name = "New loot table", Description = "" };
                LootGeneratorDataController.SaveTable(table);
                LoadTables(table.Id);
                txtTableName.Focus();
                txtTableName.SelectAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtTableName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RenameCurrentTable();
        }

        private void txtTableName_LostFocus(object sender, RoutedEventArgs e) => RenameCurrentTable();

        private void RenameCurrentTable()
        {
            string name = (txtTableName.Text ?? "").Trim();
            if (_currentTable == null || name.Length == 0 || name == _currentTable.Name) return;
            _currentTable.Name = name;
            LootGeneratorDataController.SaveTable(_currentTable);
            LoadTables(_currentTable.Id);
        }

        private void btnDeleteTable_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTable == null) return;
            if (MessageBox.Show($"Delete the loot table \"{_currentTable.Name}\"?", "Delete loot table",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            LootGeneratorDataController.DeleteTable(_currentTable.Id);
            _currentTable = null;
            LoadTables();
            if (_tables.Count > 0) lbTables.SelectedIndex = 0;
        }

        // ── Entries ──────────────────────────────────────────────────────────

        private void lbEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(lbEntries.SelectedItem is EntryView entry)) return;
            if (entry.Item != null) ShowDetails(entry.Item, allowAddToTable: false);
            else ShowNameOnly(entry.Name);
        }

        private void EntryField_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!((sender as FrameworkElement)?.DataContext is EntryView entry)) return;
            if (!LootRoller.IsValidQuantity(entry.Quantity)) entry.Quantity = "1";
            LootGeneratorDataController.SaveEntry(entry.Entry);
            UpdateChances();
        }

        private void btnRemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            if (!((sender as Button)?.Tag is EntryView entry)) return;
            LootGeneratorDataController.DeleteEntry(entry.Entry.Id);
            _entries.Remove(entry);
            UpdateChances();
        }

        private void txtNewEntry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) AddTextEntry();
        }

        private void btnAddTextEntry_Click(object sender, RoutedEventArgs e) => AddTextEntry();

        private void AddTextEntry()
        {
            string name = (txtNewEntry.Text ?? "").Trim();
            if (_currentTable == null || name.Length == 0) return;

            // Link to an item when the name matches one exactly.
            var item = _allItems.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
            var entry = new LootTableEntry
            {
                Id = Guid.NewGuid(),
                LootTableId = _currentTable.Id,
                ItemId = item?.Id ?? Guid.Empty,
                Name = item?.Name ?? name,
                Weight = 1,
                Quantity = "1",
                SortOrder = _entries.Count == 0 ? 0 : _entries.Max(x => x.Entry.SortOrder) + 1
            };
            LootGeneratorDataController.SaveEntry(entry);
            _entries.Add(new EntryView(entry, item));
            UpdateChances();
            txtNewEntry.Text = "";
        }

        // ── Rolling ──────────────────────────────────────────────────────────

        private void btnRollFromFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_filteredItems.Count == 0) return;
            int count = ParseCount(txtFilterRollCount);
            foreach (var item in LootRoller.PickDistinct(_filteredItems, count))
                AddResult(item.Name, item, 1);
            UpdateResultsTotal();
        }

        private void btnRollTable_Click(object sender, RoutedEventArgs e)
        {
            var entries = _entries.ToList();
            if (_currentTable == null || entries.Count == 0) return;

            int rolls = ParseCount(txtTableRollCount);
            for (int i = 0; i < rolls; i++)
            {
                var picked = LootRoller.PickWeighted(entries, x => x.Weight);
                if (picked == null) break;
                int quantity = LootRoller.RollQuantity(picked.Quantity);
                if (quantity > 0) AddResult(picked.Name, picked.Item, quantity);
            }
            UpdateResultsTotal();
        }

        private static int ParseCount(TextBox box)
        {
            if (!int.TryParse((box.Text ?? "").Trim(), out int n) || n < 1) n = 1;
            n = Math.Min(n, 100);
            box.Text = n.ToString();
            return n;
        }

        /// <summary>Adds to the rolled loot, merging identical results.</summary>
        private void AddResult(string name, Item item, int quantity)
        {
            var existing = _results.FirstOrDefault(r => item != null ? r.Item?.Id == item.Id : r.Item == null && r.Name == name);
            if (existing != null)
            {
                existing.Quantity += quantity;
                return;
            }
            _results.Add(new ResultView(name, item, quantity));
        }

        private void UpdateResultsTotal()
        {
            decimal total = _results.Where(r => r.Item?.ValueGp != null).Sum(r => r.Item.ValueGp.Value * r.Quantity);
            int count = _results.Sum(r => r.Quantity);
            tbResultsTotal.Text = count == 0 ? "" : $"{count} item{(count == 1 ? "" : "s")}" + (total > 0 ? $" · known value {FormatGp(total)}" : "");
        }

        private void lbResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(lbResults.SelectedItem is ResultView result)) return;
            if (result.Item != null) ShowDetails(result.Item, allowAddToTable: false);
            else ShowNameOnly(result.Name);
        }

        private void btnClearResults_Click(object sender, RoutedEventArgs e)
        {
            _results.Clear();
            UpdateResultsTotal();
        }

        private void btnCopyResults_Click(object sender, RoutedEventArgs e)
        {
            if (_results.Count == 0) return;
            var sb = new StringBuilder();
            foreach (var r in _results)
            {
                sb.Append(r.Quantity > 1 ? r.Quantity + " × " : "").Append(r.Name);
                var extra = new List<string>();
                if (r.Item != null && r.Item.IsMagic) extra.Add(r.Item.Rarity + (r.Item.RequiresAttunement ? ", attunement" : ""));
                if (r.Item?.ValueGp != null) extra.Add(FormatGp(r.Item.ValueGp.Value * r.Quantity));
                if (extra.Count > 0) sb.Append(" (").Append(string.Join(", ", extra)).Append(")");
                sb.AppendLine();
            }
            if (!string.IsNullOrEmpty(tbResultsTotal.Text)) sb.AppendLine(tbResultsTotal.Text);
            try
            {
                Clipboard.SetText(sb.ToString());
                NotificationHelper.NewNotification("Loot", "Copied to the clipboard", Notifications.Wpf.NotificationType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not copy: " + ex.Message, "Clipboard", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string FormatGp(decimal gp) => gp.ToString("#,0.##", CultureInfo.InvariantCulture) + " gp";

        // ── View models ──────────────────────────────────────────────────────

        public abstract class Notifier : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public class EntryView : Notifier
        {
            private string _chanceText;

            public EntryView(LootTableEntry entry, Item item) { Entry = entry; Item = item; }

            public LootTableEntry Entry { get; }
            public Item Item { get; }
            public string Name => Entry.Name;
            public string Rarity => Item != null && Item.IsMagic ? Item.Rarity : "";

            public int Weight
            {
                get => Entry.Weight;
                set { Entry.Weight = Math.Max(0, value); OnChanged(nameof(Weight)); }
            }

            public string Quantity
            {
                get => Entry.Quantity;
                set { Entry.Quantity = string.IsNullOrWhiteSpace(value) ? "1" : value.Trim(); OnChanged(nameof(Quantity)); }
            }

            public string ChanceText
            {
                get => _chanceText;
                set { _chanceText = value; OnChanged(nameof(ChanceText)); }
            }
        }

        public class ResultView : Notifier
        {
            private int _quantity;

            public ResultView(string name, Item item, int quantity) { Name = name; Item = item; _quantity = quantity; }

            public string Name { get; }
            public Item Item { get; }
            public string Rarity => Item != null && Item.IsMagic ? Item.Rarity : "";

            public int Quantity
            {
                get => _quantity;
                set
                {
                    _quantity = value;
                    OnChanged(nameof(Quantity));
                    OnChanged(nameof(QuantityText));
                    OnChanged(nameof(ValueText));
                }
            }

            public string QuantityText => _quantity + "×";
            public string ValueText => Item?.ValueGp != null ? FormatGp(Item.ValueGp.Value * _quantity) : "";
        }
    }
}
