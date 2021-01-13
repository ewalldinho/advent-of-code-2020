using AdventOfCode2020.Utils;
using System;
using System.Threading.Tasks;
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

        private async void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!(sender is Label label)) return;

                var tag = (string) label.Tag;
                var dayAndPart = tag.Split('_');
                var dayOfAdvent = int.Parse(dayAndPart[0]);
                var part = (Parts) int.Parse(dayAndPart[1]);

                await RunSolution(dayOfAdvent, part);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}", "Error", MessageBoxButton.OK);

                StatusBar.Visibility = Visibility.Hidden;
                StatusText.Visibility = Visibility.Hidden;
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

        private async Task RunSolution(int dayOfAdvent, Parts part)
        {
            StatusBar.Visibility = Visibility.Visible;
            StatusText.Visibility = Visibility.Visible;

            var inputData = GetData((AdventDays)dayOfAdvent);

            var puzzleSolution = PuzzleSolutionFactory.GetPuzzleSolution(dayOfAdvent);

            var result = await Task.Run(() => puzzleSolution.CalculateSolution(part, inputData));

            StatusBar.Visibility = Visibility.Hidden;
            StatusText.Visibility = Visibility.Hidden;

            ShowResult(dayOfAdvent, result);
        }

        private void ShowResult(int day, string result)
        {
            var resultWindow = new ResultWindow(day, result);
            resultWindow.ShowDialog();
        }
    }
}
