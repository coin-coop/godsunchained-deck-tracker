using GodsUnchained_Deck_Tracker.Tracker;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GodsUnchained_Deck_Tracker.Windows
{
    /// <summary>
    /// Interaction logic for CardsCollection.xaml
    /// </summary>
    public partial class CardsCollection : Window
    {
        public CardsCollection() {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = true;
            this.Hide();
        }
    }
}
