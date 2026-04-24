using Data.Objects;
using Data.VirtualObject;
using System.Collections.ObjectModel;

namespace Data.DataModels.SkillWindow
{
    public class SkillWindowDataModel : DataModel
    {
    
        private ObservableCollection<DisplaySkill> _DisplaySkills;
        private ObservableCollection<SkillType> _SkillTypes;

        public ObservableCollection<DisplaySkill> DisplaySkills
        {
            get
            {
                return _DisplaySkills;
            }
            set
            {
                if (_DisplaySkills != value)
                {
                    _DisplaySkills = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<SkillType> SkillTypes
        {
            get
            {
                return _SkillTypes;
            }
            set
            {
                if (_SkillTypes != value)
                {
                    _SkillTypes = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
