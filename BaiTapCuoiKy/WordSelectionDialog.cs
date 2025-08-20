using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    public class WordSelectionDialog : Form
    {
        public string SelectedWord { get; private set; }

        private readonly string[] _options;
        private Button _btnOption1;
        private Button _btnOption2;
        private Label _lblTitle;
        private Label _lblCountdown;
        private Timer _timer;
        private int _timeoutSeconds;

        public WordSelectionDialog(string option1, string option2)
            : this(new[] { option1, option2 }, 0)
        {
        }

        public WordSelectionDialog(string option1, string option2, int timeoutSeconds)
            : this(new[] { option1, option2 }, timeoutSeconds)
        {
        }

        public WordSelectionDialog(string[] options, int timeoutSeconds = 0)
        {
            if (options == null || options.Length < 2)
                throw new ArgumentException("Need two options", nameof(options));

            _options = options;
            _timeoutSeconds = Math.Max(0, timeoutSeconds);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Ch?n t? ?? v?";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(420, 230);
            this.BackColor = Color.White;
            this.TopMost = true;

            var header = new Panel { Dock = DockStyle.Top, Height = 56 };
            header.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(header.ClientRectangle,
                    Color.FromArgb(67, 82, 161), Color.FromArgb(45, 125, 245), LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, header.ClientRectangle);
                }
            };

            _lblTitle = new Label
            {
                Text = "Hãy ch?n m?t t? ?? v?",
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            header.Controls.Add(_lblTitle);
            Controls.Add(header);

            var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
            Controls.Add(panel);

            _btnOption1 = CreateOptionButton(_options[0]);
            _btnOption2 = CreateOptionButton(_options[1]);

            _btnOption1.Location = new Point(24, 40);
            _btnOption2.Location = new Point(this.ClientSize.Width - _btnOption2.Width - 24, 40);
            _btnOption2.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panel.Controls.Add(_btnOption1);
            panel.Controls.Add(_btnOption2);

            _lblCountdown = new Label
            {
                Text = _timeoutSeconds > 0 ? $"T? ??ng b? qua sau {_timeoutSeconds}s" : string.Empty,
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = false,
                Size = new Size(380, 22),
                Location = new Point(20, 120),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(_lblCountdown);

            var lblHint = new Label
            {
                Text = "B?m 1 ho?c 2 trên bàn phím ?? ch?n",
                ForeColor = Color.DimGray,
                AutoSize = false,
                Size = new Size(380, 24),
                Location = new Point(20, 146),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblHint);

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.D1 || e.KeyCode == Keys.NumPad1) _btnOption1.PerformClick();
                if (e.KeyCode == Keys.D2 || e.KeyCode == Keys.NumPad2) _btnOption2.PerformClick();
                if (e.KeyCode == Keys.Escape) { this.DialogResult = DialogResult.Cancel; this.Close(); }
            };

            if (_timeoutSeconds > 0)
            {
                _timer = new Timer { Interval = 1000 };
                _timer.Tick += (s, e) =>
                {
                    _timeoutSeconds--;
                    if (_timeoutSeconds <= 0)
                    {
                        _timer.Stop();
                        SelectedWord = null;
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                    else
                    {
                        _lblCountdown.Text = $"T? ??ng b? qua sau {_timeoutSeconds}s";
                    }
                };
                // Start after form shown
                this.Shown += (s, e) => _timer.Start();
                this.FormClosed += (s, e) => { try { _timer.Stop(); _timer.Dispose(); } catch { } };
            }
        }

        private Button CreateOptionButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(160, 64),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = GameEffects.LightenColor(btn.BackColor, 0.12f);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(59, 130, 246);
            btn.Click += (s, e) =>
            {
                try { _timer?.Stop(); } catch { }
                SelectedWord = btn.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            return btn;
        }
    }
}
