using GodsUnchained_Companion_App.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace GodsUnchained_Companion_App.Tracker
{
    class OpponentDeckTracker : DeckTracker
    {
        private static OpponentDeckTracker instance = null;
        private static readonly object mutex = new object();

        private static string opponentId = "0";

        OpponentDeckTracker() {
            CardsInDeck = 30;
        }

        public static OpponentDeckTracker GetInstance {
            get {
                lock (mutex) {
                    if (instance == null) {
                        instance = new OpponentDeckTracker();
                    }
                    return instance;
                }
            }
        }

        public void OpenOpponentPage() {
            FileStream logFileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logReader = new StreamReader(logFileStream);

            string searchFor = "Initialising for players";
            string line;
            while ((line = logReader.ReadLine()) != null) {
                if (line.Contains(searchFor)) {
                    string opponentPart = line.Split(searchFor).Last().Trim().Split("o:PlayerInfo(apolloId: ").Last();
                    string foundOpponentId = opponentPart.Split(",").First();

                    if(!foundOpponentId.Equals(opponentId)) {
                        opponentId = foundOpponentId;
                        string url = "https://gudecks.com/meta/user-stats?userId=" + opponentId;
                        try {
                            Process.Start(url);
                        } catch {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                                url = url.Replace("&", "^&");
                                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                                Process.Start("xdg-open", url);
                            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                                Process.Start("open", url);
                            } else {
                                throw;
                            }
                        }
                    }
                }
            }

            logReader.Close();
            logFileStream.Close();
        }
    }
}
