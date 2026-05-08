using Data.Constants;
using Data.Objects;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DMTools.Pages
{
    public partial class DamageModifierEditorWindow : Window
    {
        private static readonly string[] ModifierTypes = { "Resistance", "Vulnerability", "Immunity" };

        public string SelectedDamageType => cboDamageType.SelectedItem as string;
        public string SelectedModifierType => cboModifierType.SelectedItem as string;

        public short DiceCount
        {
            get
            {
                if (short.TryParse(txtDiceCount.Text, out short v))
                    return v;
                return 1;
            }
        }

        public short DiceSize
        {
            get
            {
                var s = cboDiceSize.SelectedItem as string;
                if (s != null && short.TryParse(s.Substring(1), out short v))
                    return v;
                return 6;
            }
        }

        public DamageModifierEditorWindow(decimal challengeRating)
        {
            InitializeComponent();

            cboDamageType.ItemsSource = DamageTypes.All;
            cboDamageType.SelectedIndex = 0;

            cboModifierType.ItemsSource = ModifierTypes;
            cboModifierType.SelectedIndex = 0;

            cboDiceSize.ItemsSource = new[] { "d4", "d6", "d8", "d10", "d12" };

            short defaultCount, defaultSize;
            DamageModifierDefaults.GetDefaultDice(challengeRating, out defaultCount, out defaultSize);
            txtDiceCount.Text = defaultCount.ToString();
            cboDiceSize.SelectedItem = "d" + defaultSize;
        }

        public DamageModifierEditorWindow(DamageModifier existing, decimal challengeRating)
            : this(challengeRating)
        {
            cboDamageType.SelectedItem   = existing.DamageType;
            cboModifierType.SelectedItem = existing.ModifierType;
            txtDiceCount.Text            = existing.DiceCount.ToString();
            cboDiceSize.SelectedItem     = "d" + existing.DiceSize;
        }

        private void cboModifierType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isImmunity = cboModifierType.SelectedItem as string == "Immunity";
            pnlDice.Visibility = isImmunity ? Visibility.Collapsed : Visibility.Visible;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
