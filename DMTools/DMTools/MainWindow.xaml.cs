using Controllers.Controllers;
using DMTools.Helpers;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

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

        protected override void OnStateChanged(EventArgs e)
        {
            btnMaxRestore.Content = WindowState == WindowState.Maximized ? "❒" : "□";
            base.OnStateChanged(e);
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            bool canGoBack = MainFrame.CanGoBack;
            btnBack.Visibility = canGoBack ? Visibility.Visible : Visibility.Collapsed;

            // Show the page title in the title bar
            if (MainFrame.Content is System.Windows.Controls.Page page && page.Title != null)
                tbPageTitle.Text = page.Title;
            else
                tbPageTitle.Text = "DM Tools";
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else if (WindowState == WindowState.Normal)
                DragMove();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        private void Window_Closed(object sender, EventArgs e)
            => Application.Current.Shutdown();
    }
}
