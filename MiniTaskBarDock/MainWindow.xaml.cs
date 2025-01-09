using ShellLink;
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

namespace MiniTaskBarDock
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            /* SETUP */
            const string path = "";
            const int gridWidthIconCount = 7;

            this.Topmost = true;
            
            string[] programPathArr = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(file => file.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || file.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            int topOffset = -1;
            int leftOffset = 0;

            for (int i = 0; i < programPathArr.Length; i++)
            {
                if (i % gridWidthIconCount == 0)
                {
                    topOffset++;
                    leftOffset = 0;
                }
                string programPath = programPathArr[i];

                string iconPath = programPath;

                if (programPath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    var shortcut = Shortcut.ReadFromFile(programPath);
                    programPath = shortcut.LinkTargetIDList.Path;
                    // TODO: Support user selected icon in shortcut
                    //if (!string.IsNullOrEmpty(shortcut.StringData.IconLocation))
                    //{
                    //    iconPath = shortcut.StringData.IconLocation + ", 0";
                    //}
                }

                // TODO: Add folder in the future
                // Skip Folder
                if (Directory.Exists(programPath))
                {
                    continue;
                }

                ImageSource? icon = IconUtils.ExtractIconImage(iconPath);
                if (icon == null)
                {
                    icon = IconUtils.GetDefaultProgramIcon();
                    if (icon == null) continue;
                }

                var btn = new Button
                {
                    Name = "btnAppIcon" + i,
                    Tag = icon,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(leftOffset * 40, 4 + (topOffset * 40), 0, 4),
                    Width = 40,
                    Height = 40,
                    DataContext = programPath
                };

                btn.Click += (sender, e) =>
                {
                    string runAppPath = (string)((Button)sender).DataContext;
                    System.Diagnostics.Process.Start(runAppPath);
                };

                leftOffset++;

                MainGrid.Children.Add(btn);
            }

            this.Width = 40 * (programPathArr.Length < gridWidthIconCount ? programPathArr.Length : gridWidthIconCount);
            this.Height = 40 * (topOffset + 1) + 8;

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}