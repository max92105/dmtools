using Controllers.Controllers;
using Data.DataModels.SkillWindow;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for SkillWindow.xaml
    /// </summary>
    public partial class SkillWindow : Window
    {
        private SkillWindowDataModel _SkillWindowDataModel = new SkillWindowDataModel();
        private Guid _MonsterId;

        public ObservableCollection<DisplaySkill> UpdatedSkillList;

        public SkillWindow(Guid monsterId)
        {
            InitializeComponent();

            try
            {
                _MonsterId = monsterId;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Load()
        {
            try
            {
                _SkillWindowDataModel = SkillWindowDataController.Load(_MonsterId);             

                DataContext = _SkillWindowDataModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Load();
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
                SaveSkillDataQuery saveSkillDataQuery = new SaveSkillDataQuery();

                saveSkillDataQuery.DisplaySkills = _SkillWindowDataModel.DisplaySkills.Where(obj => obj.InternalState == Data.Constant.InternalStates.New && obj.Save != 0).ToList();

                UpdatedSkillList = new ObservableCollection<DisplaySkill>
                    (_SkillWindowDataModel.DisplaySkills.Where(obj => (obj.InternalState == Data.Constant.InternalStates.New && obj.Save != 0) 
                    || (obj.InternalState == Data.Constant.InternalStates.Modified && obj.Save != 0)
                    || obj.InternalState == Data.Constant.InternalStates.UnModified));

                foreach (DisplaySkill displaySkill in _SkillWindowDataModel.DisplaySkills.Where(obj => obj.InternalState == Data.Constant.InternalStates.Modified && obj.Save == 0))
                {
                    displaySkill.Delete();
                    saveSkillDataQuery.DisplaySkills.Add(displaySkill);
                }

                saveSkillDataQuery.DisplaySkills.AddRange(UpdatedSkillList);

                SkillWindowDataController.SaveSkill(saveSkillDataQuery);

                DialogResult = true;

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
