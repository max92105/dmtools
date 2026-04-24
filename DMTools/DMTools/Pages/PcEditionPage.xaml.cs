using Controllers.Controllers;
using Data.DataModels.PcEditionPage;
using DMTools.Helpers;
using Notifications.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for PcEditionPage.xaml
    /// </summary>
    public partial class PcEditionPage : Page
    {
        private PcEditionPageDataModel _PcEditionPageDataModel;
        private Guid _PlayerCharacterId;

        public PcEditionPage(Guid playerCharacter)
        {
            try
            {
                InitializeComponent();

                _PlayerCharacterId = playerCharacter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Save()
        {
            try
            {
                PcEditionPageDataController.Save(_PcEditionPageDataModel);

                NotificationHelper.NewNotification("Save Player Character", "Player Character Saved Successfully", NotificationType.Success);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_PlayerCharacterId != Guid.Empty)
                    _PcEditionPageDataModel = PcEditionPageDataController.LoadPlayerCharacter(_PlayerCharacterId);
                else
                    _PcEditionPageDataModel = PcEditionPageDataController.LoadNewPlayerCharacter();

                DataContext = _PcEditionPageDataModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _PcEditionPageDataModel.PlayerCharacter.Delete();

                Save();

                NavigationService.Navigate(new PcManagerPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
