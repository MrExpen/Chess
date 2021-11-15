using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using ChessLib.Exceptions;
using ChessLib.Data;

namespace ChessLib.Figures
{
    public class Pawn : ChessFigure
    {
        public override EnumFigure EnumFigure => EnumFigure.Pawn;

        public override List<ChessPosition> GetMovePositions(Board board)
        {
            List<ChessPosition> movePositions = new List<ChessPosition>();
            if (Color == Color.White)
            {
                if (new ChessPosition(Position.X, Position.Y + 1).IsOnBoard && board.Figures[Position.X, Position.Y + 1] is null)
                {
                    movePositions.Add(new ChessPosition(Position.X, Position.Y + 1));
                }
                if (Position.Y == 1 && board.Figures[Position.X, Position.Y + 2] is null && board.Figures[Position.X, Position.Y + 1] is null)
                {
                    movePositions.Add(new ChessPosition(Position.X, Position.Y + 2));
                }
                if (new ChessPosition(Position.X + 1, Position.Y + 1).IsOnBoard && board.Figures[Position.X + 1, Position.Y + 1] is not null && board.Figures[Position.X + 1, Position.Y + 1].Color != Color)
                {
                    movePositions.Add(new ChessPosition(Position.X + 1, Position.Y + 1));
                }
                if (new ChessPosition(Position.X - 1, Position.Y + 1).IsOnBoard && board.Figures[Position.X - 1, Position.Y + 1] is not null && board.Figures[Position.X - 1, Position.Y + 1].Color != Color)
                {
                    movePositions.Add(new ChessPosition(Position.X - 1, Position.Y + 1));
                }

                var tmp = board.Fen.Split(" ")[^3];
                if (tmp != "-")
                {
                    ChessPosition PawnPosition = new ChessPosition(tmp);
                    if (Position.Y == 4 && Math.Abs(Position.X - PawnPosition.X) == 1)
                    {
                        movePositions.Add(PawnPosition);
                    }
                }
            }
            else if (Color == Color.Black)
            {
                if (new ChessPosition(Position.X, Position.Y - 1).IsOnBoard && board.Figures[Position.X, Position.Y - 1] is null)
                {
                    movePositions.Add(new ChessPosition(Position.X, Position.Y - 1));
                }
                if (Position.Y == 6 && board.Figures[Position.X, Position.Y - 2] is null && board.Figures[Position.X, Position.Y - 1] is null)
                {
                    movePositions.Add(new ChessPosition(Position.X, Position.Y - 2));
                }
                if (new ChessPosition(Position.X + 1, Position.Y - 1).IsOnBoard && board.Figures[Position.X + 1, Position.Y - 1] is not null && board.Figures[Position.X + 1, Position.Y - 1].Color != Color)
                {
                    movePositions.Add(new ChessPosition(Position.X + 1, Position.Y - 1));
                }
                if (new ChessPosition(Position.X - 1, Position.Y - 1).IsOnBoard && board.Figures[Position.X - 1, Position.Y - 1] is not null && board.Figures[Position.X - 1, Position.Y - 1].Color != Color)
                {
                    movePositions.Add(new ChessPosition(Position.X - 1, Position.Y - 1));
                }

                var tmp = board.Fen.Split(" ")[^3];
                if (tmp != "-")
                {
                    ChessPosition PawnPosition = new ChessPosition(tmp);
                    if (Position.Y == 3 && Math.Abs(Position.X - PawnPosition.X) == 1)
                    {
                        movePositions.Add(PawnPosition);
                    }
                }
            }
            return movePositions;
        }

        public override ChessMove Move(ChessPosition to, Board board, Func<Color, EnumFigure> func)
        {
            if (GetMovePositionsWithCheckCheck(board).Contains(to))
            {
                if (Position.X != to.X && board.Figures[to.X, to.Y] is null)
                {
                    if (Color == Color.White)
                    {
                        return new ChessMove(this, FigureCreater.CreateFigure(EnumFigure, Color, to), board.Figures[to.X, to.Y - 1]);
                    }
                    if (Color == Color.Black)
                    {
                        return new ChessMove(this, FigureCreater.CreateFigure(EnumFigure, Color, to), board.Figures[to.X, to.Y + 1]);
                    }
                }
                return new ChessMove(this, FigureCreater.CreateFigure((to.Y % 7 == 0) ? func(Color) : EnumFigure, Color, to), board.Figures[to.X, to.Y]);
            }
            throw new CannotMoveException();
        }

        #region Ctor
        public Pawn(Color color, ChessPosition position) : base(color, position)
        {
        }
        #endregion
    }
}
