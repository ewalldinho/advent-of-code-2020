using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AdventOfCode2020
{
    /// <summary>
    /// Interaction logic for ResultWindow.xaml
    /// </summary>
    public partial class ResultWindow : Window
    {
        private readonly int _day;
        private readonly string _result;

        public ResultWindow(int day, string result)
        {
            _day = day;
            _result = result;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LblTitle.Content = $"Solution for puzzle of the day #{_day}";
            TbResult.Text = _result;
        }

        private async void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            try
            {
                btn.IsEnabled = false;

                var text = TbResult.Text;
                Clipboard.SetText(text);

                TbResult.Select(0, text.Length);

                await ExecuteDelayed(() => { btn.IsEnabled = true; }, 1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while trying to copy result into clipboard. {ex.Message}", "Error",
                    MessageBoxButton.OK);

                btn.IsEnabled = true;
            }
        }

        public async Task ExecuteDelayed(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }

    }
}
