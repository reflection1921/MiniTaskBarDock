using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace MiniTaskBarDock
{
    internal class ScreenUtils
    {
        public static List<MONITORINFO> GetAllMonitors()
        {
            var monitors = new List<MONITORINFO>();

            unsafe BOOL MonitorEnumProc(HMONITOR hMonitor, HDC hdcMonitor, RECT* lprcMonitor, LPARAM dwData)
            {
                var monitorInfo = new MONITORINFO { cbSize = (uint)sizeof(MONITORINFO) };
                if (PInvoke.GetMonitorInfo(hMonitor, ref monitorInfo))
                {
                    monitors.Add(monitorInfo);
                }

                return true;
            }

            unsafe
            {
                LPARAM dwData = new LPARAM();
                HDC hdc = new HDC(IntPtr.Zero);
                RECT? rc = null;
                PInvoke.EnumDisplayMonitors(hdc, rc, MonitorEnumProc, dwData);
            }

            return monitors;
        }

        
    }
}
