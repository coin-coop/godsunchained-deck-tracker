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
    public partial class MainWindow : Window
    {
        private List<CardView> cardViews = new List<CardView>();

        private DeckTrackerPlayer deckTrackerPlayer = new DeckTrackerPlayer();
        private DeckTrackerOpponent deckTrackerOpponent = new DeckTrackerOpponent();

        private DispatcherTimer timerUpdate;

        private DeckCode deckCode;
        private LogPathSettings logPathSettings;
        private AccountSettings accountSettings;

        private CardsCollection cardsCollection;

        private WarningMessage warningMessage;

        public MainWindow() {
            InitializeComponent();

            try {
                if (Properties.Settings.Default.logFilePath.Contains("C:\\Users\\YourUser\\")) {
                    logPathSettings = new LogPathSettings();
                    logPathSettings.txtLogFilePath.Text = Properties.Settings.Default.logFilePath;
                    logPathSettings.ShowDialog();
                }

                DispatcherTimer timerDeckList = new DispatcherTimer {
                    Interval = TimeSpan.FromMinutes(1)
                };
                timerDeckList.Tick += UpdateDeckList;
                timerDeckList.Start();
            } catch (Exception e) {
                warningMessage = new WarningMessage();
                warningMessage.txtWarningMessage.Text = e.Message;
                warningMessage.Show();
                lblException.Text = e.StackTrace;
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

                CardPrototypeController.GetPrototypesAsync();
                DeckController.GetDecksPlayedByPlayer();

                List<Deck> decks = DeckController.GetDecks();
                if (decks?.Any() == true) {
                    PlayerDeckTracker.GetInstance.SelectedDeck = decks.Last();
                }

                AddDeckButtonsToStackPanel();

                Task<List<CardView>> cardViewTask = CardController.GetCardViews();
                await cardViewTask;
                cardViews = cardViewTask.Result;
            } catch (Exception ex) {
                warningMessage = new WarningMessage();
                warningMessage.txtWarningMessage.Text = ex.Message;
                warningMessage.Show();
                lblException.Text = ex.StackTrace;
            }
        }

        private void ImportDeckItemMenu_Click(object sender, RoutedEventArgs e) {
            deckCode = new DeckCode();

            if (Clipboard.ContainsText(TextDataFormat.Text)) {
                deckCode.txtDeckCode.Text = Clipboard.GetText(TextDataFormat.Text);
            }

            deckCode.ShowDialog();

            AddDeckButtonsToStackPanel();
        }

        private void LoadCardsItemMenu_Click(object sender, RoutedEventArgs e) {
            if(Properties.Settings.Default.userAddress.Equals("0x0")) {
                warningMessage = new WarningMessage();
                warningMessage.txtWarningMessage.Text = "User address is not setup. Cards couldn't be loaded.";
                warningMessage.Show();
            } else {
                CardController.GetCards(Properties.Settings.Default.userAddress);
            }
        }

        private void ViewCardsItemMenu_Click(object sender, RoutedEventArgs e) {
            cardsCollection = new CardsCollection();
            cardsCollection.lvCards.ItemsSource = cardViews;
            cardsCollection.Show();
        }

        private void SettingsAccountItemMenu_Click(object sender, RoutedEventArgs e) {
            accountSettings = new AccountSettings();
            accountSettings.txtUserAddress.Text = Properties.Settings.Default.userAddress;
            accountSettings.ShowDialog();
        }

        private void SettingsLogItemMenu_Click(object sender, RoutedEventArgs e) {
            logPathSettings = new LogPathSettings();
            logPathSettings.txtLogFilePath.Text = Properties.Settings.Default.logFilePath;
            logPathSettings.ShowDialog();
        }

        private void UpdateDeck(object sender, EventArgs e) {
            //deckTrackerOpponent.lblDeckTitle.Content = "Current Deck Title";
            try {
                UpdateDeckTracker();
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
            lblRankData.Content = user.Rank;
            lblRatingData.Content = user.Rating.ToString();
            lblLevelData.Content = user.Level.ToString();
            lblMatchesData.Content = (user.Wins + user.Loses).ToString();
            lblMatchesWonData.Content = user.Wins.ToString();
            lblMatchesLostData.Content = user.Loses.ToString();
            lblWinPointsData.Content = user.WinPoints.ToString();
            lblLossPointsData.Content = user.LossPoints.ToString();
        }

        private void AddDeckButtonsToStackPanel() {
            List<Deck> decks = DeckController.GetDecks();
            if (decks.Count > spDecks.Children.Count) {
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
            button.Foreground = Brushes.DodgerBlue;

            PlayerDeckTracker.GetInstance.SelectedDeck = button.Tag as Deck;

            deckTrackerPlayer.lbCards.ItemsSource = GetObservableCardViews(PlayerDeckTracker.GetInstance.GetSelectedDeckCards());

            lblDeckTitle.Content = PlayerDeckTracker.GetInstance.SelectedDeck.Name;
            lbCards.ItemsSource = GetObservableCardViews(PlayerDeckTracker.GetInstance.GetSelectedDeckCards());

            btnStartTracker.IsEnabled = true;
        }

        private void StartTrackerButton_Click(object sender, RoutedEventArgs e) {
            deckTrackerPlayer.Show();
            deckTrackerPlayer.Topmost = true;

            /*deckTrackerOpponent.Show();
            deckTrackerOpponent.Topmost = true;*/

            UpdateDeckTracker();

            timerUpdate = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(12)
            };
            timerUpdate.Tick += UpdateDeck;
            timerUpdate.Start();

            btnStartTracker.IsEnabled = false;
            btnStopTracker.IsEnabled = true;
        }

        private void StopTrackerButton_Click(object sender, RoutedEventArgs e) {
            deckTrackerPlayer.Hide();

            timerUpdate.Stop();

            btnStartTracker.IsEnabled = true;
            btnStopTracker.IsEnabled = false;
        }

        private void UpdateDeckTracker() {
            PlayerDeckTracker playerDeckTracker = PlayerDeckTracker.GetInstance;

            playerDeckTracker.UpdateDeck();
            deckTrackerPlayer.lblDeckTitle.Content = playerDeckTracker.GetDeckName();
            deckTrackerPlayer.lblCardsCount.Content = "Remaining Cards: " + playerDeckTracker.CardsInDeck.ToString();
            deckTrackerPlayer.lbCards.ItemsSource = GetObservableCardViews(playerDeckTracker.GetCurrentDeckCards());
            deckTrackerPlayer.lbExtraCards.ItemsSource = playerDeckTracker.GetExtraDrawnCards();
            deckTrackerPlayer.lbSanctumCards.ItemsSource = playerDeckTracker.GetSanctumDrawnCards();
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
