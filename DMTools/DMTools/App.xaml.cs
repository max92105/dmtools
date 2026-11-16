using DMTools.Helpers;
using System.Windows;

namespace DMTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Keep every window inside the visible area of its screen, whatever the resolution / scaling.
            WindowScreenHelper.RegisterGlobalHandlers();
            base.OnStartup(e);
        }
    }
}
