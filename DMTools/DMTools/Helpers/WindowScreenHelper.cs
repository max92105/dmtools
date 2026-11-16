using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DMTools.Helpers
{
    /// <summary>
    /// Keeps every window of the application inside the visible work area of the monitor
    /// it is displayed on (no part hidden under the taskbar or outside the screen), and
    /// makes borderless windows maximize to the work area instead of covering the taskbar.
    /// </summary>
    public static class WindowScreenHelper
    {
        private static bool _registered;

        /// <summary>
        /// Registers a global handler so every window is fitted to its screen when it loads.
        /// Call once at application startup.
        /// </summary>
        public static void RegisterGlobalHandlers()
        {
            if (_registered) return;
            _registered = true;

            EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent,
                new RoutedEventHandler(OnWindowLoaded));
        }

        /// <summary>
        /// Makes a borderless (WindowStyle=None) window maximize to the monitor work area
        /// rather than covering the whole screen and the taskbar.
        /// Call from OnSourceInitialized.
        /// </summary>
        public static void EnableWorkAreaMaximize(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;
            HwndSource.FromHwnd(hwnd)?.AddHook(MaximizeHook);
            ApplySquareCorners(window);
        }

        /// <summary>
        /// Windows 11 rounds window corners and draws a thin border: ask for square corners and no system border
        /// (the windows draw their own). Ignored on Windows 10.
        /// </summary>
        public static void ApplySquareCorners(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;
            try
            {
                int doNotRound = DWMWCP_DONOTROUND;
                DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref doNotRound, sizeof(int));
                int noBorder = DWMWA_COLOR_NONE;
                DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, ref noBorder, sizeof(int));
            }
            catch
            {
                // dwmapi missing / attribute unsupported: keep the default look.
            }
        }

        // ── Fit on load ──────────────────────────────────────────────────────

        private static void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var window = sender as Window;
            if (window == null || !ReferenceEquals(e.OriginalSource, window)) return;

            try
            {
                ApplySquareCorners(window);
                FitToScreen(window);
            }
            catch
            {
                // Never let a layout helper crash the app.
            }
        }

        private static void FitToScreen(Window window)
        {
            var mainWindow = Application.Current?.MainWindow;
            bool isMain = ReferenceEquals(window, mainWindow);

            // Reference window: the one we want to appear over (owner, else the main app window).
            Window reference = window.Owner;
            if (reference == null && !isMain) reference = mainWindow;
            if (reference != null && (!reference.IsLoaded || reference.WindowState == WindowState.Minimized))
                reference = null;

            bool center = !isMain && window.WindowStartupLocation != WindowStartupLocation.Manual;

            // Work area of the monitor the window should live on.
            Rect? workArea = center && reference != null ? GetWorkArea(reference) : GetWorkArea(window);
            if (workArea == null) return;
            Rect work = workArea.Value;

            if (isMain)
            {
                // Only make sure the restored size fits the screen; the maximized state is
                // handled by EnableWorkAreaMaximize.
                if (!double.IsNaN(window.Width) && window.Width > work.Width) window.Width = work.Width;
                if (!double.IsNaN(window.Height) && window.Height > work.Height) window.Height = work.Height;
                return;
            }

            if (window.WindowState != WindowState.Normal) return;

            // ── Size: never larger than the work area ──
            if (window.MinWidth > work.Width) window.MinWidth = work.Width;
            if (window.MinHeight > work.Height) window.MinHeight = work.Height;
            window.MaxWidth = Math.Min(window.MaxWidth, work.Width);
            window.MaxHeight = Math.Min(window.MaxHeight, work.Height);

            double width = Math.Min(window.ActualWidth, work.Width);
            double height = Math.Min(window.ActualHeight, work.Height);

            bool sizeToWidth = window.SizeToContent == SizeToContent.Width || window.SizeToContent == SizeToContent.WidthAndHeight;
            bool sizeToHeight = window.SizeToContent == SizeToContent.Height || window.SizeToContent == SizeToContent.WidthAndHeight;
            if (!sizeToWidth && window.ActualWidth > work.Width) window.Width = width;
            if (!sizeToHeight && window.ActualHeight > work.Height) window.Height = height;

            // ── Position: centered over the reference, then kept inside the work area ──
            double left = window.Left;
            double top = window.Top;

            if (center)
            {
                Rect target = reference != null ? GetVisibleBounds(reference, work) : work;
                left = target.Left + (target.Width - width) / 2;
                top = target.Top + (target.Height - height) / 2;
            }

            if (double.IsNaN(left)) left = work.Left + (work.Width - width) / 2;
            if (double.IsNaN(top)) top = work.Top + (work.Height - height) / 2;

            left = Math.Max(work.Left, Math.Min(left, work.Right - width));
            top = Math.Max(work.Top, Math.Min(top, work.Bottom - height));

            window.Left = left;
            window.Top = top;
        }

        /// <summary>Bounds of a window on screen (its work area when maximized).</summary>
        private static Rect GetVisibleBounds(Window window, Rect workArea)
        {
            if (window.WindowState == WindowState.Maximized) return workArea;
            return new Rect(window.Left, window.Top, window.ActualWidth, window.ActualHeight);
        }

        /// <summary>Work area (screen minus taskbar) of the monitor holding the window, in DIPs.</summary>
        private static Rect? GetWorkArea(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return null;

            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor == IntPtr.Zero) return null;

            var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
            if (!GetMonitorInfo(monitor, ref info)) return null;

            Matrix toDip = Matrix.Identity;
            var source = PresentationSource.FromVisual(window);
            if (source?.CompositionTarget != null)
                toDip = source.CompositionTarget.TransformFromDevice;

            Point topLeft = toDip.Transform(new Point(info.rcWork.Left, info.rcWork.Top));
            Point bottomRight = toDip.Transform(new Point(info.rcWork.Right, info.rcWork.Bottom));
            return new Rect(topLeft, bottomRight);
        }

        // ── Maximize to work area ────────────────────────────────────────────

        private static IntPtr MaximizeHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg != WM_GETMINMAXINFO) return IntPtr.Zero;

            IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor == IntPtr.Zero) return IntPtr.Zero;

            var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
            if (!GetMonitorInfo(monitor, ref info)) return IntPtr.Zero;

            var mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
            mmi.ptMaxPosition.X = info.rcWork.Left - info.rcMonitor.Left;
            mmi.ptMaxPosition.Y = info.rcWork.Top - info.rcMonitor.Top;
            mmi.ptMaxSize.X = info.rcWork.Right - info.rcWork.Left;
            mmi.ptMaxSize.Y = info.rcWork.Bottom - info.rcWork.Top;
            Marshal.StructureToPtr(mmi, lParam, true);

            // Not handled: let WPF still apply MinWidth/MinHeight/MaxWidth/MaxHeight.
            return IntPtr.Zero;
        }

        // ── Win32 interop ────────────────────────────────────────────────────

        private const int WM_GETMINMAXINFO = 0x0024;
        private const int MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWCP_DONOTROUND = 1;
        private const int DWMWA_COLOR_NONE = unchecked((int)0xFFFFFFFE);

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }
    }
}
