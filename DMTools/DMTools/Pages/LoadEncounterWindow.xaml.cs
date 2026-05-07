using Controllers.Controllers;
using Data.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DMTools.Pages
{
    public partial class LoadEncounterWindow : Window
    {
        public Encounter SelectedEncounter { get; private set; }

        private List<Encounter> _allEncounters;

        public LoadEncounterWindow()
        {
            InitializeComponent();
            _allEncounters = new List<Encounter>(EncounterBuilderPageDataController.LoadEncounters());
            lbEncounters.ItemsSource = _allEncounters;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string term = txtSearch.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
                lbEncounters.ItemsSource = _allEncounters;
            else
                lbEncounters.ItemsSource = _allEncounters
                    .Where(enc => enc.Name != null &&
                        enc.Name.IndexOf(term, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            btnLoad.IsEnabled = false;
        }

        private void lbEncounters_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => btnLoad.IsEnabled = lbEncounters.SelectedItem != null;

        private void lbEncounters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbEncounters.SelectedItem is Encounter) Confirm();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e) => Confirm();
        private void btnCancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

        private void Confirm()
        {
            SelectedEncounter = lbEncounters.SelectedItem as Encounter;
            if (SelectedEncounter != null) DialogResult = true;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }
    }
}
