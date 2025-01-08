using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Media;


namespace MiniTaskBarDock
{
    internal class IconUtils
    {
        public static ImageSource? ExtractIconImage(string filePath)
        {
            User32.SHFILEINFO shinfo = new User32.SHFILEINFO();
            IntPtr hImgLarge = User32.SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), User32.SHGFI_ICON | User32.SHGFI_LARGEICON);
            if (shinfo.hIcon == IntPtr.Zero)
                return null;

            return IconToImageSource(Icon.FromHandle(shinfo.hIcon));
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
