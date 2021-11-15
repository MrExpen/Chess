using ChessLib.Data;
using System.Collections.Generic;

namespace ChessLib.Figures
{
    public class Bishop : ChessFigure
    {
        public override EnumFigure EnumFigure => EnumFigure.Bishop;

        public override List<ChessPosition> GetMovePositions(Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();

            for (int i = 1; (Position + new ChessPosition(i, i)).IsOnBoard; i++)
            {
                if (board.Figures[Position.X + i, Position.Y + i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X + i, Position.Y + i));
                }
                else
                {
                    if (board.Figures[Position.X + i, Position.Y + i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X + i, Position.Y + i));
                    }
                    break;
                }
            }
            for (int i = 1; (Position + new ChessPosition(i, -i)).IsOnBoard; i++)
            {
                if (board.Figures[Position.X + i, Position.Y - i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X + i, Position.Y - i));
                }
                else
                {
                    if (board.Figures[Position.X + i, Position.Y - i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X + i, Position.Y - i));
                    }
                    break;
                }
            }
            for (int i = 1; (Position + new ChessPosition(-i, i)).IsOnBoard; i++)
            {
                if (board.Figures[Position.X - i, Position.Y + i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X - i, Position.Y + i));
                }
                else
                {
                    if (board.Figures[Position.X - i, Position.Y + i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X - i, Position.Y + i));
                    }
                    break;
                }
            }
            for (int i = 1; (Position + new ChessPosition(-i, -i)).IsOnBoard; i++)
            {
                if (board.Figures[Position.X - i, Position.Y - i] is null)
                {
                    chessPositions.Add(new ChessPosition(Position.X - i, Position.Y - i));
                }
                else
                {
                    if (board.Figures[Position.X - i, Position.Y - i].Color != Color)
                    {
                        chessPositions.Add(new ChessPosition(Position.X - i, Position.Y - i));
                    }
                    break;
                }
            }

            return chessPositions;
        }

        #region Ctor
        public Bishop(Color color, ChessPosition position) : base(color, position)
        {
        }
        #endregion
    }
}
