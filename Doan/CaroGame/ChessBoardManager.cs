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
            BackgroundImage = null;
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

            Image imgX = null;
            Image imgO = null;

            try
            {
                string path = Application.StartupPath + "\\Resources\\";
                if (System.IO.File.Exists(path + "Caro Game.png")) imgX = Image.FromFile(path + "Caro Game.png");
                if (System.IO.File.Exists(path + "Caro Game (1).png")) imgO = Image.FromFile(path + "Caro Game (1).png");
            }
            catch { }

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
            chessBoard.SuspendLayout();

            chessBoard.Controls.Clear(); // <- quan trọng: xóa bàn cũ
            matrix = new List<List<Button>>();

            // reset history 1 lần thôi
            moveHistory.Clear();
            playerHistory.Clear();
            undoAlready = false;
            undoUsedInBot = false;
            lastMoveBtn = null;

            // Resize panel cho đúng 20x20 ô
            chessBoard.AutoScroll = false; // hoặc true nếu panel bị nhỏ
            chessBoard.Width = Cons.CHESS_BOARD_WIDTH * Cons.CHESS_WIDTH;
            chessBoard.Height = Cons.CHESS_BOARD_HEIGHT * Cons.CHESS_HEIGHT;

            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                matrix.Add(new List<Button>());

                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.CHESS_WIDTH,
                        Height = Cons.CHESS_HEIGHT,
                        Location = new Point(j * Cons.CHESS_WIDTH, i * Cons.CHESS_HEIGHT), // đơn giản + đúng
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString(),
                        BackColor = defaultColor,
                        FlatStyle = FlatStyle.Flat,
                    };

                    btn.FlatAppearance.BorderColor = Color.Silver;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.Click += Btn_Click;

                    chessBoard.Controls.Add(btn);
                    matrix[i].Add(btn);
                }
            }

            chessBoard.ResumeLayout();
        }

        private void HighlightMove(Button btn)
        {
            if (lastMoveBtn != null)
            {
                lastMoveBtn.BackColor = defaultColor;
                lastMoveBtn.FlatAppearance.BorderColor = Color.Silver;
                lastMoveBtn.FlatAppearance.BorderSize = 1;
            }

            lastMoveBtn = btn;
            lastMoveBtn.BackColor = lastMoveColor;
            lastMoveBtn.FlatAppearance.BorderColor = Color.Blue;
            lastMoveBtn.FlatAppearance.BorderSize = 3;
        }

        private bool isThinking = false;

        public void ProcessMove(int x, int y, int playerSide)
        {
            if (y < 0 || y >= matrix.Count || x < 0 || x >= matrix[0].Count) return;

            // Tự tính toán phe dựa trên lịch sử nước đi để tránh tin tưởng hoàn toàn vào server
            // Nước chẵn (0, 2, 4...) là X, lẻ là O
            int autoSide = moveHistory.Count % 2;

            Button btn = matrix[y][x];
            if (btn.BackgroundImage != null) return;

            btn.BackgroundImage = Player[autoSide].Mark;

            HighlightMove(btn);
            moveHistory.Push(btn);
            playerHistory.Push(autoSide);

            var winCells = getWinningCells(btn);
            if (winCells != null)
            {
                HighlightWinningCells(winCells);
                EndGame(Player[autoSide].Name);
                return;
            }

            if (CurrentGameMode == GameMode.PvP)
            {
                // Nếu mình vừa đánh (autoSide trùng MySide) -> Khóa lượt
                // Nếu đối thủ vừa đánh -> Mở lượt
                if (autoSide == MySide)
                {
                    IsMyTurn = false;
                }
                else
                {
                    IsMyTurn = true;
                }
            }
        }

        public int MySide { get; set; } = -1; // 0: Player 1, 1: Player 2
        public bool IsMyTurn { get; set; } = false;

        private async void Btn_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            if (btn.BackgroundImage != null) return;
            if (CurrentGameMode == GameMode.PvE && isThinking) return;

            if (CurrentGameMode == GameMode.PvP)
            {
                if (!IsMyTurn) return;

                Point p = getChessPoint(btn);
                PlayerClickedNode?.Invoke(p); // Gửi tọa độ lên Server

                IsMyTurn = false; // Khóa ngay lập tức, đợi Server phản hồi mới vẽ
                return;
            }

            // PVE LOGIC
            int markIndex = 0;
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
                    btn.Enabled = true;
                }
            }
            CurrentPlayer = 0;
            isThinking = false;
            lastHumanRow = -1; lastHumanCol = -1; isTimeOut = false;

            moveHistory.Clear();
            playerHistory.Clear();
            undoAlready = false;
            undoUsedInBot = false;

            if (CurrentGameMode == GameMode.PvP)
            {
                IsMyTurn = (MySide == 0);
            }
            else
            {
                IsMyTurn = false;
            }
        }

        public void undoLastMove()
        {
            if (moveHistory.Count == 0) return;

            var btn = moveHistory.Pop();
            btn.BackgroundImage = null;
            btn.BackColor = defaultColor;
            btn.FlatAppearance.BorderColor = Color.Silver;
            btn.FlatAppearance.BorderSize = 1;

            if (playerHistory.Count > 0) CurrentPlayer = playerHistory.Pop();

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

        public void ExecuteUndoPvP(bool isRequester)
        {
            if (moveHistory.Count < 2) return;

            undoLastMove();
            undoLastMove();

            IsMyTurn = isRequester;
        }

        public bool undoTurnPvE()
        {
            if (moveHistory.Count < 2 || undoUsedInBot) return false;
            undoLastMove();
            undoLastMove();
            undoUsedInBot = true;
            return true;
        }

        private List<Button> getWinningCells(Button btn)
        {
            List<Button> win;
            win = getHorizontalCells(btn); if (win != null) return win;
            win = getVerticalCells(btn); if (win != null) return win;
            win = getMainDiagonalCells(btn); if (win != null) return win;
            win = getSecondaryDiagonalCells(btn); if (win != null) return win;
            return null;
        }

        private Point getChessPoint(Button btn)
        {
            int column = Convert.ToInt32(btn.Tag);
            int row = matrix[column].IndexOf(btn);
            return new Point(row, column);
        }

        private List<Button> getHorizontalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = point.X; i >= 0; i--)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage) cells.Insert(0, matrix[point.Y][i]);
                else break;
            }
            for (int i = point.X + 1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage) cells.Add(matrix[point.Y][i]);
                else break;
            }
            if (cells.Count < 5) return null;
            return cells.GetRange(0, 5);
        }

        private List<Button> getVerticalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            for (int i = point.Y; i >= 0; i--)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage) cells.Insert(0, matrix[i][point.X]);
                else break;
            }
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage) cells.Add(matrix[i][point.X]);
                else break;
            }
            if (cells.Count < 5) return null;
            return cells.GetRange(0, 5);
        }

        private List<Button> getMainDiagonalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            // lên-trái
            for (int i = 0; ; i++)
            {
                int x = point.X - i;
                int y = point.Y - i;
                if (x < 0 || y < 0) break;
                if (matrix[y][x].BackgroundImage == btn.BackgroundImage) cells.Insert(0, matrix[y][x]);
                else break;
            }

            // xuống-phải
            for (int i = 1; ; i++)
            {
                int x = point.X + i;
                int y = point.Y + i;
                if (y >= Cons.CHESS_BOARD_HEIGHT || x >= Cons.CHESS_BOARD_WIDTH) break;
                if (matrix[y][x].BackgroundImage == btn.BackgroundImage) cells.Add(matrix[y][x]);
                else break;
            }

            if (cells.Count < 5) return null;
            return cells.GetRange(0, 5);
        }

        private List<Button> getSecondaryDiagonalCells(Button btn)
        {
            Point point = getChessPoint(btn);
            List<Button> cells = new List<Button>();

            // lên-phải
            for (int i = 0; ; i++)
            {
                int x = point.X + i;
                int y = point.Y - i;
                if (x >= Cons.CHESS_BOARD_WIDTH || y < 0) break;
                if (matrix[y][x].BackgroundImage == btn.BackgroundImage) cells.Insert(0, matrix[y][x]);
                else break;
            }

            // xuống-trái
            for (int i = 1; ; i++)
            {
                int x = point.X - i;
                int y = point.Y + i;
                if (y >= Cons.CHESS_BOARD_HEIGHT || x < 0) break;
                if (matrix[y][x].BackgroundImage == btn.BackgroundImage) cells.Add(matrix[y][x]);
                else break;
            }

            if (cells.Count < 5) return null;
            return cells.GetRange(0, 5);
        }

        private void HighlightWinningCells(List<Button> cells)
        {
            foreach (var b in cells)
            {
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderColor = Color.Lime;
                b.FlatAppearance.BorderSize = 3;
            }
        }

        #region Bot AI
        // Bảng điểm tấn công và phòng thủ để đánh giá thế cờ
        long[] AttackScore = new long[] { 0, 10, 100, 1000, 100000, 10000000 };
        long[] DefendScore = new long[] { 0, 12, 120, 1500, 200000, 10000000 };
        const int EMPTY = 0;
        const int HUMAN = 1;
        const int BOT = 2;

        private async Task BotPlay()
        {
            int[,] board = GetIntBoard(); // size = matrix size thật

            if (IsBoardEmpty(board))
            {
                int midR = board.GetLength(0) / 2;
                int midC = board.GetLength(1) / 2;
                MakeMove(midR, midC);
                return;
            }

            int targetDepth = 1;
            if (Difficulty == DifficultyLevel.ExtremelyHard) { timeLimitMillis = 25000; targetDepth = 12; }
            else if (Difficulty == DifficultyLevel.Hard) { timeLimitMillis = 12000; targetDepth = 6; }
            else { timeLimitMillis = 2000; targetDepth = 2; }

            botTimer.Restart();
            isTimeOut = false;

            Point bestMove = await Task.Run(() =>
            {
                List<Point> possibleMoves = GetPossibleMoves(board);
                var orderedMoves = SortMoves(possibleMoves, board);

                foreach (var move in orderedMoves)
                {
                    board[move.X, move.Y] = BOT;
                    if (CheckWin(move.X, move.Y, board, BOT)) return move;
                    board[move.X, move.Y] = EMPTY;
                }

                foreach (var move in orderedMoves)
                {
                    board[move.X, move.Y] = HUMAN;
                    if (CheckWin(move.X, move.Y, board, HUMAN)) return move;
                    board[move.X, move.Y] = EMPTY;
                }

                Point currentFinalMove = orderedMoves.Count > 0 ? orderedMoves[0] : new Point(-1, -1);

                for (int currentDepth = 1; currentDepth <= targetDepth; currentDepth++)
                {
                    long maxScore = long.MinValue;
                    Point currentBestMove = new Point(-1, -1);
                    int movesToScan = (Difficulty >= DifficultyLevel.Hard) ? 25 : 10;

                    for (int i = 0; i < Math.Min(orderedMoves.Count, movesToScan); i++)
                    {
                        if (botTimer.ElapsedMilliseconds > timeLimitMillis) { isTimeOut = true; break; }

                        Point move = orderedMoves[i];

                        // chặn biên tuyệt đối
                        if (!InBounds(move.X, move.Y, board)) continue;

                        board[move.X, move.Y] = BOT;
                        long score = Minimax(board, currentDepth - 1, long.MinValue, long.MaxValue, false);
                        board[move.X, move.Y] = EMPTY;

                        if (score > maxScore) { maxScore = score; currentBestMove = move; }
                    }

                    if (isTimeOut) break;
                    if (currentBestMove.X != -1) currentFinalMove = currentBestMove;
                    if (maxScore > 5000000) break;
                }

                return currentFinalMove;
            });

            // chặn lần cuối
            if (bestMove.X != -1 && bestMove.Y != -1 && InBounds(bestMove.X, bestMove.Y, board))
            {
                MakeMove(bestMove.X, bestMove.Y);
            }
        }

        // Kiểm tra nhanh xem có đủ 5 quân liên tiếp không
        private bool CheckWin(int r, int c, int[,] board, int player)
        {
            if (CountConsecutive(r, c, 1, 0, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 0, 1, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 1, 1, board, player) >= 5) return true;
            if (CountConsecutive(r, c, 1, -1, board, player) >= 5) return true;
            return false;
        }

        private int CountConsecutive(int r, int c, int dr, int dc, int[,] board, int player)
        {
            int count = 1;

            for (int i = 1; i < 5; i++)
            {
                int nr = r + dr * i;
                int nc = c + dc * i;
                if (InBounds(nr, nc, board) && board[nr, nc] == player) count++;
                else break;
            }

            for (int i = 1; i < 5; i++)
            {
                int nr = r - dr * i;
                int nc = c - dc * i;
                if (InBounds(nr, nc, board) && board[nr, nc] == player) count++;
                else break;
            }

            return count;
        }


        // Sắp xếp nước đi dựa trên điểm heuristic sơ bộ
        private List<Point> SortMoves(List<Point> moves, int[,] board)
        {
            List<KeyValuePair<Point, long>> rankedMoves = new List<KeyValuePair<Point, long>>();
            foreach (var move in moves)
            {
                long atk = DiemTanCong(move.X, move.Y, board);
                long def = DiemPhongThu(move.X, move.Y, board);
                rankedMoves.Add(new KeyValuePair<Point, long>(move, atk + def));
            }
            rankedMoves.Sort((a, b) => b.Value.CompareTo(a.Value));
            List<Point> result = new List<Point>();
            foreach (var pair in rankedMoves) result.Add(pair.Key);
            return result;
        }

        private void MakeMove(int r, int c)
        {
            if (r < 0 || r >= matrix.Count || c < 0 || c >= matrix[0].Count) return;

            var btn = matrix[r][c];
            if (btn.BackgroundImage != null) return;

            btn.BackgroundImage = Player[1].Mark;
            HighlightMove(btn);

            CurrentPlayer = 0;
            moveHistory.Push(btn);
            playerHistory.Push(1);

            var winCells = getWinningCells(btn);
            if (winCells != null) { HighlightWinningCells(winCells); EndGame(Player[1].Name); }
        }


        private int[,] GetIntBoard()
        {
            int rows = matrix.Count;
            int cols = matrix[0].Count;

            int[,] board = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
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

        // Candidate Generation: Chỉ lấy các ô trống lân cận các quân cờ đã đánh
        private List<Point> GetPossibleMoves(int[,] board)
        {
            List<Point> moves = new List<Point>();
            HashSet<int> visited = new HashSet<int>();

            int h = board.GetLength(0);
            int w = board.GetLength(1);

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (board[i, j] != EMPTY)
                    {
                        int startR = Math.Max(0, i - 2);
                        int endR = Math.Min(h - 1, i + 2); // Đảm bảo không vượt quá h-1
                        int startC = Math.Max(0, j - 2);
                        int endC = Math.Min(w - 1, j + 2); // Đảm bảo không vượt quá w-1

                        for (int r = startR; r <= endR; r++)
                        {
                            for (int c = startC; c <= endC; c++)
                            {
                                if (board[r, c] == EMPTY)
                                {
                                    int key = r * 10000 + c;
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
            // Kiểm tra timeout để ngắt sớm
            if (isTimeOut || botTimer.ElapsedMilliseconds > timeLimitMillis) { isTimeOut = true; return 0; }

            long currentScore = EvaluateBoard(board);
            if (Math.Abs(currentScore) > 5000000) return currentScore; // Đã có người thắng/thua
            if (depth == 0) return currentScore;

            var moves = GetPossibleMoves(board);
            if (moves.Count == 0) return currentScore;

            var orderedMoves = SortMoves(moves, board);
            int movesCheckLimit = 20; // Giới hạn số nhánh để tăng tốc

            if (isMaximizing) // Lượt Bot
            {
                long maxScore = long.MinValue;
                for (int i = 0; i < Math.Min(orderedMoves.Count, movesCheckLimit); i++)
                {
                    Point move = orderedMoves[i];
                    board[move.X, move.Y] = BOT;
                    if (CheckWin(move.X, move.Y, board, BOT)) { board[move.X, move.Y] = EMPTY; return 100000000; }

                    long score = Minimax(board, depth - 1, alpha, beta, false);
                    board[move.X, move.Y] = EMPTY;

                    if (isTimeOut) return 0;
                    maxScore = Math.Max(maxScore, score);
                    alpha = Math.Max(alpha, score);
                    if (beta <= alpha) break; // Alpha-Beta Pruning
                }
                return maxScore;
            }
            else // Lượt Human
            {
                long minScore = long.MaxValue;
                for (int i = 0; i < Math.Min(orderedMoves.Count, movesCheckLimit); i++)
                {
                    Point move = orderedMoves[i];
                    board[move.X, move.Y] = HUMAN;
                    if (CheckWin(move.X, move.Y, board, HUMAN)) { board[move.X, move.Y] = EMPTY; return -100000000; }

                    long score = Minimax(board, depth - 1, alpha, beta, true);
                    board[move.X, move.Y] = EMPTY;

                    if (isTimeOut) return 0;
                    minScore = Math.Min(minScore, score);
                    beta = Math.Min(beta, score);
                    if (beta <= alpha) break; // Alpha-Beta Pruning
                }
                return minScore;
            }
        }

        private long EvaluateBoard(int[,] board)
        {
            long score = 0;

            // SỬA: Lấy kích thước thực tế từ mảng board đang xét
            int rows = board.GetLength(0);
            int cols = board.GetLength(1);

            // Chạy vòng lặp theo rows/cols vừa lấy, KHÔNG dùng Cons
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (board[i, j] == BOT) score += DiemTanCong(i, j, board);
                    else if (board[i, j] == HUMAN) score -= DiemTanCongCuaDich(i, j, board);
                }
            }
            return score;
        }

        long DiemTanCongCuaDich(int r, int c, int[,] board)
        {
            long score = 0;
            score += CheckAtkOfHuman(r, c, 1, 0, board);
            score += CheckAtkOfHuman(r, c, 0, 1, board);
            score += CheckAtkOfHuman(r, c, 1, 1, board);
            score += CheckAtkOfHuman(r, c, 1, -1, board);
            return score;
        }

        // Tính điểm tấn công của Human (mối đe dọa với Bot)
        long CheckAtkOfHuman(int r, int c, int dr, int dc, int[,] board)
        {
            int ta = 0;
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i;
                int nc = c + dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) ta++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i;
                int nc = c - dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) ta++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && ta < 5) return 0;

            long score = AttackScore[Math.Min(ta, 5)];
            if (blocks == 1) score /= 2;
            if (ta == 3 && blocks == 0) score *= 2;
            if (ta == 4 && blocks == 0) score *= 2;
            if (ta >= 5) score = AttackScore[5];

            return score;
        }

        long DiemTanCong(int r, int c, int[,] board)
        {
            long score = 0;
            score += CheckAtk(r, c, 1, 0, board);
            score += CheckAtk(r, c, 0, 1, board);
            score += CheckAtk(r, c, 1, 1, board);
            score += CheckAtk(r, c, 1, -1, board);
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

        long CheckAtk(int r, int c, int dr, int dc, int[,] board)
        {
            int ta = 0;
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i;
                int nc = c + dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == BOT) ta++;
                else if (board[nr, nc] == HUMAN) { blocks++; break; }
                else break;
            }

            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i;
                int nc = c - dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == BOT) ta++;
                else if (board[nr, nc] == HUMAN) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && ta < 5) return 0;

            long score = AttackScore[Math.Min(ta, 5)];
            if (blocks == 1) score /= 2;
            if (ta == 3 && blocks == 0) score *= 2;
            if (ta == 4 && blocks == 0) score *= 2;
            if (ta >= 5) score = AttackScore[5];

            return score;
        }

        long CheckDef(int r, int c, int dr, int dc, int[,] board)
        {
            int dich = 0;
            int blocks = 0;

            for (int i = 1; i < 6; i++)
            {
                int nr = r + dr * i;
                int nc = c + dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) dich++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            for (int i = 1; i < 6; i++)
            {
                int nr = r - dr * i;
                int nc = c - dc * i;

                if (!InBounds(nr, nc, board)) { blocks++; break; }
                if (board[nr, nc] == HUMAN) dich++;
                else if (board[nr, nc] == BOT) { blocks++; break; }
                else break;
            }

            if (blocks == 2 && dich < 5) return 0;

            long score = DefendScore[Math.Min(dich, 5)];
            if (dich == 3 && blocks == 0) score *= 3;
            if (dich == 4) score *= 5;

            return score;
        }

        bool InBounds(int r, int c, int[,] board)
        {
            return r >= 0 && r < board.GetLength(0) &&
                   c >= 0 && c < board.GetLength(1);
        }

        #endregion
        #endregion
    }
}