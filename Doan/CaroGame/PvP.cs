using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;
using System;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvP : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion

        public Room room;
        public int playerNumber;
        private bool undoCount = false;

        // Các biến kết nối mạng
        private string player1Name;
        private string player2Name;
        private TCPClient tcpClient;

        // Constructor cũ (giữ lại để tránh lỗi designer, nhưng hạn chế dùng)
        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;

            InitGame();
        }

        // --- CONSTRUCTOR CHÍNH (SỬA LỖI LOGIC) ---
        public PvP(Room room, int playerNumber, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            this.player1Name = p1;
            this.player2Name = p2;
            this.tcpClient = client;

            InitGame();
        }

        private void InitGame()
        {
            SetupEmojiPickerPanel();
            ChessBoard = new ChessBoardManager(pnlChessBoard);
            ChessBoard.DrawChessBoard();
        }

        private void OnGameEnded(string winner)
        {
            MessageBox.Show($"{winner} chiến thắng!!!");
            resetChess();
        }

        private void resetChess()
        {
            ChessBoard.resetGame();

            undoCount = false;
            ptbOne.Visible = true;
            ptbZero.Visible = false;
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;
        }

        private Menu menuForm;
        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (menuForm == null || menuForm.IsDisposed)
            {
                menuForm = new Menu();
                menuForm.StartPosition = FormStartPosition.Manual;
                menuForm.Location = new Point(this.Left + 22, this.Top + 50);
                menuForm.Show(this);

                // Lưu ý: New lại ChessBoard sẽ làm mất nước đi hiện tại
                ChessBoard = new ChessBoardManager(pnlChessBoard);
                ChessBoard.DrawChessBoard();

                this.room = room;
                this.playerNumber = playerNumber;
            }
            else
            {
                menuForm.Close();
                menuForm = null;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            // Quay về Dashboard và TRUYỀN LẠI CLIENT + ROOM
            var DashBoard = new Dashboard(room, playerNumber, player1Name, tcpClient);
            DashBoard.Show();
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            panelChat.Visible = !panelChat.Visible;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            AppendMessage("You", text, Color.Blue);

            // TODO: Gửi tin nhắn qua tcpClient nếu cần
            // if(tcpClient != null) tcpClient.Send(...);

            txtMessage.Clear();
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
            t.Tick += (s, ev) =>
            {
                t.Stop();
                AppendMessage("Bot", "Đã nhận: " + text, Color.Green);
            };
            t.Start();
        }

        private void AppendMessage(string sender, string message, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            rtbChat.AppendText($"{sender}: ");

            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText(message + Environment.NewLine + Environment.NewLine);

            rtbChat.ScrollToCaret();
        }

        private readonly string[] _emoticons = new string[]
        {
            "😀","😃","😄","😁","😆","😅","😂","🤣","🥲","😊","😇",
            "🙂","🙃","😉","😌","😍","🥰","😘","😗","😙","😚","😋",
            "😜","😝","😛","🤑","🤗","🤭","🤫","🤔","🤐","😐","😑",
            "😶","😶‍🌫️","🙄","😏","😒","😞","😔","😟","😕","🙁",
            "☹️","😣","😖","😫","😩","🥺","😢","😭","😤","😠","😡",
            "🤬","🤯","😳","🥵","🥶","😱","😨","😰","😥","😓","🤤",
            "😪","😴","😬","😮‍💨","🫠","😵","😵‍💫","🤐","🥴","😷",
            "🤒","🤕","🤢","🤮","🤧","😇","🥳","🥸","😎","🤓","🧐",
            "😕","😟","🙁","☹️","😮","😯","😲","😳","🥺","🥹","😦",
            "😧","😨","😩","😰","😱","😪","😵","🤐","🥴","😷","🤒",
            "🤕","🤢","🤮","🤧","😇","🥳","🥸","😎","🤓","🧐",
            // Gestures/People
            "👋","🤚","🖐️","✋","🖖","👌","🤌","🤏","✌️","🤞",
            "🤟","🤘","🤙","👈","👉","👆","🖕","👇","☝️","👍","👎",
            "✊","👊","🤛","🤜","👏","🙌","🫶","👐","🤲","🙏",
            "💪","🦾","🦵","🦿","🦶","👂","🦻","👃","👣","👀","👁️",
            "🫦","👄","🦷","🦴","👅",
            // Relations/Love
            "💋","👄","💘","💝","💖","💗","💓","💞","💕","💌","💟",
            "❣️","💔","❤️","🧡","💛","💚","💙","💜","🤎","🖤","🤍",
            // Animals/Nature
            "🐶","🐱","🐭","🐹","🐰","🦊","🐻","🐼","🐨","🐯","🦁",
            "🐮","🐷","🐸","🐵","🐔","🐧","🐦","🐤","🐣","🐥","🦆",
            "🦅","🦉","🦇","🐺","🐗","🐴","🦄","🐝","🐛","🦋","🐌",
            "🐞","🐜","🦟","🦗","🕷️","🦂","🐢","🐍","🦎","🦖","🦕",
            "🐙","🦑","🦐","🦞","🦀","🐠","🐟","🐡","🐬","🐳","🐋",
            "🦈","🐊","🐅","🐆","🦓","🦍","🦧","🐘","🦣","🦛","🦏",
            "🐪","🐫","🦒","🦘","🦥","🦦","🦨","🦡","🐁","🐀","🐇",
            "🦔",
            // Food/Drinks
            "🍏","🍎","🍐","🍊","🍋","🍌","🍉","🍇","🍓","🫐",
            "🍈","🍒","🍑","🥭","🍍","🥥","🥝","🍅","🍆","🥑","🥦",
            "🥬","🥒","🌶️","🫑","🌽","🥕","🧄","🧅","🥔","🍠",
            "🥐","🥯","🍞","🥖","🥨","🧀","🥚","🍳","🥞","🧇",
            "🥓","🥩","🍗","🍖","🦴","🌭","🍔","🍟","🍕","🫓",
            "🥪","🥙","🧆","🌮","🌯","🫔","🥗","🥘","🫕","🥫",
            "🍝","🍜","🍲","🍛","🍣","🍱","🥟","🦪","🍤","🍙",
            "🍚","🍘","🍥","🥠","🥮","🍢","🍡","🍧","🍨","🍦",
            "🥧","🧁","🍰","🎂","🍮","🍭","🍬","🍫","🍿","🧃",
            "🥤","🧋","🫖","☕","🍵","🧉","🍶","🍺","🍻","🥂",
            "🍷","🥃","🍸","🍹","🍾",
            // Activities/Objects
            "⚽","🏀","🏈","⚾","🥎","🎾","🏐","🏉","🥏","🎱",
            "🏓","🏸","🥅","🏒","🏑","🥍","🏏","🪃","🏹","🎣",
            "🤿","🥊","🥋","🎽","🛹","🛷","⛸️","🥌","🥇","🥈",
            "🥉","🏆","🏅","🎖️","🥫","🏵️","🎗️","🎫","🎟️",
            "🎪","🤹‍♂️","🤹‍♀️","🎭","🩰","🎨","🎬","🎤","🎧","🎼",
            "🎹","🥁","🎷","🎺","🎸","🪕",
            // Travel/Places
            "🚗","🚕","🚙","🚌","🚎","🏎️","🚓","🚑","🚒","🚐",
            "🚚","🚛","🚜","🛵","🏍️","🚲","🛴","🚏","🛣️","🛤️",
            "🗺️","🗿","🗽","🗼","🏰","🏯","🏟️","🎡","🎢","🎠",
            "⛲","⛱️","🏖️","🏝️","🛶","⛵","🚤","🛥️","🛳️","⛴️",
            "🚀","🛸","✈️","🛫","🛬",
            // Symbols/Flags
            "🏁","🚩","🎌","🏴","🏳️","🏳️‍🌈","🏳️‍⚧️","🏴‍☠️","🇦🇺",
            "🇨🇦","🇫🇷","🇩🇪","🇨🇳","🇯🇵","🇰🇷","🇷🇺","🇬🇧","🇺🇸"
        };

        private void SetupEmojiPickerPanel()
        {
            if (pnlEmojiPicker == null) return;
            pnlEmojiPicker.Visible = false;
            pnlEmojiPicker.Controls.Clear();
            pnlEmojiPicker.AutoScroll = true;
        }

        private void ShowEmojiPicker()
        {
            if (pnlEmojiPicker == null) return;

            if (pnlEmojiPicker.Visible && pnlEmojiPicker.Controls.Count > 0)
            {
                pnlEmojiPicker.Visible = false;
                return;
            }

            pnlEmojiPicker.Visible = true;
            pnlEmojiPicker.BringToFront();
            pnlEmojiPicker.Controls.Clear();

            int btnSize = 32;
            int cols = 8;
            int spacing = 4;

            for (int i = 0; i < _emoticons.Length; i++)
            {
                var btn = new Button();
                btn.Font = new Font("Segoe UI Emoji", 16F, FontStyle.Regular);
                btn.Text = _emoticons[i];
                btn.Width = btn.Height = btnSize;
                int col = i % cols;
                int row = i / cols;
                btn.Left = col * (btnSize + spacing);
                btn.Top = row * (btnSize + spacing);
                btn.Margin = new Padding(0);
                btn.Padding = new Padding(0);

                btn.Click += (s, e) =>
                {
                    txtMessage.Text += ((Button)s).Text;
                    txtMessage.SelectionStart = txtMessage.Text.Length;
                    txtMessage.Focus();
                };

                pnlEmojiPicker.Controls.Add(btn);
            }
        }
        private void btn_emoji_Click(object sender, EventArgs e)
        {
            ShowEmojiPicker();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            bool undoSuccess = ChessBoard.undoTurnPvP();
            {
                if (undoSuccess && !undoCount)
                {
                    ptbOne.Visible = false;
                    ptbZero.Visible = true;
                    undoCount = true;
                }
            }
        }
    }
}