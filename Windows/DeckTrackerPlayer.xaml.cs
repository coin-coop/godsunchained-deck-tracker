using System.Windows;
using System.Windows.Input;

namespace GodsUnchained_Deck_Tracker.Windows
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
