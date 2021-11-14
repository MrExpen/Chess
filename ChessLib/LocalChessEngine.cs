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
        protected virtual Board Board { get; set; }
        public virtual Color Turn => Board.Turn;
        public virtual string Fen => Board.Fen;
        public virtual Color Winner
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
        public virtual bool IsTie => IsStalemate || IsTie1 || IsTie2;
        public virtual bool InGame => Winner == Color.None && !IsTie;
        protected virtual bool IsStalemate => !Board.IsChecked(Turn) && Figures.Where(f => f.Color == Turn).All(f => GetMoves(f).Count() == 0);
        protected virtual bool IsTie1 => Board.HalfmoveClock == 100;
        protected virtual bool IsTie2 => Moves.GroupBy(m => Regex.Match(m, @"\S+\s\S+").Value).Select(x => x.Count()).Any(x => x >= 3);


        public List<string> Moves { get; protected set; } = new List<string>();

        public virtual ChessFigure GetFigure(ChessPosition chessPosition)
            => GetFigure(chessPosition.X, chessPosition.Y);
        public virtual ChessFigure GetFigure(int x, int y)
            => Board.Figures[x, y];
        public virtual IEnumerable<ChessFigure> Figures
            => Board.Figures.Cast<ChessFigure>().Where(f => f is not null);
        public virtual IEnumerable<ChessPosition> GetMoves(int x, int y)
            => GetFigure(x, y)?.GetMovePositionsWithCheckCheck(Board) ?? new List<ChessPosition>();
        public virtual IEnumerable<ChessPosition> GetMoves(ChessPosition chessPosition)
            => GetMoves(chessPosition.X, chessPosition.Y);
        public virtual IEnumerable<ChessPosition> GetMoves(ChessFigure chessFigure)
            => GetMoves(chessFigure.Position);
        public virtual bool Move(ChessPosition from, ChessPosition to, EnumFigure figure=EnumFigure.None)
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
        public LocalChessEngine(params string[] fens) : this(fens.AsEnumerable())
        {
        }
        public LocalChessEngine(IEnumerable<string> fens) : this(fens.Last())
        {
            Moves = new List<string>(fens);
        }
        #endregion
    }
}
