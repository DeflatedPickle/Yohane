using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfScreenHelper;

namespace WpfApp
{
    public partial class MainWindow
    {
        class GridItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public BitmapSource Image { get; set; }
            public int Row { get; set; }
            public int Column { get; set; }
            public int RowSpan { get; set; }
            public int ColumnSpan { get; set; }

            public List<string> ContextMenu { get; set; }

            public GridItem(string path, int row, int column)
            {
                string result;
                PathDict.TryGetValue(path, out result);

                Name = result == null ? path.Split('\\').Last() : result.Split('\\').Last().Split('.').First();
                MessageBox.Show(Name);
                
                Path = path;

                var image = (Icon) null;

                try
                {
                    image = System.Drawing.Icon.ExtractAssociatedIcon(path);
                }
                catch (SystemException e)
                {
                    Console.WriteLine(e);
                }

                Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(image.Handle, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                Row = row;
                Column = column;
                RowSpan = 1;
                ColumnSpan = 1;
                ContextMenu = new List<string>() {"Remove"};
            }
        }

        public static Dictionary<string, string> PathDict;

        public MainWindow()
        {
            InitializeComponent();

            PathDict = new Dictionary<string, string>();

            DirectoryUtil.RecurseDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                s =>
                {
                    var path = FileUtil.GetShortcutTarget(s);
                    if (path != null)
                    {
                        PathDict[path] = s;
                    }
                });

            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using (var subKey = key.OpenSubKey(subKeyName))
                    {
                        var displayName = subKey.GetValue("DisplayName") as string;
                        var installLocation = subKey.GetValue("InstallLocation") as string;

                        if (displayName != null && installLocation != null && !PathDict.ContainsKey(displayName))
                        {
                            PathDict[installLocation] = displayName;
                        }
                    }
                }
            }

            foreach (var path in Environment.GetEnvironmentVariable("Path").Split(';'))
            {
                if (path.EndsWith(@"\"))
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryUtil.RecurseDirectory(path, (s) =>
                        {
                            var name = s.Split('\\').Last();

                            if (!PathDict.ContainsKey(name))
                            {
                                PathDict[path] = name;
                            }
                        });
                    }
                }
                else
                {
                    // Get the fancy name from the StartMenu directory, if there is one
                    var name = path.Split('\\').Last();

                    if (!PathDict.ContainsKey(name))
                    {
                        PathDict[name] = path;
                    }
                }
            }

            GridItems.ItemsSource = new GridItem[]
            {
                new GridItem(@"C:\Program Files\Internet Explorer\iexplore.exe", 0, 0) {ColumnSpan = 2},
                new GridItem(@"C:\Windows\system32\notepad.exe", 0, 2),
            };
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var monitor = Screen.FromPoint(Mouse.GetPosition(this));
            Left = monitor.WorkingArea.Width / 2 - Width / 2;
            Top = monitor.WorkingArea.Bottom - Height;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            WindowState = WindowState.Minimized;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var process = new Process())
            {
                var path = (((sender as Button).Content as Panel).FindName("Text") as TextBlock).Text;
                process.StartInfo.FileName = path;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                process.StartInfo.ErrorDialog = true;
                process.Start();
            }
        }
    }
}