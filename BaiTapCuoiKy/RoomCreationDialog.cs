using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    public partial class RoomCreationDialog : Form
    {
        public string RoomCode { get; private set; }
        public Form1.GameSettings GameSettings { get; private set; }
        
        private readonly string playerName;
        private readonly Random random = new Random();
        
        // UI Components
        private TextBox txtRoomName;
        private ComboBox cbMaxPlayers;
        private ComboBox cbRounds;
        private ComboBox cbTimePerRound;
        private ComboBox cbDifficulty;
        private Label lblRoomCode;
        private Button btnCreate;
        private Button btnCancel;
        
        public RoomCreationDialog(string playerName)
        {
            this.playerName = playerName;
            InitializeComponents();
            GenerateRoomCode();
        }
        
        private void InitializeComponents()
        {
            // Form settings
            this.Text = "?? T?o Phòng Game M?i";
            this.Size = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);
            
            CreateHeaderPanel();
            CreateRoomSettingsPanel();
            CreateGameSettingsPanel();
            CreateButtonsPanel();
        }
        
        private void CreateHeaderPanel()
        {
            var headerPanel = new Panel
            {
                Size = new Size(500, 60),
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(67, 82, 161)
            };
            
            headerPanel.Paint += (s, e) => {
                using (var brush = new LinearGradientBrush(
                    headerPanel.ClientRectangle,
                    Color.FromArgb(67, 82, 161),
                    Color.FromArgb(45, 125, 245),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, headerPanel.ClientRectangle);
                }
            };
            
            var titleLabel = new Label
            {
                Text = "?? T?O PHÒNG GAME M?I",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(460, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            
            headerPanel.Controls.Add(titleLabel);
            this.Controls.Add(headerPanel);
        }
        
        private void CreateRoomSettingsPanel()
        {
            var roomPanel = new Panel
            {
                Size = new Size(220, 120),
                Location = new Point(20, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            roomPanel.Paint += DrawPanelBorder;
            
            var roomTitle = new Label
            {
                Text = "?? THÔNG TIN PHÒNG",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(67, 82, 161),
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            
            var lblRoomNameTitle = new Label
            {
                Text = "Tên phòng:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 40),
                Size = new Size(80, 20),
                BackColor = Color.Transparent
            };
            
            txtRoomName = new TextBox
            {
                Text = $"{playerName}'s Room",
                Font = new Font("Segoe UI", 9),
                Location = new Point(15, 60),
                Size = new Size(180, 23),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var lblCodeTitle = new Label
            {
                Text = "Mã phòng:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 85),
                Size = new Size(70, 20),
                BackColor = Color.Transparent
            };
            
            lblRoomCode = new Label
            {
                Text = "ABC123",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 125, 245),
                Location = new Point(90, 83),
                Size = new Size(100, 25),
                BackColor = Color.Transparent
            };
            
            roomPanel.Controls.AddRange(new Control[] {
                roomTitle, lblRoomNameTitle, txtRoomName, lblCodeTitle, lblRoomCode
            });
            
            this.Controls.Add(roomPanel);
        }
        
        private void CreateGameSettingsPanel()
        {
            var gamePanel = new Panel
            {
                Size = new Size(220, 200),
                Location = new Point(260, 80),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            gamePanel.Paint += DrawPanelBorder;
            
            var gameTitle = new Label
            {
                Text = "?? CÀI ??T GAME",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(67, 82, 161),
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            
            // Max Players
            var lblMaxPlayers = new Label
            {
                Text = "S? ng??i ch?i:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 40),
                Size = new Size(100, 20),
                BackColor = Color.Transparent
            };
            
            cbMaxPlayers = new ComboBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(120, 38),
                Size = new Size(70, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbMaxPlayers.Items.AddRange(new object[] { "4", "5", "6", "7", "8" });
            cbMaxPlayers.SelectedItem = "7";
            
            // Rounds
            var lblRounds = new Label
            {
                Text = "S? vòng:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 70),
                Size = new Size(80, 20),
                BackColor = Color.Transparent
            };
            
            cbRounds = new ComboBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(120, 68),
                Size = new Size(70, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbRounds.Items.AddRange(new object[] { "3", "5", "7", "10" });
            cbRounds.SelectedItem = "5";
            
            // Time per round
            var lblTime = new Label
            {
                Text = "Th?i gian/vòng:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 100),
                Size = new Size(100, 20),
                BackColor = Color.Transparent
            };
            
            cbTimePerRound = new ComboBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(120, 98),
                Size = new Size(70, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbTimePerRound.Items.AddRange(new object[] { "30s", "60s", "90s", "120s" });
            cbTimePerRound.SelectedItem = "60s";
            
            // Difficulty
            var lblDifficulty = new Label
            {
                Text = "?? khó:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 130),
                Size = new Size(80, 20),
                BackColor = Color.Transparent
            };
            
            cbDifficulty = new ComboBox
            {
                Font = new Font("Segoe UI", 9),
                Location = new Point(120, 128),
                Size = new Size(70, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbDifficulty.Items.AddRange(new object[] { "D?", "Bình th??ng", "Khó" });
            cbDifficulty.SelectedItem = "Bình th??ng";
            
            gamePanel.Controls.AddRange(new Control[] {
                gameTitle, lblMaxPlayers, cbMaxPlayers, lblRounds, cbRounds,
                lblTime, cbTimePerRound, lblDifficulty, cbDifficulty
            });
            
            this.Controls.Add(gamePanel);
        }
        
        private void CreateButtonsPanel()
        {
            var buttonPanel = new Panel
            {
                Size = new Size(460, 60),
                Location = new Point(20, 300),
                BackColor = Color.Transparent
            };
            
            btnCancel = new Button
            {
                Text = "? H?y",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Size = new Size(100, 40),
                Location = new Point(180, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += BtnCancel_Click;
            
            btnCreate = new Button
            {
                Text = "?? T?o Phòng",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Size = new Size(120, 40),
                Location = new Point(300, 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += BtnCreate_Click;
            
            // Room preview panel
            var previewPanel = new Panel
            {
                Size = new Size(150, 80),
                Location = new Point(10, 10),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var previewTitle = new Label
            {
                Text = "?? XEM TR??C",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(67, 82, 161),
                Location = new Point(10, 5),
                Size = new Size(130, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            
            var previewInfo = new Label
            {
                Text = $"Host: {playerName}\nPlayers: 1/7\nRounds: 5",
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, 25),
                Size = new Size(130, 50),
                BackColor = Color.Transparent
            };
            
            previewPanel.Controls.AddRange(new Control[] { previewTitle, previewInfo });
            buttonPanel.Controls.AddRange(new Control[] { previewPanel, btnCancel, btnCreate });
            
            this.Controls.Add(buttonPanel);
            
            // Update preview when settings change
            EventHandler updatePreview = (s, e) => UpdatePreview(previewInfo);
            cbMaxPlayers.SelectedIndexChanged += updatePreview;
            cbRounds.SelectedIndexChanged += updatePreview;
            cbTimePerRound.SelectedIndexChanged += updatePreview;
            cbDifficulty.SelectedIndexChanged += updatePreview;
        }
        
        private void UpdatePreview(Label previewInfo)
        {
            previewInfo.Text = $"Host: {playerName}\nPlayers: 1/{cbMaxPlayers.SelectedItem}\nRounds: {cbRounds.SelectedItem}\nTime: {cbTimePerRound.SelectedItem}";
        }
        
        private void DrawPanelBorder(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;
            
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Subtle gradient background
            using (var brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.White,
                Color.FromArgb(248, 250, 252),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, panel.ClientRectangle);
            }
            
            // Border
            using (var pen = new Pen(Color.FromArgb(67, 82, 161), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            }
        }
        
        private void GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] code = new char[6];
            
            for (int i = 0; i < 6; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            
            RoomCode = new string(code);
            if (lblRoomCode != null)
                lblRoomCode.Text = RoomCode;
        }
        
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtRoomName.Text))
            {
                MessageBox.Show("Vui lòng nh?p tên phòng!", "Thông báo", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Create game settings
            GameSettings = new Form1.GameSettings
            {
                RoomName = txtRoomName.Text.Trim(),
                MaxPlayers = int.Parse(cbMaxPlayers.SelectedItem.ToString()),
                Rounds = int.Parse(cbRounds.SelectedItem.ToString()),
                TimePerRound = int.Parse(cbTimePerRound.SelectedItem.ToString().Replace("s", "")),
                Difficulty = cbDifficulty.SelectedItem.ToString()
            };
            
            // Show confirmation dialog
            var confirmMessage = $"T?o phòng game v?i thông tin sau:\n\n" +
                               $"?? Tên phòng: {GameSettings.RoomName}\n" +
                               $"?? Mã phòng: {RoomCode}\n" +
                               $"?? S? ng??i ch?i: {GameSettings.MaxPlayers}\n" +
                               $"?? S? vòng: {GameSettings.Rounds}\n" +
                               $"?? Th?i gian: {GameSettings.TimePerRound} giây/vòng\n" +
                               $"?? ?? khó: {GameSettings.Difficulty}\n\n" +
                               $"Xác nh?n t?o phòng?";
            
            var result = MessageBox.Show(confirmMessage, "Xác nh?n t?o phòng", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            // Draw background gradient
            using (var brush = new LinearGradientBrush(
                this.ClientRectangle,
                Color.FromArgb(240, 244, 248),
                Color.FromArgb(248, 250, 252),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
    }
}