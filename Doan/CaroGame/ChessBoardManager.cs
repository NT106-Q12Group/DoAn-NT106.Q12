using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard,
        ExtremelyHard
    }

    public enum GameMode
    {
        PvE,
        PvP
    }
    public class Tile
    {
        public Image BackgroundImage { get; set; }

        public Tile()
        {
            BackgroundImage = null; // Khởi tạo các ô cờ
        }
    }

    internal class ChessBoardManager
    {

        #region Properties
        private Panel chessBoard;
        private Stack<Button> moveHistory = new Stack<Button>();
        public int MoveCount => moveHistory.Count;
        private Stack<int> playerHistory = new Stack<int>();
        private bool undoAlready = false;
        private bool undoUsedInBot = false;
        public event Action<Point> PlayerClickedNode;
        public event Action<string> GameEnded;

        public Panel ChesBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        public GameMode CurrentGameMode { get; set; }

        private List<Player> player;
        public List<Player> Player
        {
            get { return player; }
            set { player = value; }
        }
        private int currentPlayer;
        public int CurrentPlayer
        {
            get { return currentPlayer; }
            set { currentPlayer = value; }
        }

        private List<List<Button>> matrix;
        public List<List<Button>> Matrix
        {
            get { return matrix; }
            set { matrix = value; }
        }
        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, GameMode mode = GameMode.PvE)
        {
            this.chessBoard = chessBoard;
            this.CurrentGameMode = mode;

            // --- FIX 1: SAFETY IMAGE LOADER ---
            // This prevents the game from crashing or having null images
            Image imgX = null;
            Image imgO = null;

            try
            {
                // Try loading from file
                string path = Application.StartupPath + "\\Resources\\";
                if (System.IO.File.Exists(path + "Caro Game.png")) imgX = Image.FromFile(path + "Caro Game.png");
                if (System.IO.File.Exists(path + "Caro Game (1).png")) imgO = Image.FromFile(path + "Caro Game (1).png");
            }
            catch { }

            // If loading failed, CREATE a backup image (Red/Blue square)
            // This ensures you never have "null" images causing the green screen bug
            if (imgX == null) imgX = CreateFallbackImage(Color.Red, "X");
            if (imgO == null) imgO = CreateFallbackImage(Color.Blue, "O");

            this.Player = new List<Player>()
            {
                new Player("Player 1", imgX),
                new Player("Player 2", imgO)
            };
            CurrentPlayer = 0;
        }

        private Image CreateFallbackImage(Color color, string text)
        {
            Bitmap bmp = new Bitmap(Cons.CHESS_WIDTH, Cons.CHESS_HEIGHT);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
                using (Pen pen = new Pen(color, 2))
                {
                    g.DrawRectangle(pen, 2, 2, bmp.Width - 4, bmp.Height - 4);
                    if (text == "X")
                    {
                        g.DrawLine(pen, 5, 5, bmp.Width - 5, bmp.Height - 5);
                        g.DrawLine(pen, bmp.Width - 5, 5, 5, bmp.Height - 5);
                    }
                    else
                    {
                        g.DrawEllipse(pen, 5, 5, bmp.Width - 10, bmp.Height - 10);
                    }
                }
            }
            return bmp;
        }
        #endregion

        #region Methods
        private int lastHumanRow = -1;
        private int lastHumanCol = -1;

        private Stopwatch botTimer = new Stopwatch();
        private long timeLimitMillis = 0;
        private bool isTimeOut = false;

        private Button lastMoveBtn = null;
        private Color defaultColor = Color.WhiteSmoke;
        private Color lastMoveColor = Color.Yellow;

        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy;
        public void DrawChessBoard()
        {
            matrix = new List<List<Button>>();

            Button oldButton = new Button()
            {
                Width = 0,
                Location = new Point(0, 0),
            };

            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                matrix.Add(new List<Button>());
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString(),

                        // --- THÊM 2 DÒNG NÀY ---
                        BackColor = defaultColor,
                        FlatStyle = FlatStyle.Flat, // Chuyển sang Flat để màu nền hiện rõ hơn
                                                    // -----------------------
                    };

                    // Muốn đẹp hơn thì thêm viền (Optional)
                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.FlatAppearance.BorderSize = 1;

                    btn.Click += Btn_Click;

                    chessBoard.Controls.Add(btn);

                    matrix[i].Add(btn);

                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;

                //kích hoạt cơ chế undo
                moveHistory = new Stack<Button>();
                playerHistory = new Stack<int>();
                undoAlready = false;
                undoUsedInBot = false;
            }
        }

        private void HighlightMove(Button btn)
        {
            // 1. Trả lại trạng thái cũ
            if (lastMoveBtn != null)
            {
                lastMoveBtn.BackColor = defaultColor;
                lastMoveBtn.FlatAppearance.BorderColor = Color.Silver; // Màu viền thường
                lastMoveBtn.FlatAppearance.BorderSize = 1; // Độ dày viền thường
            }

            // 2. Gán nút mới
            lastMoveBtn = btn;

            // 3. Highlight
            // Cách 1: Vẫn dùng màu nền (nhớ sửa FlatStyle ở trên)
            lastMoveBtn.BackColor = lastMoveColor;

            // Cách 2: Highlight bằng viền đỏ (nếu ảnh che mất màu nền)
            lastMoveBtn.FlatAppearance.BorderColor = Color.Blue;
            lastMoveBtn.FlatAppearance.BorderSize = 3; // Viền dày lên
        }

        // Thêm biến này vào khu vực #region Methods hoặc khai báo biến
        private bool isThinking = false; // Biến khóa bàn cờ khi Bot đang nghĩ

        // --- HÀM XỬ LÝ NƯỚC ĐI TỪ SERVER (FIX BUG LOCK TURN & SAI HÌNH) ---
        public void ProcessMove(int x, int y, int playerSide)
        {
            // Kiểm tra tọa độ
            if (y < 0 || y >= matrix.Count || x < 0 || x >= matrix[0].Count) return;

            // --- [FIX] CHẶN SIDE -1 NGAY LẬP TỨC ---
            // Nếu side < 0, thoát luôn để không vẽ bậy và không check thắng thua tào lao
            if (playerSide < 0 || playerSide >= Player.Count) return;
            // ---------------------------------------

            Button btn = matrix[y][x];
            if (btn.BackgroundImage != null) return;

            // ... (Code phía dưới giữ nguyên) ...
            // Lúc này playerSide chắc chắn đúng, code sẽ chạy mượt.
            btn.BackgroundImage = Player[playerSide].Mark;

            // 2. Highlight và lưu lịch sử
            HighlightMove(btn);
            moveHistory.Push(btn);
            playerHistory.Push(playerSide);

            // 3. Kiểm tra thắng thua
            var winCells = getWinningCells(btn);
            if (winCells != null)
            {
                HighlightWinningCells(winCells);
                EndGame(Player[playerSide].Name);
                return;
            }

            // 4. [FIX] Xử lý chuyển lượt PvP
            if (CurrentGameMode == GameMode.PvP)
            {
                // Nếu người vừa đi (playerSide) KHÁC với phe của mình (MySide)
                // => Tức là đối thủ vừa đi => Giờ đến lượt mình
                if (playerSide != MySide)
                {
                    IsMyTurn = true;
                }
                else
                {
                    // Nếu người vừa đi là mình (Server xác nhận lại) => Hết lượt, chờ đối thủ
                    IsMyTurn = false;
                }
            }
        }

        public int MySide { get; set; } = -1; // -1: Chưa gán, 0: Player 1, 1: Player 2
        public bool IsMyTurn { get; set; } = false;

        private async void Btn_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            if (btn.BackgroundImage != null) return;
            if (CurrentGameMode == GameMode.PvE && isThinking) return;

            // --- PVP LOGIC ---
            if (CurrentGameMode == GameMode.PvP)
            {
                if (!IsMyTurn) return; // Chưa đến lượt thì chặn

                // Lấy tọa độ và gửi lên Server
                Point p = getChessPoint(btn);
                PlayerClickedNode?.Invoke(p);

                // Khóa bàn cờ NGAY LẬP TỨC để tránh spam click
                IsMyTurn = false;

                // KHÔNG vẽ gì cả ở đây. Chờ Server gửi lại lệnh ProcessMove để vẽ.
                return;
            }

            // --- PVE LOGIC (Giữ nguyên) ---
            int markIndex = 0; // Người luôn là 0
            btn.BackgroundImage = Player[markIndex].Mark;
            HighlightMove(btn);
            moveHistory.Push(btn);
            playerHistory.Push(markIndex);

            var winCells = getWinningCells(btn);
            if (winCells != null) { HighlightWinningCells(winCells); EndGame(Player[markIndex].Name); return; }

            if (CurrentGameMode == GameMode.PvE && CurrentPlayer == 0)
            {
                CurrentPlayer = 1;
                isThinking = true;
                await Task.Delay(100);
                await BotPlay();
                isThinking = false;
            }
        }

        private void EndGame(string winnerName)
        {
            GameEnded?.Invoke(winnerName);
        }

        public void resetGame()
        {
            if (lastMoveBtn != null)
            {
                lastMoveBtn.BackColor = defaultColor;
                lastMoveBtn.FlatAppearance.BorderColor = Color.Silver;
                lastMoveBtn.FlatAppearance.BorderSize = 1;
                lastMoveBtn = null;
            }

            foreach (var row in matrix)
            {
                foreach (var btn in row)
                {
                    btn.BackgroundImage = null;
                    btn.BackColor = defaultColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.FlatAppearance.BorderSize = 1;
                }
            }
            CurrentPlayer = 0;
            isThinking = false;
            lastHumanRow = -1; lastHumanCol = -1; isTimeOut = false;

            moveHistory.Clear();
            playerHistory.Clear();
            undoAlready = false;
            undoUsedInBot = false;

            // Reset lượt PvP: X (Side 0) đi trước
            if (CurrentGameMode == GameMode.PvP)
            {
                IsMyTurn = (MySide == 0);
            }
            else
            {
                IsMyTurn = false;
            }
        }

        // --- HÀM UNDO PVP ĐÃ SỬA ---
        public void undoLastMove()
        {
            if (moveHistory.Count == 0) return;

            var btn = moveHistory.Pop();
            btn.BackgroundImage = null;
            btn.BackColor = defaultColor;

            // Xóa highlight cũ
            btn.FlatAppearance.BorderColor = Color.Silver;
            btn.FlatAppearance.BorderSize = 1;

            if (playerHistory.Count > 0) CurrentPlayer = playerHistory.Pop();

            // Highlight lại nước trước đó (nếu còn)
            if (moveHistory.Count > 0)
            {
                lastMoveBtn = moveHistory.Peek();
                lastMoveBtn.FlatAppearance.BorderColor = Color.Blue;
                lastMoveBtn.FlatAppearance.BorderSize = 3;
            }
            else
            {
                lastMoveBtn = null;
            }
        }

        // [FIXED] Cho phép Undo ngay từ nước đầu tiên
        public void ExecuteUndoPvP()
        {
            if (moveHistory.Count == 0) return;

            // Case 1: Mới đi 1 nước mà muốn Undo (quay lại từ đầu)
            if (moveHistory.Count == 1)
            {
                undoLastMove();
                // Bàn cờ trống -> Người cầm X (Side 0) được đi
                IsMyTurn = (MySide == 0);
                return;
            }

            // Case 2: Đã đi nhiều nước -> Undo 2 nước (về lượt mình)
            undoLastMove();
            undoLastMove();

            // Sau khi xóa 2 nước, chắc chắn quay về lượt người xin Undo
            IsMyTurn = true;
        }

        public bool undoTurnPvE()
        {
            if (moveHistory.Count < 2 || undoUsedInBot) return false;
            undoLastMove();
            undoLastMove();
            undoUsedInBot = true;
            return true;
        }

        //Lấy các ô thắng
        private List<Button> getWinningCells(Button btn)
        {
            List<Button> win;

            win = getHorizontalCells(btn);
            if (win != null) return win;

            win = getVerticalCells(btn);
            if (win != null) return win;

            win = getMainDiagonalCells(btn);
            if (win != null) return win;

            win = getSecondaryDiagonalCells(btn);
            if (win != null) return win;

            return null;
        }

        //Lấy vị trí các ô
        private Point getChessPoint(Button btn)
        {
            int column = Convert.ToInt32(btn.Tag);
            int row = matrix[column].IndexOf(btn);
            return new Point(row, column);
        }

        //Các ô cờ thắng vị trí ngang
        private List<Button> getHorizontalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = point.X; i >= 0; i--)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Insert(0, matrix[point.Y][i]);
                }
                else
                {
                    break;
                }

            }

            for (int i = point.X + 1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Add(matrix[point.Y][i]);
                }
                else
                {
                    break;
                }

            }

            if (cells.Count < 5)
            {
                return null;
            }

            return cells.GetRange(0, 5);
        }

        //Các ô cờ thắng vị trí dọc
        private List<Button> getVerticalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = point.Y; i >= 0; i--)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Insert(0, matrix[i][point.X]);
                }
                else
                {
                    break;
                }

            }

            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Add(matrix[i][point.X]);
                }
                else
                {
                    break;
                }

            }

            if (cells.Count < 5)
            {
                return null;
            }

            return cells.GetRange(0, 5);
        }

        //Các ô cờ thắng vị trí chéo chính
        private List<Button> getMainDiagonalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                {
                    break;
                }

                if (matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Insert(0, matrix[point.Y - i][point.X - i]);
                }
                else
                {
                    break;
                }

            }

            for (int i = 1; i <= Cons.CHESS_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X + i >= Cons.CHESS_BOARD_WIDTH)
                {
                    break;
                }

                if (matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Add(matrix[point.Y + i][point.X + i]);
                }
                else
                {
                    break;
                }

            }

            if (cells.Count < 5)
            {
                return null;
            }

            return cells.GetRange(0, 5);
        }

        //Các ô cờ thắng vị trí chéo phụ
        private List<Button> getSecondaryDiagonalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i >= Cons.CHESS_BOARD_WIDTH || point.Y - i < 0)
                {
                    break;
                }

                if (matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Insert(0, matrix[point.Y - i][point.X + i]);
                }
                else
                {
                    break;
                }

            }

            for (int i = 1; i <= Cons.CHESS_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0)
                {
                    break;
                }

                if (matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    cells.Add(matrix[point.Y + i][point.X - i]);
                }
                else
                {
                    break;
                }

            }

            if (cells.Count < 5)
            {
                return null;
            }

            return cells.GetRange(0, 5);
        }

        //Làm nổi bật các ô thắng
        private void HighlightWinningCells(List<Button> cells)
        {
            foreach (var b in cells)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderColor = Color.Lime;
                b.FlatAppearance.BorderSize = 3;
            }
        }
        #region Bot
        long[] AttackScore = new long[] { 0, 10, 100, 1000, 100000, 10000000 };
        long[] DefendScore = new long[] { 0, 12, 120, 1500, 200000, 10000000 };
        // --- CÁC BIẾN CẦN THIẾT ---
        // Khai báo hằng số cho gọn
        const int EMPTY = 0;
        const int HUMAN = 1;
        const int BOT = 2;

        private async Task BotPlay() // Đổi thành async Task
        {
            // --- PHẦN 1: CHUẨN BỊ (Chạy trên UI Thread) ---
            int[,] board = GetIntBoard();

            // Nếu bàn cờ trống
            if (IsBoardEmpty(board))
            {
                MakeMove(Cons.CHESS_BOARD_HEIGHT / 2, Cons.CHESS_BOARD_WIDTH / 2);
                return;
            }

            // Cấu hình độ khó
            int targetDepth = 1;
            if (Difficulty == DifficultyLevel.ExtremelyHard)
            {
                timeLimitMillis = 25000;
                targetDepth = 12;
            }
            else if (Difficulty == DifficultyLevel.Hard)
            {
                timeLimitMillis = 12000;
                targetDepth = 6;
            }
            else
            {
                timeLimitMillis = 2000;
                targetDepth = 2;
            }

            botTimer.Restart();
            isTimeOut = false;

            // --- PHẦN 2: TÍNH TOÁN (Chạy trên Background Thread) ---
            // Đưa toàn bộ việc suy nghĩ nặng nhọc vào Task.Run để không làm đơ ứng dụng
            Point bestMove = await Task.Run(() =>
            {
                List<Point> possibleMoves = GetPossibleMoves(board);
                var orderedMoves = SortMoves(possibleMoves, board);

                // Check thắng ngay
                foreach (var move in orderedMoves)
                {
                    board[move.X, move.Y] = BOT;
                    if (CheckWin(move.X, move.Y, board, BOT)) return move;
                    board[move.X, move.Y] = EMPTY;
                }
                // Check thua ngay
                foreach (var move in orderedMoves)
                {
                    board[move.X, move.Y] = HUMAN;
                    if (CheckWin(move.X, move.Y, board, HUMAN)) return move;
                    board[move.X, move.Y] = EMPTY;
                }

                Point currentFinalMove = orderedMoves[0];

                // Iterative Deepening Loop
                for (int currentDepth = 1; currentDepth <= targetDepth; currentDepth++)
                {
                    long maxScore = long.MinValue;
                    Point currentBestMove = new Point(-1, -1);
                    int movesToScan = (Difficulty >= DifficultyLevel.Hard) ? 25 : 10;

                    for (int i = 0; i < Math.Min(orderedMoves.Count, movesToScan); i++)
                    {
                        if (botTimer.ElapsedMilliseconds > timeLimitMillis) { isTimeOut = true; break; }

                        Point move = orderedMoves[i];
                        board[move.X, move.Y] = BOT;
                        long score = Minimax(board, currentDepth - 1, long.MinValue, long.MaxValue, false);
                        board[move.X, move.Y] = EMPTY;

                        if (score > maxScore)
                        {
                            maxScore = score;
                            currentBestMove = move;
                        }
                    }

                    if (isTimeOut) break;
                    if (currentBestMove.X != -1) currentFinalMove = currentBestMove;
                    if (maxScore > 5000000) break;
                    if (currentDepth == targetDepth) break;
                }

                return currentFinalMove; // Trả về kết quả cho luồng chính
            });

            // --- PHẦN 3: CẬP NHẬT GIAO DIỆN (Quay lại UI Thread) ---
            MakeMove(bestMove.X, bestMove.Y);
        }

        // Hàm kiểm tra nhanh xem tại vị trí (r,c) phe 'player' có thắng không (đủ 5 con)
        private bool CheckWin(int r, int c, int[,] board, int player)
        {
            // Kiểm tra 4 hướng: Ngang, Dọc, Chéo Chính, Chéo Phụ
            if (CountConsecutive(r, c, 1, 0, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 0, 1, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 1, 1, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 1, -1, board, player) >= 5) return true;
            return false;
        }

        // Hàm đếm số quân liên tiếp
        private int CountConsecutive(int r, int c, int dr, int dc, int[,] board, int player)
        {
            int count = 1; // Tính cả quân vừa đánh

            // Loang ra 2 phía
            for (int i = 1; i < 5; i++)
            {
                int nr = r + dr * i; int nc = c + dc * i;
                if (InBounds(nr, nc) && board[nr, nc] == player) count++; else break;
            }
            for (int i = 1; i < 5; i++)
            {
                int nr = r - dr * i; int nc = c - dc * i;
                if (InBounds(nr, nc) && board[nr, nc] == player) count++; else break;
            }
            return count;
        }

        // Hàm sắp xếp nước đi (Move Ordering) tách riêng cho gọn
        private List<Point> SortMoves(List<Point> moves, int[,] board)
        {
            List<KeyValuePair<Point, long>> rankedMoves = new List<KeyValuePair<Point, long>>();
            foreach (var move in moves)
            {
                long atk = DiemTanCong(move.X, move.Y, board);
                long def = DiemPhongThu(move.X, move.Y, board);
                // Điểm heuristic sơ bộ = Công + Thủ
                rankedMoves.Add(new KeyValuePair<Point, long>(move, atk + def));
            }

            // Sắp xếp giảm dần
            rankedMoves.Sort((a, b) => b.Value.CompareTo(a.Value));

            // Trả về list point đã sort
            List<Point> result = new List<Point>();
            foreach (var pair in rankedMoves) result.Add(pair.Key);
            return result;
        }

        // Hàm thực hiện nước đi lên giao diện
        private void MakeMove(int r, int c)
        {
            matrix[r][c].BackgroundImage = Player[1].Mark;
            HighlightMove(matrix[r][c]);
            CurrentPlayer = 0;

            moveHistory.Push(matrix[r][c]);
            playerHistory.Push(1);

            var winCells = getWinningCells(matrix[r][c]);
            if (winCells != null)
            {
                HighlightWinningCells(winCells);
                EndGame(Player[1].Name);
            }
        }

        // Chuyển đổi ma trận Button sang ma trận số nguyên (0, 1, 2)
        private int[,] GetIntBoard()
        {
            int[,] board = new int[Cons.CHESS_BOARD_HEIGHT, Cons.CHESS_BOARD_WIDTH];
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    if (matrix[i][j].BackgroundImage == Player[0].Mark) board[i, j] = HUMAN;
                    else if (matrix[i][j].BackgroundImage == Player[1].Mark) board[i, j] = BOT;
                    else board[i, j] = EMPTY;
                }
            }
            return board;
        }

        private bool IsBoardEmpty(int[,] board)
        {
            foreach (int cell in board) if (cell != EMPTY) return false;
            return true;
        }

        // Lấy danh sách các ô trống có hàng xóm (phạm vi 2 ô)
        // Đây là kỹ thuật "Candidate Generation" giúp loại bỏ 90% ô vô nghĩa
        private List<Point> GetPossibleMoves(int[,] board)
        {
            List<Point> moves = new List<Point>();
            HashSet<int> visited = new HashSet<int>();

            int h = Cons.CHESS_BOARD_HEIGHT;
            int w = Cons.CHESS_BOARD_WIDTH;

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (board[i, j] != EMPTY)
                    {
                        // Mở rộng phạm vi tìm kiếm lên 2 (giữ nguyên là ok, nhưng phải chắc chắn quét đủ)
                        int startR = Math.Max(0, i - 2);
                        int endR = Math.Min(h - 1, i + 2);
                        int startC = Math.Max(0, j - 2);
                        int endC = Math.Min(w - 1, j + 2);

                        for (int r = startR; r <= endR; r++)
                        {
                            for (int c = startC; c <= endC; c++)
                            {
                                if (board[r, c] == EMPTY)
                                {
                                    int key = r * 1000 + c;
                                    if (!visited.Contains(key))
                                    {
                                        visited.Add(key);
                                        moves.Add(new Point(r, c));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return moves;
        }

        private long Minimax(int[,] board, int depth, long alpha, long beta, bool isMaximizing)
        {
            // 1. Kiểm tra hết giờ -> Cắt luôn
            if (isTimeOut || botTimer.ElapsedMilliseconds > timeLimitMillis)
            {
                isTimeOut = true;
                return 0;
            }

            long currentScore = EvaluateBoard(board);
            if (Math.Abs(currentScore) > 5000000) return currentScore;
            if (depth == 0) return currentScore;

            var moves = GetPossibleMoves(board);
            if (moves.Count == 0) return currentScore;

            var orderedMoves = SortMoves(moves, board);

            // MỞ RỘNG TẦM NHÌN:
            // Vì bạn cho nhiều thời gian, ta tăng số lượng nhánh con được duyệt lên.
            // Duyệt 20 nước tốt nhất ở mỗi tầng đệ quy (thay vì 12).
            int movesCheckLimit = 20;

            if (isMaximizing)
            {
                long maxScore = long.MinValue;
                for (int i = 0; i < Math.Min(orderedMoves.Count, movesCheckLimit); i++)
                {
                    Point move = orderedMoves[i];

                    board[move.X, move.Y] = BOT;
                    if (CheckWin(move.X, move.Y, board, BOT))
                    {
                        board[move.X, move.Y] = EMPTY;
                        return 100000000;
                    }

                    long score = Minimax(board, depth - 1, alpha, beta, false);
                    board[move.X, move.Y] = EMPTY;

                    // Nếu bị timeout trong lúc đệ quy con -> thoát luôn
                    if (isTimeOut) return 0;

                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha) break;
                }
                return maxScore;
            }
            else
            {
                long minScore = long.MaxValue;
                for (int i = 0; i < Math.Min(orderedMoves.Count, movesCheckLimit); i++)
                {
                    Point move = orderedMoves[i];

                    board[move.X, move.Y] = HUMAN;
                    if (CheckWin(move.X, move.Y, board, HUMAN))
                    {
                        board[move.X, move.Y] = EMPTY;
                        return -100000000;
                    }

                    long score = Minimax(board, depth - 1, alpha, beta, true);
                    board[move.X, move.Y] = EMPTY;

                    if (isTimeOut) return 0;

                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);
                    if (beta <= alpha) break;
                }
                return minScore;
            }
        }
        private long EvaluateBoard(int[,] board)
        {
            long score = 0;
            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    if (board[i, j] == BOT)
                    {
                        score += DiemTanCong(i, j, board);
                    }
                    else if (board[i, j] == HUMAN)
                    {
                        // Trừ đi sức tấn công của Human (chính là mối đe dọa với Bot)
                        // Lưu ý: Dùng hàm DiemTanCong nhưng cho quân HUMAN
                        score -= DiemTanCongCuaDich(i, j, board);
                    }
                }
            }
            return score;
        }

        // Thêm hàm này: Tính điểm tấn công nhưng là của Human (để EvaluateBoard dùng)
        long DiemTanCongCuaDich(int r, int c, int[,] board)
        {
            long score = 0;
            // Truyền tham số checkForHuman = true
            score += CheckAtkOfHuman(r, c, 1, 0, board);
            score += CheckAtkOfHuman(r, c, 0, 1, board);
            score += CheckAtkOfHuman(r, c, 1, 1, board);
            score += CheckAtkOfHuman(r, c, 1, -1, board);
            return score;
        }

        // Helper cho hàm trên (Logic y hệt CheckAtk nhưng đổi vai trò BOT/HUMAN)
        long CheckAtkOfHuman(int r, int c, int dr, int dc, int[,] board)
        {
            int ta = 0; // "Ta" ở đây là Human
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i; int nc = c + dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) ta++;      // Đổi thành HUMAN
                else if (board[nr, nc] == BOT) { blocks++; break; } // Đổi thành BOT
                else break;
            }
            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i; int nc = c - dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) ta++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && ta < 5) return 0;

            // Dùng AttackScore để đánh giá độ nguy hiểm của địch
            long score = AttackScore[Math.Min(ta, 5)];
            if (blocks == 1) score /= 2;
            if (ta == 3 && blocks == 0) score *= 2;
            if (ta == 4 && blocks == 0) score *= 2;
            if (ta >= 5) score = AttackScore[5];

            return score;
        }

        // --- CẬP NHẬT CÁC HÀM TẤN CÔNG/PHÒNG THỦ DÙNG INT[,] ---
        long DiemTanCong(int r, int c, int[,] board)
        {
            long score = 0;
            score += CheckAtk(r, c, 1, 0, board);
            score += CheckAtk(r, c, 0, 1, board);
            score += CheckAtk(r, c, 1, 1, board); // Chéo chính
            score += CheckAtk(r, c, 1, -1, board); // Chéo phụ
            return score;
        }

        long DiemPhongThu(int r, int c, int[,] board)
        {
            long score = 0;
            score += CheckDef(r, c, 1, 0, board);
            score += CheckDef(r, c, 0, 1, board);
            score += CheckDef(r, c, 1, 1, board);
            score += CheckDef(r, c, 1, -1, board);
            return score;
        }

        // Logic kiểm tra dùng số nguyên (Siêu nhanh)
        long CheckAtk(int r, int c, int dr, int dc, int[,] board)
        {
            int ta = 0;
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i; int nc = c + dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == BOT) ta++;
                else if (board[nr, nc] == HUMAN) { blocks++; break; }
                else break;
            }
            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i; int nc = c - dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == BOT) ta++;
                else if (board[nr, nc] == HUMAN) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && ta < 5) return 0;

            // AttackScore mới của bạn
            long score = AttackScore[Math.Min(ta, 5)];
            if (blocks == 1) score /= 2;
            if (ta == 3 && blocks == 0) score *= 2;
            if (ta == 4 && blocks == 0) score *= 2;
            if (ta >= 5) score = AttackScore[5];

            return score;
        }

        // Hàm xác định khoảng cách tối đa để Bot quan tâm (dùng cho mức Dễ/Trung bình)
        private int GetMaxDistForDifficulty()
        {
            switch (Difficulty)
            {
                case DifficultyLevel.Medium:
                    return 4; // Trung bình: Quan tâm trong 4 ô
                case DifficultyLevel.Hard:
                    return 3;
                case DifficultyLevel.ExtremelyHard:
                    return 2;
                default:
                    return 5; // Dễ: Quan tâm rộng hơn (đánh lan man hơn)
            }
        }

        long CheckDef(int r, int c, int dr, int dc, int[,] board)
        {
            int dich = 0;
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i; int nc = c + dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) dich++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }
            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i; int nc = c - dc * i;
                if (!InBounds(nr, nc)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) dich++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && dich < 5) return 0;

            // DefendScore mới của bạn
            long score = DefendScore[Math.Min(dich, 5)];
            if (dich == 3 && blocks == 0) score *= 3;
            if (dich == 4) score *= 5;

            return score;
        }

        bool InBounds(int r, int c)
        {
            return r >= 0 && r < Cons.CHESS_BOARD_HEIGHT && c >= 0 && c < Cons.CHESS_BOARD_WIDTH;
        }
        #endregion
        #endregion
    }
}