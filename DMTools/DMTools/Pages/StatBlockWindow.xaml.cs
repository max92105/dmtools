using CefSharp;
using DMTools.Helpers;
using System;
using System.Threading;
using System.Windows;

namespace DMTools.Pages
{
    /// <summary>
    /// Interaction logic for StatBlockWindow.xaml
    /// </summary>
    public partial class StatBlockWindow : Window
    {
        private String _Html;

        public StatBlockWindow(String html)
        {
            _Html = html;

            InitializeComponent();
        }

        private void wbStatBlock_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                wbStatBlock.LoadHtml(_Html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
