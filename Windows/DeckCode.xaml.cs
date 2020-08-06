using GodsUnchained_Deck_Tracker.Controller;
using System.Windows;

namespace GodsUnchained_Deck_Tracker.Windows
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
