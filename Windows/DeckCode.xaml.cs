using GodsUnchained_Companion_App.Controller;
using System.Windows;

namespace GodsUnchained_Companion_App.Windows
{
    /// <summary>
    /// Interaction logic for DeckCode.xaml
    /// </summary>
    public partial class DeckCode : Window
    {
        public DeckCode() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            DeckController.ImportDeckFromCode(txtDeckCode.Text);

            this.Close();
        }
    }
}
