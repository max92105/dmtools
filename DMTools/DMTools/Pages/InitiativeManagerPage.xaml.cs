using Controllers.Controllers;
using Controllers.Factories;
using Data.DataModels.InitiativeManagerPage;
using Data.Objects;
using Data.VirtualObject;
using DMTools.Helpers;
using Notifications.Wpf;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for InitiativeManagerPage.xaml
    /// </summary>
    public partial class InitiativeManagerPage : Page
    {
        private InitiativeManagerPageDataModel _InitiativeManagerPageDataModel;

        private static readonly DependencyProperty _SelectedMonsterProperty =
        DependencyProperty.Register(
        "SelectedMonster", typeof(Monster),
        typeof(InitiativeManagerPage));

        public Monster SelectedMonster
        {
            get { return (Monster)GetValue(_SelectedMonsterProperty); }
            set { SetValue(_SelectedMonsterProperty, value); }
        }

        private static readonly DependencyProperty _SelectedDisplayPlayerCharacterProperty =
        DependencyProperty.Register(
        "SelectedDisplayPlayerCharacter", typeof(DisplayPlayerCharacter),
        typeof(InitiativeManagerPage));

        public DisplayPlayerCharacter SelectedPlayerCharacter
        {
            get { return (DisplayPlayerCharacter)GetValue(_SelectedDisplayPlayerCharacterProperty); }
            set { SetValue(_SelectedDisplayPlayerCharacterProperty, value); }
        }

        private static readonly DependencyProperty _SelectedInitativeEntryProperty =
        DependencyProperty.Register(
        "SelectedInitativeEntry", typeof(InitiativeEntry),
        typeof(InitiativeManagerPage));

        public InitiativeEntry SelectedInitativeEntry
        {
            get { return (InitiativeEntry)GetValue(_SelectedInitativeEntryProperty); }
            set { SetValue(_SelectedInitativeEntryProperty, value); }
        }

        private static readonly DependencyProperty _MonsterQuantityProperty =
        DependencyProperty.Register(
        "MonsterQuantity", typeof(Int32),
        typeof(InitiativeManagerPage));

        public Int32 MonsterQuantity
        {
            get { return (Int32)GetValue(_MonsterQuantityProperty); }
            set { SetValue(_MonsterQuantityProperty, value); }
        }

        public InitiativeManagerPage()
        {
            InitializeComponent();
        }

        private void Load()
        {
            try
            {
                NavigationService.Navigating += new NavigatingCancelEventHandler(NavigationService_Navigating);

                _InitiativeManagerPageDataModel = InitiativeManagerPageDataController.Load();
                DataContext = _InitiativeManagerPageDataModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear()
        {
            lbMonsters.SelectedItem = null;
            MonsterQuantity = 0;
        }

        private void OrderInitiativeEntries()
        {
            _InitiativeManagerPageDataModel.InitiativeEntries = new ObservableCollection<InitiativeEntry>(
                        _InitiativeManagerPageDataModel.InitiativeEntries.OrderByDescending(obj => obj.Initiative).ThenBy(obj => obj.TieBreaker));
        }

        private void SetTurn(int changeIndex)
        {
            if (!_InitiativeManagerPageDataModel.InitiativeEntries.Any(obj => obj.ItsTurn))
            {
                _InitiativeManagerPageDataModel.InitiativeEntries.First().ItsTurn = true;
            }
            else
            {
                var initiativeEntry = _InitiativeManagerPageDataModel.InitiativeEntries.First(obj => obj.ItsTurn);
                initiativeEntry.ItsTurn = false;

                var turnIndex = _InitiativeManagerPageDataModel.InitiativeEntries.IndexOf(initiativeEntry);

                var newturnIndex = turnIndex + changeIndex;

                if (newturnIndex < 0)
                    newturnIndex = _InitiativeManagerPageDataModel.InitiativeEntries.Count + (newturnIndex);

                if (newturnIndex >= _InitiativeManagerPageDataModel.InitiativeEntries.Count)
                    newturnIndex = 0;

                _InitiativeManagerPageDataModel.InitiativeEntries.ElementAt(newturnIndex).ItsTurn = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Load();
                if (EncounterService.PendingEntries != null && EncounterService.PendingEntries.Count > 0)
                {
                    LoadPendingEncounter(EncounterService.PendingEntries);
                    EncounterService.PendingEntries = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPendingEncounter(System.Collections.Generic.List<EncounterPendingEntry> pending)
        {
            var monsterFactory = new MonsterFactory();
            var random = new Random();

            foreach (var entry in pending)
            {
                var monster = monsterFactory.GetObject(entry.MonsterId);
                if (monster.Id == Guid.Empty) continue;

                var dexModel = InitiativeManagerPageDataController.LoadDexterity(monster.Id);

                for (int i = 0; i < entry.Quantity; i++)
                {
                    int initiative = random.Next(1, 21) + dexModel.Dexterity.Modifier;
                    int tiebreaker = _InitiativeManagerPageDataModel.InitiativeEntries
                        .Count(obj => obj.Initiative == initiative);
                    int nbOfMonster = _InitiativeManagerPageDataModel.InitiativeEntries
                        .Count(obj => obj.MonsterId != Guid.Empty) + 1;

                    _InitiativeManagerPageDataModel.InitiativeEntries.Add(new InitiativeEntry
                    {
                        MonsterId = monster.Id,
                        ArmorClass = monster.ArmorClass,
                        HitPoints = monster.HitPoints,
                        DexterityModifier = (Int16)dexModel.Dexterity.Modifier,
                        Initiative = initiative,
                        TieBreaker = tiebreaker,
                        Name = nbOfMonster + " - " + monster.Name
                    });
                }
            }

            OrderInitiativeEntries();
        }

        private void Page_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Right)
                SetTurn(1);

            if (e.Key == System.Windows.Input.Key.Left)
                SetTurn(-1);
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                if(_InitiativeManagerPageDataModel.InitiativeEntries.Count > 0)
                    e.Cancel = true;
            }
        }

        //Monster
        private void lbMonsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (SelectedMonster != null)
                {                  
                    InitiativeManagerPageDataModel initiativeManagerPageDataModel = InitiativeManagerPageDataController.LoadDexterity(SelectedMonster.Id);
                    _InitiativeManagerPageDataModel.Dexterity = initiativeManagerPageDataModel.Dexterity;
                    MonsterQuantity = 1;
                }
               
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
                InitiativeManagerPageDataModel initiativeManagerPageDataModel = InitiativeManagerPageDataController.Search(txtSearchName.Text);
                _InitiativeManagerPageDataModel.Monsters = initiativeManagerPageDataModel.Monsters;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedMonster != null)
                {
                    Random random = new Random();

                    for (Int32 i = 0; i < MonsterQuantity; i++)
                    {
                        Int32 initiative = random.Next(1, 21) + _InitiativeManagerPageDataModel.Dexterity.Modifier;
                        Int32 tiebreaker = _InitiativeManagerPageDataModel.InitiativeEntries.Where(obj => obj.Initiative == initiative).Count();

                        InitiativeEntry initiativeEntry = new InitiativeEntry
                        {
                            MonsterId = SelectedMonster.Id,
                            ArmorClass = SelectedMonster.ArmorClass,
                            HitPoints = SelectedMonster.HitPoints,
                            DexterityModifier = (Int16)_InitiativeManagerPageDataModel.Dexterity.Modifier,
                            Initiative = initiative,
                            TieBreaker = tiebreaker
                        };

                        Int32 nbOfMonster = _InitiativeManagerPageDataModel.InitiativeEntries.Where(obj => obj.MonsterId != Guid.Empty).Count() + 1;
                        initiativeEntry.Name = nbOfMonster + " - " + SelectedMonster.Name;
                        _InitiativeManagerPageDataModel.InitiativeEntries.Add(initiativeEntry);
                    }

                    Clear();
                    OrderInitiativeEntries();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Player
        private void btnAddToInitiative_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedPlayerCharacter != null)
                {
                    dgPlayers.CommitEdit();
                    dgPlayers.Items.Refresh();

                    Int32 tiebreaker = _InitiativeManagerPageDataModel.InitiativeEntries.Where(obj => obj.Initiative == SelectedPlayerCharacter.Initiative).Count();

                    InitiativeEntry initiativeEntry = new InitiativeEntry
                    {
                        ItsTurn = false,
                        Name = SelectedPlayerCharacter.Name,
                        PlayerCharacterId = SelectedPlayerCharacter.Id,
                        ArmorClass = SelectedPlayerCharacter.ArmorClass,
                        DexterityModifier = SelectedPlayerCharacter.InitiativeBonus,
                        Initiative = SelectedPlayerCharacter.Initiative,
                        TieBreaker = tiebreaker
                    };

                    _InitiativeManagerPageDataModel.InitiativeEntries.Add(initiativeEntry);
                    OrderInitiativeEntries();

                    NotificationHelper.NewNotification("Player Added", SelectedPlayerCharacter.Name + " As been added to initiative", NotificationType.Success);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSeeHtml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedMonster != null)
                    StatBlockHelper.ShowStatBlock(SelectedMonster);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Initiative

        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetTurn(-1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetTurn(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedInitativeEntry != null)
                {
                    SelectedInitativeEntry.Initiative++;
                    OrderInitiativeEntries();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedInitativeEntry != null)
                {
                    SelectedInitativeEntry.Initiative--;
                    OrderInitiativeEntries();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRerollInitiative_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedInitativeEntry != null)
                {
                    Random random = new Random();

                    SelectedInitativeEntry.Initiative = random.Next(1,21) + SelectedInitativeEntry.DexterityModifier;
                    OrderInitiativeEntries();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRerollAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Random random = new Random();

                foreach (InitiativeEntry initiativeEntry in _InitiativeManagerPageDataModel.InitiativeEntries)
                    initiativeEntry.Initiative = random.Next(1, 21) + initiativeEntry.DexterityModifier;
                
                OrderInitiativeEntries();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnResetInitiative_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _InitiativeManagerPageDataModel.InitiativeEntries.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txt_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        private void dgInitiativeEntries_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (SelectedInitativeEntry != null)
            {
                try
                {
                    (sender as DataGrid).RowEditEnding -= dgInitiativeEntries_RowEditEnding;
                    (sender as DataGrid).CommitEdit();
                    (sender as DataGrid).Items.Refresh();
                    (sender as DataGrid).RowEditEnding += dgInitiativeEntries_RowEditEnding;

                    if (SelectedInitativeEntry.MonsterId != Guid.Empty && SelectedInitativeEntry.HitPoints == 0)
                        _InitiativeManagerPageDataModel.InitiativeEntries.Remove(SelectedInitativeEntry);

                    OrderInitiativeEntries();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                return;
        }

        private void btnShowStatBlock_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedInitativeEntry != null && SelectedInitativeEntry.MonsterId != Guid.Empty)
                    StatBlockHelper.ShowStatBlockWithMonsterId(SelectedInitativeEntry.MonsterId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
