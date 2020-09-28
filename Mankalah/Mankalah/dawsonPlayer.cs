using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Mankalah
{
    public class DawsonPlayer : Player
    {
        public int timeLimit;
        Position p;
        int first = 0;
        int last = 0;
        int p2first = 0;
        int p2last = 0;
        CancellationTokenSource cancellationToken;

        public DawsonPlayer(Position pos, int setTimeLimit) : base(pos, "Dawson", setTimeLimit)
        {
            timeLimit = setTimeLimit;
            p = pos;
            if (p == Position.Top)
            {
                first = 7;
                last = 12;
                p2first = 0;
                p2last = 5;
            }
            else
            {
                first = 0;
                last = 5;
                p2first = 7;
                p2last = 12;
            }
        }

        public override string gloat()
        {
            return "I WIN! YOU'RE DUMBER THAN DAWSON!!";
        }



        public override int chooseMove(Board b)
        {
            int d = 3;
            Result r = new Result(first, -1);



            cancellationToken = new CancellationTokenSource(TimeSpan.FromMilliseconds(timeLimit));
            try
            {
                while (true)
                {
                    r = new Result(miniMax(b, d, int.MinValue, int.MaxValue));
                    d++;
                    //Console.WriteLine("DEPTH " + d);
                }
            }
            catch (OperationCanceledException e)
            {
                return r.move;
            }
        }

        public Result miniMax(Board b, int d, int alpha, int beta)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            int bestMove = first;
            int bestVal = 0;
            Result res;
            Position opp;

            if (b.gameOver() || d == 0)
                return new Result(0, evaluate(b));

            if (b.whoseMove() == p)
            {     //top is max
                bestVal = -int.MaxValue;
                for (int move = first; move <= last; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);  //duplicate board
                        b1.makeMove(move, false);        //make the move
                        res = miniMax(b1, d - 1, alpha, beta);   //find its value

                        alpha = Math.Max(alpha, res.val);

                        if (res.val > bestVal)
                        {
                            bestVal = res.val;
                            bestMove = move;
                        }
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            }
            else
            { // similarly for bottom’s move
                bestVal = int.MaxValue;
                for (int move = p2first; move <= p2last; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);  //duplicate board
                        b1.makeMove(move, false);        //make the move
                        res = miniMax(b1, d - 1, alpha, beta);   //find its value

                        beta = Math.Min(beta, res.val);

                        if (res.val < bestVal)         //remember if best
                        {
                            bestVal = res.val;
                            bestMove = move;
                        }
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            }

            return new Result(bestMove, bestVal);
        }

        public override int evaluate(Board b)
        {
            int score = b.stonesAt(last + 1) - b.stonesAt(p2last + 1);
            int stones = 0;

            for (int i = first; i <= last; i++)
            {
                stones += b.stonesAt(i);
            }
            int totalStones = stones;

            for (int i = p2first; i <= p2last; i++)
            {
                totalStones += b.stonesAt(i);
            }

            int stonePercent = 0;

            if (totalStones != 0)
            {
                stonePercent = stones / totalStones;
            }

            int finalScore = score * 100 + stonePercent * 2;

            if (b.gameOver())
            {
                finalScore += 10000;
            }

            return finalScore;
        }

        public override String getImage() { return "Dawson.png"; }
    }

    public class Result
    {
        public int move;
        public int val;

        public Result(int setMove, int setVal)
        {
            move = setMove;
            val = setVal;
        }

        public Result(Result res)
        {
            move = res.move;
            val = res.val;
        }
    }
}
