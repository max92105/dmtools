using Controllers.Controllers;
using Data.DataModels.PcManagerPage;
using Data.Objects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for PcManagerPage.xaml
    /// </summary>
    public partial class PcManagerPage : Page
    {
        private PcManagerPageDataModel _PcManagerPageDataModel;

        private static readonly DependencyProperty _SelectedPlayerCharacterProperty =
        DependencyProperty.Register(
        "SelectedPlayerCharacter", typeof(PlayerCharacter),
        typeof(PcManagerPage));

        public PlayerCharacter SelectedPlayerCharacter
        {
            get { return (PlayerCharacter)GetValue(_SelectedPlayerCharacterProperty); }
            set { SetValue(_SelectedPlayerCharacterProperty, value); }
        }

        public PcManagerPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterPlayerCharacter()
        {
            try
            {
                PcManagerPageDataModel pcManagerPageDataModel = PcManagerPageDataController.SearchMonster(txtSearchName.Text);
                _PcManagerPageDataModel.PlayerCharaters = pcManagerPageDataModel.PlayerCharaters;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPlayerCharacter()
        {
            try
            {
                if (SelectedPlayerCharacter != null)
                    NavigationService.Navigate(new PcEditionPage(SelectedPlayerCharacter.Id));
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
                _PcManagerPageDataModel = PcManagerPageDataController.Load();
                DataContext = _PcManagerPageDataModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FilterPlayerCharacter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewPc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new PcEditionPage(Guid.Empty));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditPc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EditPlayerCharacter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgPlayerCharacters_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                EditPlayerCharacter();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
