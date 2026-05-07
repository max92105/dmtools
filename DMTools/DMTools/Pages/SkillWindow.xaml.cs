using Controllers.Controllers;
using Data.Constants;
using Data.DataModels.SkillWindow;
using Data.VirtualObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace DMTools.Pages
{
    public partial class SkillWindow : Window
    {
        private SkillWindowDataModel _SkillWindowDataModel = new SkillWindowDataModel();
        private readonly Guid _MonsterId;
        private readonly Dictionary<string, int> _abilityModifiers;
        private readonly int _proficiencyBonus;

        public ObservableCollection<DisplaySkill> UpdatedSkillList;

        // Maps each skill type GUID to its governing ability.
        private static readonly Dictionary<Guid, string> _skillAbilityMap = new Dictionary<Guid, string>
        {
            { SkillTypes.AthleticsId,      "STR" },
            { SkillTypes.AcrobaticsId,     "DEX" },
            { SkillTypes.SleightOfHandId,  "DEX" },
            { SkillTypes.StealthId,        "DEX" },
            { SkillTypes.ArcanaId,         "INT" },
            { SkillTypes.HistoryId,        "INT" },
            { SkillTypes.InvestigationId,  "INT" },
            { SkillTypes.NatureId,         "INT" },
            { SkillTypes.ReligionId,       "INT" },
            { SkillTypes.AnimalHandlingId, "WIS" },
            { SkillTypes.InsightId,        "WIS" },
            { SkillTypes.MedicineId,       "WIS" },
            { SkillTypes.PerceptionId,     "WIS" },
            { SkillTypes.SurvivalId,       "WIS" },
            { SkillTypes.DeceptionId,      "CHA" },
            { SkillTypes.IntimidationId,   "CHA" },
            { SkillTypes.PerformanceId,    "CHA" },
            { SkillTypes.PersuasionId,     "CHA" },
        };

        private static int AbilityOrder(string key)
        {
            switch (key)
            {
                case "STR": return 0;
                case "DEX": return 1;
                case "INT": return 2;
                case "WIS": return 3;
                case "CHA": return 4;
                default:    return 5;
            }
        }

        public SkillWindow(Guid monsterId, Dictionary<string, int> abilityModifiers = null, int proficiencyBonus = 2)
        {
            InitializeComponent();
            _MonsterId = monsterId;
            _abilityModifiers = abilityModifiers ?? new Dictionary<string, int>();
            _proficiencyBonus = proficiencyBonus;
        }

        private void Load()
        {
            try
            {
                _SkillWindowDataModel = SkillWindowDataController.Load(_MonsterId);
                DecorateAndSort();
                DataContext = _SkillWindowDataModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DecorateAndSort()
        {
            foreach (var skill in _SkillWindowDataModel.DisplaySkills)
            {
                if (_skillAbilityMap.TryGetValue(skill.SkillTypeId, out string ability))
                {
                    skill.AbilityKey = ability;
                    if (_abilityModifiers.TryGetValue(ability, out int mod))
                    {
                        skill.AbilityModifierValue = mod;
                        skill.DefaultModifier = mod >= 0 ? $"+{mod}" : mod.ToString();
                    }
                }
            }

            var sorted = _SkillWindowDataModel.DisplaySkills
                .OrderBy(s => AbilityOrder(s.AbilityKey))
                .ThenBy(s => s.Name)
                .ToList();
            _SkillWindowDataModel.DisplaySkills = new ObservableCollection<DisplaySkill>(sorted);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { Load(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveQuery = new SaveSkillDataQuery();

                saveQuery.DisplaySkills = _SkillWindowDataModel.DisplaySkills
                    .Where(s => s.InternalState == Data.Constant.InternalStates.New && s.Save != 0)
                    .ToList();

                UpdatedSkillList = new ObservableCollection<DisplaySkill>(
                    _SkillWindowDataModel.DisplaySkills
                        .Where(s => (s.InternalState == Data.Constant.InternalStates.New      && s.Save != 0)
                                 || (s.InternalState == Data.Constant.InternalStates.Modified && s.Save != 0)
                                 ||  s.InternalState == Data.Constant.InternalStates.UnModified));

                foreach (var skill in _SkillWindowDataModel.DisplaySkills
                    .Where(s => s.InternalState == Data.Constant.InternalStates.Modified && s.Save == 0))
                {
                    skill.Delete();
                    saveQuery.DisplaySkills.Add(skill);
                }

                saveQuery.DisplaySkills.AddRange(UpdatedSkillList);
                SkillWindowDataController.SaveSkill(saveQuery);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Handles all "P" proficiency buttons inside the skill DataTemplate via event bubbling.
        private void OnProfButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e.OriginalSource is System.Windows.Controls.Button btn && btn.Tag is DisplaySkill skill)
                skill.Save = (short)(skill.AbilityModifierValue + _proficiencyBonus);
        }
    }
}
