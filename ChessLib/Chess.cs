using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public class Chess
    {
        public Board Board { get; private set; }
        public Color Turn => Board.Turn;
        public string Fen => Board.Fen;
        public Color Winner => Board.Winner;
        public bool IsTie => Board.IsTie;
        public bool InGame => Winner == Color.None && !IsTie;


        public void Restart(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") 
            => Board = new Board(fen);
        public void Move(ChessPosition from, ChessPosition to, EnumFigure figure=EnumFigure.None)
            => Board.Move(from, to, figure);
        public void Move(ChessMove chessMove)
            => Board.Move(chessMove);

        #region Ctor
        public Chess(string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            Board = new Board(fen);
        }
        #endregion
    }
}
