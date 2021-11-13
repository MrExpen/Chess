using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Figures
{
    public abstract class ChessFigure
    {
        public ChessPosition Position { get; private set; }
        public Color Color { get; private set; }
        public abstract EnumFigure EnumFigure { get; }

        public virtual ChessMove Move(ChessPosition to, Board board, EnumFigure figure=EnumFigure.None)
        {
            if (GetMovePositionsWithCheckCheck(board).Contains(to))
            {
                return new ChessMove(this, FigureCreater.CreateFigure(figure == EnumFigure.None ? EnumFigure : figure, Color, to), board.Figures[to.X, to.Y]);
            }
            throw new ArgumentException();
        }
        public virtual List<ChessPosition> GetMovePositionsWithCheckCheck(Board board)
            => RemoveChecks(GetMovePositions(board), board);
        public abstract List<ChessPosition> GetMovePositions(Board board);
        protected List<ChessPosition> RemoveChecks(List<ChessPosition> move, Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();

            foreach (ChessPosition movePosition in move)
            {
                Board NewBoard = new Board(board, new ChessMove(this, FigureCreater.CreateFigure(EnumFigure, Color, movePosition)));
                if (!NewBoard.IsChecked(board.Turn))
                {
                    chessPositions.Add(movePosition);
                }
            }

            return chessPositions;
        }
        protected List<ChessPosition> RemoveSelfEat(List<ChessPosition> move, Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();

            foreach (ChessPosition movePosition in move)
            {
                if (board.Figures[movePosition.X, movePosition.Y] is null || board.Figures[movePosition.X, movePosition.Y].Color != Color)
                {
                    chessPositions.Add(movePosition);
                }
            }

            return chessPositions;
        }

        public abstract char Char { get; }

        #region EqualsOverride
        public override string ToString()
        {
            return $"{Position} {Color} {EnumFigure}";
        }
        public override bool Equals(object obj)
        {
            if (obj is ChessFigure figure)
            {
                return this.EnumFigure == figure.EnumFigure && this.Position == figure.Position;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public static bool operator ==(ChessFigure left, ChessFigure right)
            => left.Equals(right);
        public static bool operator !=(ChessFigure left, ChessFigure right)
            => !left.Equals(right);
        #endregion

        #region Ctor
        public ChessFigure(Color color, ChessPosition position)
        {
            Color = color;
            Position = position;
        }
        #endregion

    }
}
