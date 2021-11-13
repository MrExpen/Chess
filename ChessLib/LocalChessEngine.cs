using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChessLib
{
    public class LocalChessEngine : IChessEngine
    {
        protected Board Board { get; private set; }
        public Color Turn => Board.Turn;
        public string Fen => Board.Fen;
        public Color Winner
        {
            get
            {
                if (Board.IsChecked(Turn) && Figures.Where(f => f.Color == Turn).All(f => GetMoves(f).Count() == 0))
                {
                    return Turn.Flip();
                }
                return Color.None;
            }
        }
        public bool IsTie => IsStalemate || IsTie1 || IsTie2;
        public bool InGame => Winner == Color.None && !IsTie;
        protected bool IsStalemate => !Board.IsChecked(Turn) && Figures.Where(f => f.Color == Turn).All(f => GetMoves(f).Count() == 0);
        protected bool IsTie1 => Board.HalfmoveClock == 100;
        protected bool IsTie2 => Moves.GroupBy(m => Regex.Match(m, @"\S+\s\S+").Value).Select(x => x.Count()).Any(x => x >= 3);


        public List<string> Moves { get; private set; } = new List<string>();

        public ChessFigure GetFigure(ChessPosition chessPosition)
            => GetFigure(chessPosition.X, chessPosition.Y);
        public ChessFigure GetFigure(int x, int y)
            => Board.Figures[x, y];
        public IEnumerable<ChessFigure> Figures
            => Board.Figures.Cast<ChessFigure>().Where(f => f is not null);
        public IEnumerable<ChessPosition> GetMoves(int x, int y)
            => GetFigure(x, y)?.GetMovePositionsWithCheckCheck(Board) ?? new List<ChessPosition>();
        public IEnumerable<ChessPosition> GetMoves(ChessPosition chessPosition)
            => GetMoves(chessPosition.X, chessPosition.Y);
        public IEnumerable<ChessPosition> GetMoves(ChessFigure chessFigure)
            => GetMoves(chessFigure.Position);
        public void Restart(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") 
            => Board = new Board(fen);
        public bool Move(ChessPosition from, ChessPosition to, EnumFigure figure=EnumFigure.None)
        {
            var tmp = Board.Move(from, to, figure);
            if (tmp)
            {
                Moves.Add(Board.Fen);
            }
            return tmp;
        }

        #region Ctor
        public LocalChessEngine(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            Moves.Add(fen);
            Board = new Board(fen);
        }
        #endregion
    }
}
