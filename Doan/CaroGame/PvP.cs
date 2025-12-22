using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace CaroGame
{
    public partial class PvP : Form
    {
        private ChessBoardManager ChessBoard;
        public Room room;

        public int MySide { get; set; } // 0: X, 1: O

        private bool isMyUndoRequest = false;
        private bool myUndoUsed = false;
        private bool oppUndoUsed = false;

        // label1 cạnh icon X, label2 cạnh icon O (UI cố định)
        private string player1Name; // X slot
        private string player2Name; // O slot

        private TCPClient tcpClient;

        private bool _gameEnded = false;
        private bool _waitingRematch = false;
        private bool _waitingReset = false;
        private bool _resultSent = false;

        private bool _hasIncomingResetOffer = false;
        private string _incomingResetFrom = "";

        private bool _hasIncomingRematchOffer = false;
        private string _incomingRematchFrom = "";

        private bool _waitingServerAck = false;

        private ResultDialog _resultDialog = null;
        private WaitingRematchDialog _waitingRematchDialog = null;
        private WaitingResetDialog _waitingResetDialog = null;

        // Undo waiting UI
        private bool _waitingUndo = false;
        private WaitingUndoDialog _waitingUndoDialog = null;

        private readonly System.Windows.Forms.Timer _turnFillTimer = new System.Windows.Forms.Timer();
        private int _turnSide = 0; // 0: X, 1: O

        private readonly ToolTip _nameTip = new ToolTip();

        public PvP(Room room, int mySide, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = mySide;

            this.player1Name = p1;
            this.player2Name = p2;

            this.tcpClient = client;

            this.Load += (_, __) => DisableAllCountdownTimersHard();
            this.Shown += (_, __) =>
            {
                DisableAllCountdownTimersHard();
                EnsureNameLabelsLayout();
            };

            this.Resize += (_, __) => EnsureNameLabelsLayout();

            InitGame();
        }

        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = (playerNumber == 1) ? 0 : 1;
            this.player1Name = "Player 1";
            this.player2Name = "Player 2";

            this.Load += (_, __) => DisableAllCountdownTimersHard();
            this.Shown += (_, __) =>
            {
                DisableAllCountdownTimersHard();
                EnsureNameLabelsLayout();
            };

            this.Resize += (_, __) => EnsureNameLabelsLayout();

            InitGame();
        }

        private void DisableAllCountdownTimersHard()
        {
            try
            {
                var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var fields = this.GetType().GetFields(flags);

                foreach (var f in fields)
                {
                    if (f.GetValue(this) is System.Windows.Forms.Timer t)
                    {
                        if (ReferenceEquals(t, _turnFillTimer)) continue;
                        HardKillTimer(t);
                    }
                }

                if (components != null)
                {
                    foreach (IComponent c in components.Components)
                    {
                        if (c is System.Windows.Forms.Timer t2)
                        {
                            if (ReferenceEquals(t2, _turnFillTimer)) continue;
                            HardKillTimer(t2);
                        }
                    }
                }
            }
            catch { }
        }

        private void HardKillTimer(System.Windows.Forms.Timer t)
        {
            try
            {
                t.Stop();
                t.Enabled = false;
                t.Interval = int.MaxValue;
                RemoveAllTimerTickHandlers(t);
            }
            catch { }
        }

        private void RemoveAllTimerTickHandlers(System.Windows.Forms.Timer timer)
        {
            try
            {
                var timerType = typeof(System.Windows.Forms.Timer);
                var keyField = timerType.GetField("EVENT_TICK", BindingFlags.NonPublic | BindingFlags.Static);
                if (keyField == null) return;

                object tickKey = keyField.GetValue(null);

                var eventsProp = typeof(Component).GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
                if (eventsProp == null) return;

                var list = eventsProp.GetValue(timer) as EventHandlerList;
                if (list == null) return;

                list.RemoveHandler(tickKey, list[tickKey]);
            }
            catch { }
        }

        private void SetupProgressBars()
        {
            if (pgbP1 != null)
            {
                pgbP1.Visible = true;
                pgbP1.Minimum = 0;
                pgbP1.Maximum = 100;
                pgbP1.Value = 0;
                pgbP1.Style = ProgressBarStyle.Blocks;
            }

            if (pgbP2 != null)
            {
                pgbP2.Visible = true;
                pgbP2.Minimum = 0;
                pgbP2.Maximum = 100;
                pgbP2.Value = 0;
                pgbP2.Style = ProgressBarStyle.Blocks;
            }

            _turnFillTimer.Stop();
            _turnFillTimer.Interval = 30;
            _turnFillTimer.Tick -= TurnFillTimer_Tick;
            _turnFillTimer.Tick += TurnFillTimer_Tick;
            _turnFillTimer.Start();
        }

        private void TurnFillTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_gameEnded) return;

                if (_turnSide == 0)
                {
                    if (pgbP1 != null && pgbP1.Value < 100) pgbP1.Value++;
                    if (pgbP2 != null) pgbP2.Value = 0;
                }
                else
                {
                    if (pgbP2 != null && pgbP2.Value < 100) pgbP2.Value++;
                    if (pgbP1 != null) pgbP1.Value = 0;
                }
            }
            catch { }
        }

        private void TurnUIBySide(int turnSide)
        {
            _turnSide = (turnSide == 0) ? 0 : 1;
            try
            {
                if (pgbP1 != null) pgbP1.Value = 0;
                if (pgbP2 != null) pgbP2.Value = 0;
            }
            catch { }
        }

        private void TurnUIEnd()
        {
            try
            {
                if (pgbP1 != null) pgbP1.Value = 0;
                if (pgbP2 != null) pgbP2.Value = 0;
            }
            catch { }
        }

        private void ApplyTurnAfterMove(int lastMoveSide)
        {
            int nextSide = 1 - lastMoveSide;
            ChessBoard.IsMyTurn = (nextSide == MySide);
            TurnUIBySide(nextSide);
        }

        private int GetMoverSideFromServer(int receivedSide)
        {
            int expectedMover = ChessBoard.MoveCount % 2; // 0:X, 1:O
            if (receivedSide != 0 && receivedSide != 1) return expectedMover;
            if (receivedSide == expectedMover) return receivedSide;
            return expectedMover;
        }

        private void InitGame()
        {
            CheckForIllegalCrossThreadCalls = false;

            SetupEmojiPickerPanel();
            if (pnlChessBoard != null) pnlChessBoard.BringToFront();

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;

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
            EnsureNameLabelsLayout();

            DisableAllCountdownTimersHard();
            SetupProgressBars();

            _waitingServerAck = false;
            _gameEnded = false;

            ChessBoard.IsMyTurn = (this.MySide == 0);
            TurnUIBySide(0);

            this.Text = $"PvP - Bạn là {(this.MySide == 0 ? "X (Đi trước)" : "O (Đi sau)")}";
            ChessBoard.DrawChessBoard();
        }

        private void SetupPlayerInfo()
        {
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

            EnsureNameLabelsLayout();
        }

        private void SyncChessBoardNamesWithUI()
        {
            if (ChessBoard?.Player != null && ChessBoard.Player.Count >= 2)
            {
                ChessBoard.Player[0].Name = player1Name;
                ChessBoard.Player[1].Name = player2Name;
            }
        }

        private void SwapNamesForStaticIcons()
        {
            string tmp = player1Name;
            player1Name = player2Name;
            player2Name = tmp;

            SetupPlayerInfo();
            SyncChessBoardNamesWithUI();
        }

        private void EnsureNameLabelsLayout()
        {
            try
            {
                FixOneNameLabel(label1);
                FixOneNameLabel(label2);
            }
            catch { }
        }

        private void FixOneNameLabel(Label lbl)
        {
            if (lbl == null) return;
            if (lbl.Parent == null) return;

            lbl.Visible = true;                 // đảm bảo không bị hide
            lbl.AutoSize = false;
            lbl.AutoEllipsis = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.UseCompatibleTextRendering = true;

            int padding = 8;

            // 1) tìm icon PictureBox nằm bên trái label (icon X/O)
            PictureBox leftIcon = null;

            foreach (Control c in lbl.Parent.Controls)
            {
                if (!c.Visible) continue;
                if (c is PictureBox pb)
                {
                    // ưu tiên picturebox nào nằm cùng hàng (overlap theo Y) và gần bên trái label
                    bool overlapY = pb.Top < lbl.Bottom && pb.Bottom > lbl.Top;

                    if (!overlapY) continue;

                    // pb nằm bên trái label
                    if (pb.Right <= lbl.Left + 30) // +30 để bắt trường hợp label đang bị kéo lệch
                    {
                        if (leftIcon == null || pb.Right > leftIcon.Right)
                            leftIcon = pb;
                    }
                }
            }

            // 2) đặt Left của label theo icon (nếu tìm thấy)
            if (leftIcon != null)
            {
                lbl.Left = leftIcon.Right + padding;
            }

            // 3) tính right limit: không đè lên control bên phải
            int rightLimit = lbl.Parent.ClientSize.Width - padding;

            foreach (Control c in lbl.Parent.Controls)
            {
                if (c == lbl || !c.Visible) continue;

                bool overlapY = c.Top < lbl.Bottom && c.Bottom > lbl.Top;
                if (!overlapY) continue;

                if (c.Left > lbl.Left)
                    rightLimit = Math.Min(rightLimit, c.Left - padding);
            }

            // 4) set Width: tên ngắn thì fit content, tên dài thì maxWidth + ellipsis
            int maxWidth = rightLimit - lbl.Left;
            if (maxWidth < 120) maxWidth = 120;

            // đo text width (cộng thêm chút đệm)
            int textW = TextRenderer.MeasureText(lbl.Text ?? "", lbl.Font).Width + 12;

            // label width = min(textW, maxWidth) nhưng không nhỏ hơn 120
            int desiredW = Math.Min(textW, maxWidth);
            if (desiredW < 120) desiredW = 120;

            lbl.Width = desiredW;

            // 5) set Height theo font
            int needH = TextRenderer.MeasureText("Ag", lbl.Font).Height + 2;
            if (lbl.Height < needH) lbl.Height = needH;

            // 6) luôn nổi lên trên
            lbl.BringToFront();

            // tooltip
            _nameTip.SetToolTip(lbl, lbl.Text ?? "");
        }

        private void CloseAllPopups()
        {
            try { if (_resultDialog != null && !_resultDialog.IsDisposed) _resultDialog.Close(); } catch { }
            try { if (_waitingRematchDialog != null && !_waitingRematchDialog.IsDisposed) _waitingRematchDialog.Close(); } catch { }
            try { if (_waitingResetDialog != null && !_waitingResetDialog.IsDisposed) _waitingResetDialog.Close(); } catch { }
            try { if (_waitingUndoDialog != null && !_waitingUndoDialog.IsDisposed) _waitingUndoDialog.Close(); } catch { }

            _resultDialog = null;
            _waitingRematchDialog = null;
            _waitingResetDialog = null;
            _waitingUndoDialog = null;

            _waitingUndo = false;
        }

        private void CloseUndoPopupOnly()
        {
            try { if (_waitingUndoDialog != null && !_waitingUndoDialog.IsDisposed) _waitingUndoDialog.Close(); } catch { }
            _waitingUndoDialog = null;
            _waitingUndo = false;
        }

        private void ShowOpponentLeftAndExit()
        {
            CloseAllPopups();
            MessageBox.Show("Đối thủ đã thoát. Bạn đã chiến thắng! Trận đấu sẽ kết thúc.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ExitMatch(sendSurrenderIfNeeded: false);
        }

        private void ChessBoard_GameEnded(string winnerRaw)
        {
            if (_gameEnded) return;
            _gameEnded = true;

            TurnUIEnd();

            bool iWon = ComputeWinByWinnerRaw(winnerRaw);
            SendGameResultOnce(iWon);

            CloseAllPopups();

            _resultDialog = new ResultDialog(iWon);

            _resultDialog.RematchClicked += () =>
            {
                try { if (_resultDialog != null && !_resultDialog.IsDisposed) _resultDialog.Close(); } catch { }
                _resultDialog = null;
                RematchFlow();
            };

            _resultDialog.ExitClicked += () =>
            {
                try { if (_resultDialog != null && !_resultDialog.IsDisposed) _resultDialog.Close(); } catch { }
                _resultDialog = null;
                ExitMatch(sendSurrenderIfNeeded: false);
            };

            _resultDialog.FormClosed += (s, e) => { _resultDialog = null; };
            _resultDialog.Show(this);
        }

        private bool ComputeWinByWinnerRaw(string winnerRaw)
        {
            string w = (winnerRaw ?? "").Trim();
            if (string.IsNullOrEmpty(w)) return false;

            if (string.Equals(w, "X", StringComparison.OrdinalIgnoreCase)) return MySide == 0;
            if (string.Equals(w, "O", StringComparison.OrdinalIgnoreCase)) return MySide == 1;

            if (string.Equals(w, player1Name, StringComparison.OrdinalIgnoreCase)) return MySide == 0;
            if (string.Equals(w, player2Name, StringComparison.OrdinalIgnoreCase)) return MySide == 1;

            return false;
        }

        private void SendGameResultOnce(bool iWon)
        {
            if (_resultSent) return;
            _resultSent = true;

            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.Send($"GAME_RESULT|{(iWon ? "WIN" : "LOSE")}");
        }

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

                if (res == DialogResult.Yes) tcpClient.Send("REMATCH_ACCEPT");
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
                try { if (_waitingRematchDialog != null && !_waitingRematchDialog.IsDisposed) _waitingRematchDialog.Close(); } catch { }
                _waitingRematchDialog = null;

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
            _waitingServerAck = false;

            _hasIncomingRematchOffer = false;
            _incomingRematchFrom = "";

            int oldSide = MySide;
            MySide = ((sideRaw ?? "").Trim().ToUpper() == "X") ? 0 : 1;

            ChessBoard.resetGame();
            ChessBoard.MySide = MySide;

            DisableAllCountdownTimersHard();
            SetupProgressBars();

            ChessBoard.IsMyTurn = (MySide == 0);
            TurnUIBySide(0);

            myUndoUsed = false;
            oppUndoUsed = false;
            isMyUndoRequest = false;
            if (btnUndo != null) btnUndo.Enabled = true;

            if (ptbOne != null) ptbOne.Visible = true;
            if (ptbZero != null) ptbZero.Visible = false;

            if (oldSide != MySide) SwapNamesForStaticIcons();
            else
            {
                SetupPlayerInfo();
                SyncChessBoardNamesWithUI();
            }

            EnsureNameLabelsLayout();
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
                try { if (_waitingResetDialog != null && !_waitingResetDialog.IsDisposed) _waitingResetDialog.Close(); } catch { }
                _waitingResetDialog = null;

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

            try { tcpClient.Send("RESET_REQUEST"); } catch { }
        }

        private void StartReset()
        {
            CloseAllPopups();

            _gameEnded = false;
            _waitingReset = false;
            _resultSent = false;
            _waitingServerAck = false;

            _hasIncomingResetOffer = false;
            _incomingResetFrom = "";

            ChessBoard.resetGame();

            DisableAllCountdownTimersHard();
            SetupProgressBars();

            ChessBoard.IsMyTurn = (MySide == 0);
            TurnUIBySide(0);

            myUndoUsed = false;
            oppUndoUsed = false;
            isMyUndoRequest = false;
            if (btnUndo != null) btnUndo.Enabled = true;

            if (ptbOne != null) ptbOne.Visible = true;
            if (ptbZero != null) ptbZero.Visible = false;

            SetupPlayerInfo();
            SyncChessBoardNamesWithUI();
            EnsureNameLabelsLayout();
        }

        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (_waitingServerAck) return;

            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.SendPacket(new Packet("MOVE", point));

            _waitingServerAck = true;
            ChessBoard.IsMyTurn = false;
        }

        private void ShowUndoWaitingDialog()
        {
            if (_waitingUndoDialog != null && !_waitingUndoDialog.IsDisposed) return;

            _waitingUndo = true;
            _waitingUndoDialog = new WaitingUndoDialog();
            _waitingUndoDialog.CancelClicked += () =>
            {
                // huỷ pending trên server
                try { if (tcpClient != null && tcpClient.IsConnected()) tcpClient.Send("UNDO_CANCEL"); } catch { }

                _waitingServerAck = false;
                isMyUndoRequest = false;
                _waitingUndo = false;

                try { if (_waitingUndoDialog != null && !_waitingUndoDialog.IsDisposed) _waitingUndoDialog.Close(); } catch { }
                _waitingUndoDialog = null;

                if (!myUndoUsed && btnUndo != null) btnUndo.Enabled = true;
            };
            _waitingUndoDialog.FormClosed += (s, e) =>
            {
                _waitingUndoDialog = null;
                _waitingUndo = false;
            };
            _waitingUndoDialog.Show(this);
        }

        private void HandleServerMessage(string data)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
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
                                int receivedSide = int.Parse(parts[3]);

                                int moverSide = GetMoverSideFromServer(receivedSide);

                                ChessBoard.ProcessMove(x, y, moverSide);

                                _waitingServerAck = false;
                                ApplyTurnAfterMove(moverSide);
                                break;
                            }

                        case "CHAT":
                            {
                                if (parts.Length >= 2)
                                    AppendMessage(parts.Length > 2 ? parts[2] : "Opponent", parts[1], Color.Red);
                                break;
                            }

                        case "UNDO_OFFER":
                            {
                                string from = (parts.Length >= 2) ? parts[1] : "Opponent";

                                DialogResult res = MessageBox.Show(
                                    $"{from} muốn Undo 1 nước.\nBạn có đồng ý không?",
                                    "Undo request",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                if (res == DialogResult.Yes) tcpClient.Send("UNDO_ACCEPT");
                                else tcpClient.Send("UNDO_DECLINE");
                                break;
                            }

                        case "UNDO_SENT":
                            {
                                // requester side: show waiting UI (nếu chưa show)
                                if (isMyUndoRequest) ShowUndoWaitingDialog();
                                break;
                            }

                        case "UNDO_NOT_AVAILABLE":
                            {
                                CloseUndoPopupOnly();
                                _waitingServerAck = false;
                                isMyUndoRequest = false;

                                if (!myUndoUsed && btnUndo != null) btnUndo.Enabled = true;

                                string msg = parts.Length >= 2 ? parts[1] : "Chưa đủ điều kiện Undo.";
                                MessageBox.Show(msg, "Undo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                        case "UNDO_DECLINED":
                            {
                                CloseUndoPopupOnly();

                                _waitingServerAck = false;
                                isMyUndoRequest = false;

                                if (!myUndoUsed && btnUndo != null) btnUndo.Enabled = true;

                                MessageBox.Show("Đối thủ từ chối Undo.", "Undo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                        case "UNDO_CANCELLED":
                            {
                                // receiver side được báo requester đã huỷ
                                // parts[1] = requester
                                MessageBox.Show("Đối thủ đã huỷ yêu cầu Undo.", "Undo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            }

                        case "UNDO_CANCELLED_SELF":
                            {
                                // requester side: server confirm đã huỷ
                                CloseUndoPopupOnly();
                                break;
                            }

                        case "UNDO_SUCCESS":
                            {
                                CloseUndoPopupOnly();

                                ChessBoard.ExecuteUndoPvP();
                                _waitingServerAck = false;

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
                                    if (!myUndoUsed && btnUndo != null) btnUndo.Enabled = true;
                                }

                                isMyUndoRequest = false;

                                int nextSide = ChessBoard.MoveCount % 2;
                                TurnUIBySide(nextSide);
                                ChessBoard.IsMyTurn = (MySide == nextSide);
                                break;
                            }

                        case "OPPONENT_LEFT":
                            {
                                ShowOpponentLeftAndExit();
                                break;
                            }

                        case "REMATCH_OFFER":
                            {
                                _hasIncomingRematchOffer = true;
                                _incomingRematchFrom = (parts.Length >= 2) ? parts[1] : "Opponent";

                                CloseAllPopups();

                                DialogResult res = MessageBox.Show(
                                    $"{_incomingRematchFrom} muốn Rematch.\nBạn có đồng ý không?",
                                    "Rematch",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                if (res == DialogResult.Yes) tcpClient.Send("REMATCH_ACCEPT");
                                else
                                {
                                    tcpClient.Send("REMATCH_DECLINE");
                                    ExitMatch(sendSurrenderIfNeeded: false);
                                }

                                _hasIncomingRematchOffer = false;
                                _incomingRematchFrom = "";
                                break;
                            }

                        case "REMATCH_START":
                            {
                                CloseAllPopups();
                                if (parts.Length >= 2) StartRematch(parts[1]);
                                break;
                            }

                        case "REMATCH_DECLINED":
                            {
                                CloseAllPopups();
                                MessageBox.Show("Đối thủ từ chối Rematch. Trận đấu kết thúc!", "Rematch");
                                _waitingRematch = false;
                                _hasIncomingRematchOffer = false;
                                _incomingRematchFrom = "";
                                ExitMatch(sendSurrenderIfNeeded: false);
                                break;
                            }

                        case "RESET_OFFER":
                            {
                                _hasIncomingResetOffer = true;
                                _incomingResetFrom = (parts.Length >= 2) ? parts[1] : "Opponent";

                                CloseAllPopups();

                                DialogResult response = MessageBox.Show(
                                    $"{_incomingResetFrom} muốn reset ván đấu.\nBạn có đồng ý không?",
                                    "Reset Game",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question
                                );

                                if (response == DialogResult.Yes) tcpClient.Send("RESET_ACCEPT");
                                else tcpClient.Send("RESET_DECLINE");

                                _hasIncomingResetOffer = false;
                                _incomingResetFrom = "";
                                break;
                            }

                        case "RESET_EXECUTE":
                            {
                                CloseAllPopups();
                                StartReset();
                                break;
                            }

                        case "RESET_DECLINED":
                            {
                                CloseAllPopups();
                                MessageBox.Show("Đối thủ từ chối Reset. Trận đấu tiếp tục...", "Reset Game");
                                _waitingReset = false;
                                _hasIncomingResetOffer = false;
                                _incomingResetFrom = "";
                                break;
                            }
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

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (myUndoUsed) return;
            if (!ChessBoard.IsMyTurn) return;
            if (_waitingServerAck) return;
            if (tcpClient == null || !tcpClient.IsConnected()) return;

            // Luật: cả 2 bên phải đi ít nhất 1 nước (tổng >= 2)
            if (ChessBoard.MoveCount < 2)
            {
                MessageBox.Show("Chưa thể Undo: cả 2 bên phải đi ít nhất 1 nước trước.", "Undo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            isMyUndoRequest = true;
            _waitingServerAck = true;

            if (btnUndo != null) btnUndo.Enabled = false;

            // show “đã gửi yêu cầu undo, đang chờ phản hồi…”
            ShowUndoWaitingDialog();

            tcpClient.Send("UNDO_REQUEST");
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
                "Bạn có chắc chắn muốn thoát trận đấu?\nThoát giữa trận sẽ bị xử thua!",
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

            try
            {
                _turnFillTimer.Stop();
                _turnFillTimer.Tick -= TurnFillTimer_Tick;
            }
            catch { }

            if (tcpClient != null)
                tcpClient.OnMessageReceived -= HandleServerMessage;

            base.OnFormClosing(e);
        }

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

        private class WaitingUndoDialog : Form
        {
            public event Action CancelClicked;

            public WaitingUndoDialog()
            {
                Text = "Undo";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                StartPosition = FormStartPosition.CenterScreen;
                Size = new Size(380, 170);
                TopMost = true;

                var lbl = new Label()
                {
                    AutoSize = false,
                    Dock = DockStyle.Top,
                    Height = 70,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Text = "Đã gửi yêu cầu Undo.\nĐang chờ phản hồi từ đối thủ..."
                };

                var btnCancel = new Button()
                {
                    Text = "Hủy",
                    Width = 120,
                    Height = 34,
                    Left = 130,
                    Top = 85
                };

                btnCancel.Click += (s, e) => CancelClicked?.Invoke();

                Controls.Add(lbl);
                Controls.Add(btnCancel);
            }
        }
    }
}
