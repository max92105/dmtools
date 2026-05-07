using Data.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DMTools.Pages
{
    public partial class ConditionsWindow : Window
    {
        private readonly InitiativeEntry _entry;
        private readonly Dictionary<string, ToggleButton> _toggleMap;

        public ConditionsWindow(InitiativeEntry entry)
        {
            InitializeComponent();
            _entry = entry;
            tbTitle.Text = $"Conditions — {entry.Name}";
            tbExhLevel.Text = entry.ExhaustionLevel.ToString();

            _toggleMap = new Dictionary<string, ToggleButton>
            {
                { "Blinded",       tbBlinded },
                { "Charmed",       tbCharmed },
                { "Deafened",      tbDeafened },
                { "Frightened",    tbFrightened },
                { "Grappled",      tbGrappled },
                { "Incapacitated", tbIncapacitated },
                { "Invisible",     tbInvisible },
                { "Paralyzed",     tbParalyzed },
                { "Petrified",     tbPetrified },
                { "Poisoned",      tbPoisoned },
                { "Prone",         tbProne },
                { "Restrained",    tbRestrained },
                { "Stunned",       tbStunned },
                { "Unconscious",   tbUnconscious }
            };

            SyncFromEntry();
            MarkImmunities();
        }

        private void SyncFromEntry()
        {
            tbBlinded.IsChecked       = _entry.IsBlinded;
            tbCharmed.IsChecked       = _entry.IsCharmed;
            tbDeafened.IsChecked      = _entry.IsDeafened;
            tbFrightened.IsChecked    = _entry.IsFrightened;
            tbGrappled.IsChecked      = _entry.IsGrappled;
            tbIncapacitated.IsChecked = _entry.IsIncapacitated;
            tbInvisible.IsChecked     = _entry.IsInvisible;
            tbParalyzed.IsChecked     = _entry.IsParalyzed;
            tbPetrified.IsChecked     = _entry.IsPetrified;
            tbPoisoned.IsChecked      = _entry.IsPoisoned;
            tbProne.IsChecked         = _entry.IsProne;
            tbRestrained.IsChecked    = _entry.IsRestrained;
            tbStunned.IsChecked       = _entry.IsStunned;
            tbUnconscious.IsChecked   = _entry.IsUnconscious;
        }

        private void MarkImmunities()
        {
            if (string.IsNullOrWhiteSpace(_entry.ConditionImmunities)) return;

            var immuneList = new List<string>();
            foreach (var kvp in _toggleMap)
            {
                if (IsImmune(kvp.Key))
                {
                    kvp.Value.Style = (Style)Resources["ImmuneToggle"];
                    kvp.Value.ToolTip = $"Immune to {kvp.Key}";
                    immuneList.Add(kvp.Key);
                }
            }

            if (immuneList.Count > 0)
            {
                tbImmuneNote.Text = $"⚠ Immune: {string.Join(", ", immuneList)}";
                tbImmuneNote.Visibility = Visibility.Visible;
            }
        }

        private bool IsImmune(string condition)
        {
            if (string.IsNullOrWhiteSpace(_entry.ConditionImmunities)) return false;
            return _entry.ConditionImmunities.Split(',')
                .Any(c => c.Trim().IndexOf(condition, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void ToggleCond_Click(object sender, RoutedEventArgs e)
        {
            var tb = (ToggleButton)sender;
            string condition = (string)tb.CommandParameter;
            bool newValue = tb.IsChecked == true;

            if (newValue && IsImmune(condition))
            {
                var result = MessageBox.Show(
                    $"{_entry.Name} appears to be immune to {condition}.\nApply anyway?",
                    "Immunity Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                {
                    tb.IsChecked = false;
                    return;
                }
            }

            SetCondition(condition, newValue);
        }

        private void SetCondition(string condition, bool value)
        {
            switch (condition)
            {
                case "Blinded":       _entry.IsBlinded = value; break;
                case "Charmed":       _entry.IsCharmed = value; break;
                case "Deafened":      _entry.IsDeafened = value; break;
                case "Frightened":    _entry.IsFrightened = value; break;
                case "Grappled":      _entry.IsGrappled = value; break;
                case "Incapacitated": _entry.IsIncapacitated = value; break;
                case "Invisible":     _entry.IsInvisible = value; break;
                case "Paralyzed":     _entry.IsParalyzed = value; break;
                case "Petrified":     _entry.IsPetrified = value; break;
                case "Poisoned":      _entry.IsPoisoned = value; break;
                case "Prone":         _entry.IsProne = value; break;
                case "Restrained":    _entry.IsRestrained = value; break;
                case "Stunned":       _entry.IsStunned = value; break;
                case "Unconscious":   _entry.IsUnconscious = value; break;
            }
        }

        private void btnDecExh_Click(object sender, RoutedEventArgs e)
        {
            _entry.ExhaustionLevel = Math.Max(0, _entry.ExhaustionLevel - 1);
            tbExhLevel.Text = _entry.ExhaustionLevel.ToString();
        }

        private void btnIncExh_Click(object sender, RoutedEventArgs e)
        {
            _entry.ExhaustionLevel = Math.Min(6, _entry.ExhaustionLevel + 1);
            tbExhLevel.Text = _entry.ExhaustionLevel.ToString();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) => Close();
    }
}
