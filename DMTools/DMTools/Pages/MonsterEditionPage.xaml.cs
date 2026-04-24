using Controllers.Controllers;
using Data.DataModels.MonsterEditionPage;
using Data.Objects;
using Data.VirtualObject;
using DMTools.Helpers;
using Notifications.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for MonsterManagerPage.xaml
    /// </summary>
    public partial class MonsterEditionPage : Page
    {
        private MonsterEditionPageDataModel _MonsterEditionPageDataModel;
        private Guid _MonsterId;

        private static readonly DependencyProperty _SelectedSkillProperty =
        DependencyProperty.Register(
        "SelectedSkill", typeof(DisplaySkill),
        typeof(MonsterManagerPage));

        public DisplaySkill SelectedSkill
        {
            get { return (DisplaySkill)GetValue(_SelectedSkillProperty); }
            set { SetValue(_SelectedSkillProperty, value); }
        }

        private static readonly DependencyProperty _SelectedSpecialAbilityProperty =
        DependencyProperty.Register(
        "SelectedSpecialAbility", typeof(SpecialAbility),
        typeof(MonsterManagerPage));

        public SpecialAbility SelectedSpecialAbility
        {
            get { return (SpecialAbility)GetValue(_SelectedSpecialAbilityProperty); }
            set { SetValue(_SelectedSpecialAbilityProperty, value); }
        }

        private static readonly DependencyProperty _SelectedActionProperty =
        DependencyProperty.Register(
        "SelectedAction", typeof(Data.Objects.Action),
        typeof(MonsterManagerPage));

        public Data.Objects.Action SelectedAction
        {
            get { return (Data.Objects.Action)GetValue(_SelectedActionProperty); }
            set { SetValue(_SelectedActionProperty, value); }
        }

        public MonsterEditionPage(Guid monsterId)
        {
            InitializeComponent();

            _MonsterId = monsterId;
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
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        private void btnEditSkills_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Save())
                {
                    SkillWindow skillWindow = new SkillWindow(_MonsterEditionPageDataModel.Monster.Id);
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

        private void btnNewSpecialAbilities_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SpecialAbility specialAbility = new SpecialAbility() { Id = Guid.NewGuid(), MonsterId = _MonsterEditionPageDataModel.Monster.Id };
                SpecialAbilitiesWindow specialAbilitiesWindow = new SpecialAbilitiesWindow(specialAbility);

                specialAbilitiesWindow.ShowDialog();

                if (specialAbilitiesWindow.DialogResult == true)
                {
                    _MonsterEditionPageDataModel.SpecialAbilities.Add(specialAbility);
                }
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
                    SpecialAbilitiesWindow specialAbilitiesWindow = new SpecialAbilitiesWindow(SelectedSpecialAbility);
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

        private void btnNewActions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Data.Objects.Action action = new Data.Objects.Action() { Id = Guid.NewGuid(), MonsterId = _MonsterEditionPageDataModel.Monster.Id };
                ActionWindow specialAbilitiesWindow = new ActionWindow(action);

                specialAbilitiesWindow.ShowDialog();
                if (specialAbilitiesWindow.DialogResult == true)
                {
                    _MonsterEditionPageDataModel.Actions.Add(action);
                }
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
                    ActionWindow actionWindow = new ActionWindow(SelectedAction);
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
                NotificationHelper.NewNotification("Monster Copy", "Error !", NotificationType.Error);
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

                foreach (SpecialAbility specialAbility in _MonsterEditionPageDataModel.SpecialAbilities)
                    specialAbility.Delete();

                foreach (Data.Objects.Action action in _MonsterEditionPageDataModel.Actions)
                    action.Delete();

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
                if(Save())
                    StatBlockHelper.ShowStatBlock(_MonsterEditionPageDataModel.Monster);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}