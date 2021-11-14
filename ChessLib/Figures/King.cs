using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Figures
{
    public class King : ChessFigure
    {
        public override EnumFigure EnumFigure => EnumFigure.King;

        public override char Char => Color == Color.White ? 'K' : 'k';

        public override List<ChessPosition> GetMovePositions(Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if ((x == 0 && y == 0) || !(Position + new ChessPosition(x, y)).IsOnBoard)
                    {
                        continue;
                    }

                    if (board.Figures[Position.X + x, Position.Y + y] is null || board.Figures[Position.X + x, Position.Y + y].Color != Color)
                    {
                        chessPositions.Add(Position + new ChessPosition(x, y));
                    }
                }
            }
            return chessPositions;
        }

        public override List<ChessPosition> GetMovePositionsWithCheckCheck(Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>(GetMovePositions(board));

            if (Color == Color.White)
            {
                if (board.WhiteLongCastling)
                {
                    bool can = true;
                    for (int i = Position.X - 1; i >= 1; i--)
                    {
                        if (board.Figures[i, 0] is not null)
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (board.Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.Black).Any(f => f.GetMovePositions(board).Contains(Position + new ChessPosition(-i, 0))))
                            {
                                can = false;
                                break;
                            }
                        }
                    }

                    if (can)
                    {
                        chessPositions.Add(new ChessPosition(2, 0));
                    }
                }
                if (board.WhiteShortCastling)
                {
                    bool can = true;
                    for (int i = Position.X + 1; i < 7; i++)
                    {
                        if (board.Figures[i, 0] is not null)
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (board.Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.Black).Any(f => f.GetMovePositions(board).Contains(Position + new ChessPosition(i, 0))))
                            {
                                can = false;
                                break;
                            }
                        }
                    }

                    if (can)
                    {
                        chessPositions.Add(new ChessPosition(6, 0));
                    }
                }
            }
            else if (Color == Color.Black)
            {
                if (board.BlackLongCastling)
                {
                    bool can = true;
                    for (int i = Position.X - 1; i >= 1; i--)
                    {
                        if (board.Figures[i, 7] is not null)
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (board.Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.White).Any(f => f.GetMovePositions(board).Contains(Position + new ChessPosition(-i, 0))))
                            {
                                can = false;
                                break;
                            }
                        }
                    }

                    if (can)
                    {
                        chessPositions.Add(new ChessPosition(2, 7));
                    }
                }
                if (board.BlackShortCastling)
                {
                    bool can = true;
                    for (int i = Position.X + 1; i < 7; i++)
                    {
                        if (board.Figures[i, 7] is not null)
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (board.Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.White).Any(f => f.GetMovePositions(board).Contains(Position + new ChessPosition(i, 0))))
                            {
                                can = false;
                                break;
                            }
                        }
                    }

                    if (can)
                    {
                        chessPositions.Add(new ChessPosition(6, 7));
                    }
                }
            }

            return RemoveChecks(chessPositions, board);
        }

        #region Ctor
        public King(Color color, ChessPosition position) : base(color, position)
        {
        }
        #endregion
    }
}
