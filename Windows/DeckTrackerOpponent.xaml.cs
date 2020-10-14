using System.Windows;
using System.Windows.Input;

namespace GodsUnchained_Companion_App.Windows
{
    /// <summary>
    /// Interaction logic for DeckTrackerOpponent.xaml
    /// </summary>
    public partial class DeckTrackerOpponent : Window
    {
        public DeckTrackerOpponent() {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }
    }
}
