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

namespace GodsUnchained_Companion_App.Windows
{
    /// <summary>
    /// Interaction logic for DeckTrackerPlayer.xaml
    /// </summary>
    public partial class DeckTrackerPlayer : Window
    {
        public DeckTrackerPlayer() {
            InitializeComponent();
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
