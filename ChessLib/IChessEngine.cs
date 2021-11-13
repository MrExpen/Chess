using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public interface IChessEngine
    {
        Color Turn { get; }
        string Fen { get; }
        bool InGame { get; }

        IEnumerable<ChessFigure> Figures { get; }

        ChessFigure GetFigure(ChessPosition chessPosition);
        ChessFigure GetFigure(int x, int y);

        IEnumerable<ChessPosition> GetMoves(ChessFigure chessFigure);
        IEnumerable<ChessPosition> GetMoves(ChessPosition chessPosition);
        IEnumerable<ChessPosition> GetMoves(int x, int y);

        bool Move(ChessPosition from, ChessPosition to, EnumFigure figure = EnumFigure.None);
    }
}
