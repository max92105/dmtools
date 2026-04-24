using Controllers.Controllers;
using Data.DataModels.MonsterManagerPage;
using Data.Objects;
using Data.VirtualObject;
using DMTools.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for MonsterManagerPage.xaml
    /// </summary>
    public partial class MonsterManagerPage : Page
    {
        private MonsterManagerPageDataModel _MonsterManagerPageDataModel;

        private static readonly DependencyProperty _SelectedMonsterProperty =
        DependencyProperty.Register(
        "SelectedMonster", typeof(DisplayMonster),
        typeof(MonsterManagerPage));

        public DisplayMonster SelectedMonster
        {
            get { return (DisplayMonster)GetValue(_SelectedMonsterProperty); }
            set { SetValue(_SelectedMonsterProperty, value); }
        }

        public MonsterManagerPage()
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

        private void Load()
        {
            try
            {
                _MonsterManagerPageDataModel = MonsterManagerPageDataController.Load();
                DataContext = _MonsterManagerPageDataModel;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 

        private void FilterMonster()
        {
            try
            {
                MonsterManagerPageDataModel monsterManagerPageDataModel = MonsterManagerPageDataController.SearchMonster(txtSearchName.Text);
                _MonsterManagerPageDataModel.Monsters = monsterManagerPageDataModel.Monsters;
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
                Load();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FilterMonster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgmonsters_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedMonster != null)
                    NavigationService.Navigate(new MonsterEditionPage(SelectedMonster.Id));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnShowStatBlock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(SelectedMonster != null)
                    StatBlockHelper.ShowStatBlockWithMonsterId(SelectedMonster.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new MonsterEditionPage(Guid.Empty));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(SelectedMonster != null)
                    NavigationService.Navigate(new MonsterEditionPage(SelectedMonster.Id));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_MonsterManagerPageDataModel.Monsters.Any(obj => obj.IsSelected))
                {
                    MonsterFileHelper.ExportMonsters(_MonsterManagerPageDataModel.Monsters
                        .Where(obj => obj.IsSelected).Select(obj => obj.Id).ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonsterFileHelper.ImportMonsters();

                MonsterManagerPageDataModel monsterManagerPageDataModel = MonsterManagerPageDataController.SearchMonster(txtSearchName.Text);
                _MonsterManagerPageDataModel.Monsters = monsterManagerPageDataModel.Monsters;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}