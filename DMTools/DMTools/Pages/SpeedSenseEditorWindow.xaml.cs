using System.Windows;

namespace DMTools.Pages
{
    public partial class SpeedSenseEditorWindow : Window
    {
        public string SelectedType => cboType.SelectedItem as string;

        public short SelectedValue
        {
            get
            {
                if (short.TryParse(txtValue.Text, out short v))
                    return v;
                return 0;
            }
        }

        public SpeedSenseEditorWindow(string title, string[] types)
        {
            InitializeComponent();
            Title = title;
            lblHeader.Text = title;
            cboType.ItemsSource = types;
            if (types.Length > 0)
                cboType.SelectedIndex = 0;
        }

        public SpeedSenseEditorWindow(string title, string[] types, string existingType, short existingValue)
            : this(title, types)
        {
            cboType.SelectedItem = existingType;
            txtValue.Text = existingValue.ToString();
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
