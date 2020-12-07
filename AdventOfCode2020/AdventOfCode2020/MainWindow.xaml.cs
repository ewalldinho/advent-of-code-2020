using System;
using AdventOfCode2020.Solutions;
using AdventOfCode2020.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AdventOfCode2020
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!(sender is Label label)) return;
                var tag = (string) label.Tag;
                var dayAndPart = tag.Split('_');
                var dayOfAdvent = int.Parse(dayAndPart[0]);
                var part = (Parts) int.Parse(dayAndPart[1]);

                var inputData = GetData((AdventDays) dayOfAdvent);

                switch (dayOfAdvent)
                {
                    case 1:
                        var data1 = GetData(AdventDays.Day01);
                        var result1 = Day01.CalculateSolution(part, data1);
                        ShowResult(dayOfAdvent, result1);
                        break;
                    case 2:
                        var data2 = GetData(AdventDays.Day02);
                        var result2 = Day02.CalculateSolution(part, data2);
                        ShowResult(dayOfAdvent, result2);
                        break;
                    case 3:
                        var data3 = GetData(AdventDays.Day03);
                        var result3 = Day03.CalculateSolution(part, data3);
                        ShowResult(dayOfAdvent, result3);
                        break;
                    case 4:
                        var data4 = GetData(AdventDays.Day04);
                        var result4 = Day04.CalculateSolution(part, data4);
                        ShowResult(dayOfAdvent, result4);
                        break;
                    case 5:
                        var data5 = GetData(AdventDays.Day05);
                        var result5 = Day05.CalculateSolution(part, data5);
                        ShowResult(dayOfAdvent, result5);
                        break;
                    case 6:
                        var data6 = GetData(AdventDays.Day06);
                        var result6 = Day06.CalculateSolution(part, data6);
                        ShowResult(dayOfAdvent, result6);
                        break;
                    default:
                        MessageBox.Show($"Solution for day {dayOfAdvent} not implemented.", "No solution",
                            MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}", "Error", MessageBoxButton.OK);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (var day = 1; day <= 25; day++)
            {
                var labelPart1 = new Label
                {
                    Tag = $"{day}_1", 
                    Content = $"{day}.1",
                    FontSize = 28,
                    Foreground = Brushes.DimGray,
                    Background = Brushes.LightGray,
                    Margin = new Thickness(2, 2, 2, 51),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                labelPart1.SetValue(Grid.RowProperty, 1 + (day-1) / 5);
                labelPart1.SetValue(Grid.ColumnProperty, 1 + (day - 1) % 5);
                labelPart1.MouseLeftButtonDown += Label_MouseLeftButtonDown;

                var labelPart2 = new Label
                {
                    Tag = $"{day}_2", 
                    Content = $"{day}.2",
                    FontSize = 28,
                    Foreground = Brushes.DarkGray,
                    Background = Brushes.WhiteSmoke,
                    Margin = new Thickness(2, 51, 2, 2),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };
                labelPart2.SetValue(Grid.RowProperty, 1 + (day - 1) / 5);
                labelPart2.SetValue(Grid.ColumnProperty, 1 + (day - 1) % 5);
                labelPart2.MouseLeftButtonDown += Label_MouseLeftButtonDown;

                DaysContainer.Children.Add(labelPart1);
                DaysContainer.Children.Add(labelPart2);

            }
            
        }

        private string GetData(AdventDays day)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                var inputWindow = new InputDataWindow();
                if (inputWindow.ShowDialog() ?? false)
                    return inputWindow.InputData;
            }
            
            return PuzzleData.GetData(day);
        }

        private void ShowResult(int day, string result)
        {
            MessageBox.Show($"Solution for day {day}: {result}", "Solution", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
