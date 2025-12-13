using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvP : Form
    {
        private ChessBoardManager ChessBoard;
        public Room room;

        // 0: X (P1), 1: O (P2)
        public int MySide { get; set; }

        private bool isMyUndoRequest = false;
        private bool undoCount = false;

        private string player1Name;
        private string player2Name;
        private TCPClient tcpClient;

        private bool _gameEnded = false;
        private bool _waitingRematch = false;
        private bool _resultSent = false;

        // ✅ nếu đối thủ đã gửi rematch offer thì mình chỉ Accept/Decline
        private bool _hasIncomingRematchOffer = false;
        private string _incomingOfferFrom = "";

        public PvP(Room room, int mySide, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = mySide;
            this.player1Name = p1;
            this.player2Name = p2;
            this.tcpClient = client;

            InitGame();
        }

        // Offline/Fallback
        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = (playerNumber == 1) ? 0 : 1;
            this.player1Name = "Player 1";
            this.player2Name = "Player 2";
            InitGame();
        }

        private void InitGame()
        {
            CheckForIllegalCrossThreadCalls = false;

            SetupEmojiPickerPanel();
            if (pnlChessBoard != null) pnlChessBoard.BringToFront();

            SetupPlayerInfo();

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;

            ChessBoard.IsMyTurn = (this.MySide == 0);

            this.Text = $"PvP - Bạn là {(this.MySide == 0 ? "X (Đi trước)" : "O (Đi sau)")}";

            ChessBoard.PlayerClickedNode -= ChessBoard_PlayerClickedNode;
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            ChessBoard.GameEnded -= ChessBoard_GameEnded;
            ChessBoard.GameEnded += ChessBoard_GameEnded;

            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            ChessBoard.DrawChessBoard();
        }

        private void SetupPlayerInfo()
        {
            if (label1 != null) label1.Text = player1Name;
            if (label2 != null) label2.Text = player2Name;

            if (label1 != null) { label1.ForeColor = Color.Black; label1.Font = new Font(label1.Font, FontStyle.Regular); }
            if (label2 != null) { label2.ForeColor = Color.Black; label2.Font = new Font(label2.Font, FontStyle.Regular); }

            if (MySide == 0)
            {
                if (label1 != null) { label1.ForeColor = Color.Red; label1.Font = new Font(label1.Font, FontStyle.Bold); }
            }
            else
            {
                if (label2 != null) { label2.ForeColor = Color.Blue; label2.Font = new Font(label2.Font, FontStyle.Bold); }
            }

            try
            {
                if (ptbAvaP1 != null) { ptbAvaP1.Image = null; ptbAvaP1.BackColor = Color.LightGray; }
                if (ptbAvaP2 != null) { ptbAvaP2.Image = null; ptbAvaP2.BackColor = Color.LightGray; }
            }
            catch { }
        }

        // ================= GAME END + RESULT =================

        private void ChessBoard_GameEnded(string winnerRaw)
        {
            if (_gameEnded) return;
            _gameEnded = true;

            // ✅ winnerRaw có thể là "X"/"O" hoặc tên người
            bool iWon = ComputeWinByWinnerRaw(winnerRaw);

            SendGameResultOnce(iWon);

            DialogResult result = MessageBox.Show(
                $"Kết quả: {(iWon ? "Bạn thắng 🎉" : "Bạn thua 😢")}\n\nRematch hay Exit?",
                "Kết thúc trận",
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Information
            );

            if (result == DialogResult.Retry)
                RematchFlow();
            else
                ExitMatch(sendSurrenderIfNeeded: false);
        }

        // ✅ Tính thắng thua chắc chắn (không còn “cả 2 thua”)
        private bool ComputeWinByWinnerRaw(string winnerRaw)
        {
            string w = (winnerRaw ?? "").Trim();

            // Case 1: ChessBoard gửi "X"/"O"
            if (string.Equals(w, "X", StringComparison.OrdinalIgnoreCase))
                return MySide == 0;
            if (string.Equals(w, "O", StringComparison.OrdinalIgnoreCase))
                return MySide == 1;

            // Case 2: ChessBoard gửi tên
            if (!string.IsNullOrEmpty(player1Name) && string.Equals(w, player1Name, StringComparison.OrdinalIgnoreCase))
                return MySide == 0;
            if (!string.IsNullOrEmpty(player2Name) && string.Equals(w, player2Name, StringComparison.OrdinalIgnoreCase))
                return MySide == 1;

            // Fallback: nếu không chắc -> coi như thua (để tránh “2 thằng đều thắng”)
            return false;
        }

        private void SendGameResultOnce(bool iWon)
        {
            if (_resultSent) return;
            _resultSent = true;

            // ✅ ĐÚNG FORMAT SERVER của bạn: GAME_RESULT|WIN/LOSE (username lấy theo currentUsername)
            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.Send($"GAME_RESULT|{(iWon ? "WIN" : "LOSE")}");
        }

        // ================= REMATCH FLOW =================
        // ✅ Nếu đối thủ đã gửi offer -> mình chỉ ACCEPT / DECLINE (không cần gửi REQUEST nữa)

        private void RematchFlow()
        {
            if (tcpClient == null) return;

            if (_hasIncomingRematchOffer)
            {
                // đã có offer từ đối thủ => chỉ Accept/Decline
                DialogResult res = MessageBox.Show(
                    $"{_incomingOfferFrom} muốn Rematch.\nBạn có đồng ý không?",
                    "Rematch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (res == DialogResult.Yes)
                    tcpClient.Send("REMATCH_ACCEPT");
                else
                    tcpClient.Send("REMATCH_DECLINE");

                _hasIncomingRematchOffer = false;
                _incomingOfferFrom = "";
                return;
            }

            // chưa có offer => mình là người gửi request
            RequestRematch();
        }

        private void RequestRematch()
        {
            if (_waitingRematch) return;
            _waitingRematch = true;

            MessageBox.Show("Đã gửi yêu cầu Rematch.\nĐang chờ đối thủ...", "Rematch");

            if (tcpClient != null)
                tcpClient.Send("REMATCH_REQUEST");
        }

        private void StartRematch(string sideRaw)
        {
            _gameEnded = false;
            _waitingRematch = false;
            _resultSent = false;

            _hasIncomingRematchOffer = false;
            _incomingOfferFrom = "";

            MySide = (sideRaw.ToUpper() == "X") ? 0 : 1;

            ChessBoard.resetGame();
            ChessBoard.MySide = MySide;
            ChessBoard.IsMyTurn = (MySide == 0);

            SetupPlayerInfo();
            this.Text = $"PvP - Rematch ({(MySide == 0 ? "X" : "O")})";
        }

        // ================= NETWORK =================

        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.SendPacket(new Packet("MOVE", point));
        }

        private void HandleServerMessage(string data)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    string[] parts = data.Split('|');
                    string cmd = parts[0];

                    switch (cmd)
                    {
                        case "MOVE":
                            if (parts.Length < 4) return;
                            int x = int.Parse(parts[1]);
                            int y = int.Parse(parts[2]);
                            int side = int.Parse(parts[3]);
                            if (side == -1) side = ChessBoard.MoveCount % 2;
                            ChessBoard.ProcessMove(x, y, side);
                            break;

                        case "CHAT":
                            if (parts.Length >= 2)
                                AppendMessage(parts.Length > 2 ? parts[2] : "Opponent", parts[1], Color.Red);
                            break;

                        case "UNDO_SUCCESS":
                            ChessBoard.ExecuteUndoPvP();
                            if (isMyUndoRequest)
                            {
                                undoCount = true;
                                if (ptbOne != null) ptbOne.Visible = false;
                                if (ptbZero != null) ptbZero.Visible = true;
                                isMyUndoRequest = false;
                            }
                            break;

                        case "OPPONENT_LEFT":
                            MessageBox.Show("Đối thủ đã thoát. Bạn thắng!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ExitMatch(sendSurrenderIfNeeded: false);
                            break;

                        case "REMATCH_OFFER":
                            // ✅ chỉ set flag, để khi mình bấm Rematch thì accept/decline
                            _hasIncomingRematchOffer = true;
                            _incomingOfferFrom = (parts.Length >= 2) ? parts[1] : "Opponent";

                            // Nếu bạn muốn: pop-up ngay lập tức thay vì đợi bấm Rematch:
                            // RematchFlow();
                            break;

                        case "REMATCH_START":
                            if (parts.Length >= 2) StartRematch(parts[1]);
                            break;

                        case "REMATCH_DECLINED":
                            MessageBox.Show("Đối thủ từ chối Rematch.", "Rematch");
                            _waitingRematch = false;
                            _hasIncomingRematchOffer = false;
                            _incomingOfferFrom = "";
                            break;

                        case "REMATCH_SENT":
                            // server confirm đã gửi
                            break;
                    }
                }
                catch { }
            });
        }

        private void ExitMatch(bool sendSurrenderIfNeeded)
        {
            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;

                if (sendSurrenderIfNeeded && !_gameEnded)
                    tcpClient.Send("SURRENDER");
            }
            Close();
        }

        // ================= UI HANDLERS (GIỮ ĐÚNG TÊN CONTROL CỦA BẠN) =================

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoCount) return;
            if (tcpClient != null)
            {
                isMyUndoRequest = true;
                tcpClient.Send("REQUEST_UNDO");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage?.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);
            if (tcpClient != null) tcpClient.SendPacket(new Packet("CHAT", text));
            txtMessage.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (_gameEnded)
            {
                ExitMatch(sendSurrenderIfNeeded: false);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát trận đấu?\nThoát giữa trận sẽ bị xử thua.",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
                ExitMatch(sendSurrenderIfNeeded: true);
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat != null) panelChat.Visible = !panelChat.Visible;
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
            }
            else { menuForm.Close(); menuForm = null; }
        }

        // alias handlers nếu Designer gọi dạng btn_...
        private void btn_Undo_Click(object sender, EventArgs e) => btnUndo_Click(sender, e);
        private void btn_Send_Click(object sender, EventArgs e) => btnSend_Click(sender, e);
        private void btn_Exit_Click(object sender, EventArgs e) => btnExit_Click(sender, e);
        private void btn_Menu_Click(object sender, EventArgs e) => btnMenu_Click(sender, e);
        private void btn_Chat_Click(object sender, EventArgs e) => btnChat_Click(sender, e);

        private void AppendMessage(string sender, string message, Color color)
        {
            if (rtbChat == null) return;

            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            rtbChat.AppendText($"{sender}: ");

            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText(message + Environment.NewLine + Environment.NewLine);
            rtbChat.ScrollToCaret();
        }

        // ================= EMOJI =================
        private readonly string[] _emoticons = new string[] {
            "😀","😃","😄","😁","😆","😅","😂","🤣","🥲","☺️","😊","😇",
            "🙂","🙃","😉","😌","😍","🥰","😘","😗","😋","😛","😝","😜",
            "🤪","🤨","🧐","🤓","😎","🥸","🤩","🥳","😏","😒","😞","😔",
            "😟","😕","🙁","☹️","😣","😖","😫","😩","🥺","😢","😭","😤",
            "😠","😡","🤬","🤯","😳","🥵","🥶","😱","😨","😰","😥","😓",
            "🤗","🤔","🤭","🤫","🤥","😶","😐","😑","😬","🙄","😯","😦",
            "😧","😮","😲","🥱","😴","🤤","😪","😵","🤐","🥴","🤢","🤮",
            "🤧","😷","🤒","🤕","🤑","🤠","😈","👿","👹","👺","🤡","💩",
            "👻","💀","☠️","👽","👾","🤖","🎃",
            "😺","😸","😹","😻","😼","😽","🙀","😿","😾",
            "👋","🤚","🖐","✋","🖖","👌","🤌","🤏","✌️","🤞","🤟","🤘",
            "🤙","👈","👉","👆","👇","☝️","👍","👎","✊","👊","🤛","🤜",
            "👏","🙌","👐","🤲","🤝","🙏","💪","💅","🤳",
            "❤️","🧡","💛","💚","💙","💜","🖤","🤍","🤎","💔","❣️","💕",
            "💞","💓","💗","💖","💘","💝","💋","💌",
            "👀","👁","🧠","🔥","✨","🌟","💫","💥","💢","💦","💤","🎵",
            "🎶","✅","❌","💯","⚠️","⛔️","🎉","🎈","🎁"
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
            if (pnlEmojiPicker == null || txtMessage == null) return;

            if (pnlEmojiPicker.Visible && pnlEmojiPicker.Controls.Count > 0)
            {
                pnlEmojiPicker.Visible = false;
                return;
            }

            pnlEmojiPicker.Visible = true;
            pnlEmojiPicker.BringToFront();
            pnlEmojiPicker.Controls.Clear();

            int btnSize = 32, cols = 8, spacing = 4;

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

                btn.Click += (s, e) =>
                {
                    txtMessage.Text += ((Button)s).Text;
                    txtMessage.SelectionStart = txtMessage.Text.Length;
                    txtMessage.Focus();
                };

                pnlEmojiPicker.Controls.Add(btn);
            }
        }

        private void btn_emoji_Click(object sender, EventArgs e) => ShowEmojiPicker();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (tcpClient != null)
                tcpClient.OnMessageReceived -= HandleServerMessage;

            base.OnFormClosing(e);
        }

        private void Btn_Click(object? sender, EventArgs e) { }
    }
}
