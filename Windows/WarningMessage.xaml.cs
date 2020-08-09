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
    /// Interaction logic for WarningMessage.xaml
    /// </summary>
    public partial class WarningMessage : Window
    {
        public WarningMessage() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
