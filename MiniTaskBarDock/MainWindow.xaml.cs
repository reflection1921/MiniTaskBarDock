using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace MiniTaskBarDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Configuration _config;
        private List<MONITORINFO> _monitors;
        public MainWindow()
        {
            InitializeComponent();

            _config = new Configuration("config.json");
            _monitors = ScreenUtils.GetAllMonitors();

            if (_config.Data.DockDataPath == null)
            {
                while (!SetDockDataPath())
                {
                    MessageBox.Show("Please select a valid folder path.", "MiniTaskBarDock");
                }
            }

            Topmost = true;

            CreateIconGrid();

            this.Loaded += (s, e) => PositionWindow(_config.Data.MonitorIndex);
        }

        private bool SetDockDataPath()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() == true)
            {
                _config.Data.DockDataPath = dialog.FolderName;
                _config.Save();

                return true;
            }

            return false;
        }

        private void CreateIconGrid()
        {
            string path = _config.Data.DockDataPath!;

            string[] filesList = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            int topOffset = -1;
            int leftOffset = 0;

            for (int i = 0; i < filesList.Length; i++)
            {
                if (i % _config.Data.IconCountPerLine == 0)
                {
                    topOffset++;
                    leftOffset = 0;
                }
                string filePath = filesList[i];

                string destinationFilePath = filePath;

                ImageSource? icon;

                if (filePath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    destinationFilePath = IconUtils.GetDestinationPathFromShortcut(filePath);

                    icon = IconUtils.ExtractShortcutIconImage(filePath) ?? IconUtils.ExtractIconImage(destinationFilePath);
                }
                else
                {
                    icon = IconUtils.ExtractIconImage(filePath);
                }

                if (icon == null)
                {
                    icon = IconUtils.GetDefaultProgramIcon();
                    if (icon == null) continue;
                }

                Button btn = CreateShortcutButton(i, destinationFilePath, icon, leftOffset, topOffset, _config.Data.IconSize);

                leftOffset++;

                MainGrid.Children.Add(btn);
            }

            this.Width = _config.Data.IconSize * (filesList.Length < _config.Data.IconCountPerLine ? filesList.Length : _config.Data.IconCountPerLine);
            this.Height = _config.Data.IconSize * (topOffset + 1) + 8;
        }

        private Button CreateShortcutButton(int index, string destinationFilePath, ImageSource icon, int leftOffset,
            int topOffset, int iconSize)
        {
            var btn = new Button
            {
                Name = $"btnAppIcon{index}",
                Tag = icon,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(leftOffset * iconSize, 4 + (topOffset * iconSize), 0, 4),
                Width = iconSize,
                Height = iconSize,
                DataContext = destinationFilePath,
                ToolTip = Path.GetFileNameWithoutExtension(destinationFilePath)
            };

            btn.Click += (sender, _) =>
            {
                string path = (string)((Button)sender).DataContext;
                LaunchPath(path);
            };

            return btn;
        }

        private void LaunchPath(string path)
        {
            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
                return;
            }

            Process.Start(path);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void PositionWindow(int index)
        {
            RECT GetMonitorWorkingArea()
            {
                if (index == -1)
                {
                    MONITORINFO? monitor = ScreenUtils.GetActiveMonitor();
                    if (monitor != null)
                    {
                        return monitor.Value.rcWork;
                    }
                }

                if (index < 0 || index >= _monitors.Count)
                {
                    index = 0;
                }

                return _monitors[index].rcWork;
            }
            
            var monitor = GetMonitorWorkingArea();

            this.Left = monitor.left + (monitor.right - monitor.left - this.Width) / 2; //width: Center
            this.Top = monitor.Height - Height; //height: above taskbar

            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }
    }
}