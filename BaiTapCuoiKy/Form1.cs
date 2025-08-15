using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    public partial class Form1 : Form
    {
        // Drawing variables
        private bool isDrawing = false;
        private Point lastPoint;
        private Graphics drawingGraphics;
        private Bitmap drawingBitmap;
        private Color currentColor = Color.Black;
        private int brushSize = 3;
        private bool isErasing = false;
        
        // Game variables
        private string[] words = { "CAT", "DOG", "HOUSE", "CAR", "TREE", "FLOWER", "SUN", "MOON", "STAR", "BIRD", "FISH", "BOOK", "APPLE", "BANANA" };
        private string currentWord = "";
        private string wordToGuess = "";
        private int timeLeft = 60;
        private int currentRound = 1;
        private int totalRounds = 5;
        private int playerScore = 0;
        private bool isGameActive = false;
        private bool isPlayerDrawing = false;
        private string playerName = "";
        private string roomCode = "";
        
        // Players list
        private List<string> players = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeDrawing();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGame();
        }

        private void InitializeDrawing()
        {
            // Initialize drawing canvas
            drawingBitmap = new Bitmap(panelDrawing.Width, panelDrawing.Height);
            drawingGraphics = Graphics.FromImage(drawingBitmap);
            drawingGraphics.Clear(Color.White);
            panelDrawing.BackgroundImage = drawingBitmap;
            
            // Set initial tool state
            btnPencil.BackColor = Color.Blue;
            panelSelectedColor.BackColor = currentColor;
        }

        private void InitializeGame()
        {
            // Set initial UI state
            lblWordToGuess.Text = "_ _ _ _ _ _ _ _";
            lblTimeLeft.Text = "01:00";
            lblCurrentRound.Text = $"{currentRound}/{totalRounds}";
            lblPlayerScore.Text = "0";
            progressBarTime.Value = 60;
            
            // Disable game controls initially
            btnStartGame.Enabled = false;
            btnLeaveGame.Enabled = false;
            panelDrawing.Enabled = false;
            
            toolStripStatusLabel.Text = "Nh?p t�n v� t?o ph�ng ho?c tham gia game";
            lblGameStatus.Text = "Ch? ng??i ch?i tham gia...";
            
            // Initialize sample players for demo
            AddPlayer("Player1");
            AddChatMessage("System", "Ch�o m?ng ??n v?i Drawing Guess Game!");
        }

        #region Drawing Events

        private void panelDrawing_MouseDown(object sender, MouseEventArgs e)
        {
            if (isGameActive && isPlayerDrawing && e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
            }
        }

        private void panelDrawing_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && isGameActive && isPlayerDrawing)
            {
                using (Graphics g = Graphics.FromImage(drawingBitmap))
                {
                    Pen pen = new Pen(isErasing ? Color.White : currentColor, brushSize);
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    
                    g.DrawLine(pen, lastPoint, e.Location);
                    pen.Dispose();
                }
                
                lastPoint = e.Location;
                panelDrawing.Invalidate();
            }
        }

        private void panelDrawing_MouseUp(object sender, MouseEventArgs e)
        {
            isDrawing = false;
        }

        private void panelDrawing_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(drawingBitmap, 0, 0);
        }

        #endregion

        #region Tool Events

        private void btnPencil_Click(object sender, EventArgs e)
        {
            isErasing = false;
            btnPencil.BackColor = Color.Blue;
            btnEraser.BackColor = SystemColors.Control;
            panelDrawing.Cursor = Cursors.Cross;
        }

        private void btnEraser_Click(object sender, EventArgs e)
        {
            isErasing = true;
            btnEraser.BackColor = Color.Pink;
            btnPencil.BackColor = SystemColors.Control;
            panelDrawing.Cursor = Cursors.Cross;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Button colorBtn = sender as Button;
            currentColor = colorBtn.BackColor;
            panelSelectedColor.BackColor = currentColor;
            isErasing = false;
            btnPencil.BackColor = Color.Blue;
            btnEraser.BackColor = SystemColors.Control;
        }

        private void trackBarBrushSize_ValueChanged(object sender, EventArgs e)
        {
            brushSize = trackBarBrushSize.Value;
        }

        private void btnClearCanvas_Click(object sender, EventArgs e)
        {
            if (isGameActive && isPlayerDrawing)
            {
                drawingGraphics.Clear(Color.White);
                panelDrawing.Invalidate();
                AddChatMessage("System", $"{playerName} ?� x�a b?ng v?");
            }
        }

        #endregion

        #region Chat Events

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            SendChatMessage();
        }

        private void txtChatInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SendChatMessage();
                e.Handled = true;
            }
        }

        private void SendChatMessage()
        {
            string message = txtChatInput.Text.Trim();
            if (!string.IsNullOrEmpty(message) && !string.IsNullOrEmpty(playerName))
            {
                // Check if it's a guess
                if (isGameActive && !isPlayerDrawing && message.ToUpper() == currentWord.ToUpper())
                {
                    // Correct guess!
                    AddChatMessage("System", $"?? {playerName} ?� ?o�n ?�ng: {currentWord}!");
                    playerScore += CalculateScore();
                    lblPlayerScore.Text = playerScore.ToString();
                    NextRound();
                }
                else
                {
                    AddChatMessage(playerName, message);
                }
                
                txtChatInput.Clear();
            }
        }

        private void AddChatMessage(string sender, string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm");
            listBoxChat.Items.Add($"[{timestamp}] {sender}: {message}");
            
            // Auto-scroll to bottom
            listBoxChat.TopIndex = Math.Max(0, listBoxChat.Items.Count - listBoxChat.Height / listBoxChat.ItemHeight);
        }

        #endregion

        #region Game Control Events

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlayerName.Text.Trim()))
            {
                MessageBox.Show("Vui l�ng nh?p t�n ng??i ch?i!", "Th�ng b�o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            playerName = txtPlayerName.Text.Trim();
            roomCode = GenerateRoomCode();
            txtRoomCode.Text = roomCode;
            
            AddPlayer(playerName);
            btnStartGame.Enabled = true;
            btnLeaveGame.Enabled = true;
            btnCreateRoom.Enabled = false;
            btnJoinGame.Enabled = false;
            
            toolStripStatusLabel.Text = $"Ph�ng {roomCode} ?� ???c t?o";
            lblGameStatus.Text = "Ch? ng??i ch?i kh�c tham gia...";
            AddChatMessage("System", $"Ph�ng {roomCode} ?� ???c t?o b?i {playerName}");
        }

        private void btnJoinGame_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPlayerName.Text.Trim()))
            {
                MessageBox.Show("Vui l�ng nh?p t�n ng??i ch?i!", "Th�ng b�o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtRoomCode.Text.Trim()))
            {
                MessageBox.Show("Vui l�ng nh?p m� ph�ng!", "Th�ng b�o", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            playerName = txtPlayerName.Text.Trim();
            roomCode = txtRoomCode.Text.Trim();
            
            AddPlayer(playerName);
            btnStartGame.Enabled = true;
            btnLeaveGame.Enabled = true;
            btnCreateRoom.Enabled = false;
            btnJoinGame.Enabled = false;
            
            toolStripStatusLabel.Text = $"?� tham gia ph�ng {roomCode}";
            lblGameStatus.Text = "Ch? host b?t ??u game...";
            AddChatMessage("System", $"{playerName} ?� tham gia ph�ng");
        }

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void btnLeaveGame_Click(object sender, EventArgs e)
        {
            LeaveGame();
        }

        #endregion

        #region Game Logic

        private void StartNewGame()
        {
            isGameActive = true;
            currentRound = 1;
            playerScore = 0;
            timeLeft = 60;
            
            lblCurrentRound.Text = $"{currentRound}/{totalRounds}";
            lblPlayerScore.Text = "0";
            
            btnStartGame.Enabled = false;
            panelDrawing.Enabled = true;
            
            StartNewRound();
            
            toolStripStatusLabel.Text = "Game ?ang di?n ra";
            AddChatMessage("System", "?? Game b?t ??u! Ch�c c�c b?n ch?i vui v?!");
        }

        private void StartNewRound()
        {
            // Select random word
            Random rand = new Random();
            currentWord = words[rand.Next(words.Length)];
            
            // Create word display with blanks
            wordToGuess = "";
            for (int i = 0; i < currentWord.Length; i++)
            {
                wordToGuess += "_ ";
            }
            lblWordToGuess.Text = wordToGuess.Trim();
            
            // Set drawing player (for demo, always current player draws)
            isPlayerDrawing = true;
            lblCurrentWord.Text = $"T? c?n v?: {currentWord}";
            
            // Reset timer
            timeLeft = 60;
            progressBarTime.Value = timeLeft;
            timerGame.Start();
            
            // Clear canvas
            drawingGraphics.Clear(Color.White);
            panelDrawing.Invalidate();
            
            lblGameStatus.Text = isPlayerDrawing ? "L??t c?a b?n - H�y v?!" : "H�y ?o�n t? kh�a!";
            AddChatMessage("System", $"Round {currentRound}: {(isPlayerDrawing ? "B?n ?ang v?" : "H�y ?o�n t? kh�a!")}");
        }

        private void NextRound()
        {
            timerGame.Stop();
            
            if (currentRound < totalRounds)
            {
                currentRound++;
                lblCurrentRound.Text = $"{currentRound}/{totalRounds}";
                
                // Wait a moment then start next round
                Timer delayTimer = new Timer();
                delayTimer.Interval = 3000; // 3 seconds
                delayTimer.Tick += (s, e) => {
                    delayTimer.Stop();
                    delayTimer.Dispose();
                    StartNewRound();
                };
                delayTimer.Start();
            }
            else
            {
                EndGame();
            }
        }

        private void EndGame()
        {
            isGameActive = false;
            timerGame.Stop();
            panelDrawing.Enabled = false;
            
            lblGameStatus.Text = "Game k?t th�c!";
            toolStripStatusLabel.Text = "Game ?� k?t th�c";
            
            string finalMessage = $"?? Game k?t th�c!\n?i?m s? cu?i c�ng: {playerScore}\nC?m ?n b?n ?� ch?i!";
            AddChatMessage("System", finalMessage);
            
            MessageBox.Show(finalMessage, "Game K?t Th�c", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            // Reset for new game
            btnStartGame.Enabled = true;
        }

        private void LeaveGame()
        {
            isGameActive = false;
            timerGame.Stop();
            
            // Reset UI
            btnCreateRoom.Enabled = true;
            btnJoinGame.Enabled = true;
            btnStartGame.Enabled = false;
            btnLeaveGame.Enabled = false;
            panelDrawing.Enabled = false;
            
            // Clear players list
            listBoxPlayers.Items.Clear();
            players.Clear();
            
            // Clear canvas
            drawingGraphics.Clear(Color.White);
            panelDrawing.Invalidate();
            
            toolStripStatusLabel.Text = "?� r?i kh?i game";
            lblGameStatus.Text = "Ch? tham gia game m?i...";
            AddChatMessage("System", $"{playerName} ?� r?i kh?i ph�ng");
            
            playerName = "";
            roomCode = "";
            txtRoomCode.Clear();
        }

        #endregion

        #region Timer Event

        private void timerGame_Tick(object sender, EventArgs e)
        {
            if (isGameActive)
            {
                timeLeft--;
                lblTimeLeft.Text = $"00:{timeLeft:00}";
                progressBarTime.Value = Math.Max(0, timeLeft);
                
                if (timeLeft <= 0)
                {
                    // Time's up!
                    AddChatMessage("System", $"? H?t gi?! T? c?n ?o�n l�: {currentWord}");
                    NextRound();
                }
            }
        }

        #endregion

        #region Helper Methods

        private string GenerateRoomCode()
        {
            Random rand = new Random();
            return rand.Next(1000, 9999).ToString();
        }

        private void AddPlayer(string name)
        {
            if (!players.Contains(name))
            {
                players.Add(name);
                listBoxPlayers.Items.Add($"?? {name}");
            }
        }

        private int CalculateScore()
        {
            // Score based on time left (more points for faster guesses)
            return Math.Max(10, timeLeft * 2);
        }

        #endregion
    }
}
