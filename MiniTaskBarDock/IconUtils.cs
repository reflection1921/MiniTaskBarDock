using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.Win32;
using Windows.Win32.UI.Shell;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Controls;
using Windows.Win32.Foundation;
using System.IO;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Com;
using System.Text;
using System.Reflection.Metadata;
using System.Reflection;
using Windows.Win32.UI.Shell.Common;


namespace MiniTaskBarDock
{
    internal class IconUtils
    {

        public static ImageSource? ExtractIconImage(string filePath)
        {
            SHFILEINFOW fileInfo = default;
            SHGFI_FLAGS flags = SHGFI_FLAGS.SHGFI_ICON | SHGFI_FLAGS.SHGFI_LARGEICON;

            unsafe
            {
                PInvoke.SHGetFileInfo(filePath, FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL, &fileInfo, (uint)Marshal.SizeOf(typeof(SHFILEINFOW)), flags);
            }

            if (fileInfo.hIcon == IntPtr.Zero)
                return null;

            return IconToImageSource(Icon.FromHandle(fileInfo.hIcon));
        }

        private static ImageSource IconToImageSource(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        public static ImageSource? GetDefaultProgramIcon()
        {
            DrawingImage? template = (DrawingImage)Application.Current.TryFindResource("DefaultProgramImage");
            if (template == null)
                return null;

            return template;
        }

        public static ImageSource? ExtractShortcutIconImage(string shortcutPath)
        {
            Windows.Win32.UI.Shell.ShellLink shellLink = new Windows.Win32.UI.Shell.ShellLink();
            IShellLinkW? shellLinkW = (IShellLinkW)shellLink;

            ((IPersistFile)shellLink).Load(shortcutPath, STGM.STGM_READ);

            int iconIndex = 0;

            unsafe
            {
                fixed (char* iconPathPtr = new char[PInvoke.MAX_PATH])
                {
                    shellLinkW.GetIconLocation(iconPathPtr, (int)PInvoke.MAX_PATH, out iconIndex);
                    var icon = PInvoke.ExtractIcon(HINSTANCE.Null, iconPathPtr, (uint)iconIndex);
                    if (icon == IntPtr.Zero)
                        return null;

                    return IconToImageSource(Icon.FromHandle(icon));
                }
            }
        }
    }
}
