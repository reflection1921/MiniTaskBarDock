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
    }
}
