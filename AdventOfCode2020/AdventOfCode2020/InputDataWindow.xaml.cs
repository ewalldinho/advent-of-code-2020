using System.Windows;

namespace AdventOfCode2020
{
    /// <summary>
    /// Interaction logic for PageFunction1.xaml
    /// </summary>
    public partial class InputDataWindow : Window
    {
        public InputDataWindow()
        {
            InitializeComponent();
        }

        public string InputData => tbInput.Text;

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
