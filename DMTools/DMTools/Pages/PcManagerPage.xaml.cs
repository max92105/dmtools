using Controllers.Factories;
using Data.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DMTools.Pages
{
    public partial class PcManagerPage : Page
    {
        private List<PlayerCharacter> _allPcs = new List<PlayerCharacter>();
        private bool _suppressFilter = false;
        private bool _sidebarExpanded = true;

        public PcManagerPage()
        {
            InitializeComponent();
        }

        private void btnToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            _sidebarExpanded = !_sidebarExpanded;
            tbSidebarArrow.Text = _sidebarExpanded ? "❮" : "❯";

            var anim = new DoubleAnimation
            {
                To = _sidebarExpanded ? 200 : 0,
                Duration = TimeSpan.FromMilliseconds(220),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            sidebarWrapper.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                tbSidebarArrow.Text = "❮";
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh()
        {
            _allPcs = new List<PlayerCharacter>(new PlayerCharacterFactory().GetObjects());
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<PlayerCharacter> filtered = _allPcs;

            string name = txtSearchName.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
                filtered = filtered.Where(pc => pc.Name != null &&
                    pc.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (short.TryParse(txtMinLevel.Text, out short minLvl))
                filtered = filtered.Where(pc => pc.Level >= minLvl);
            if (short.TryParse(txtMaxLevel.Text, out short maxLvl))
                filtered = filtered.Where(pc => pc.Level <= maxLvl);

            if (short.TryParse(txtMinAC.Text, out short minAC))
                filtered = filtered.Where(pc => pc.ArmorClass >= minAC);
            if (short.TryParse(txtMaxAC.Text, out short maxAC))
                filtered = filtered.Where(pc => pc.ArmorClass <= maxAC);

            if (short.TryParse(txtMinInit.Text, out short minInit))
                filtered = filtered.Where(pc => pc.InitiativeBonus >= minInit);
            if (short.TryParse(txtMaxInit.Text, out short maxInit))
                filtered = filtered.Where(pc => pc.InitiativeBonus <= maxInit);

            if (short.TryParse(txtMinPP.Text, out short minPP))
                filtered = filtered.Where(pc => pc.PassivePerception >= minPP);
            if (short.TryParse(txtMaxPP.Text, out short maxPP))
                filtered = filtered.Where(pc => pc.PassivePerception <= maxPP);

            if (short.TryParse(txtMinHP.Text, out short minHP))
                filtered = filtered.Where(pc => pc.MaxHp >= minHP);
            if (short.TryParse(txtMaxHP.Text, out short maxHP))
                filtered = filtered.Where(pc => pc.MaxHp <= maxHP);

            lbPcs.ItemsSource = new ObservableCollection<PlayerCharacter>(filtered);
            btnEdit.IsEnabled = false;
        }

        private void OpenEditor(Guid id)
        {
            try
            {
                var window = new PcEditionWindow(id);
                window.Owner = Application.Current.MainWindow;
                if (window.ShowDialog() == true)
                    Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            if (_suppressFilter) return;
            try { ApplyFilter(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _suppressFilter = true;
            txtSearchName.Text = string.Empty;
            txtMinLevel.Text = string.Empty;
            txtMaxLevel.Text = string.Empty;
            txtMinAC.Text = string.Empty;
            txtMaxAC.Text = string.Empty;
            txtMinInit.Text = string.Empty;
            txtMaxInit.Text = string.Empty;
            txtMinPP.Text = string.Empty;
            txtMaxPP.Text = string.Empty;
            txtMinHP.Text = string.Empty;
            txtMaxHP.Text = string.Empty;
            _suppressFilter = false;
            ApplyFilter();
        }

        private void lbPcs_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            mainScroll.ScrollToVerticalOffset(mainScroll.VerticalOffset - e.Delta * 0.5);
            e.Handled = true;
        }

        private void lbPcs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEdit.IsEnabled = lbPcs.SelectedItem != null;
        }

        private void lbPcs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbPcs.SelectedItem is PlayerCharacter pc)
                OpenEditor(pc.Id);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
            => OpenEditor(Guid.Empty);

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lbPcs.SelectedItem is PlayerCharacter pc)
                OpenEditor(pc.Id);
        }
    }
}
