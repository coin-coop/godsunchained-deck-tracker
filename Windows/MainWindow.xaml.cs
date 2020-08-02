using GodsUnchained_Deck_Tracker.Controller;
using GodsUnchained_Deck_Tracker.Model.Entities;
using GodsUnchained_Deck_Tracker.Tracker;
using GodsUnchained_Deck_Tracker.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GodsUnchained_Deck_Tracker.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private DeckTrackerPlayer deckTrackerPlayer;
        private DeckTrackerOpponent deckTrackerOpponent;
        private Settings settings;
        private Deck selectedDeck;

        public MainWindow()
        {
            InitializeComponent();

            try {
                deckTrackerPlayer = new DeckTrackerPlayer();
                deckTrackerPlayer.Show();
                deckTrackerPlayer.Topmost = true;

                /*deckTrackerOpponent = new DeckTrackerOpponent();
                deckTrackerOpponent.Show();
                deckTrackerOpponent.Topmost = true;*/

                //if (Properties.Settings.Default.logFilePath.Contains("C:\\Users\\YourUser\\")) {
                settings = new Settings();
                    settings.ShowDialog();
                //}

                DispatcherTimer timerUpdate = new DispatcherTimer {
                    Interval = TimeSpan.FromSeconds(12)
                };
                timerUpdate.Tick += UpdateDeck;
                timerUpdate.Start();

                DispatcherTimer timerDeckList = new DispatcherTimer {
                    Interval = TimeSpan.FromMinutes(1)
                };
                timerDeckList.Tick += UpdateDeckList;
                timerDeckList.Start();
            } catch(Exception e) {
                lblException.Text = e.Message + e.StackTrace;
            }
        }

        protected override void OnClosed(EventArgs e) {
            deckTrackerPlayer.Close();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e) {
            try {
                Task<string> userDataTask = UserController.GetUserDataAsync();
                await userDataTask;
                string userDataResult = userDataTask.Result;

                Task<string> userRankTask = UserController.GetUserRankAsync();
                await userRankTask;
                string userRankResult = userRankTask.Result;

                DisplayUserInfo(UserController.GetUser(userDataResult, userRankResult));

                CardPrototypeManager.GetPrototypesAsync();
                DeckManager.GetDecksPlayedByPlayer();

                List<Deck> decks = DeckManager.GetDecks();
                if (decks?.Any() == true) {
                    selectedDeck = decks.Last();
                }

                AddDeckButtonsToStackPanel();
            } catch(Exception ex) {
                lblException.Text = ex.Message + ex.StackTrace;
            }
}

        private void UpdateDeck(object sender, EventArgs e) {
            //deckTrackerOpponent.lblDeckTitle.Content = "Current Deck Title";
            try {
                DeckManager.UpdateDeck(selectedDeck);
                deckTrackerPlayer.lblDeckTitle.Content = DeckManager.GetDeckName();
                deckTrackerPlayer.lblCardsCount.Content = "Remaining Cards: " + DeckManager.GetCardsInDeck().ToString();
                deckTrackerPlayer.lbCards.ItemsSource = GetObservableCardViews(DeckManager.GetCurrentDeckCards());
                deckTrackerPlayer.lbExtraCards.ItemsSource = DeckManager.GetExtraDrawnCards();
                deckTrackerPlayer.lbSanctumCards.ItemsSource = DeckManager.GetSanctumDrawnCards();
            } catch (Exception ex) {
                lblException.Text = ex.Message + ex.StackTrace;
            }
        }

        private void UpdateDeckList(object sender, EventArgs e) {
            AddDeckButtonsToStackPanel();
        }

        private void DisplayUserInfo(User user) {
            txtLoading.Text = "";
            txtPlayerName.Text = user.Name;
            lblRankData.Content =  user.Rank;
            lblRatingData.Content = user.Rating.ToString();
            lblLevelData.Content = user.Level.ToString();
            lblMatchesData.Content = (user.Wins + user.Loses).ToString();
            lblMatchesWonData.Content = user.Wins.ToString();
            lblMatchesLostData.Content = user.Loses.ToString();
            lblWinPointsData.Content = user.WinPoints.ToString();
            lblLossPointsData.Content = user.LossPoints.ToString();
        }

        private void AddDeckButtonsToStackPanel() {
            List<Deck> decks = DeckManager.GetDecks();
            if(decks.Count > spDecks.Children.Count) {
                spDecks.Children.Clear();
                foreach (Deck deck in decks) {
                    ImageBrush brush = new ImageBrush();
                    BitmapImage godImage = new BitmapImage(new Uri(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\..\\Resources\\Images\\Decks\\deck-" + deck.God.ToString().ToLower() + "-50.png", UriKind.Absolute));
                    //BitmapImage godImage = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\Resources\\Images\\Decks\\deck-" + deck.God.ToString().ToLower() + "-50.png", UriKind.Absolute));
                    brush.ImageSource = godImage;
                    brush.ImageSource = godImage;
                    brush.Stretch = Stretch.None;
                    brush.AlignmentX = AlignmentX.Left;
                    brush.AlignmentY = AlignmentY.Top;

                    Button deckButton = new Button {
                        Content = "               " + deck.Name,
                        Foreground = Brushes.AliceBlue,
                        FontWeight = FontWeights.Bold,
                        Height = 53,
                        Margin = new Thickness(2),
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        Background = brush,
                        Tag = deck
                    };

                    deckButton.Click += DeckButton_Click;

                    spDecks.Children.Add(deckButton);
                }
            }
        }

        private void DeckButton_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;

            Deck deck = button.Tag as Deck;

            selectedDeck = deck;

            deckTrackerPlayer.lbCards.ItemsSource = GetObservableCardViews(DeckManager.GetCardsView(selectedDeck.Cards));

            lblDeckTitle.Content = deck.Name;
            lbCards.ItemsSource = GetObservableCardViews(DeckManager.GetCardsView(deck.Cards));
        }

        private ObservableCollection<CardView> GetObservableCardViews(List<CardView> cardsView) {
            ObservableCollection<CardView> cardsToDisplay = new ObservableCollection<CardView>();

            foreach (CardView cardView in cardsView) {
                cardsToDisplay.Add(cardView);
            }

            return cardsToDisplay;
        }
    }
}
