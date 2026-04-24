using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMonsterManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MonsterManagerPage());
        }

        private void btnInitiativeManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new InitiativeManagerPage());
        }

        private void btnPcManager_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PcManagerPage());
        }

        private void btnEncounterBuilder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EncounterBuilderPage());
        }
    }
}
