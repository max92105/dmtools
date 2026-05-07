using System.Windows;

namespace DMTools.Pages
{
    public partial class ArmorClassEditorWindow : Window
    {
        public string AcLabel => txtLabel.Text;

        public short AcValue
        {
            get
            {
                if (short.TryParse(txtValue.Text, out short v))
                    return v;
                return 10;
            }
        }

        public ArmorClassEditorWindow()
        {
            InitializeComponent();
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
