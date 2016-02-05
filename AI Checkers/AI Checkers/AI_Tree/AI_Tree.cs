using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace AICheckers
{
    class AI_Tree : IAI
    {
        int AI_DEPTH = 5;
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
            while (bestBoard == null && currentDepth > 0)
            {
                //currentDepth -= 1;
                KeyValuePair<Square[,], float> minmax = MaxMove(Board, currentDepth);
                bestBoard = minmax.Key;
                bestValue = minmax.Value;
            }
            if (bestBoard == null)
            {
                Console.WriteLine("WOCKICA");
                return new KeyValuePair<Square[,], float>();
            }
            else
                return new KeyValuePair<Square[,], float>(bestBoard, bestValue);
        }

        public KeyValuePair<Square[,], float> MinMax2(Square[,] Board)
        {
            Square[,] bestBoard = null;
            int currentDepth = AI_DEPTH;
            float bestValue = 0;
            while (bestBoard == null && currentDepth > 0)
            {
                //currentDepth -= 1;
                KeyValuePair<Square[,], float> minmax = MinMove2(Board, currentDepth);
                bestBoard = minmax.Key;
                bestValue = minmax.Value;
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
            return MinMaxBoard(board, depth, (float)-1 / 0);
        }

       private KeyValuePair<Square[,], float> MaxMove2(Square[,] board, int depth)
        {
            return MinMaxBoard2(board, depth, (float)-1 / 0);
        }
        private KeyValuePair<Square[,], float> MinMove(Square[,] board, int depth)
        {
            return MinMaxBoard(board, depth, (float)1 / 0);
        }
        private KeyValuePair<Square[,], float> MinMove2(Square[,] board, int depth)
        {
            return MinMaxBoard2(board, depth, (float)1 / 0);
        }

        private KeyValuePair<Square[,], float> MinMaxBoard(Square[,] Board, int currentDepth, float bestvalue)
        {
            if (currentDepth<=0)
                return new KeyValuePair<Square[,], float>(Board, evalBoard(Board));
            
            float bestValue = bestvalue;
            Square[,] bestBoard = null;

            if (bestvalue == (float)-1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Black);
                foreach (Move m in moves)
                {
                    Square[,] maxBoard = DeepCopy(Board);
                    maxBoard = ExecuteVirtualMove(m, maxBoard);
                    float value = MinMove(maxBoard, currentDepth - 1).Value;
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestBoard = maxBoard;
                    }
                    else if(value == bestValue)
                    {
                        Random rnd = new Random();
                        if (rnd.NextDouble() > 0.5)
                        {
                            bestValue = value;
                            bestBoard = maxBoard;
                        }
                    }

                }
            }
            else if (bestvalue == (float)1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Red);
                foreach (Move m in moves)
                {
                    Square[,] minBoard = DeepCopy(Board);
                    minBoard = ExecuteVirtualMove(m, minBoard);
                    float value = MaxMove(minBoard, currentDepth - 1).Value;
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestBoard = minBoard;
                    }
                    else if(value == bestValue)
                    {
                        Random rnd = new Random();
                        if (rnd.NextDouble() > 0.5)
                        {
                            bestValue = value;
                            bestBoard = minBoard;
                        }
                    }
                }
            }
            else
                Console.WriteLine("WOCE");

            return new KeyValuePair<Square[,], float>(bestBoard, bestValue);
        }

       private KeyValuePair<Square[,], float> MinMaxBoard2(Square[,] Board, int currentDepth, float bestvalue)
        {
            if (currentDepth <= 0)
                return new KeyValuePair<Square[,], float>(Board, evalBoard2(Board));

            float bestValue = bestvalue;
            Square[,] bestBoard = null;

            if (bestvalue == (float)-1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Black);
                foreach (Move m in moves)
                {
                    Square[,] maxBoard = DeepCopy(Board);
                    maxBoard = ExecuteVirtualMove(m, maxBoard);
                    float value = MinMove2(maxBoard, currentDepth - 1).Value;
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestBoard = maxBoard;
                    }

                }
            }
            else if (bestvalue == (float)1 / 0)
            {
                List<Move> moves = getAllMoves(Board, CheckerColour.Red);
                foreach (Move m in moves)
                {
                    Square[,] minBoard = DeepCopy(Board);
                    minBoard = ExecuteVirtualMove(m, minBoard);
                    float value = MaxMove2(minBoard, currentDepth - 1).Value;
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestBoard = minBoard;
                    }
                }
            }
            else
                Console.WriteLine("WOCE");

            return new KeyValuePair<Square[,], float>(bestBoard, bestValue);
        }
        private float evalBoard(Square[,] Board)
        {
            List<Point> redPieces = getAllPosition(Board, CheckerColour.Red);
            List<Point> blackPieces = getAllPosition(Board, CheckerColour.Black);
            float score = 0;
            int reds = 0;
            int blacks = 0;
            int redkings = 0;
            int blackkings = 0;
            foreach (Point p in redPieces)
            {
                
                    reds++;
                    if (Board[p.Y, p.X].King)
                        redkings++;
                
            }
            foreach (Point p in blackPieces)
            {
                
                    blacks++;
                    if (Board[p.Y, p.X].King)
                        blackkings++;
                
            }

            float distance = 0.0f;
            foreach (Point piece1 in blackPieces)
            {
                foreach (Point piece2 in blackPieces)
                {
                    if (!piece1.Equals(piece2))
                    {
                        float dx = Math.Abs(piece1.X - piece2.X);
                        float dy = Math.Abs(piece1.Y - piece2.Y);
                        distance += (float)(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    }
                }
            }
            distance /= blackPieces.Count;
            float distance2 = 0.0f;
            foreach (Point piece1 in redPieces)
            {
                foreach (Point piece2 in redPieces)
                {
                    if (!piece1.Equals(piece2))
                    {
                        float dx = Math.Abs(piece1.X - piece2.X);
                        float dy = Math.Abs(piece1.Y - piece2.Y);
                        distance2 += (float)(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    }
                }
            }
            distance2 /= redPieces.Count;

            score = blacks - reds + (blackkings*1.5f - redkings*2.0f) ;
            score += 100.0f/distance;
            score -= 100.0f/distance2;

            return score;

        }

       private float evalBoard2(Square[,] Board)
        {
            List<Point> redPieces = getAllPosition(Board, CheckerColour.Red);
            List<Point> blackPieces = getAllPosition(Board, CheckerColour.Black);
            float score = 0;
            int reds = 0;
            int blacks = 0;
            int redkings = 0;
            int blackkings = 0;
            foreach (Point p in redPieces)
            {
                if (Utils.GetOpenSquares(Board, p).Length > 0)
                {
                    reds++;
                    if (Board[p.Y, p.X].King)
                        redkings++;
                }
            }
            foreach (Point p in blackPieces)
            {
                if (Utils.GetOpenSquaresBlack(Board, p).Length > 0)
                {
                    blacks++;
                    if (Board[p.Y, p.X].King)
                        blackkings++;
                }
            }

            float distance = 0.0f;
            foreach (Point piece1 in redPieces)
            {
                foreach (Point piece2 in redPieces)
                {
                    if (!piece1.Equals(piece2))
                    {
                        float dx = Math.Abs(piece1.X - piece2.X);
                        float dy = Math.Abs(piece1.Y - piece2.Y);
                        distance += (float)(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    }
                }
            }
            distance /= redPieces.Count;


            float distance2 = 0.0f;
            foreach (Point piece1 in blackPieces)
            {
                foreach (Point piece2 in blackPieces)
                {
                    if (!piece1.Equals(piece2))
                    {
                        float dx = Math.Abs(piece1.X - piece2.X);
                        float dy = Math.Abs(piece1.Y - piece2.Y);
                        distance2 += (float)(Math.Pow(dx, 2) + Math.Pow(dy, 2));
                    }
                }
            }
            distance2 /= blackPieces.Count;

            score = blacks - 2*reds + (blackkings * 3.0f - redkings * 2.0f);
            score -= 100.0f / distance ;
            score += 100.0f / distance2;
            return score;
        
        }

        public static List<Point> getAllPosition(Square[,] Board, CheckerColour cColor)
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
        public static List<Move> getAllMoves(Square[,] Board, CheckerColour cColor)
        {
            List<Move> moves = new List<Move>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Board[j, i].Colour == cColor)
                    {
                        if (cColor == CheckerColour.Black)
                        {
                            moves.AddRange(Utils.GetOpenSquaresBlack(Board, new Point(i, j)));
                            if (moves == null)
                                Console.WriteLine("No more possible moves! You WON!");
                        }
                        else if (cColor == CheckerColour.Red)
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
