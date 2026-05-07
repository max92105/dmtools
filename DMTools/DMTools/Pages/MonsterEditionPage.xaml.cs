using Controllers.Controllers;
using Data.Constants;
using Data.DataModels.MonsterEditionPage;
using Data.Objects;
using Data.VirtualObject;
using DMTools.Helpers;
using Notifications.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    public partial class MonsterEditionPage : Page
    {
        private MonsterEditionPageDataModel _MonsterEditionPageDataModel;
        private Guid _MonsterId;

        private static readonly DependencyProperty _SelectedSkillProperty =
            DependencyProperty.Register("SelectedSkill", typeof(DisplaySkill), typeof(MonsterEditionPage));

        public DisplaySkill SelectedSkill
        {
            get { return (DisplaySkill)GetValue(_SelectedSkillProperty); }
            set { SetValue(_SelectedSkillProperty, value); }
        }

        private static readonly DependencyProperty _SelectedSpecialAbilityProperty =
            DependencyProperty.Register("SelectedSpecialAbility", typeof(SpecialAbility), typeof(MonsterEditionPage));

        public SpecialAbility SelectedSpecialAbility
        {
            get { return (SpecialAbility)GetValue(_SelectedSpecialAbilityProperty); }
            set { SetValue(_SelectedSpecialAbilityProperty, value); }
        }

        private static readonly DependencyProperty _SelectedActionProperty =
            DependencyProperty.Register("SelectedAction", typeof(Data.Objects.Action), typeof(MonsterEditionPage));

        public Data.Objects.Action SelectedAction
        {
            get { return (Data.Objects.Action)GetValue(_SelectedActionProperty); }
            set { SetValue(_SelectedActionProperty, value); }
        }

        public MonsterEditionPage(Guid monsterId)
        {
            InitializeComponent();
            _MonsterId = monsterId;

            // Populate combo boxes
            cboSize.ItemsSource = MonsterSizes.All;
            cboType.ItemsSource = MonsterTypes.All;
            cboSubtype.ItemsSource = MonsterSubtypes.All;
            cboAlignment.ItemsSource = Alignments.All;
        }

        private void Load()
        {
            try
            {
                if (_MonsterId != Guid.Empty)
                    _MonsterEditionPageDataModel = MonsterEditionPageDataController.LoadMonster(_MonsterId);
                else
                    _MonsterEditionPageDataModel = MonsterEditionPageDataController.LoadNewMonster();

                DataContext = _MonsterEditionPageDataModel;

                // Populate condition immunities checkboxes
                LoadConditionImmunities();

                // Populate languages checkboxes
                LoadLanguages();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConditionImmunities()
        {
            lbConditionImmunities.ItemsSource = Conditions.All;

            // Parse existing condition immunities and select them
            if (!String.IsNullOrWhiteSpace(_MonsterEditionPageDataModel.Monster.ConditionImmunities))
            {
                var existing = _MonsterEditionPageDataModel.Monster.ConditionImmunities
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToList();

                lbConditionImmunities.SelectionChanged -= lbConditionImmunities_SelectionChanged;
                foreach (var condition in existing)
                {
                    var match = Conditions.All.FirstOrDefault(c => c.Equals(condition, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        lbConditionImmunities.SelectedItems.Add(match);
                }
                lbConditionImmunities.SelectionChanged += lbConditionImmunities_SelectionChanged;
            }
        }

        private void LoadLanguages()
        {
            lbLanguages.ItemsSource = Data.Constants.Languages.All;

            lbLanguages.SelectionChanged -= lbLanguages_SelectionChanged;
            foreach (var lang in _MonsterEditionPageDataModel.Languages)
            {
                var match = Data.Constants.Languages.All.FirstOrDefault(l => l.Equals(lang, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    lbLanguages.SelectedItems.Add(match);
            }
            lbLanguages.SelectionChanged += lbLanguages_SelectionChanged;
        }

        private void lbConditionImmunities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = lbConditionImmunities.SelectedItems.Cast<string>().ToList();
            _MonsterEditionPageDataModel.Monster.ConditionImmunities = String.Join(", ", selected);
        }

        private void lbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _MonsterEditionPageDataModel.Languages.Clear();
            foreach (string lang in lbLanguages.SelectedItems)
            {
                _MonsterEditionPageDataModel.Languages.Add(lang);
            }
        }

        #region Speed
        private void btnAddSpeed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new SpeedSenseEditorWindow("Speed", SpeedTypes.All);
                if (window.ShowDialog() == true)
                {
                    var speed = new Speed
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = _MonsterEditionPageDataModel.Monster.Id,
                        SpeedType = window.SelectedType,
                        Value = window.SelectedValue
                    };
                    _MonsterEditionPageDataModel.Speeds.Add(speed);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveSpeed_Click(object sender, RoutedEventArgs e)
        {
            if (lbSpeeds.SelectedItem is Speed selected)
            {
                if (selected.InternalState != Data.Constant.InternalStates.New)
                    MonsterEditionPageDataController.DeleteSpeed(selected);
                _MonsterEditionPageDataModel.Speeds.Remove(selected);
            }
        }
        #endregion

        #region Sense
        private void btnAddSense_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new SpeedSenseEditorWindow("Sense", SenseTypes.All);
                if (window.ShowDialog() == true)
                {
                    var sense = new Sense
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = _MonsterEditionPageDataModel.Monster.Id,
                        SenseType = window.SelectedType,
                        Value = window.SelectedValue
                    };
                    _MonsterEditionPageDataModel.Senses.Add(sense);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveSense_Click(object sender, RoutedEventArgs e)
        {
            if (lbSenses.SelectedItem is Sense selected)
            {
                if (selected.InternalState != Data.Constant.InternalStates.New)
                    MonsterEditionPageDataController.DeleteSense(selected);
                _MonsterEditionPageDataModel.Senses.Remove(selected);
            }
        }
        #endregion

        #region Armor Class
        private void btnAddAc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new ArmorClassEditorWindow();
                if (window.ShowDialog() == true)
                {
                    var entry = new ArmorClassEntry
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = _MonsterEditionPageDataModel.Monster.Id,
                        Label = window.AcLabel,
                        Value = window.AcValue
                    };
                    _MonsterEditionPageDataModel.ArmorClassEntries.Add(entry);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveAc_Click(object sender, RoutedEventArgs e)
        {
            if (lbArmorClass.SelectedItem is ArmorClassEntry selected)
            {
                if (selected.InternalState != Data.Constant.InternalStates.New)
                    MonsterEditionPageDataController.DeleteArmorClassEntry(selected);
                _MonsterEditionPageDataModel.ArmorClassEntries.Remove(selected);
            }
        }
        #endregion

        #region Damage Modifiers
        private void btnAddDamageModifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var window = new DamageModifierEditorWindow(_MonsterEditionPageDataModel.Monster.ChallengeRating);
                if (window.ShowDialog() == true)
                {
                    var modifier = new DamageModifier
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = _MonsterEditionPageDataModel.Monster.Id,
                        DamageType = window.SelectedDamageType,
                        ModifierType = window.SelectedModifierType,
                        DiceCount = window.DiceCount,
                        DiceSize = window.DiceSize
                    };
                    _MonsterEditionPageDataModel.DamageModifiers.Add(modifier);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveDamageModifier_Click(object sender, RoutedEventArgs e)
        {
            if (lbDamageModifiers.SelectedItem is DamageModifier selected)
            {
                if (selected.InternalState != Data.Constant.InternalStates.New)
                    MonsterEditionPageDataController.DeleteDamageModifier(selected);
                _MonsterEditionPageDataModel.DamageModifiers.Remove(selected);
            }
        }
        #endregion

        #region Skills
        private void btnEditSkills_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Save())
                {
                    SkillWindow skillWindow = new SkillWindow(_MonsterEditionPageDataModel.Monster.Id, GetAbilityModifiers(), GetEffectiveProficiencyBonus());
                    skillWindow.ShowDialog();

                    if (skillWindow.DialogResult != null && (Boolean)skillWindow.DialogResult)
                        _MonsterEditionPageDataModel.DisplaySkills = skillWindow.UpdatedSkillList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Special Abilities
        private void btnNewSpecialAbilities_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpecialAbility specialAbility = new SpecialAbility() { Id = Guid.NewGuid(), MonsterId = _MonsterEditionPageDataModel.Monster.Id };
                SpecialAbilitiesWindow specialAbilitiesWindow = new SpecialAbilitiesWindow(
                    specialAbility,
                    _MonsterEditionPageDataModel.Monster.Name,
                    _MonsterEditionPageDataModel.Monster.ChallengeRating,
                    GetAbilityModifiers());
                specialAbilitiesWindow.ShowDialog();

                if (specialAbilitiesWindow.DialogResult == true)
                    _MonsterEditionPageDataModel.SpecialAbilities.Add(specialAbility);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditSpecialAbilities_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSpecialAbility != null)
                {
                    SpecialAbilitiesWindow specialAbilitiesWindow = new SpecialAbilitiesWindow(
                        SelectedSpecialAbility,
                        _MonsterEditionPageDataModel.Monster.Name,
                        _MonsterEditionPageDataModel.Monster.ChallengeRating,
                        GetAbilityModifiers());
                    specialAbilitiesWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteSpecialAbilities_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedSpecialAbility != null)
                {
                    if (SelectedSpecialAbility.InternalState != Data.Constant.InternalStates.New)
                    {
                        SelectedSpecialAbility.Delete();
                        MonsterEditionPageDataController.DeleteSpecialAbility(SelectedSpecialAbility);
                    }
                    _MonsterEditionPageDataModel.SpecialAbilities.Remove(SelectedSpecialAbility);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private static int AbilityModifier(short score) => (int)Math.Floor((score - 10) / 2.0);

        private Dictionary<string, int> GetAbilityModifiers()
        {
            return new Dictionary<string, int>
            {
                ["STR"] = AbilityModifier(_MonsterEditionPageDataModel.Strength.Score),
                ["DEX"] = AbilityModifier(_MonsterEditionPageDataModel.Dexterity.Score),
                ["CON"] = AbilityModifier(_MonsterEditionPageDataModel.Constitution.Score),
                ["INT"] = AbilityModifier(_MonsterEditionPageDataModel.Intelligence.Score),
                ["WIS"] = AbilityModifier(_MonsterEditionPageDataModel.Wisdom.Score),
                ["CHA"] = AbilityModifier(_MonsterEditionPageDataModel.Charisma.Score),
            };
        }

        private int GetEffectiveProficiencyBonus()
        {
            short stored = _MonsterEditionPageDataModel.Monster.ProficiencyBonus;
            if (stored != 0) return stored;
            decimal cr = _MonsterEditionPageDataModel.Monster.ChallengeRating;
            if      (cr <= 4)  return 2;
            if      (cr <= 8)  return 3;
            if      (cr <= 12) return 4;
            if      (cr <= 16) return 5;
            if      (cr <= 20) return 6;
            if      (cr <= 24) return 7;
            if      (cr <= 28) return 8;
            return 9;
        }

        #region Proficiency Save Buttons
        private void btnProfStr_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Strength.Save    = (short)(AbilityModifier(_MonsterEditionPageDataModel.Strength.Score)    + GetEffectiveProficiencyBonus());
        private void btnProfDex_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Dexterity.Save   = (short)(AbilityModifier(_MonsterEditionPageDataModel.Dexterity.Score)   + GetEffectiveProficiencyBonus());
        private void btnProfCon_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Constitution.Save = (short)(AbilityModifier(_MonsterEditionPageDataModel.Constitution.Score) + GetEffectiveProficiencyBonus());
        private void btnProfInt_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Intelligence.Save = (short)(AbilityModifier(_MonsterEditionPageDataModel.Intelligence.Score) + GetEffectiveProficiencyBonus());
        private void btnProfWis_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Wisdom.Save      = (short)(AbilityModifier(_MonsterEditionPageDataModel.Wisdom.Score)      + GetEffectiveProficiencyBonus());
        private void btnProfCha_Click(object sender, RoutedEventArgs e)
            => _MonsterEditionPageDataModel.Charisma.Save    = (short)(AbilityModifier(_MonsterEditionPageDataModel.Charisma.Score)    + GetEffectiveProficiencyBonus());
        #endregion

        #region Actions
        private void btnNewActions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.Objects.Action action = new Data.Objects.Action() { Id = Guid.NewGuid(), MonsterId = _MonsterEditionPageDataModel.Monster.Id };
                ActionWindow actionWindow = new ActionWindow(action, _MonsterEditionPageDataModel.Monster.Name, GetEffectiveProficiencyBonus(), GetAbilityModifiers());
                actionWindow.ShowDialog();

                if (actionWindow.DialogResult == true)
                    _MonsterEditionPageDataModel.Actions.Add(action);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditActions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedAction != null)
                {
                    ActionWindow actionWindow = new ActionWindow(SelectedAction, _MonsterEditionPageDataModel.Monster.Name, GetEffectiveProficiencyBonus(), GetAbilityModifiers());
                    actionWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteActions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedAction != null)
                {
                    if (SelectedAction.InternalState != Data.Constant.InternalStates.New)
                    {
                        SelectedAction.Delete();
                        MonsterEditionPageDataController.DeleteAction(SelectedAction);
                    }
                    _MonsterEditionPageDataModel.Actions.Remove(SelectedAction);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Save / Copy / Delete
        private Boolean Save()
        {
            try
            {
                MonsterEditionPageDataController.Save(_MonsterEditionPageDataModel);
                NotificationHelper.NewNotification("Monster Save", "Succesfull !", NotificationType.Success);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                NotificationHelper.NewNotification("Monster Save", "Error !", NotificationType.Error);
                return false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MonsterEditionPageDataController.Copy(_MonsterEditionPageDataModel);
                NotificationHelper.NewNotification("Monster Copy", "Succesfull !", NotificationType.Success);
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
                _MonsterEditionPageDataModel.Monster.Delete();
                _MonsterEditionPageDataModel.Strength.Delete();
                _MonsterEditionPageDataModel.Dexterity.Delete();
                _MonsterEditionPageDataModel.Constitution.Delete();
                _MonsterEditionPageDataModel.Intelligence.Delete();
                _MonsterEditionPageDataModel.Wisdom.Delete();
                _MonsterEditionPageDataModel.Charisma.Delete();

                foreach (Skill skill in _MonsterEditionPageDataModel.DisplaySkills)
                    skill.Delete();
                foreach (SpecialAbility sa in _MonsterEditionPageDataModel.SpecialAbilities)
                    sa.Delete();
                foreach (Data.Objects.Action action in _MonsterEditionPageDataModel.Actions)
                    action.Delete();
                foreach (Speed speed in _MonsterEditionPageDataModel.Speeds)
                    speed.Delete();
                foreach (Sense sense in _MonsterEditionPageDataModel.Senses)
                    sense.Delete();
                foreach (DamageModifier dm in _MonsterEditionPageDataModel.DamageModifiers)
                    dm.Delete();
                foreach (ArmorClassEntry ace in _MonsterEditionPageDataModel.ArmorClassEntries)
                    ace.Delete();

                MonsterEditionPageDataController.Save(_MonsterEditionPageDataModel);
                NavigationService.Navigate(new MonsterManagerPage());
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
                if (Save())
                    StatBlockHelper.ShowStatBlock(_MonsterEditionPageDataModel.Monster);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}