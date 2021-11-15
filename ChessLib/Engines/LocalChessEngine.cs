using ChessLib.Data;
using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLib.Engines
{
    public class LocalChessEngine : IChessEngine
    {
        protected Board Board { get; set; }
        public virtual Color Turn
        {
            get
            {
                lock (_lock)
                {
                    return Board.Turn;
                }
            }
        }
        public virtual string Fen
        {
            get
            {
                lock (_lock)
                {
                    return Board.Fen;
                }
            }
        }
        public virtual Color Winner
        {
            get
            {
                lock (_lock)
                {
                    if (Board.IsChecked(Turn) && Figures.Where(f => f.Color == Turn).All(f => GetMoves(f).Count() == 0))
                    {
                        return Turn.Flip();
                    }
                    return Color.None;
                }
            }
        }

        #region GameResults
        public virtual bool InGame => Winner == Color.None && !IsTie;
        public virtual bool IsTie => IsStalemate || IsTie1 || IsTie2;
        protected virtual bool IsStalemate
        {
            get
            {
                lock (_lock)
                {
                    return !Board.IsChecked(Turn) && Figures.Where(f => f.Color == Turn).All(f => GetMoves(f).Count() == 0);
                }
            }
        }
        protected virtual bool IsTie1
        {
            get
            {
                lock (_lock)
                {
                    return Board.HalfmoveClock == 100;
                }
            }
        }
        protected virtual bool IsTie2
        {
            get
            {
                lock (_lock)
                {
                    return Moves.GroupBy(m => m.Split(" ").Take(2)).Select(x => x.Count()).Any(x => x >= 3);
                }
            }
        }
        #endregion

        public virtual List<string> Moves { get; protected set; }
        public object _lock = new object();

        public virtual event Action<object, TurnChangedEventArgs> OnTurnChanged;

        public virtual IEnumerable<ChessFigure> Figures
        {
            get
            {
                lock (_lock)
                {
                    return Board.Figures.Cast<ChessFigure>().Where(f => f is not null).ToArray();
                }
            }
        }

        public virtual ChessFigure GetFigure(ChessPosition chessPosition)
            => GetFigure(chessPosition.X, chessPosition.Y);
        public virtual ChessFigure GetFigure(int x, int y)
        {
            lock (_lock)
            {
                return Board.Figures[x, y];
            }
        }

        public virtual IEnumerable<ChessPosition> GetMoves(ChessFigure chessFigure)
            => GetMoves(chessFigure.Position);
        public virtual IEnumerable<ChessPosition> GetMoves(ChessPosition chessPosition)
            => GetMoves(chessPosition.X, chessPosition.Y);
        public virtual IEnumerable<ChessPosition> GetMoves(int x, int y)
        {
            lock (_lock)
            {
                return GetFigure(x, y)?.GetMovePositionsWithCheckCheck(Board)?.ToArray() ?? Array.Empty<ChessPosition>();
            }
        }


        public virtual bool Move(ChessPosition from, ChessPosition to, EnumFigure figure = EnumFigure.Queen)
            => Move(from, to, (col) => figure);
        public virtual bool Move(ChessPosition from, ChessPosition to, Func<Color, EnumFigure> func)
        {
            (bool Success, bool IsEat) result;
            lock (_lock)
            {
                result = Board.Move(from, to, func);
                if (result.Success)
                {
                    Moves.Add(Board.Fen);
                }
            }
            OnTurnChanged?.Invoke(this, new TurnChangedEventArgs { IsEat = result.IsEat, From = from, To = to, TurnNow = Turn, IsChecked = Board.IsChecked(Turn) });
            return result.Success;
        }
            
        #region Ctor
        public LocalChessEngine(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            lock (_lock)
            {
                Moves = new List<string>();
                Moves.Add(fen);
                Board = new Board(fen);
            }
        }
        public LocalChessEngine(params string[] fens) : this(fens.AsEnumerable())
        {
        }
        public LocalChessEngine(IEnumerable<string> fens) : this(fens.Last())
        {
            lock (_lock)
            {
                Moves = new List<string>(fens);
            }
        }
        #endregion
    }
}
