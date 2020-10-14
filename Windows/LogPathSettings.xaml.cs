using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace GodsUnchained_Companion_App.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class LogPathSettings : Window
    {
        public LogPathSettings() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.logFilePath = txtLogFilePath.Text;
            Properties.Settings.Default.Save();
            this.Close();
        }
    }
}
