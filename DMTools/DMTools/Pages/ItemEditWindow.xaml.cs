using Data.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace DMTools.Pages
{
    /// <summary>Creates or edits an item of the loot library.</summary>
    public partial class ItemEditWindow : DMTools.Helpers.DarkWindow
    {
        public static readonly string[] Rarities =
            { "Common", "Uncommon", "Rare", "Very Rare", "Legendary", "Artifact", "Varies", "Unknown (Magic)", "Mundane" };

        private readonly Item _original;

        /// <summary>The saved item (a new object; the caller applies it). Null when cancelled.</summary>
        public Item Result { get; private set; }

        /// <param name="item">Item to edit, or null to create one.</param>
        public ItemEditWindow(Item item, IEnumerable<string> categories, IEnumerable<string> sources)
        {
            InitializeComponent();
            _original = item;

            cboCategory.ItemsSource = categories.Where(c => !string.IsNullOrEmpty(c)).Distinct().OrderBy(c => c).ToList();
            cboSource.ItemsSource = sources.Where(s => !string.IsNullOrEmpty(s)).Distinct().OrderBy(s => s).ToList();
            cboRarity.ItemsSource = Rarities;

            if (item == null)
            {
                Title = tbHeader.Text = "New Item";
                cboCategory.Text = "Wondrous Item";
                cboRarity.SelectedItem = "Uncommon";
                cboSource.Text = "Homebrew";
            }
            else
            {
                Title = tbHeader.Text = "Edit Item";
                txtName.Text = item.Name;
                cboCategory.Text = item.Category;
                cboRarity.SelectedItem = Rarities.Contains(item.Rarity) ? item.Rarity : null;
                if (cboRarity.SelectedItem == null && !string.IsNullOrEmpty(item.Rarity))
                {
                    // Keep an unusual rarity from the import ("Unknown") selectable.
                    cboRarity.ItemsSource = Rarities.Concat(new[] { item.Rarity }).ToList();
                    cboRarity.SelectedItem = item.Rarity;
                }
                cboSource.Text = item.Source;
                txtValue.Text = item.ValueGp?.ToString("0.##", CultureInfo.CurrentCulture) ?? "";
                txtWeight.Text = item.Weight?.ToString("0.##", CultureInfo.CurrentCulture) ?? "";
                chkAttunement.IsChecked = item.RequiresAttunement;
                txtAttunementNote.Text = item.AttunementNote;
                txtProperties.Text = item.Properties;
                txtDescription.Text = item.Description;
            }

            Loaded += (s, e) => { txtName.Focus(); txtName.SelectAll(); };
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string name = (txtName.Text ?? "").Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("The item needs a name.", "Item", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (!TryParseOptional(txtValue.Text, out decimal? value))
            {
                MessageBox.Show("The value must be a number (in gold pieces), or empty.", "Item", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtValue.Focus();
                return;
            }
            if (!TryParseOptional(txtWeight.Text, out decimal? weight))
            {
                MessageBox.Show("The weight must be a number (in pounds), or empty.", "Item", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtWeight.Focus();
                return;
            }

            string rarity = cboRarity.SelectedItem as string ?? "Mundane";
            bool attunement = chkAttunement.IsChecked == true;

            Result = new Item
            {
                Id = _original?.Id ?? Guid.NewGuid(),
                Name = name,
                Category = NullIfEmpty(cboCategory.Text) ?? "Other",
                Rarity = rarity,
                IsMagic = rarity != "Mundane",
                Source = NullIfEmpty(cboSource.Text),
                ValueGp = value,
                Weight = weight,
                RequiresAttunement = attunement,
                AttunementNote = attunement ? NullIfEmpty(txtAttunementNote.Text) : null,
                Properties = NullIfEmpty(txtProperties.Text),
                Description = NullIfEmpty(txtDescription.Text),
            };

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string NullIfEmpty(string s)
        {
            s = (s ?? "").Trim();
            return s.Length == 0 ? null : s;
        }

        /// <summary>Empty → null; accepts "12.5" and "12,5" whatever the Windows language.</summary>
        private static bool TryParseOptional(string text, out decimal? result)
        {
            result = null;
            string t = (text ?? "").Trim().Replace(" ", "");
            if (t.Length == 0) return true;
            if (decimal.TryParse(t, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal v) ||
                decimal.TryParse(t.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out v))
            {
                if (v < 0) return false;
                result = v;
                return true;
            }
            return false;
        }
    }
}
