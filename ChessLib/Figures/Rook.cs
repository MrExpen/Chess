using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Figures
{
    public class Rook : ChessFigure
    {
        public override EnumFigure EnumFigure => EnumFigure.Rook;

        public override char Char => Color == Color.White ? 'R' : 'r';

        public override List<ChessPosition> GetMovePositions(Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();

            for (int i = Position.X + 1; i < 8; i++)
            {
                if (board.Figures[i, Position.Y] is null)
                {
                    chessPositions.Add(new ChessPosition(i, Position.Y));
                }
                else
                {
                    if (board.Figures[i, Position.Y].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(i, Position.Y));
                    }
                    break;
                }
            }
            for (int i = Position.X - 1; i >= 0; i--)
            {
                if (board.Figures[i, Position.Y] is null)
                {
                    chessPositions.Add(new ChessPosition(i, Position.Y));
                }
                else
                {
                    if (board.Figures[i, Position.Y].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(i, Position.Y));
                    }
                    break;
                }
            }
            for (int i = Position.Y + 1; i < 8; i++)
            {
                if (board.Figures[Position.X, i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X, i));
                }
                else
                {
                    if (board.Figures[Position.X, i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X, i));
                    }
                    break;
                }
            }
            for (int i = Position.Y - 1; i >= 0; i--)
            {
                if (board.Figures[Position.X, i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X, i));
                }
                else
                {
                    if (board.Figures[Position.X, i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X, i));
                    }
                    break;
                }
            }

            return chessPositions;
        }

        #region Ctor
        public Rook(Color color, ChessPosition position) : base(color, position)
        {
        }
        #endregion
    }
}
