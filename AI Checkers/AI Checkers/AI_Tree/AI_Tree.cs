using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace AICheckers
{
    class AI_Tree : IAI
    {
        int AI_DEPTH = 6;
        CheckerColour colour;

        public CheckerColour Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        private Square[,] DeepCopy(Square[,] sourceBoard)
        {
            Square[,] result = new Square[8, 8];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    result[i, j] = new Square();
                    result[i, j].Colour = sourceBoard[i, j].Colour;
                    result[i, j].King = sourceBoard[i, j].King;
                }
            }

            return result;
        }

        public KeyValuePair<Square[,], float> MinMax(Square[,] Board)
        {
            Square[,] bestBoard = null;
            int currentDepth = AI_DEPTH;
            float bestValue = 0;
            while (bestBoard==null && currentDepth > 0)
            {
                currentDepth -= 1;
                bestBoard  = MaxMove(Board, currentDepth).Key;
                bestValue = MaxMove(Board, currentDepth).Value;
            }
            if (bestBoard == null)
            {
                Console.WriteLine("WOCKICA");
                return new KeyValuePair<Square[,], float>();
            }
            else
                return new KeyValuePair<Square[,], float>(bestBoard, bestValue);
        }

        private KeyValuePair<Square[,], float> MaxMove(Square[,] board, int depth)
        {
            return MinMaxBoard(board, depth - 1, (float)-1 / 0);
        }

        private KeyValuePair<Square[,], float> MinMove(Square[,] board, int depth)
        {
            return MinMaxBoard(board, depth - 1, (float)1 / 0);
        }

        private KeyValuePair<Square[,],float > MinMaxBoard(Square[,] Board, int currentDepth, float bestmove)
        {
            if (currentDepth<=0)
                return new KeyValuePair<Square[,],float>(Board,evalBoard(Board));

            float bestMove = bestmove;
            Square[,] bestBoard = null;

            if (bestmove == (float)-1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Black);
                foreach (Move m in moves)
                {
                    Square[,] maxBoard = DeepCopy(Board);
                    maxBoard = ExecuteVirtualMove(m, maxBoard);
                    float value = MinMove(maxBoard, currentDepth - 1).Value;
                    if (value > bestMove)
                    {
                        bestMove = value;
                        bestBoard = maxBoard;
                    }

                }
            }
            else if (bestmove == (float)1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Red);
                foreach (Move m in moves)
                {
                    Square[,] minBoard = DeepCopy(Board);
                    minBoard = ExecuteVirtualMove(m, minBoard);
                    float value = MaxMove(minBoard, currentDepth - 1).Value;
                    if (value < bestMove)
                    {
                        bestMove = value;
                        bestBoard = minBoard;
                    }
                }
            }
            else
                Console.WriteLine("WOCE");

            return new KeyValuePair<Square[,], float>(bestBoard, bestMove);
        }

        private float evalBoard(Square[,] Board)
        {
            float score = 0.0f;
            List<Point> pieces = null;
            int scoreMod = 0;
            if (BoardPanel.currentTurn == CheckerColour.Black)
            {
                pieces = getAllPosition(Board, CheckerColour.Black);
                Console.WriteLine("Number of blacks: " + pieces.Count);
                scoreMod = 1;
            }
            if (BoardPanel.currentTurn == CheckerColour.Red)
            {
                pieces = getAllPosition(Board, CheckerColour.Red);
                Console.WriteLine("Number of reds: " + pieces.Count);
                scoreMod = -1;
            }
            float distance = 0.0f;
            foreach (Point piece1 in pieces)
            {
                foreach (Point piece2 in pieces)
                {
                    if (!piece1.Equals(piece2))
                    {
                        float dx = Math.Abs(piece1.X - piece2.X);
                        float dy = Math.Abs(piece1.Y - piece2.Y);
                        distance += (float)(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    }
                }
            }
            distance /= pieces.Count;
            score = 1.0f / distance * scoreMod;

            return score;
        }
        private List<Point> getAllPosition(Square[,] Board, CheckerColour cColor)
        {
            List<Point> positions = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[i, j].Colour == cColor)
                    {
                        positions.Add(new Point(i, j));
                    }
                }
            }
            return positions;
        }
        private List<Move> getAllMoves(Square[,] Board, CheckerColour cColor)
        {
            List<Move> moves = new List<Move>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[j, i].Colour == cColor)
                    {
                        if(cColor == CheckerColour.Black)
                            moves.AddRange(Utils.GetOpenSquaresBlack(Board, new Point(i, j)));
                        else if(cColor==CheckerColour.Red)
                            moves.AddRange(Utils.GetOpenSquares(Board, new Point(i, j)));
                    }
                }
            }
            return moves;
        }
  
        private Square[,] ExecuteVirtualMove(Move move, Square[,] Board)
        {
            Board[move.Destination.Y, move.Destination.X].Colour = Board[move.Source.Y, move.Source.X].Colour;
            Board[move.Destination.Y, move.Destination.X].King = Board[move.Source.Y, move.Source.X].King;
            Board[move.Source.Y, move.Source.X].Colour = CheckerColour.Empty;
            Board[move.Source.Y, move.Source.X].King = false;

            foreach (Point point in move.Captures)
            {
                Board[point.Y, point.X].Colour = CheckerColour.Empty;
                Board[point.Y, point.X].King = false;
            }

            //Kinging
            if ((move.Destination.Y == 7 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Red)
                || (move.Destination.Y == 0 && Board[move.Destination.Y, move.Destination.X].Colour == CheckerColour.Black))
            {
                Board[move.Destination.Y, move.Destination.X].King = true;
            }

            return Board;
        }
    }
}
