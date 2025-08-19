using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    // Lightweight, self-contained game UI similar to the screenshot
    public class GameViewControl : UserControl
    {
        // Public events to integrate with Form1
        public event EventHandler StartGameRequested;
        public event EventHandler LeaveRequested;
        public event EventHandler BackLobbyRequested;
        public event Action<string> MessageSubmitted;

        // Left: Drawing
        private Panel panelTopBar;
        private Label lblHeader;
        private Panel panelDrawing;
        private Panel panelTools;
        private Button btnPencil;
        private Button btnEraser;
        private Button btnClear;
        private Panel panelSelectedColor;
        private Label lblColor;
        private TrackBar trackBrush;
        private Label lblBrush;
        private Button btnCBlack, btnCWhite, btnCRed, btnCBlue, btnCGreen, btnCYellow;

        // Middle/right: Leaderboard & Chat
        private GroupBox groupLeaderboard;
        private ListView lvLeaderboard;
        private ColumnHeader colRank, colPlayer, colScore, colStatus;

        private GroupBox groupChat;
        private ListBox lbChat;
        private TextBox txtChat;
        private Button btnSend;

        // Right sidebar
        private Panel panelSidebar;
        private PictureBox avatarBox;
        private Label lblYou, lblPlayerName, lblScoreTitle, lblScoreValue;
        private GroupBox groupRoundInfo;
        private Label lblWordTitle, lblWord, lblTimeTitle, lblTime, lblRoundTitle, lblRound;
        private ProgressBar progressTime;
        private GroupBox groupRoomInfo;
        private Label lblRoomCodeTitle, lblRoomCode, lblPlayersOnlineTitle, lblPlayersOnline;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameViewControl
            // 
            this.Name = "GameViewControl";
            this.ResumeLayout(false);

        }

        private Button btnStartGame, btnLeave, btnBackLobby;

        // Drawing state
        private Bitmap _canvas;
        private Graphics _g;
        private bool _drawing;
        private Point _last;
        private Color _currentColor = Color.Black;
        private int _brushSize = 4;
        private bool _eraser;

        public string RoomCode
        {
            get => lblRoomCode?.Text; 
            set { if (lblRoomCode != null) lblRoomCode.Text = value; }
        }

        public string PlayerName
        {
            get => lblPlayerName?.Text; 
            set { if (lblPlayerName != null) lblPlayerName.Text = value; }
        }

        public int PlayersOnline
        {
            set { if (lblPlayersOnline != null) lblPlayersOnline.Text = value.ToString(); }
        }

        public GameViewControl()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(245, 247, 250);
            BuildLayout();
            InitCanvas();

            // Responsive layout
            this.Resize += (s, e) => PerformResponsiveLayout();
            PerformResponsiveLayout();
        }

        private void BuildLayout()
        {
            SuspendLayout();

            // Top bar
            panelTopBar = new Panel { Height = 40, Dock = DockStyle.Top };
            panelTopBar.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(panelTopBar.ClientRectangle,
                    Color.FromArgb(67, 82, 161), Color.FromArgb(45, 125, 245), LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, panelTopBar.ClientRectangle);
                }
            };
            lblHeader = new Label
            {
                Text = "?? Multiplayer Game Room",
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            panelTopBar.Controls.Add(lblHeader);
            Controls.Add(panelTopBar);

            // Left drawing panel
            panelDrawing = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            panelDrawing.Paint += (s, e) => { if (_canvas != null) e.Graphics.DrawImage(_canvas, 0, 0); };
            panelDrawing.MouseDown += (s, e) => { _drawing = true; _last = e.Location; };
            panelDrawing.MouseMove += (s, e) =>
            {
                if (!_drawing || _g == null) return;
                using (var pen = new Pen(_eraser ? Color.White : _currentColor, _brushSize))
                { pen.StartCap = LineCap.Round; pen.EndCap = LineCap.Round; _g.DrawLine(pen, _last, e.Location); }
                _last = e.Location; panelDrawing.Invalidate();
            };
            panelDrawing.MouseUp += (s, e) => { _drawing = false; };
            panelDrawing.Resize += (s, e) => ResizeCanvasToPanel();
            Controls.Add(panelDrawing);

            // Tools panel
            panelTools = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(250, 252, 255)
            };
            btnPencil = new Button { Text = "?? Pencil", Size = new Size(80, 30) };
            btnEraser = new Button { Text = "?? Eraser", Size = new Size(80, 28) };
            btnPencil.Click += (s, e) => { _eraser = false; btnPencil.BackColor = Color.LightSkyBlue; btnEraser.BackColor = SystemColors.Control; };
            btnEraser.Click += (s, e) => { _eraser = true; btnEraser.BackColor = Color.LightPink; btnPencil.BackColor = SystemColors.Control; };

            btnCBlack = MakeColorButton(Color.Black, Point.Empty);
            btnCWhite = MakeColorButton(Color.White, Point.Empty);
            btnCRed   = MakeColorButton(Color.Red,   Point.Empty);
            btnCBlue  = MakeColorButton(Color.RoyalBlue, Point.Empty);
            btnCGreen = MakeColorButton(Color.ForestGreen, Point.Empty);
            btnCYellow= MakeColorButton(Color.Gold,  Point.Empty);

            lblColor = new Label { Text = "?? Color:", AutoSize = true };
            panelSelectedColor = new Panel { BackColor = _currentColor, Size = new Size(24, 24), BorderStyle = BorderStyle.FixedSingle };

            lblBrush = new Label { Text = "??? Brush Size:", AutoSize = true };
            trackBrush = new TrackBar { Minimum = 1, Maximum = 30, Value = _brushSize, TickStyle = TickStyle.None, Width = 180 };
            trackBrush.ValueChanged += (s, e) => _brushSize = trackBrush.Value;

            btnClear = new Button { Text = "??? Clear", Size = new Size(70, 36), BackColor = Color.IndianRed, ForeColor = Color.White };
            btnClear.Click += (s, e) => { if (_g != null) { _g.Clear(Color.White); panelDrawing.Invalidate(); } };

            panelTools.Controls.AddRange(new Control[] {
                btnPencil, btnEraser, btnCBlack, btnCWhite, btnCRed, btnCBlue, btnCGreen, btnCYellow,
                lblColor, panelSelectedColor, lblBrush, trackBrush, btnClear
            });
            Controls.Add(panelTools);

            // Leaderboard
            groupLeaderboard = new GroupBox
            {
                Text = "?? Leaderboard",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            lvLeaderboard = new ListView
            {
                View = View.Details, FullRowSelect = true, GridLines = true
            };
            colRank = new ColumnHeader { Text = "#", Width = 30 };
            colPlayer = new ColumnHeader { Text = "Player", Width = 150 };
            colScore = new ColumnHeader { Text = "Score", Width = 70 };
            colStatus = new ColumnHeader { Text = "Status", Width = 80 };
            lvLeaderboard.Columns.AddRange(new[] { colRank, colPlayer, colScore, colStatus });
            groupLeaderboard.Controls.Add(lvLeaderboard);
            Controls.Add(groupLeaderboard);

            // Chat
            groupChat = new GroupBox
            {
                Text = "?? Chat & Guess",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            lbChat = new ListBox();
            txtChat = new TextBox();
            txtChat.KeyPress += TxtChat_KeyPress; // Add Enter key support
            btnSend = new Button { Text = "Send", BackColor = Color.SeaGreen, ForeColor = Color.White };
            btnSend.Click += (s, e) =>
            {
                var text = txtChat.Text.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    MessageSubmitted?.Invoke(text);
                    txtChat.Clear();
                }
            };
            groupChat.Controls.AddRange(new Control[] { lbChat, txtChat, btnSend });
            Controls.Add(groupChat);

            // Right sidebar
            panelSidebar = new Panel { BackColor = Color.Transparent };
            avatarBox = new PictureBox { Size = new Size(70, 70), BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };
            lblYou = new Label { Text = "You", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            lblPlayerName = new Label { Text = "Player1", AutoSize = true, ForeColor = Color.RoyalBlue };
            lblScoreTitle = new Label { Text = "Score:", AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            lblScoreValue = new Label { Text = "0", AutoSize = true, ForeColor = Color.SeaGreen };

            groupRoundInfo = new GroupBox { Text = "?? Word to draw:" };
            lblWordTitle = new Label { Text = "Word:", AutoSize = true };
            lblWord = new Label { Text = "- - - - -", AutoSize = true, ForeColor = Color.IndianRed };
            lblTimeTitle = new Label { Text = "Time Left:", AutoSize = true };
            lblTime = new Label { Text = "00:60", AutoSize = true, ForeColor = Color.IndianRed, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            lblRoundTitle = new Label { Text = "Round:", AutoSize = true };
            lblRound = new Label { Text = "1/5", AutoSize = true };
            progressTime = new ProgressBar { Value = 60, Maximum = 60 };
            groupRoundInfo.Controls.AddRange(new Control[] { lblWordTitle, lblWord, lblTimeTitle, lblTime, lblRoundTitle, lblRound, progressTime });

            groupRoomInfo = new GroupBox { Text = "Room Info" };
            lblRoomCodeTitle = new Label { Text = "Room Code:", AutoSize = true };
            lblRoomCode = new Label { Text = "ABC123", AutoSize = true, ForeColor = Color.RoyalBlue, Cursor = Cursors.Hand };
            lblRoomCode.Click += (s, e) => { try { Clipboard.SetText(lblRoomCode.Text); } catch { } };
            lblPlayersOnlineTitle = new Label { Text = "Players Online:", AutoSize = true };
            lblPlayersOnline = new Label { Text = "3/8", AutoSize = true };
            groupRoomInfo.Controls.AddRange(new Control[] { lblRoomCodeTitle, lblRoomCode, lblPlayersOnlineTitle, lblPlayersOnline });

            btnStartGame = new Button { Text = "?? Start Game", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLeave = new Button { Text = "?? Leave", BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBackLobby = new Button { Text = "?? Lobby", BackColor = Color.Goldenrod, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnStartGame.Click += (s, e) => StartGameRequested?.Invoke(this, EventArgs.Empty);
            btnLeave.Click += (s, e) => LeaveRequested?.Invoke(this, EventArgs.Empty);
            btnBackLobby.Click += (s, e) => BackLobbyRequested?.Invoke(this, EventArgs.Empty);

            panelSidebar.Controls.AddRange(new Control[] {
                avatarBox, lblYou, lblPlayerName, lblScoreTitle, lblScoreValue,
                groupRoundInfo, groupRoomInfo, btnStartGame, btnLeave, btnBackLobby
            });
            Controls.Add(panelSidebar);

            ResumeLayout(false);
        }

        private Button MakeColorButton(Color color, Point location)
        {
            var btn = new Button { BackColor = color, Location = location, Size = new Size(32, 24) };
            btn.FlatStyle = FlatStyle.Flat; btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => { _currentColor = color; panelSelectedColor.BackColor = color; _eraser = false; };
            return btn;
        }

        private void InitCanvas()
        {
            // Initialize with a reasonable default; will resize with panel
            _canvas = new Bitmap(Math.Max(10, panelDrawing.Width), Math.Max(10, panelDrawing.Height));
            _g = Graphics.FromImage(_canvas);
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            _g.Clear(Color.White);
        }

        private void ResizeCanvasToPanel()
        {
            if (panelDrawing.Width <= 0 || panelDrawing.Height <= 0) return;

            var newBmp = new Bitmap(panelDrawing.Width, panelDrawing.Height);
            using (var g = Graphics.FromImage(newBmp))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                if (_canvas != null)
                {
                    // draw existing content scaled to new size (simple fit)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(_canvas, new Rectangle(0, 0, newBmp.Width, newBmp.Height));
                }
            }

            _g?.Dispose();
            _canvas?.Dispose();
            _canvas = newBmp;
            _g = Graphics.FromImage(_canvas);
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            panelDrawing.Invalidate();
        }

        private void PerformResponsiveLayout()
        {
            // Layout parameters
            int margin = 16;
            int topBarH = panelTopBar?.Height ?? 40;
            int gap = 12;
            int rightWidth = 240; // sidebar fixed width
            int midWidth = 300;   // leaderboard/chat column width

            // Available area under top bar
            int availW = Math.Max(400, this.ClientSize.Width - margin * 2);
            int availH = Math.Max(300, this.ClientSize.Height - topBarH - margin);

            // Compute left width to fit all columns within parent
            int leftWidth = availW - (rightWidth + midWidth + gap * 2);
            if (leftWidth < 560) // minimal left column width
            {
                // If too small, reduce midWidth proportionally but keep minimums
                int deficit = 560 - leftWidth;
                int reduceMid = Math.Min(deficit, midWidth - 240);
                midWidth -= reduceMid;
                leftWidth = availW - (rightWidth + midWidth + gap * 2);
            }
            if (leftWidth < 560) leftWidth = 560;

            // Column X positions
            int xLeft = margin;
            int xMid = xLeft + leftWidth + gap;
            int xRight = xMid + midWidth + gap;

            // Left column: drawing + tools
            int toolsH = 72;
            int drawH = availH - toolsH - gap;
            panelDrawing.Location = new Point(xLeft, topBarH + gap);
            panelDrawing.Size = new Size(leftWidth, drawH);

            panelTools.Location = new Point(xLeft, panelDrawing.Bottom + gap);
            panelTools.Size = new Size(leftWidth, toolsH);

            // Arrange tools content horizontally with padding
            int tx = 8; int ty = 8; int tgap = 8;
            btnPencil.Location = new Point(tx, ty); btnPencil.Size = new Size(80, 30); tx += 80 + tgap;
            btnEraser.Location = new Point(tx, ty + 32); btnEraser.Size = new Size(80, 28); tx = 8 + 2 * (80 + tgap);
            btnCBlack.Location = new Point(tx, ty); tx += 36; btnCWhite.Location = new Point(tx, ty); tx += 36;
            btnCRed.Location = new Point(tx, ty); tx += 36; btnCBlue.Location = new Point(tx, ty); tx += 36;
            btnCGreen.Location = new Point(tx, ty); tx += 36; btnCYellow.Location = new Point(tx, ty); tx += 44;
            lblColor.Location = new Point(tx, ty + 4); tx += lblColor.Width + 6;
            panelSelectedColor.Location = new Point(tx, ty); tx += panelSelectedColor.Width + 24;
            lblBrush.Location = new Point(tx, ty + 4); tx += lblBrush.Width + 6;
            trackBrush.Location = new Point(tx, ty);
            btnClear.Location = new Point(panelTools.Width - btnClear.Width - 8, panelTools.Height - btnClear.Height - 8);

            // Middle column: leaderboard (top) and chat (bottom)
            groupLeaderboard.Location = new Point(xMid, topBarH + gap);
            int midColH = availH;
            int leaderH = (int)(midColH * 0.48);
            int chatH = midColH - leaderH - gap;
            groupLeaderboard.Size = new Size(midWidth, leaderH);

            // Layout leaderboard content with padding
            int pad = 10;
            lvLeaderboard.Location = new Point(pad, 22 + pad);
            lvLeaderboard.Size = new Size(groupLeaderboard.Width - pad * 2, groupLeaderboard.Height - (22 + pad * 2));

            groupChat.Location = new Point(xMid, groupLeaderboard.Bottom + gap);
            groupChat.Size = new Size(midWidth, chatH);

            // Layout chat content
            lbChat.Location = new Point(pad, 22 + pad);
            lbChat.Size = new Size(groupChat.Width - pad * 2, groupChat.Height - (22 + pad * 3) - 30);
            txtChat.Size = new Size(groupChat.Width - pad * 3 - 70, 24);
            txtChat.Location = new Point(pad, groupChat.Bottom - pad - 26);
            btnSend.Size = new Size(64, 26);
            btnSend.Location = new Point(groupChat.Right - pad - btnSend.Width, txtChat.Top);

            // Right sidebar column
            int sidebarH = availH;
            panelSidebar.Location = new Point(xRight, topBarH + gap);
            panelSidebar.Size = new Size(rightWidth, sidebarH);

            // Sidebar content layout
            int sx = 10, sy = 10;
            avatarBox.Location = new Point(sx, sy);
            lblYou.Location = new Point(sx + avatarBox.Width + 10, sy);
            lblPlayerName.Location = new Point(lblYou.Left, sy + 20);
            lblScoreTitle.Location = new Point(lblYou.Left, sy + 44);
            lblScoreValue.Location = new Point(lblScoreTitle.Right + 6, sy + 44);

            groupRoundInfo.Location = new Point(sx, avatarBox.Bottom + 15);
            groupRoundInfo.Size = new Size(rightWidth - sx * 2, 190);
            // round info inner
            int gx = 10, gy = 22 + 10;
            lblWordTitle.Location = new Point(gx, gy);
            lblWord.Location = new Point(gx + 60, gy);
            lblTimeTitle.Location = new Point(gx, gy + 28);
            lblTime.Location = new Point(gx + 80, gy + 28);
            lblRoundTitle.Location = new Point(gx, gy + 56);
            lblRound.Location = new Point(gx + 80, gy + 56);
            progressTime.Location = new Point(gx, gy + 86);
            progressTime.Size = new Size(groupRoundInfo.Width - gx * 2, 18);

            groupRoomInfo.Location = new Point(sx, groupRoundInfo.Bottom + 10);
            groupRoomInfo.Size = new Size(rightWidth - sx * 2, 110);
            // room info inner
            lblRoomCodeTitle.Location = new Point(gx, 22 + 10);
            lblRoomCode.Location = new Point(gx + 90, 22 + 10);
            lblPlayersOnlineTitle.Location = new Point(gx, 22 + 40);
            lblPlayersOnline.Location = new Point(gx + 110, 22 + 40);

            // Buttons at bottom of sidebar
            int btnW = (rightWidth - sx * 2 - 6) / 2;
            btnStartGame.Size = new Size(rightWidth - sx * 2, 38);
            btnStartGame.Location = new Point(sx, groupRoomInfo.Bottom + 10);
            btnLeave.Size = new Size(btnW, 34);
            btnBackLobby.Size = new Size(btnW, 34);
            btnLeave.Location = new Point(sx, btnStartGame.Bottom + 8);
            btnBackLobby.Location = new Point(btnLeave.Right + 6, btnStartGame.Bottom + 8);
        }

        private void TxtChat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                var text = txtChat.Text.Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    MessageSubmitted?.Invoke(text);
                    txtChat.Clear();
                }
                e.Handled = true;
            }
        }

        public void AddChat(string text)
        {
            lbChat.Items.Add(text);
            if (lbChat.Items.Count > 0)
                lbChat.TopIndex = lbChat.Items.Count - 1;
            
            // Limit chat history
            if (lbChat.Items.Count > 100)
            {
                lbChat.Items.RemoveAt(0);
            }
        }

        public void SetRoundInfo(string word, int seconds, int round, int maxRound)
        {
            lblWord.Text = word;
            lblTime.Text = $"00:{seconds:00}";
            progressTime.Maximum = seconds; progressTime.Value = Math.Min(seconds, progressTime.Maximum);
            lblRound.Text = $"{round}/{maxRound}";
        }

        public void UpdateTime(int seconds)
        {
            if (seconds < 0) seconds = 0;
            if (progressTime.Maximum < seconds) progressTime.Maximum = seconds;
            progressTime.Value = Math.Min(seconds, progressTime.Maximum);
            lblTime.Text = $"00:{seconds:00}";
        }

        public void SetLeaderboard((int rank, string player, int score, string status)[] entries)
        {
            lvLeaderboard.Items.Clear();
            foreach (var e in entries)
            {
                var item = new ListViewItem(e.rank.ToString());
                item.SubItems.Add(e.player);
                item.SubItems.Add(e.score.ToString());
                item.SubItems.Add(e.status);
                lvLeaderboard.Items.Add(item);
            }
        }
    }
}
