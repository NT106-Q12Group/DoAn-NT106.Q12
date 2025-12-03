using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    internal class ChessBoardManager
    {

        #region Properties
        private Panel chessBoard;
        public Panel ChesBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

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
        public ChessBoardManager(Panel chessBoard)
        {
            this.chessBoard = chessBoard;
            this.Player = new List<Player>()
            {
                new Player ("Player1",Image.FromFile(Application.StartupPath + "\\Resources\\Caro Game.png")),
                new Player ("Player2",Image.FromFile(Application.StartupPath + "\\Resources\\Caro Game (1).png")),
            };

            CurrentPlayer = 0;

        }
        #endregion

        #region Methods
        private int lastHumanRow = -1;
        private int lastHumanCol = -1;
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
                        Tag = i.ToString()
                    };

                    btn.Click += Btn_Click;

                    chessBoard.Controls.Add(btn);

                    matrix[i].Add(btn);

                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.CHESS_HEIGHT);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;

            //Phần PVE
            if (btn.BackgroundImage != null)
                return;

            // Người chơi luôn là Player[0], bot là Player[1]
            if (CurrentPlayer != 0)
                return;

            btn.BackgroundImage = Player[CurrentPlayer].Mark;

            Point p = getChessPoint(btn);
            lastHumanRow = p.Y;
            lastHumanCol = p.X;

            if (isEndGame(btn))
            {
                EndGame();
                return;
            }

            CurrentPlayer = 1;

            BotPlay();


            //Phần PVP
            /*
            if (btn.BackgroundImage != null)
                return;
             
            btn.BackgroundImage = Player[CurrentPlayer].Mark;
            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            if (isEndGame(btn))
            {
                EndGame();
            }
            */
        }

        private void EndGame()
        {
            MessageBox.Show("Trò chơi kết thúc!");
        }

        private bool isEndGame(Button btn)
        {
            return isEndHorizontal(btn) || isEndVertical(btn) || isEndMainDiagonal(btn) || isEndSecondaryDiagonal(btn);
        }

        private Point getChessPoint(Button btn)
        {  
            int column = Convert.ToInt32(btn.Tag);
            int row = matrix[column].IndexOf(btn);

            Point point = new Point(row, column);

            return point;
        }
        private bool isEndHorizontal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countLeft++;
                }
                else
                {
                    break;
                }

            }

            int countRight = 0;
            for (int i = point.X + 1; i < Cons.CHESS_BOARD_WIDTH; i++)
            {
                if (matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    countRight++;
                }
                else
                {
                    break;
                }

            }

            return countLeft + countRight >= 5;
        }

        private bool isEndVertical(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }

            }

            int countBottom = 0;
            for (int i = point.Y + 1; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                if (matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }

            }

            return countTop + countBottom >= 5;
        }

        private bool isEndMainDiagonal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)
                {
                    break;
                }

                if (matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }

            }

            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X + i >= Cons.CHESS_BOARD_WIDTH)
                {
                    break;
                }

                if (matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }

            }

            return countTop + countBottom >= 5;
        }

        private bool isEndSecondaryDiagonal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.CHESS_BOARD_WIDTH || point.Y - i < 0)
                {
                    break;
                }

                if (matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                {
                    break;
                }

            }

            int countBottom = 0;
            for (int i = 1; i <= Cons.CHESS_WIDTH - point.X; i++)
            {
                if (point.Y + i >= Cons.CHESS_BOARD_HEIGHT || point.X - i < 0)
                {
                    break;
                }

                if (matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                {
                    break;
                }

            }

            return countTop + countBottom >= 5;
        }

        long[] AttackScore = new long[] { 0, 8, 80, 800, 8000, 80000, 80000, 800000 };
        long[] DefendScore = new long[] { 0, 10, 100, 1000, 10000, 100000, 1000000 };
        private void BotPlay()
        {
            long maxScore = long.MinValue;
            int bestRow = -1;
            int bestCol = -1;

            for (int i = 0; i < Cons.CHESS_BOARD_HEIGHT; i++)
            {
                for (int j = 0; j < Cons.CHESS_BOARD_WIDTH; j++)
                {
                    if (matrix[i][j].BackgroundImage != null)
                        continue;

                    long atk = DiemTanCong(i, j);
                    long def = DiemPhongThu(i, j);

                    long score = Math.Max(atk, def);

                    if (lastHumanRow != -1 && lastHumanCol != -1)
                    {
                        int dist = Math.Abs(i - lastHumanRow) + Math.Abs(j - lastHumanCol); 

                        int maxDist = 5;
                        if (dist <= maxDist)
                        {
                            long proximityBonus = 600 - dist * 100;
                            score += proximityBonus;
                        }
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;
                        bestRow = i;
                        bestCol = j;
                    }
                }
            }

            if (bestRow != -1)
            {
                matrix[bestRow][bestCol].BackgroundImage = Player[1].Mark;
                CurrentPlayer = 0;

                if (isEndGame(matrix[bestRow][bestCol]))
                    EndGame();
            }
        }



        long DiemTanCong(int dong, int cot)
        {
            long Diem = 0;
            Diem += CheckDirectionAttack(dong, cot, 1, 0); 
            Diem += CheckDirectionAttack(dong, cot, 0, 1); 
            Diem += CheckDirectionAttack(dong, cot, 1, 1) * 2; 
            Diem += CheckDirectionAttack(dong, cot, 1, -1) * 2; 
            return Diem;
        }

        long DiemPhongThu(int dong, int cot)
        {
            long Diem = 0;
            Diem += CheckDirectionDefend(dong, cot, 1, 0);
            Diem += CheckDirectionDefend(dong, cot, 0, 1);
            Diem += CheckDirectionDefend(dong, cot, 1, 1) * 2;
            Diem += CheckDirectionDefend(dong, cot, 1, -1) * 2;
            return Diem;
        }

        long CheckDirectionAttack(int dong, int cot, int drow, int dcol)
        {
            if (matrix[dong][cot].BackgroundImage != null)
            {
                return 0;
            }

            Image botMark = Player[1].Mark;
            Image humanMark = Player[0].Mark;

            int SoQuanTa = 0;
            int SoQuanDich = 0;

            for (int i = 1; i < 6; i++)
            {
                int nd = dong + drow * i;
                int nc = cot + dcol * i;
                if (!InBounds(nd, nc)) break;

                var img = matrix[nd][nc].BackgroundImage;
                if (img == botMark) SoQuanTa++;
                else if (img == humanMark) { SoQuanDich++; break; }
                else break;
            }

            // hướng -1
            for (int i = 1; i < 6; i++)
            {
                int nd = dong - drow * i;
                int nc = cot - dcol * i;
                if (!InBounds(nd, nc)) break;

                var img = matrix[nd][nc].BackgroundImage;
                if (img == botMark) SoQuanTa++;
                else if (img == humanMark) { SoQuanDich++; break; }
                else break;
            }

            if (SoQuanDich >= 2) return 0;

            int atkIndex = Math.Min(SoQuanTa, AttackScore.Length - 1);
            return AttackScore[atkIndex];
        }

        long CheckDirectionDefend(int dong, int cot, int drow, int dcol)
        {
            if (matrix[dong][cot].BackgroundImage != null)
            {
                return 0;
            }

            Image botMark = Player[1].Mark;
            Image humanMark = Player[0].Mark;

            int SoQuanTa = 0;
            int SoQuanDich = 0;

            // hướng +1
            for (int i = 1; i < 6; i++)
            {
                int nd = dong + drow * i;
                int nc = cot + dcol * i;
                if (!InBounds(nd, nc)) break;

                var img = matrix[nd][nc].BackgroundImage;
                if (img == humanMark) SoQuanDich++;
                else if (img == botMark) { SoQuanTa++; break; }
                else break;
            }

            // hướng -1
            for (int i = 1; i < 6; i++)
            {
                int nd = dong - drow * i;
                int nc = cot - dcol * i;
                if (!InBounds(nd, nc)) break;

                var img = matrix[nd][nc].BackgroundImage;
                if (img == humanMark) SoQuanDich++;
                else if (img == botMark) { SoQuanTa++; break; }
                else break;
            }

            if (SoQuanTa >= 2) return 0;

            int defIndex = Math.Min(SoQuanDich, DefendScore.Length - 1);
            return DefendScore[defIndex];
        }

        bool InBounds(int dong, int cot)
        {
            return dong >= 0 && dong < Cons.CHESS_BOARD_HEIGHT && cot >= 0 && cot < Cons.CHESS_BOARD_WIDTH;
        }

        #endregion
    }
}