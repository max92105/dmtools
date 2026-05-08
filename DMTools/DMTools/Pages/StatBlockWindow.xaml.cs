using System;
using System.Windows;
using System.Windows.Input;

namespace DMTools.Pages
{
    public partial class StatBlockWindow : Window
    {
        private string _Html;

        public StatBlockWindow(string html)
        {
            _Html = html;
            InitializeComponent();
        }

        private void wbStatBlock_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                wbStatBlock.NavigateToString(_Html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else if (WindowState == WindowState.Normal)
                DragMove();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();
    }
}
