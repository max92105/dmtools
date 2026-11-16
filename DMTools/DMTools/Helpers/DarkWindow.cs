using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace DMTools.Helpers
{
    /// <summary>
    /// Base class for the pop-up windows: replaces the white Windows title bar with the dark DM Tools one
    /// (same look as the main window), square corners, and keeps moving / resizing / closing working.
    /// Usage in XAML: &lt;h:DarkWindow x:Class="..." xmlns:h="clr-namespace:DMTools.Helpers" ...&gt;
    /// </summary>
    public class DarkWindow : Window
    {
        public DarkWindow()
        {
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, (s, e) => SystemCommands.CloseWindow(this)));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, (s, e) => SystemCommands.MinimizeWindow(this)));
        }

        /// <summary>Runs after the XAML (ResizeMode, Title...) has been applied, before the window is shown.</summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            WindowStyle = WindowStyle.None;
            WindowChrome.SetWindowChrome(this, new WindowChrome
            {
                CaptionHeight = 32,                                   // the title bar row of the template
                ResizeBorderThickness = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip
                    ? new Thickness(6) : new Thickness(0),
                GlassFrameThickness = new Thickness(0),
                CornerRadius = new CornerRadius(0),
                UseAeroCaptionButtons = false,
            });

            if (Application.Current?.TryFindResource("DarkWindowTemplate") is System.Windows.Controls.ControlTemplate template)
                Template = template;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            // Borderless + resizable: maximize to the work area (not over the taskbar).
            WindowScreenHelper.EnableWorkAreaMaximize(this);
        }
    }
}
