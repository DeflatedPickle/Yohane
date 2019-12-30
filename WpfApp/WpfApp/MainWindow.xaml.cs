using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using WpfScreenHelper;

namespace WpfApp
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;

            var paths = PathUtil.DiscoverPaths();
            foreach (var path in paths)
            {
                ComboBox.Items.Add(path);
            }

            Grid.Children.Add(ItemUtil.AddButton(@"C:\Program Files\Internet Explorer\iexplore.exe", 0, 0, 1, 2));
            Grid.Children.Add(ItemUtil.AddButton(@"C:\Windows\system32\notepad.exe", 0, 2));
            Grid.Children.Add(ItemUtil.AddButton(@"C:\Windows\system32\mspaint.exe", 1, 2, 2, 2));
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Left = Screen.PrimaryScreen.WorkingArea.Width / 2 - Width / 2;
            // Top = Screen.PrimaryScreen.WorkingArea.Bottom;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                {
                    break;
                }
                default:
                {
                    ComboBox.Visibility = Visibility.Visible;
                    ComboBox.Focus();
                    ComboBox.UpdateLayout();
                    break;
                }
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.OldFocus == null)
            {
                var show = new DoubleAnimation(
                    Screen.PrimaryScreen.WorkingArea.Bottom,
                    Screen.PrimaryScreen.WorkingArea.Bottom - Height,
                    new Duration(new TimeSpan(0, 0, 0, 1)));
                BeginAnimation(TopProperty, show);
                
                (FindResource("Show") as Storyboard).Begin();
            }
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == null)
            {
                var hide = new DoubleAnimation(
                    Screen.PrimaryScreen.WorkingArea.Bottom - Height,
                    Screen.PrimaryScreen.WorkingArea.Bottom,
                    new Duration(new TimeSpan(0, 0, 0, 1)));
                BeginAnimation(TopProperty, hide);
                
                (FindResource("Hide") as Storyboard).Begin();
            }
        }
    }
}