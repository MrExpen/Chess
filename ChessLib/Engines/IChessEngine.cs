using ChessLib.Data;
using ChessLib.Figures;
using System;
using System.Collections.Generic;

namespace ChessLib.Engines
{
    public interface IChessEngine
    {
        Color Turn { get; }
        string Fen { get; }
        bool InGame { get; }
        Color Winner { get; }
        bool IsTie { get; }

        IEnumerable<ChessFigure> Figures { get; }

        ChessFigure GetFigure(ChessPosition chessPosition);
        ChessFigure GetFigure(int x, int y);

        IEnumerable<ChessPosition> GetMoves(ChessFigure chessFigure);
        IEnumerable<ChessPosition> GetMoves(ChessPosition chessPosition);
        IEnumerable<ChessPosition> GetMoves(int x, int y);

        bool Move(ChessPosition from, ChessPosition to, EnumFigure figure = EnumFigure.Queen);
        bool Move(ChessPosition from, ChessPosition to, Func<Color, EnumFigure> func);

        event Action<object, TurnChangedEventArgs> OnTurnChanged;
    }
}
