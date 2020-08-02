using System.Windows;

namespace GodsUnchained_Deck_Tracker.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.logFilePath = txtLogFilePath.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
