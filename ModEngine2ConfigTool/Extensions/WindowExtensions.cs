using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;

namespace ModEngine2ConfigTool.Extensions
{
    public static class WindowExtensions
    {
        // Rectangle (used by MONITORINFOEX)
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Monitor information (used by GetMonitorInfo())
        [StructLayout(LayoutKind.Sequential)]
        public class MonitorInfoEx
        {
            public int cbSize;
            public Rect rcMonitor; // Total area
            public Rect rcWork; // Working area
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
            public char[] szDevice;
        }

        // To get a handle to the specified monitor
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        // To get the working area of the specified monitor
        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MonitorInfoEx monitorInfo);

        private static MonitorInfoEx GetMonitorInfo(Window window, IntPtr monitorPtr)
        {
            var monitorInfo = new MonitorInfoEx();

            monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
            GetMonitorInfo(new HandleRef(window, monitorPtr), monitorInfo);

            return monitorInfo;
        }

        public static void Maximize(this Window window)
        {
            window.ResizeMode = ResizeMode.NoResize;

            // Get handle for nearest monitor to this window
            var wih = new WindowInteropHelper(window);

            // Nearest monitor to window
            const int MONITOR_DEFAULTTONEAREST = 2;
            var hMonitor = MonitorFromWindow(wih.Handle, MONITOR_DEFAULTTONEAREST);

            // Get monitor info
            var monitorInfo = GetMonitorInfo(window, hMonitor);

            // Create working area dimensions, converted to DPI-independent values
            var source = HwndSource.FromHwnd(wih.Handle);

            if (source?.CompositionTarget == null)
            {
                return;
            }

            var matrix = source.CompositionTarget.TransformFromDevice;
            var workingArea = monitorInfo.rcWork;

            var dpiIndependentSize =
                matrix.Transform(
                    new Point(workingArea.Right - workingArea.Left,
                              workingArea.Bottom - workingArea.Top));

            // Maximize the window to the device-independent working area ie
            // the area without the taskbar.
            window.Top = workingArea.Top;
            window.Left = workingArea.Left;

            window.MaxWidth = dpiIndependentSize.X + 10;
            window.MaxHeight = dpiIndependentSize.Y + 10;

            window.WindowState = WindowState.Maximized;
        }
    }
}
