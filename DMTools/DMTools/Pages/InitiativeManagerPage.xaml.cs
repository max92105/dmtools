using Controllers.Controllers;
using Controllers.Factories;
using Controllers.VirtualFactories;
using Data.DataModels.InitiativeManagerPage;
using Data.Objects;
using Data.VirtualObject;
using DMTools.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace DMTools.Pages
{
    public partial class InitiativeManagerPage : Page
    {
        private InitiativeManagerPageDataModel _model;
        private List<Monster> _allMonsters = new List<Monster>();
        private List<DisplayPlayerCharacter> _allPlayers = new List<DisplayPlayerCharacter>();
        private bool _sidebarExpanded = true;
        private readonly Random _random = new Random();
        private string _selectedWaveColor = "#E69A28";
        private Border _selectedWaveColorBorder;
        private readonly Dictionary<Guid, int> _statBlockScrollPositions = new Dictionary<Guid, int>();
        private Guid _currentStatBlockMonsterId = Guid.Empty;
        private bool _statBlockPendingScrollRestore = false;

        public InitiativeManagerPage()
        {
            InitializeComponent();
        }

        // ── Load ─────────────────────────────────────────────────────────────

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigating += NavigationService_Navigating;

                _model = InitiativeManagerPageDataController.Load();
                DataContext = _model;

                _allMonsters = new List<Monster>(_model.Monsters);
                _allPlayers = new List<DisplayPlayerCharacter>(_model.DisplayPlayerCharacters);

                PopulateTypeFilter();
                ApplyMonsterFilter();
                icPlayers.ItemsSource = _allPlayers;

                tbSidebarArrow.Text = "❮";
                _selectedWaveColorBorder = wvColor1;

                LoadSavedInitiative();

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

        private void PopulateTypeFilter()
        {
            var types = new List<string> { "" };
            types.AddRange(_allMonsters
                .Where(m => !string.IsNullOrWhiteSpace(m.Type))
                .Select(m => m.Type)
                .Distinct()
                .OrderBy(t => t));
            cmbTypeFilter.ItemsSource = types;
            cmbTypeFilter.SelectedIndex = 0;
        }

        // ── Monster filter ────────────────────────────────────────────────────

        private void txtMonsterSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyMonsterFilter();
        private void txtCrFilter_TextChanged(object sender, TextChangedEventArgs e) => ApplyMonsterFilter();
        private void cmbTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyMonsterFilter();

        private void ApplyMonsterFilter()
        {
            IEnumerable<Monster> filtered = _allMonsters;

            string name = txtMonsterSearch.Text?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(name))
                filtered = filtered.Where(m => m.Name != null &&
                    m.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (decimal.TryParse(txtCrMin.Text, out decimal crMin))
                filtered = filtered.Where(m => m.ChallengeRating >= crMin);
            if (decimal.TryParse(txtCrMax.Text, out decimal crMax))
                filtered = filtered.Where(m => m.ChallengeRating <= crMax);

            string typeFilter = cmbTypeFilter.SelectedItem as string;
            if (!string.IsNullOrEmpty(typeFilter))
                filtered = filtered.Where(m => string.Equals(m.Type, typeFilter, StringComparison.OrdinalIgnoreCase));

            lbMonsters.ItemsSource = new ObservableCollection<Monster>(filtered);
        }

        // ── Player filter ─────────────────────────────────────────────────────

        private void txtPlayerSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyPlayerFilter();

        private void ApplyPlayerFilter()
        {
            string term = txtPlayerSearch.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
                icPlayers.ItemsSource = _allPlayers;
            else
                icPlayers.ItemsSource = _allPlayers
                    .Where(p => p.Name != null &&
                        p.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
        }

        // ── Monster selection / add ───────────────────────────────────────────

        private void lbMonsters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var monster = lbMonsters.SelectedItem as Monster;
            if (monster == null)
            {
                tbSelectedMonster.Text = "— select a monster —";
                btnAddMonster.IsEnabled = false;
                return;
            }

            tbSelectedMonster.Text = monster.Name;
            txtOverrideHp.Text = monster.HitPoints.ToString();
            txtOverrideAc.Text = monster.ArmorClass.ToString();
            txtOverrideName.Text = string.Empty;
            btnAddMonster.IsEnabled = true;
        }

        private void btnAddMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var monster = lbMonsters.SelectedItem as Monster;
                if (monster == null) return;

                int qty = int.TryParse(txtMonsterQty.Text, out int q) ? Math.Max(1, q) : 1;

                short hp = short.TryParse(txtOverrideHp.Text, out short h) ? h : monster.HitPoints;
                short ac = short.TryParse(txtOverrideAc.Text, out short a) ? a : monster.ArmorClass;
                string nameOverride = txtOverrideName.Text?.Trim();

                var dexModel = InitiativeManagerPageDataController.LoadDexterity(monster.Id);
                int dexMod = dexModel.Dexterity?.Modifier ?? 0;

                int wave = int.TryParse(txtWave.Text, out int w) ? Math.Max(1, w) : 1;

                for (int i = 0; i < qty; i++)
                {
                    int initiative = _random.Next(1, 21) + dexMod;
                    int tiebreaker = _model.InitiativeEntries.Count(obj => obj.Initiative == initiative);
                    int nbMonsters = _model.InitiativeEntries.Count(obj => obj.MonsterId != Guid.Empty) + 1;

                    string entryName = !string.IsNullOrEmpty(nameOverride)
                        ? (qty > 1 ? $"{nameOverride} {nbMonsters}" : nameOverride)
                        : $"{nbMonsters} - {monster.Name}";

                    var entry = new InitiativeEntry
                    {
                        MonsterId = monster.Id,
                        ArmorClass = ac,
                        HitPoints = hp,
                        MaxHitPoints = hp,
                        DexterityModifier = (short)dexMod,
                        Initiative = initiative,
                        TieBreaker = tiebreaker,
                        Name = entryName,
                        Wave = wave,
                        WaveColor = _selectedWaveColor,
                        ConditionImmunities = monster.ConditionImmunities
                    };

                    _model.InitiativeEntries.Add(entry);
                }

                OrderInitiativeEntries();
                RefreshEntries();
                txtMonsterQty.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Player panel ──────────────────────────────────────────────────────

        private void btnRollPlayerInit_Click(object sender, RoutedEventArgs e)
        {
            var pc = (sender as Button)?.Tag as DisplayPlayerCharacter;
            if (pc == null) return;
            pc.Initiative = (short)(_random.Next(1, 21) + pc.InitiativeBonus);
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pc = (sender as Button)?.Tag as DisplayPlayerCharacter;
                if (pc == null) return;

                int tiebreaker = _model.InitiativeEntries.Count(obj => obj.Initiative == pc.Initiative);

                var entry = new InitiativeEntry
                {
                    PlayerCharacterId = pc.Id,
                    Name = pc.Name,
                    ArmorClass = pc.ArmorClass,
                    DexterityModifier = pc.InitiativeBonus,
                    Initiative = pc.Initiative,
                    TieBreaker = tiebreaker,
                    Wave = 0,
                    WaveColor = "#6699FF"
                };

                entry.HitPoints = pc.MaxHp;
                entry.MaxHitPoints = pc.MaxHp;

                _model.InitiativeEntries.Add(entry);
                OrderInitiativeEntries();
                RefreshEntries();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewPlayer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new PcEditionWindow(Guid.Empty) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true)
                {
                    _allPlayers = new List<DisplayPlayerCharacter>(
                        new DisplayPlayerCharacterFactory().GetObjects());
                    ApplyPlayerFilter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PlayerItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;
            OpenPlayerEditor((sender as FrameworkElement)?.DataContext as DisplayPlayerCharacter);
        }

        private void btnEditPlayer_Click(object sender, RoutedEventArgs e)
        {
            OpenPlayerEditor((sender as Button)?.Tag as DisplayPlayerCharacter);
        }

        private void OpenPlayerEditor(DisplayPlayerCharacter pc)
        {
            if (pc == null) return;
            try
            {
                var win = new PcEditionWindow(pc.Id) { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() == true)
                {
                    _allPlayers = new List<DisplayPlayerCharacter>(
                        new DisplayPlayerCharacterFactory().GetObjects());
                    ApplyPlayerFilter();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── Load encounter ────────────────────────────────────────────────────

        private void btnLoadEncounter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new LoadEncounterWindow { Owner = Window.GetWindow(this) };
                if (win.ShowDialog() != true || win.SelectedEncounter == null) return;

                var entries = Controllers.Controllers.EncounterBuilderPageDataController
                    .LoadEncounterEntries(win.SelectedEncounter.Id);

                var pending = entries.Select(en => new EncounterPendingEntry
                {
                    MonsterId = en.MonsterId,
                    Quantity = en.Quantity
                }).ToList();

                LoadPendingEncounter(pending);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPendingEncounter(List<EncounterPendingEntry> pending)
        {
            var monsterFactory = new MonsterFactory();
            int wave = int.TryParse(txtWave.Text, out int w) ? Math.Max(1, w) : 1;

            foreach (var entry in pending)
            {
                var monster = monsterFactory.GetObject(entry.MonsterId);
                if (monster.Id == Guid.Empty) continue;

                var dexModel = InitiativeManagerPageDataController.LoadDexterity(monster.Id);
                int dexMod = dexModel.Dexterity?.Modifier ?? 0;

                for (int i = 0; i < entry.Quantity; i++)
                {
                    int initiative = _random.Next(1, 21) + dexMod;
                    int tiebreaker = _model.InitiativeEntries.Count(obj => obj.Initiative == initiative);
                    int nbMonsters = _model.InitiativeEntries.Count(obj => obj.MonsterId != Guid.Empty) + 1;

                    _model.InitiativeEntries.Add(new InitiativeEntry
                    {
                        MonsterId = monster.Id,
                        ArmorClass = monster.ArmorClass,
                        HitPoints = monster.HitPoints,
                        MaxHitPoints = monster.HitPoints,
                        DexterityModifier = (short)dexMod,
                        Initiative = initiative,
                        TieBreaker = tiebreaker,
                        Name = $"{nbMonsters} - {monster.Name}",
                        Wave = wave,
                        WaveColor = _selectedWaveColor,
                        ConditionImmunities = monster.ConditionImmunities
                    });
                }
            }

            OrderInitiativeEntries();
            RefreshEntries();
        }

        // ── Initiative navigation / reroll / reset ────────────────────────────

        private void Page_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right) SetTurn(1);
            if (e.Key == Key.Left) SetTurn(-1);
        }

        private void btnPrevious_Click(object sender, RoutedEventArgs e) => SetTurn(-1);
        private void btnNext_Click(object sender, RoutedEventArgs e) => SetTurn(1);

        private void SetTurn(int delta)
        {
            if (!_model.InitiativeEntries.Any()) return;

            InitiativeEntry active;
            if (!_model.InitiativeEntries.Any(e => e.ItsTurn))
            {
                active = _model.InitiativeEntries.First();
                active.ItsTurn = true;
            }
            else
            {
                var current = _model.InitiativeEntries.First(e => e.ItsTurn);
                current.ItsTurn = false;
                int idx = _model.InitiativeEntries.IndexOf(current);
                int next = (idx + delta + _model.InitiativeEntries.Count) % _model.InitiativeEntries.Count;
                active = _model.InitiativeEntries[next];
                active.ItsTurn = true;

                var container = icEntries.ItemContainerGenerator.ContainerFromIndex(next) as FrameworkElement;
                container?.BringIntoView();
            }

            if (active.MonsterId != Guid.Empty)
                ShowStatBlockInPanel(active.MonsterId);
            else
                statBlockPanel.Visibility = Visibility.Collapsed;
        }

        private void btnInitUp_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            entry.Initiative++;
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnInitDown_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            entry.Initiative--;
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void InitiativeEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnRerollOne_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            entry.Initiative = _random.Next(1, 21) + entry.DexterityModifier;
            entry.TieBreaker = _model.InitiativeEntries.Count(o => o.Initiative == entry.Initiative && o != entry);
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnRerollMonsters_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entry in _model.InitiativeEntries.Where(e2 => e2.MonsterId != Guid.Empty))
                entry.Initiative = _random.Next(1, 21) + entry.DexterityModifier;
            RecomputeTiebreakers();
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnRerollPlayers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entry in _model.InitiativeEntries.Where(e2 => e2.MonsterId == Guid.Empty))
                entry.Initiative = _random.Next(1, 21) + entry.DexterityModifier;
            RecomputeTiebreakers();
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnRerollAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var entry in _model.InitiativeEntries)
                entry.Initiative = _random.Next(1, 21) + entry.DexterityModifier;
            RecomputeTiebreakers();
            OrderInitiativeEntries();
            RefreshEntries();
        }

        private void btnResetInitiative_Click(object sender, RoutedEventArgs e)
        {
            _model.InitiativeEntries.Clear();
            txtWave.Text = "1";
            RefreshEntries();
            new InitiativeStateFactory().ClearState();
        }

        private void RecomputeTiebreakers()
        {
            var groups = _model.InitiativeEntries.GroupBy(e => e.Initiative);
            foreach (var g in groups)
            {
                int i = 0;
                foreach (var entry in g)
                    entry.TieBreaker = i++;
            }
        }

        // ── HP ────────────────────────────────────────────────────────────────

        private void btnApplyDamage_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            if (!int.TryParse(entry.ApplyValue, out int val)) return;
            entry.HitPoints = (short)Math.Max(0, entry.HitPoints - val);
            entry.ApplyValue = "0";

            if (entry.HitPoints <= 0 && entry.MonsterId != Guid.Empty)
            {
                _model.InitiativeEntries.Remove(entry);
                RefreshEntries();
            }
        }

        private void btnApplyHeal_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            if (!int.TryParse(entry.ApplyValue, out int val)) return;
            entry.HitPoints = (short)Math.Min(entry.MaxHitPoints > 0 ? entry.MaxHitPoints : short.MaxValue,
                entry.HitPoints + val);
            entry.ApplyValue = "0";
        }

        // ── Conditions ────────────────────────────────────────────────────────

        private void btnOpenConditions_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry == null) return;
            var win = new ConditionsWindow(entry) { Owner = Window.GetWindow(this) };
            win.Show();
        }

        private void btnRemoveCondition_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            var entry = btn.Tag as InitiativeEntry;
            string condition = btn.CommandParameter as string;
            if (entry == null || condition == null) return;

            switch (condition)
            {
                case "Blinded":       entry.IsBlinded = false; break;
                case "Charmed":       entry.IsCharmed = false; break;
                case "Deafened":      entry.IsDeafened = false; break;
                case "Frightened":    entry.IsFrightened = false; break;
                case "Grappled":      entry.IsGrappled = false; break;
                case "Incapacitated": entry.IsIncapacitated = false; break;
                case "Invisible":     entry.IsInvisible = false; break;
                case "Paralyzed":     entry.IsParalyzed = false; break;
                case "Petrified":     entry.IsPetrified = false; break;
                case "Poisoned":      entry.IsPoisoned = false; break;
                case "Prone":         entry.IsProne = false; break;
                case "Restrained":    entry.IsRestrained = false; break;
                case "Stunned":       entry.IsStunned = false; break;
                case "Unconscious":   entry.IsUnconscious = false; break;
            }
        }

        // ── Remove entry ──────────────────────────────────────────────────────

        private void btnRemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry != null)
            {
                _model.InitiativeEntries.Remove(entry);
                RefreshEntries();
            }
        }

        // ── Stat block ────────────────────────────────────────────────────────

        private void btnShowStatBlock_Click(object sender, RoutedEventArgs e)
        {
            var entry = (sender as Button)?.Tag as InitiativeEntry;
            if (entry != null && entry.MonsterId != Guid.Empty)
                ShowStatBlockInPanel(entry.MonsterId);
        }

        // ── Stat block panel ──────────────────────────────────────────────────

        private void ShowStatBlockInPanel(Guid monsterId)
        {
            if (monsterId == Guid.Empty) return;
            try
            {
                SaveStatBlockScroll();
                _currentStatBlockMonsterId = monsterId;

                var monster = _allMonsters.FirstOrDefault(m => m.Id == monsterId);
                tbStatBlockName.Text = monster?.Name ?? "Stat Block";

                string html = StatBlockHelper.GenerateMonsterWithId(monsterId);
                const string darkCss =
                    "<style>" +
                    "html,body{background:#0a0000!important;margin:0!important;padding:8px 6px!important;}" +
                    ".stat-block{width:auto!important;max-width:100%!important;}" +
                    ".stat-block .content-wrap{margin-left:0!important;margin-right:0!important;}" +
                    "</style>";
                html = html.Replace("</head>", darkCss + "</head>");

                _statBlockPendingScrollRestore = true;
                statBlockPanel.Visibility = Visibility.Visible;
                wbStatBlock.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveStatBlockScroll()
        {
            if (_currentStatBlockMonsterId == Guid.Empty) return;
            try
            {
                var scrollY = wbStatBlock.InvokeScript("eval",
                    new object[] { "document.documentElement.scrollTop || document.body.scrollTop" });
                if (scrollY != null && int.TryParse(scrollY.ToString(), out int y))
                    _statBlockScrollPositions[_currentStatBlockMonsterId] = y;
            }
            catch { }
        }

        private void wbStatBlock_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!_statBlockPendingScrollRestore || _currentStatBlockMonsterId == Guid.Empty) return;
            _statBlockPendingScrollRestore = false;
            if (_statBlockScrollPositions.TryGetValue(_currentStatBlockMonsterId, out int y) && y > 0)
            {
                try
                {
                    wbStatBlock.InvokeScript("eval",
                        new object[] { $"window.scrollTo(0, {y})" });
                }
                catch { }
            }
        }


        // ── Wave color ────────────────────────────────────────────────────────

        private void WaveColor_Click(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag == null) return;

            _selectedWaveColor = border.Tag.ToString();

            if (_selectedWaveColorBorder != null)
                _selectedWaveColorBorder.BorderThickness = new Thickness(1);
            border.BorderThickness = new Thickness(2);
            _selectedWaveColorBorder = border;
        }

        // ── Sidebar toggle ────────────────────────────────────────────────────

        private void SidebarTab_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _sidebarExpanded = !_sidebarExpanded;
            tbSidebarArrow.Text = _sidebarExpanded ? "❮" : "❯";

            var anim = new DoubleAnimation
            {
                To = _sidebarExpanded ? 440 : 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            sidebarWrapper.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        // ── Mode toggle (monsters / players) ──────────────────────────────────

        private void tbMonsterMode_Click(object sender, RoutedEventArgs e)
        {
            tbMonsterMode.IsChecked = true;
            tbPlayerMode.IsChecked = false;
            panelMonsters.Visibility = Visibility.Visible;
            panelPlayers.Visibility = Visibility.Collapsed;
        }

        private void tbPlayerMode_Click(object sender, RoutedEventArgs e)
        {
            tbPlayerMode.IsChecked = true;
            tbMonsterMode.IsChecked = false;
            panelPlayers.Visibility = Visibility.Visible;
            panelMonsters.Visibility = Visibility.Collapsed;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void OrderInitiativeEntries()
        {
            _model.InitiativeEntries = new ObservableCollection<InitiativeEntry>(
                _model.InitiativeEntries
                    .OrderByDescending(e => e.Initiative)
                    .ThenBy(e => e.TieBreaker));
        }

        private void RefreshEntries()
        {
            icEntries.ItemsSource = null;
            icEntries.ItemsSource = _model.InitiativeEntries;
        }

        // ── Persistence ───────────────────────────────────────────────────────

        private void LoadSavedInitiative()
        {
            try
            {
                var saved = new InitiativeStateFactory().LoadState();
                if (saved.Count == 0) return;

                foreach (var entry in saved)
                    _model.InitiativeEntries.Add(entry);

                RefreshEntries();

                int maxWave = saved.Max(e => e.Wave);
            }
            catch { }
        }

        private void SaveInitiative()
        {
            try
            {
                new InitiativeStateFactory().SaveState(_model.InitiativeEntries);
            }
            catch { }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try { wbStatBlock.LoadCompleted -= wbStatBlock_LoadCompleted; } catch { }
            try
            {
                var nav = NavigationService;
                if (nav != null) nav.Navigating -= NavigationService_Navigating;
            }
            catch { }
            SaveInitiative();
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
        }
    }
}
