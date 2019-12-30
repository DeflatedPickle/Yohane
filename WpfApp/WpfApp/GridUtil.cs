using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace WpfApp
{
    public class ItemUtil
    {
        
        public static Button AddButton(string path, int row, int column, int rowspan = 1, int columnspan = 1)
        {
            var button = new Button
            {
                ToolTip = path,
                Margin = new Thickness(2), 
                Style = Application.Current.FindResource("ButtonRevealStyle") as Style
            };
            
            button.Click += (s, e) =>
            {
                ProcessUtil.OpenProcess(path);
            };

            var buttonContent = new Grid();
            buttonContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            buttonContent.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
            PathUtil.PathDict.TryGetValue(path, out var result);

            var name = result == null ? path.Split('\\').Last() : result.Split('\\').Last().Split('.').First();

            var image = (Icon) null;

            try
            {
                image = Icon.ExtractAssociatedIcon(path);
            }
            catch (SystemException e)
            {
                Console.WriteLine(e);
            }

            var bitmapSource = Imaging.CreateBitmapSourceFromHIcon(image.Handle, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            Grid.SetRowSpan(button, rowspan);
            Grid.SetColumnSpan(button, columnspan);

            var imageElement = new Image() { Source = bitmapSource };
            RenderOptions.SetBitmapScalingMode(imageElement, BitmapScalingMode.Fant);
            Grid.SetRow(imageElement, 0);

            var nameElement = new Label()
            {
                Name = "Text",
                Content = name,
                FontSize = 16,
                Foreground = Brushes.Azure
            };
            Grid.SetRow(nameElement, 1);
            
            buttonContent.Children.Add(imageElement);
            buttonContent.Children.Add(nameElement);
            
            button.Content = buttonContent;

            return button;
        }
    }
}