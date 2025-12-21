using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvP : Form
    {
        private ChessBoardManager ChessBoard;
        public Room room;

        // 0: X, 1: O
        public int MySide { get; set; }

        // Undo
        private bool isMyUndoRequest = false;
        private bool myUndoUsed = false;
        private bool oppUndoUsed = false;

        // Names đang hiển thị theo "slot UI"
        // QUAN TRỌNG: vì icon tĩnh: label1 luôn đứng cạnh icon X, label2 cạnh icon O
        // => khi rematch đổi quân, ta swap 2 tên này để khớp icon
        private string player1Name; // UI slot X
        private string player2Name; // UI slot O

        private TCPClient tcpClient;

        private bool _gameEnded = false;
        private bool _waitingRematch = false;
        private bool _waitingReset = false;

        private bool _resultSent = false;

        //Reset
        private bool _hasIncomingResetOffer = false;
        private string _incomingResetFrom = "";

        // Rematch
        private bool _hasIncomingRematchOffer = false;
        private string _incomingRematchFrom = "";

        // dialogs (modeless)
        private ResultDialog _resultDialog = null;
        private WaitingRematchDialog _waitingRematchDialog = null;
        private WaitingResetDialog _waitingResetDialog = null;

        public PvP(Room room, int mySide, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = mySide;

            // ban đầu (vào game lần 1): p1 là X, p2 là O (theo UI bạn thiết kế tĩnh)
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

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;

            // ✅ Mapping tên trong ChessBoard theo icon tĩnh:
            // Player[0] là X => lấy theo label1 (player1Name)
            // Player[1] là O => lấy theo label2 (player2Name)
            SyncChessBoardNamesWithUI();

            ChessBoard.PlayerClickedNode -= ChessBoard_PlayerClickedNode;
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            ChessBoard.GameEnded -= ChessBoard_GameEnded;
            ChessBoard.GameEnded += ChessBoard_GameEnded;

            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            SetupPlayerInfo();

            // X luôn đi trước khi bắt đầu ván mới
            ChessBoard.IsMyTurn = (this.MySide == 0);

            // ✅ UI lượt phải theo side X/O, không theo "lượt mình"
            TurnUIBySide(0);

            this.Text = $"PvP - Bạn là {(this.MySide == 0 ? "X (Đi trước)" : "O (Đi sau)")}";

            ChessBoard.DrawChessBoard();
        }

        // =============== CORE UI HELPERS ===============
        private void SetupPlayerInfo()
        {
            // label1 luôn là người cạnh icon X
            if (label1 != null) label1.Text = player1Name;
            if (label2 != null) label2.Text = player2Name;

            if (label1 != null)
            {
                label1.ForeColor = Color.Black;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
            }
            if (label2 != null)
            {
                label2.ForeColor = Color.Black;
                label2.Font = new Font(label2.Font, FontStyle.Regular);
            }

            // highlight người chơi theo MySide hiện tại
            // MySide==0 => mình là X => highlight label1
            // MySide==1 => mình là O => highlight label2
            if (MySide == 0)
            {
                if (label1 != null)
                {
                    label1.ForeColor = Color.Red;
                    label1.Font = new Font(label1.Font, FontStyle.Bold);
                }
            }
            else
            {
                if (label2 != null)
                {
                    label2.ForeColor = Color.Blue;
                    label2.Font = new Font(label2.Font, FontStyle.Bold);
                }
            }
        }

        private void SyncChessBoardNamesWithUI()
        {
            if (ChessBoard?.Player != null && ChessBoard.Player.Count >= 2)
            {
                ChessBoard.Player[0].Name = player1Name; // X
                ChessBoard.Player[1].Name = player2Name; // O
            }
        }

        private void SwapNamesForStaticIcons()
        {
            // swap tên 2 label để khớp icon tĩnh (label1 luôn X, label2 luôn O)
            string tmp = player1Name;
            player1Name = player2Name;
            player2Name = tmp;

            SetupPlayerInfo();
            SyncChessBoardNamesWithUI();
        }

        // ================= TURN UI (FIXED) =================
        // 0 = lượt X (slot label1/pgbP1), 1 = lượt O (slot label2/pgbP2)
        private void TurnUIBySide(int turnSide)
        {
            if (pgbP1 == null || pgbP2 == null) return;

            bool xTurn = (turnSide == 0);

            pgbP1.Visible = true;
            pgbP1.Style = ProgressBarStyle.Blocks;
            pgbP1.Value = xTurn ? 100 : 0;

            pgbP2.Visible = true;
            pgbP2.Style = ProgressBarStyle.Blocks;
            pgbP2.Value = xTurn ? 0 : 100;
        }

        private void TurnUIEnd()
        {
            if (pgbP1 == null || pgbP2 == null) return;

            pgbP1.Visible = true;
            pgbP1.Style = ProgressBarStyle.Blocks;
            pgbP1.Value = 0;

            pgbP2.Visible = true;
            pgbP2.Style = ProgressBarStyle.Blocks;
            pgbP2.Value = 0;
        }

        // side vừa đi xong => lượt kế tiếp là 1 - side
        private void ApplyTurnAfterMove(int lastMoveSide)
        {
            int nextSide = 1 - lastMoveSide;

            ChessBoard.IsMyTurn = (nextSide == MySide);
            TurnUIBySide(nextSide);
        }

        // ================= DIALOG HELPERS =================
        private void CloseResultDialog()
        {
            try
            {
                if (_resultDialog != null && !_resultDialog.IsDisposed)
                    _resultDialog.Close();
            }
            catch { }
            _resultDialog = null;
        }

        private void CloseWaitingRematchDialog()
        {
            try
            {
                if (_waitingRematchDialog != null && !_waitingRematchDialog.IsDisposed)
                    _waitingRematchDialog.Close();
            }
            catch { }
            _waitingRematchDialog = null;
        }

        private void CloseWaitingResetDialog()
        {
            try
            {
                if (_waitingResetDialog != null && !_waitingResetDialog.IsDisposed)
                    _waitingResetDialog.Close();
            }
            catch { }
            _waitingResetDialog = null;
        }

        private void CloseAllPopups()
        {
            CloseResultDialog();
            CloseWaitingRematchDialog();
            CloseWaitingResetDialog();
        }

        private void ShowOpponentLeftAndExit()
        {
            CloseAllPopups();

            MessageBox.Show("Đối thủ đã thoát. Trận đấu sẽ kết thúc.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            ExitMatch(sendSurrenderIfNeeded: false);
        }

        // ================= GAME END + RESULT =================
        private void ChessBoard_GameEnded(string winnerRaw)
        {
            if (_gameEnded) return;
            _gameEnded = true;

            // ✅ end UI lượt
            TurnUIEnd();

            bool iWon = ComputeWinByWinnerRaw(winnerRaw);
            SendGameResultOnce(iWon);

            CloseAllPopups();

            _resultDialog = new ResultDialog(iWon);

            _resultDialog.RematchClicked += () =>
            {
                CloseResultDialog();
                RematchFlow();
            };

            _resultDialog.ExitClicked += () =>
            {
                CloseResultDialog();
                ExitMatch(sendSurrenderIfNeeded: false);
            };

            _resultDialog.FormClosed += (s, e) => { _resultDialog = null; };
            _resultDialog.Show(this);
        }

        private bool ComputeWinByWinnerRaw(string winnerRaw)
        {
            string w = (winnerRaw ?? "").Trim();
            if (string.IsNullOrEmpty(w)) return false;

            // Case 1: board gửi "X"/"O"
            if (string.Equals(w, "X", StringComparison.OrdinalIgnoreCase))
                return MySide == 0;
            if (string.Equals(w, "O", StringComparison.OrdinalIgnoreCase))
                return MySide == 1;

            // Case 2: board gửi tên (đã sync theo UI)
            if (string.Equals(w, player1Name, StringComparison.OrdinalIgnoreCase)) // X
                return MySide == 0;
            if (string.Equals(w, player2Name, StringComparison.OrdinalIgnoreCase)) // O
                return MySide == 1;

            return false;
        }

        private void SendGameResultOnce(bool iWon)
        {
            if (_resultSent) return;
            _resultSent = true;

            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.Send($"GAME_RESULT|{(iWon ? "WIN" : "LOSE")}");
        }

        // ================= REMATCH FLOW =================
        private void RematchFlow()
        {
            if (tcpClient == null || !tcpClient.IsConnected()) return;

            if (_hasIncomingRematchOffer)
            {
                CloseAllPopups();

                DialogResult res = MessageBox.Show(
                    $"{_incomingRematchFrom} muốn Rematch.\nBạn có đồng ý không?",
                    "Rematch",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (res == DialogResult.Yes)
                    tcpClient.Send("REMATCH_ACCEPT");
                else
                {
                    tcpClient.Send("REMATCH_DECLINE");
                    ExitMatch(sendSurrenderIfNeeded: false);
                }

                _hasIncomingRematchOffer = false;
                _incomingRematchFrom = "";
                return;
            }

            RequestRematch();
        }

        private void RequestRematch()
        {
            if (_waitingRematch) return;
            _waitingRematch = true;

            CloseAllPopups();

            _waitingRematchDialog = new WaitingRematchDialog();
            _waitingRematchDialog.CancelClicked += () =>
            {
                CloseWaitingRematchDialog();
                try
                {
                    if (tcpClient != null && tcpClient.IsConnected())
                        tcpClient.Send("REMATCH_DECLINE");
                }
                catch { }

                ExitMatch(sendSurrenderIfNeeded: false);
            };

            _waitingRematchDialog.FormClosed += (s, e) => { _waitingRematchDialog = null; };
            _waitingRematchDialog.Show(this);

            try
            {
                if (tcpClient != null && tcpClient.IsConnected())
                    tcpClient.Send("REMATCH_REQUEST");
            }
            catch { }
        }

        private void StartRematch(string sideRaw)
        {
            CloseAllPopups();

            _gameEnded = false;
            _waitingRematch = false;
            _resultSent = false;

            _hasIncomingRematchOffer = false;
            _incomingRematchFrom = "";

            int oldSide = MySide;

            // server gửi side cho MÌNH: "X" / "O"
            MySide = ((sideRaw ?? "").Trim().ToUpper() == "X") ? 0 : 1;

            ChessBoard.resetGame();
            ChessBoard.MySide = MySide;

            // X luôn đi trước ván mới
            ChessBoard.IsMyTurn = (MySide == 0);
            TurnUIBySide(0);

            // reset undo
            myUndoUsed = false;
            oppUndoUsed = false;
            isMyUndoRequest = false;
            if (btnUndo != null) btnUndo.Enabled = true;

            // reset UI undo của mình (ptbOne/ptbZero)
            if (ptbOne != null) ptbOne.Visible = true;
            if (ptbZero != null) ptbZero.Visible = false;

            // ✅ icon tĩnh: label1 luôn cạnh X, label2 luôn cạnh O
            // => nếu side của mình đổi so với ván trước thì swap tên hiển thị để khớp icon
            if (oldSide != MySide)
                SwapNamesForStaticIcons();
            else
            {
                SetupPlayerInfo();
                SyncChessBoardNamesWithUI();
            }

            this.Text = $"PvP - Rematch ({(MySide == 0 ? "X" : "O")})";
        }

        private void RequestReset()
        {
            if (_waitingReset) return;
            if (tcpClient == null || !tcpClient.IsConnected()) return;

            _waitingReset = true;
            CloseAllPopups();

            _waitingResetDialog = new WaitingResetDialog();
            _waitingResetDialog.CancelClicked += () =>
            {
                CloseWaitingResetDialog();
                _waitingReset = false;

                try
                {
                    if (tcpClient != null && tcpClient.IsConnected())
                        tcpClient.Send("RESET_DECLINE");
                }
                catch { }
            };

            _waitingResetDialog.FormClosed += (s, e) => { _waitingResetDialog = null; };
            _waitingResetDialog.Show(this);

            try
            {
                tcpClient.Send("RESET_REQUEST");
            }
            catch { }
        }

        private void StartReset()
        {
            CloseAllPopups();

            _gameEnded = false;
            _waitingReset = false;
            _resultSent = false;

            _hasIncomingResetOffer = false;
            _incomingResetFrom = "";

            ChessBoard.resetGame();

            // X luôn đi trước sau reset ván
            ChessBoard.IsMyTurn = (MySide == 0);
            TurnUIBySide(0);

            // Reset Undo
            myUndoUsed = false;
            oppUndoUsed = false;
            isMyUndoRequest = false;
            if (btnUndo != null) btnUndo.Enabled = true;

            // reset UI undo của mình (ptbOne/ptbZero)
            if (ptbOne != null) ptbOne.Visible = true;
            if (ptbZero != null) ptbZero.Visible = false;

            SetupPlayerInfo();
            SyncChessBoardNamesWithUI();
        }

        // ================= NETWORK =================
        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.SendPacket(new Packet("MOVE", point));

            // mình vừa đi xong => lượt đối thủ
            ChessBoard.IsMyTurn = false;
            TurnUIBySide(1 - MySide);
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
                            {
                                if (parts.Length < 4) return;
                                int x = int.Parse(parts[1]);
                                int y = int.Parse(parts[2]);
                                int side = int.Parse(parts[3]);

                                // nếu server gửi -1 => suy ra theo move count trước khi add move
                                if (side == -1) side = ChessBoard.MoveCount % 2;

                                ChessBoard.ProcessMove(x, y, side);

                                // ✅ FIX: cập nhật lượt đúng theo X/O
                                ApplyTurnAfterMove(side);
                                break;
                            }

                        case "CHAT":
                            if (parts.Length >= 2)
                                AppendMessage(parts.Length > 2 ? parts[2] : "Opponent", parts[1], Color.Red);
                            break;

                        case "UNDO_SUCCESS":
                            ChessBoard.ExecuteUndoPvP();

                            if (isMyUndoRequest)
                            {
                                myUndoUsed = true;
                                if (btnUndo != null) btnUndo.Enabled = false;

                                if (ptbOne != null) ptbOne.Visible = false;
                                if (ptbZero != null) ptbZero.Visible = true;
                            }
                            else
                            {
                                oppUndoUsed = true;
                                // nếu bạn có UI undo của đối thủ thì update ở đây
                            }

                            isMyUndoRequest = false;
                            break;

                        case "OPPONENT_LEFT":
                            ShowOpponentLeftAndExit();
                            break;

                        case "REMATCH_OFFER":
                            _hasIncomingRematchOffer = true;
                            _incomingRematchFrom = (parts.Length >= 2) ? parts[1] : "Opponent";

                            CloseAllPopups();

                            DialogResult res = MessageBox.Show(
                                $"{_incomingRematchFrom} muốn Rematch.\nBạn có đồng ý không?",
                                "Rematch",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (res == DialogResult.Yes)
                                tcpClient.Send("REMATCH_ACCEPT");
                            else
                            {
                                tcpClient.Send("REMATCH_DECLINE");
                                ExitMatch(sendSurrenderIfNeeded: false);
                            }

                            _hasIncomingRematchOffer = false;
                            _incomingRematchFrom = "";
                            break;

                        case "REMATCH_START":
                            CloseAllPopups();
                            if (parts.Length >= 2) StartRematch(parts[1]);
                            break;

                        case "REMATCH_DECLINED":
                            CloseAllPopups();
                            MessageBox.Show("Đối thủ từ chối Rematch. Trận sẽ thoát.", "Rematch");
                            _waitingRematch = false;
                            _hasIncomingRematchOffer = false;
                            _incomingRematchFrom = "";
                            ExitMatch(sendSurrenderIfNeeded: false);
                            break;

                        case "REMATCH_SENT":
                            // server confirm đã gửi
                            break;

                        case "RESET_OFFER":
                            _hasIncomingResetOffer = true;
                            _incomingResetFrom = (parts.Length >= 2) ? parts[1] : "Opponent";

                            CloseAllPopups();

                            DialogResult response = MessageBox.Show(
                                $"{_incomingResetFrom} muốn reset ván đấu.\nBạn có đồng ý không?",
                                "Reset",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question
                            );

                            if (response == DialogResult.Yes)
                                tcpClient.Send("RESET_ACCEPT");
                            else
                                tcpClient.Send("RESET_DECLINE");

                            _hasIncomingResetOffer = false;
                            _incomingResetFrom = "";
                            break;

                        case "RESET_EXECUTE":
                            CloseAllPopups();
                            StartReset();
                            break;

                        case "RESET_DECLINED":
                            CloseAllPopups();
                            MessageBox.Show("Đối thủ từ chối Reset. Trận đấu tiếp tục...", "Reset Game");
                            _waitingReset = false;
                            _hasIncomingResetOffer = false;
                            _incomingResetFrom = "";
                            break;

                        case "RESET_SENT":
                            break;
                    }
                }
                catch { }
            });
        }

        private void ExitMatch(bool sendSurrenderIfNeeded)
        {
            CloseAllPopups();

            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;

                if (sendSurrenderIfNeeded && !_gameEnded)
                {
                    try { tcpClient.Send("SURRENDER"); } catch { }
                }
            }

            Close();
        }

        // ================= UI HANDLERS =================
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (myUndoUsed) return;
            if (!ChessBoard.IsMyTurn) return;
            if (tcpClient == null || !tcpClient.IsConnected()) return;

            isMyUndoRequest = true;
            if (btnUndo != null) btnUndo.Enabled = false;
            tcpClient.Send("REQUEST_UNDO");
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
            if (panelChat != null)
                panelChat.Visible = !panelChat.Visible;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            RequestReset();
        }

        // alias handlers nếu Designer gọi dạng btn_...
        private void btn_Undo_Click(object sender, EventArgs e) => btnUndo_Click(sender, e);
        private void btn_Send_Click(object sender, EventArgs e) => btnSend_Click(sender, e);
        private void btn_Exit_Click(object sender, EventArgs e) => btnExit_Click(sender, e);
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
            CloseAllPopups();

            if (tcpClient != null)
                tcpClient.OnMessageReceived -= HandleServerMessage;

            base.OnFormClosing(e);
        }

        private void Btn_Click(object sender, EventArgs e) { }

        // ==========================================================
        // ✅ NESTED DIALOG CLASSES
        // ==========================================================
        private class ResultDialog : Form
        {
            public event Action RematchClicked;
            public event Action ExitClicked;

            public ResultDialog(bool iWon)
            {
                Text = "Kết thúc trận";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterScreen;
                Size = new Size(380, 210);
                TopMost = true;

                var lbl = new Label()
                {
                    AutoSize = false,
                    Dock = DockStyle.Top,
                    Height = 80,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    Text = $"Kết quả: {(iWon ? "Bạn thắng 🎉" : "Bạn thua 😢")}"
                };

                var btnRematch = new Button()
                {
                    Text = "Rematch",
                    Width = 120,
                    Height = 36,
                    Left = 60,
                    Top = 95
                };

                var btnExit = new Button()
                {
                    Text = "Thoát",
                    Width = 120,
                    Height = 36,
                    Left = 200,
                    Top = 95
                };

                btnRematch.Click += (s, e) => RematchClicked?.Invoke();
                btnExit.Click += (s, e) => ExitClicked?.Invoke();

                Controls.Add(lbl);
                Controls.Add(btnRematch);
                Controls.Add(btnExit);
            }
        }

        private class WaitingRematchDialog : Form
        {
            public event Action CancelClicked;

            public WaitingRematchDialog()
            {
                Text = "Rematch";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterScreen;
                Size = new Size(380, 180);
                TopMost = true;

                var lbl = new Label()
                {
                    AutoSize = false,
                    Dock = DockStyle.Top,
                    Height = 70,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Text = "Đã gửi yêu cầu Rematch.\nĐang chờ đối thủ..."
                };

                var btnCancel = new Button()
                {
                    Text = "Huỷ & Thoát",
                    Width = 140,
                    Height = 34,
                    Left = 110,
                    Top = 80
                };

                btnCancel.Click += (s, e) => CancelClicked?.Invoke();

                Controls.Add(lbl);
                Controls.Add(btnCancel);
            }
        }

        private class WaitingResetDialog : Form
        {
            public event Action CancelClicked;

            public WaitingResetDialog()
            {
                Text = "Reset";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterScreen;
                Size = new Size(380, 180);
                TopMost = true;

                var lbl = new Label()
                {
                    AutoSize = false,
                    Dock = DockStyle.Top,
                    Height = 70,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Text = "Đã gửi yêu cầu Reset.\nĐang chờ đối thủ..."
                };

                var btnCancel = new Button()
                {
                    Text = "Hủy",
                    Width = 140,
                    Height = 34,
                    Left = 110,
                    Top = 80
                };

                btnCancel.Click += (s, e) => CancelClicked?.Invoke();

                Controls.Add(lbl);
                Controls.Add(btnCancel);
            }
        }
    }
}
