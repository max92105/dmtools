using Controllers.Controllers;
using DMTools.Helpers;
using System.Windows;

namespace DMTools
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StatBlockHelper.Initialize();
            LegacyMigrationController.MigrateLegacyData();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
