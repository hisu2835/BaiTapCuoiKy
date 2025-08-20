using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace BaiTapCuoiKy
{
    public partial class Form1
    {
        private int roundDurationTotal;
        private bool halfHintShown;
        private bool isSpectator = false;
        private Button btnSpectateMode;

        // Turn management
        private int currentDrawerIndex = 0; // index trong connectedPlayers

        #region TransitionToGame and Game Logic

        private void TransitionToGame()
        {
            try
            {
                isInWaitingRoom = false;
                isInLobby = false;
                isInGame = true;

                if (lobbyAnimationTimer != null)
                {
                    lobbyAnimationTimer.Stop();
                    lobbyAnimationTimer.Dispose();
                    lobbyAnimationTimer = null;
                }

                if (lobbyPanel != null)
                {
                    this.Controls.Remove(lobbyPanel);
                    lobbyPanel.Dispose();
                    lobbyPanel = null;
                }

                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(1400, 800);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.Text = $"DrawMaster - Ph?ng {currentRoomCode}";
                this.BackColor = Color.FromArgb(248, 249, 250);

                if (gameView == null)
                {
                    gameView = new GameViewControl { Dock = DockStyle.Fill };
                }

                gameView.RoomCode = currentRoomCode;
                gameView.PlayerName = currentUser;
                gameView.PlayersOnline = connectedPlayers.Count;

                gameView.StartGameRequested += GameView_StartGameRequested;
                gameView.LeaveRequested += GameView_LeaveRequested;
                gameView.BackLobbyRequested += GameView_BackLobbyRequested;
                gameView.MessageSubmitted += GameView_MessageSubmitted;
                gameView.AnswerSubmitted += GameView_AnswerSubmitted;
                gameView.ChatSubmitted += GameView_ChatSubmitted;

                this.Controls.Add(gameView);
                gameView.BringToFront();

                SetupInitialGameState();
                SetupGameTimer();

                gameView.AddChat($"Ch?o m?ng ??n ph?ng {currentRoomCode}!");
                gameView.AddChat("Nh?n 'B?t ??u' ?? v?o l??t v? v? ch?i!");
                gameView.SetRoundInfo("- - - - -", gameTimeLeft, currentRound, maxRounds);
                UpdateGameViewLeaderboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"L?i khi chuy?n sang giao di?n game: {ex.Message}", "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupInitialGameState()
        {
            playerScore = 0;
            currentRound = 1;
            isPlayerDrawing = false;
            currentDrawerIndex = 0;
        }

        private void SetupGameTimer()
        {
            gameTimer = new Timer { Interval = 1000 };
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (gameTimeLeft > 0)
            {
                gameTimeLeft--;
                gameView?.UpdateTime(gameTimeLeft);

                if (gameTimeLeft <= roundDurationTotal / 2 && !halfHintShown)
                {
                    halfHintShown = true;
                }
            }
            else
            {
                HandleTimeUp();
            }
        }

        private void HandleTimeUp()
        {
            gameTimer.Stop();
            gameView?.AddChat("? H?t gi?!");
            EndDrawingTurn();
        }

        private void EndDrawingTurn()
        {
            isPlayerDrawing = false;
            if (connectedPlayers.Count > 0 && currentDrawerIndex >= 0 && currentDrawerIndex < connectedPlayers.Count)
            {
                connectedPlayers[currentDrawerIndex].IsDrawing = false;
            }
            AdvanceTurn();
        }

        private void AdvanceTurn()
        {
            currentDrawerIndex++;
            if (currentDrawerIndex >= connectedPlayers.Count)
            {
                currentDrawerIndex = 0;
                currentRound++;
                if (currentRound > maxRounds)
                {
                    EndGame();
                    return;
                }
            }

            StartNextTurn();
        }

        private void StartNextTurn()
        {
            if (connectedPlayers.Count == 0) return;

            var drawer = connectedPlayers[currentDrawerIndex];
            foreach (var p in connectedPlayers) p.IsDrawing = false;
            drawer.IsDrawing = true;

            isPlayerDrawing = drawer.Name == currentUser;

            gameTimeLeft = currentGameSettings?.TimePerRound ?? 60;
            roundDurationTotal = gameTimeLeft;
            halfHintShown = false;

            HandleWordSelectionOrSkip();
        }

        private void HandleWordSelectionOrSkip()
        {
            var drawer = connectedPlayers[currentDrawerIndex];
            bool isBot = drawer.Name != null && drawer.Name.StartsWith("BOT_", StringComparison.OrdinalIgnoreCase);

            if (!isPlayerDrawing)
            {
                gameView?.SetRoundInfo("- - - - -", gameTimeLeft, currentRound, maxRounds);
                if (isBot)
                {
                    // Bot: t? ch?n nhanh
                    Timer botTimer = new Timer { Interval = 800 };
                    botTimer.Tick += (s, e) =>
                    {
                        try { botTimer.Stop(); botTimer.Dispose(); } catch { }
                        string word = wordBank != null && wordBank.Length > 0 ? wordBank[random.Next(wordBank.Length)] : "TREE";
                        StartDrawingWithWord(word);
                    };
                    botTimer.Start();
                }
                else
                {
                    // Ng??i ch?i khác: ch? t?i ?a 30s ?? ch?n, n?u không -> b? l??t
                    int countdown = 30;
                    gameView?.AddChat($"{drawer.Name} ?ang ch?n ch? ?? v? (còn {countdown}s)...");
                    Timer waitTimer = new Timer { Interval = 1000 };
                    waitTimer.Tick += (s, e) =>
                    {
                        countdown--;
                        if (countdown <= 0)
                        {
                            try { waitTimer.Stop(); waitTimer.Dispose(); } catch { }
                            gameView?.AddChat($"{drawer.Name} không ch?n trong 30s. B? l??t.");
                            EndDrawingTurn();
                        }
                    };
                    waitTimer.Start();
                }
                return;
            }

            // Mình là ng??i v? -> hi?n th? dialog ch?n t? v?i timeout 30s
            EnsureWordBankSeeded();
            string option1 = wordBank[random.Next(wordBank.Length)];
            string option2 = option1;
            int safety = 0;
            while (option2 == option1 && safety++ < 20)
            {
                option2 = wordBank[random.Next(wordBank.Length)];
            }

            using (var dlg = new WordSelectionDialog(option1, option2, 30))
            {
                var result = dlg.ShowDialog(this);
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.SelectedWord))
                {
                    gameView?.AddChat("B?n không ch?n ch? ?? v? trong 30 giây. B? l??t.");
                    EndDrawingTurn();
                    return;
                }
                StartDrawingWithWord(dlg.SelectedWord.Trim());
            }
        }

        private void StartDrawingWithWord(string word)
        {
            currentWord = word;
            gameView?.SetRoundInfo(currentWord, gameTimeLeft, currentRound, maxRounds);
            gameView?.AddChat($"?? {connectedPlayers[currentDrawerIndex].Name} ?ang v?: {currentWord}");
            UpdateGameViewLeaderboard();
            gameTimer?.Start();
            try { GameEffects.PlaySuccessSound(); } catch { }
        }

        private void NextRound()
        {
            AdvanceTurn();
        }

        private void EndGame()
        {
            gameView?.AddChat("?? Trò ch?i k?t thúc! C?m ?n ?ã ch?i!");
        }

        private void RestartGame()
        {
            currentRound = 1;
            playerScore = 0;
            foreach (var p in connectedPlayers) p.Score = 0;
            currentDrawerIndex = 0;
            TransitionToGame();
        }

        private void UpdateGameViewLeaderboard()
        {
            var leaderboardData = connectedPlayers
                .OrderByDescending(p => p.Score)
                .Select((p, index) => (
                    rank: index + 1,
                    player: p.Name,
                    score: p.Score,
                    status: p.IsDrawing ? "?ang v?" : (p.Name == currentUser ? "B?n" : "?ang ?oán")
                )).ToArray();
            gameView?.SetLeaderboard(leaderboardData);
        }

        private void GameView_StartGameRequested(object sender, EventArgs e)
        {
            EnsureWordBankSeeded();
            StartNextTurn();
        }

        private void EnsureWordBankSeeded()
        {
            if (wordBank == null || wordBank.Length == 0)
            {
                wordBank = new string[] {
                    "CAT", "DOG", "HOUSE", "TREE", "CAR", "BOOK", "APPLE", "STAR",
                    "FISH", "BIRD", "FLOWER", "SUN", "MOON", "BANANA", "GUITAR",
                    "PHONE", "COMPUTER", "CHAIR", "TABLE", "WINDOW", "DOOR", "LAMP",
                    "BOTTLE", "CUP", "PLATE", "FORK", "KNIFE", "SPOON", "BOWL"
                };
            }
        }

        // Handlers và ti?n ích còn thi?u (khôi ph?c)
        private void GameView_LeaveRequested(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "B?n có ch?c ch?n mu?n r?i phòng không?",
                "R?i phòng",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ShowWaitingRoom();
            }
        }

        private void GameView_BackLobbyRequested(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Quay l?i s?nh ch?? L??t ch?i hi?n t?i s? k?t thúc.",
                "V? Lobby",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ShowLobbyInterface();
            }
        }

        private void GameView_AnswerSubmitted(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer)) return;
            var data = new NetworkData(Command.Guess, currentUser, answer);
            gameClient?.SendDataAsync(data);
        }

        private void GameView_ChatSubmitted(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var data = new NetworkData(Command.Message, currentUser, message);
            gameClient?.SendDataAsync(data);
        }

        private void GameView_MessageSubmitted(string message)
        {
            try
            {
                GameView_AnswerSubmitted(message);
            }
            catch (Exception ex)
            {
                gameView?.AddChat($"L?i g?i tin nh?n: {ex.Message}");
            }
        }

        private void UpdateLobbyInterface()
        {
            try
            {
                if (listViewLobbyPlayers != null)
                {
                    listViewLobbyPlayers.Items.Clear();
                    for (int i = 0; i < connectedPlayers.Count; i++)
                    {
                        var player = connectedPlayers[i];
                        var item = new ListViewItem((i + 1).ToString());
                        item.SubItems.Add(player.Name);
                        item.SubItems.Add(player.Name == currentUser ? "?? Host" : "?? Player");
                        item.SubItems.Add(player.IsOnline ? "Online" : "Offline");

                        if (player.Name == currentUser)
                        {
                            item.BackColor = Color.FromArgb(220, 252, 231);
                            item.ForeColor = Color.FromArgb(22, 101, 52);
                        }
                        listViewLobbyPlayers.Items.Add(item);
                    }
                }

                if (lblLobbyStatus != null)
                {
                    if (connectedPlayers.Count >= 2)
                    {
                        lblLobbyStatus.Text = "?? ng??i ch?i - Có th? b?t ??u!";
                        lblLobbyStatus.ForeColor = Color.FromArgb(40, 167, 69);
                        if (btnStartGameLobby != null) btnStartGameLobby.Enabled = true;
                    }
                    else
                    {
                        lblLobbyStatus.Text = "C?n thêm ng??i ch?i ?? b?t ??u...";
                        lblLobbyStatus.ForeColor = Color.FromArgb(255, 140, 0);
                        if (btnStartGameLobby != null) btnStartGameLobby.Enabled = false;
                    }
                }

                if (listBoxLobbyChat != null && listBoxLobbyChat.Items.Count == 0)
                {
                    AddLobbyMessage("System", $"Chào m?ng ??n lobby phòng {currentRoomCode}!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLobbyInterface Exception: {ex}");
            }
        }

        private void WelcomePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new LinearGradientBrush(welcomePanel.ClientRectangle,
                Color.FromArgb(240, 248, 255), Color.FromArgb(208, 232, 255), 90F))
            {
                e.Graphics.FillRectangle(brush, welcomePanel.ClientRectangle);
            }

            foreach (var particle in particles)
            {
                if (particle.Life > 0)
                {
                    using (var b = new SolidBrush(Color.FromArgb((int)(particle.Life * 255), particle.Color)))
                    {
                        e.Graphics.FillEllipse(b, particle.X, particle.Y, particle.Size, particle.Size);
                    }
                }
            }
        }

        private void LobbyPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new LinearGradientBrush(lobbyPanel.ClientRectangle,
                Color.FromArgb(240, 248, 255), Color.FromArgb(220, 240, 255), 90F))
            {
                e.Graphics.FillRectangle(brush, lobbyPanel.ClientRectangle);
            }
        }

        #endregion

        private void StartWordSelection()
        {
            // Chuy?n qua c? ch? ?i?u ph?i l??t
            GameView_StartGameRequested(this, EventArgs.Empty);
        }
    }
}