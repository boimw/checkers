using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using AICheckers.Properties;

namespace AICheckers
{
    public class BoardPanel : Panel
    {
        IAI AI = null;
        IAI AI2 = null;

        //Assets
        Image checkerRed = Resources.checkerred;
        Image checkerRedKing = Resources.checkerredking;
        Image checkerBlack = Resources.checkerblack;
        Image checkerBlackKing = Resources.checkerblackking;
        int squareWidth = 0;
        bool AITurnBool = false;
        Point selectedChecker = new Point(-1, -1);
        List<Move> possibleMoves = new List<Move>();
        public static CheckerColour currentTurn = CheckerColour.Red;

        Square[,] Board = new Square[8, 8];

        public BoardPanel()
            : base()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint, true);

            //Initialize board
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Board[i, j] = new Square();
                    Board[i, j].Colour = CheckerColour.Empty;
                }
            }

            //Setup Pieces
            for (int i = 0; i < 8; i += 1)
            {
                int offset = 0;
                if (i % 2 != 0)
                {
                    offset++;
                }
                for (int j = offset; j < 8; j += 2)
                {
                    if (i < 3)
                    {
                        Board[i, j].Colour = CheckerColour.Red;
                    }
                    if (i > 4)
                    {
                        Board[i, j].Colour = CheckerColour.Black;
                    }
                }
            }

            AI = new AI_Tree();
            AI.Colour = CheckerColour.Black;
            AI2 = new AI_Tree();
            AI2.Colour = CheckerColour.Red;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Logic

            //Draw
            base.OnPaint(e);
            e.Graphics.Clear(Color.Black);

            //Draw the board
            squareWidth = (Width) / 8;
            for (int c = 0; c < Width; c += squareWidth)
            {
                int offset = 0;
                if ((c / squareWidth) % 2 != 0)
                {
                    offset += squareWidth;
                }
                for (int i = offset; i < Width; i += (squareWidth * 2))
                {
                    e.Graphics.FillRectangle(Brushes.White, c, i, squareWidth, squareWidth);
                }
            }

            //Draw possible moves
            foreach (Move move in possibleMoves)
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, move.Destination.X * squareWidth, move.Destination.Y * squareWidth, squareWidth, squareWidth);
            }

            //Draw selected checker
            if (selectedChecker.X >= 0 && selectedChecker.Y >= 0)
            {
                e.Graphics.FillRectangle(Brushes.DarkKhaki, selectedChecker.X * squareWidth, selectedChecker.Y * squareWidth, squareWidth, squareWidth);
            }

            //Draw Border
            e.Graphics.DrawRectangle(Pens.DarkGray,
            e.ClipRectangle.Left,
            e.ClipRectangle.Top,
            e.ClipRectangle.Width - 1,
            e.ClipRectangle.Height - 1);

            //Draw Checker Images
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Colour == CheckerColour.Red)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerRedKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerRed, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                    else if (Board[i, j].Colour == CheckerColour.Black)
                    {
                        if (Board[i, j].King)
                        {
                            e.Graphics.DrawImage(checkerBlackKing, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                        else
                        {
                            e.Graphics.DrawImage(checkerBlack, new Rectangle(j * squareWidth, i * squareWidth, squareWidth, squareWidth));
                        }
                    }
                }
            }

        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int clickedX = (int)(((double)e.X / (double)Width) * 8.0d);
            int clickedY = (int)(((double)e.Y / (double)Height) * 8.0d);

            Point clickedPoint = new Point(clickedX, clickedY);

            //Determine if this is the correct player
            if (Board[clickedY, clickedX].Colour != CheckerColour.Empty
                && Board[clickedY, clickedX].Colour != currentTurn)
                return;

            //Determine if this is a move or checker selection
            List<Move> matches = possibleMoves.Where(m => m.Destination == clickedPoint).ToList<Move>();
            if (matches.Count > 0)
            {
                //Move the checker to the clicked square
                MoveChecker(matches[0]);
            }
            else if (Board[clickedY, clickedX].Colour != CheckerColour.Empty)
            {
                //Select the clicked checker
                selectedChecker.X = clickedX;
                selectedChecker.Y = clickedY;
                possibleMoves.Clear();

                Console.WriteLine("Selected Checker: {0}", selectedChecker.ToString());
                Move[] OpenSquares = Utils.GetOpenSquares(Board, selectedChecker);
                possibleMoves.AddRange(OpenSquares);
                this.Invalidate();
            }
        }

        private void MoveChecker(Move move)
        {
            Console.WriteLine(move.ToString());
                       
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            ResetSquare(move.Source);

            foreach (Point point in move.Captures)
            {
                ResetSquare(point);
            }

            selectedChecker.X = -1;
            selectedChecker.Y = -1;

            //Kinging
            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }
            possibleMoves.Clear();


            int reds = AI_Tree.getAllPosition(Board, CheckerColour.Red).Count;
            int blacks = AI_Tree.getAllPosition(Board, CheckerColour.Black).Count;

            Label redslb = (Label)Application.OpenForms["FormMain"].Controls.Find("redsNum", false).FirstOrDefault();
            Label blackslb = (Label)Application.OpenForms["FormMain"].Controls.Find("blacksNum", false).FirstOrDefault();

            redslb.Text = reds.ToString();
            blackslb.Text = blacks.ToString();

            if (AI_Tree.getAllPosition(Board, CheckerColour.Red).Count == 0)
            {
                DialogResult blackWins = new WinLose("Better luck next time", "You lost!").ShowDialog();
                if (blackWins == DialogResult.Yes)
                {
                    Application.Restart();
                }
            }
            else if (AI_Tree.getAllPosition(Board, CheckerColour.Black).Count == 0)
            {
                DialogResult redWins = new WinLose("Congratulations!", "You win!").ShowDialog();
                if (redWins == DialogResult.Yes)
                {
                    Application.Restart();
                }
            }

            // AI pravi potez
            AdvanceTurn();

            this.Refresh();
        }

        private void ResetSquare(Point square)
        {
            //Reset the square and the selected checker
            Board[square.Y, square.X].Colour = CheckerColour.Empty;
            Board[square.Y, square.X].King = false;
        }

        public void AITurn()
        {
            AITurnBool = true;
            currentTurn = CheckerColour.Red;

            this.Refresh();

            if (AI2 != null && AI2.Colour == currentTurn)
            {
                Board = AI2.MinMax2(Board).Key;

                this.Refresh();
                int reds = AI_Tree.getAllPosition(Board, CheckerColour.Red).Count;
                int blacks = AI_Tree.getAllPosition(Board, CheckerColour.Black).Count;

                Label redslb = (Label)Application.OpenForms["FormMain"].Controls.Find("redsNum", false).FirstOrDefault();
                Label blackslb = (Label)Application.OpenForms["FormMain"].Controls.Find("blacksNum", false).FirstOrDefault();

                redslb.Text = reds.ToString();
                blackslb.Text = blacks.ToString();
                /*
                Label moveBlack = (Label)Application.OpenForms["FormMain"].Controls.Find("labelMove", false).FirstOrDefault();
                moveBlack.Text = "Move: BLACK";
                */

                if (AI_Tree.getAllPosition(Board, CheckerColour.Red).Count == 0)
                {
                    DialogResult blackWins = new WinLose("Black wins", "Black wins!").ShowDialog();
                    if (blackWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllMoves(Board, CheckerColour.Black).Count == 0)
                {
                    DialogResult redWins = new WinLose("Red wins", "Red wins!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllPosition(Board, CheckerColour.Black).Count == 0)
                {
                    DialogResult redWins = new WinLose("Red wins!", "Red wins!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllMoves(Board, CheckerColour.Red).Count == 0)
                {
                    DialogResult redWins = new WinLose("Black wins", "Black wins!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                
                currentTurn = CheckerColour.Black;
            }

            AdvanceTurn();
        }


        private void AdvanceTurn()
        {
            
       //     Label moveRed = (Label)Application.OpenForms["FormMain"].Controls.Find("labelMove", false).FirstOrDefault();
      //      moveRed.Text = "RED";
            
            if (!AITurnBool)
            {
                if (currentTurn == CheckerColour.Red)
                {
                    currentTurn = CheckerColour.Black;
                  //  Label moveBlack = (Label)Application.OpenForms["FormMain"].Controls.Find("labelMove", false).FirstOrDefault();
                  //  moveBlack.Text = "BLACK";
                }
                else
                {
                    currentTurn = CheckerColour.Red;
                }
            }
            this.Refresh();

            if (AI != null && AI.Colour == currentTurn)
            {
                Board = AI.MinMax(Board).Key;

                this.Refresh();
                int reds = AI_Tree.getAllPosition(Board, CheckerColour.Red).Count;
                int blacks = AI_Tree.getAllPosition(Board, CheckerColour.Black).Count;

                Label redslb = (Label)Application.OpenForms["FormMain"].Controls.Find("redsNum", false).FirstOrDefault();
                Label blackslb = (Label)Application.OpenForms["FormMain"].Controls.Find("blacksNum", false).FirstOrDefault();

                redslb.Text = reds.ToString();
                blackslb.Text = blacks.ToString();
                /*
                Label moveBlack = (Label)Application.OpenForms["FormMain"].Controls.Find("labelMove", false).FirstOrDefault();
                moveBlack.Text = "Move: BLACK";
                */
                if (AI_Tree.getAllPosition(Board, CheckerColour.Red).Count == 0)
                {
                    DialogResult blackWins = new WinLose("Better luck next time", "You lost!").ShowDialog();
                    if (blackWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllMoves(Board, CheckerColour.Black).Count == 0)
                {
                    DialogResult redWins = new WinLose("Red wins!", "Black has no more moves! Red wins!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllMoves(Board, CheckerColour.Red).Count == 0)
                {
                    DialogResult redWins = new WinLose("Black wins", "Red has no more moves! Black wins!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                else if (AI_Tree.getAllPosition(Board, CheckerColour.Black).Count == 0)
                {
                    DialogResult redWins = new WinLose("Congratulations!", "You win!").ShowDialog();
                    if (redWins == DialogResult.Yes)
                    {
                        Application.Restart();
                    }
                }
                currentTurn = CheckerColour.Red;
                if (AITurnBool)
                    AITurn();
            }
        }
    }
}
