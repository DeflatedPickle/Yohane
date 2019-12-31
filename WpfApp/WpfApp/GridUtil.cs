using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace WpfApp
{
    public struct Pair
    {
        public int X;
        public int Y;
    }
    
    public class GridUtil
    {
        public static Button AddButton(string path, int row, int column, int rowspan = 1, int columnspan = 1, bool hasLabel = true)
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
            
            button.PreviewMouseLeftButtonDown += ((s, e) =>
            {
                var dataObject = new DataObject();
                dataObject.SetData("button", s);

                DragDrop.DoDragDrop(button, dataObject, DragDropEffects.Move);
            });

            var buttonContent = new Grid();
            buttonContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            if (hasLabel) buttonContent.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            
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
            
            buttonContent.Children.Add(imageElement);

            if (hasLabel)
            {
                var nameElement = new Label()
                {
                    Name = "Text",
                    Content = name,
                    FontSize = 16,
                    Foreground = Brushes.Azure
                };
                Grid.SetRow(nameElement, 1);
            
                buttonContent.Children.Add(nameElement);
            }
            
            button.Content = buttonContent;

            return button;
        }

        public static Button AddFolder(string name, IEnumerable<Control> children, int rowCount, int columnCount, int row, int column, int margin = 2, int rowspan = 1, int columnspan = 1)
        {
            var button = new Button
            {
                ToolTip = name,
                Margin = new Thickness(2), 
                Style = Application.Current.FindResource("ButtonRevealStyle") as Style
            };

            var buttonContent = new Grid();

            for (var i = 0; i < rowCount; i++)
            {
                buttonContent.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
            }

            for (var i = 0; i < columnCount; i++)
            {
                buttonContent.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
            }

            foreach (var child in children)
            {
                child.Margin = new Thickness(margin);
                buttonContent.Children.Add(child);
            }
            
            button.Content = buttonContent;

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            Grid.SetRowSpan(button, rowspan);
            Grid.SetColumnSpan(button, columnspan);

            return button;
        }

        public static Pair GetPointFromPos(Point point, Grid grid)
        {
            var rowStart = 0.0;
            var columnStart = 0.0;

            var row = 0;
            var column = 0;

            foreach (var rowDefinition in grid.RowDefinitions)
            {
                rowStart += rowDefinition.ActualHeight;

                if (point.Y < rowStart)
                {
                    break;
                }
                row++;
            }
                
            foreach (var columnDefinition in grid.ColumnDefinitions)
            {
                columnStart += columnDefinition.ActualWidth;

                if (point.X < columnStart)
                {
                    break;
                }

                column++;
            }
            
            return new Pair { X = column, Y = row};
        }
    }
}