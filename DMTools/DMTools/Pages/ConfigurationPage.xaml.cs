using Controllers.Controllers;
using System.Windows;
using System.Windows.Controls;
using TypeEntry = Data.Objects.Type;

namespace DMTools.Pages
{
    public partial class ConfigurationPage : Page
    {
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lbTypes.ItemsSource    = ConfigurationPageDataController.LoadMonsterTypes();
            lbSubtypes.ItemsSource = ConfigurationPageDataController.LoadMonsterSubtypes();
        }

        // ── Types ──────────────────────────────────────────────────────────────

        private void lbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbTypes.SelectedItem is TypeEntry t)
                txtTypeName.Text = t.Name;
        }

        private void btnAddType_Click(object sender, RoutedEventArgs e)
        {
            string name = txtTypeName.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var entry = new TypeEntry { Id = System.Guid.NewGuid() };
            entry.Name = name;
            ConfigurationPageDataController.SaveType(entry);
            txtTypeName.Clear();
            lbTypes.ItemsSource = ConfigurationPageDataController.LoadMonsterTypes();
        }

        private void btnRenameType_Click(object sender, RoutedEventArgs e)
        {
            if (!(lbTypes.SelectedItem is TypeEntry selected)) return;
            string name = txtTypeName.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            selected.Name = name;
            ConfigurationPageDataController.SaveType(selected);
            txtTypeName.Clear();
            lbTypes.ItemsSource = ConfigurationPageDataController.LoadMonsterTypes();
        }

        private void btnDeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (!(lbTypes.SelectedItem is TypeEntry selected)) return;
            selected.Delete();
            ConfigurationPageDataController.SaveType(selected);
            txtTypeName.Clear();
            lbTypes.ItemsSource = ConfigurationPageDataController.LoadMonsterTypes();
        }

        // ── Subtypes ───────────────────────────────────────────────────────────

        private void lbSubtypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbSubtypes.SelectedItem is TypeEntry t)
                txtSubtypeName.Text = t.Name;
        }

        private void btnAddSubtype_Click(object sender, RoutedEventArgs e)
        {
            string name = txtSubtypeName.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            var entry = new TypeEntry { Id = System.Guid.NewGuid() };
            entry.Name = name;
            ConfigurationPageDataController.SaveSubtype(entry);
            txtSubtypeName.Clear();
            lbSubtypes.ItemsSource = ConfigurationPageDataController.LoadMonsterSubtypes();
        }

        private void btnRenameSubtype_Click(object sender, RoutedEventArgs e)
        {
            if (!(lbSubtypes.SelectedItem is TypeEntry selected)) return;
            string name = txtSubtypeName.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;
            selected.Name = name;
            ConfigurationPageDataController.SaveSubtype(selected);
            txtSubtypeName.Clear();
            lbSubtypes.ItemsSource = ConfigurationPageDataController.LoadMonsterSubtypes();
        }

        private void btnDeleteSubtype_Click(object sender, RoutedEventArgs e)
        {
            if (!(lbSubtypes.SelectedItem is TypeEntry selected)) return;
            selected.Delete();
            ConfigurationPageDataController.SaveSubtype(selected);
            txtSubtypeName.Clear();
            lbSubtypes.ItemsSource = ConfigurationPageDataController.LoadMonsterSubtypes();
        }
    }
}
