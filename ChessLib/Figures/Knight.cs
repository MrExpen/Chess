using ChessLib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Figures
{
    public class Knight : ChessFigure
    {
        public Knight(Color color, ChessPosition position) : base(color, position)
        {
        }

        public override EnumFigure EnumFigure => EnumFigure.Knight;

        public override List<ChessPosition> GetMovePositions(Board board)
        {
            List<ChessPosition> chessPositions = new List<ChessPosition>();

            chessPositions.AddRange(new[]
            {
                Position + new ChessPosition(1, 2),
                Position + new ChessPosition(1, -2),
                Position + new ChessPosition(-1, 2),
                Position + new ChessPosition(-1, -2),

                Position + new ChessPosition(2, 1),
                Position + new ChessPosition(2, -1),
                Position + new ChessPosition(-2, 1),
                Position + new ChessPosition(-2, -1),
            }.Where(x => x.IsOnBoard && (board.Figures[x.X, x.Y]?.Color != Color || board.Figures[x.X, x.Y] is null)));

            return chessPositions;
        }
    }
}
