using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessLib
{
    public class Board
    {
        public ChessFigure[,] Figures { get; private set; } = new ChessFigure[8, 8];
        public bool Check(Color color)
        {
            switch (color)
            {
                case Color.None:
                    return false;
                case Color.Black:
                    {
                        ChessFigure BKing = Figures.Cast<ChessFigure>().First(f => f?.EnumFigure == EnumFigure.King && f?.Color == Color.Black);
                        foreach (var Wfigure in Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.White))
                        {
                            if (Wfigure.GetMovePositions(this).Contains(BKing.Position))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case Color.White:
                    {
                        ChessFigure WKing = Figures.Cast<ChessFigure>().First(f => f?.EnumFigure == EnumFigure.King && f?.Color == Color.White);
                        foreach (var Bfigure in Figures.Cast<ChessFigure>().Where(f => f?.Color == Color.Black))
                        {
                            if (Bfigure.GetMovePositions(this).Contains(WKing.Position))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                default:
                    throw new ArgumentException();
            }
        }
        public Color Turn { get; private set; }
        public List<string> Moves { get; private set; } = new List<string>();
        protected bool IsStalemate => !Check(Turn) && Figures.Cast<ChessFigure>().Where(f => f?.Color == Turn).All(f => f.GetMovePositionsWithCheckCheck(this).Count == 0);
        protected bool IsTie1 => HalfmoveClock == 100;
        protected bool IsTie2 => Moves.GroupBy(m => Regex.Match(m, @"\S+\s\S+").Value).Select(x => x.Count()).Any(x => x >= 3);
        public bool IsTie => IsStalemate || IsTie1 || IsTie2;
        public Color Winner
        {
            get
            {
                if (Check(Turn) && Figures.Cast<ChessFigure>().Where(f => f?.Color == Turn).All(f => f.GetMovePositionsWithCheckCheck(this).Count == 0))
                {
                    return Turn.Flip();
                }
                return Color.None;
            }
        }
        public int HalfmoveClock { get; private set; } = 0;
        public int FullmoveNumber { get;private set; } = 0;
        public string Fen { get; private set; }

        public bool WhiteLongCastling { get; set; } = false;
        public bool WhiteShortCastling { get; set; } = false;
        public bool BlackLongCastling { get; set; } = false;
        public bool BlackShortCastling { get; set; } = false;

        public void Move(ChessPosition from, ChessPosition to, EnumFigure figure=EnumFigure.None)
        {
            if (Figures[from.X, from.Y] is null)
            {
                throw new ArgumentException();
            }
            Move(Figures[from.X, from.Y].Move(to, this));
        }
        public void Move(ChessMove chessMove)
        {
            if (chessMove.From.Color != Turn)
            {
                throw new ArgumentException();
            }
            Figures[chessMove.From.Position.X, chessMove.From.Position.Y] = null;
            Figures[chessMove.To.Position.X, chessMove.To.Position.Y] = chessMove.To;
            if (chessMove.Eat is not null && chessMove.Eat.Position != chessMove.To.Position)
            {
                Figures[chessMove.Eat.Position.X, chessMove.Eat.Position.Y] = null;
            }
            if (chessMove.From.EnumFigure == EnumFigure.Rook)
            {
                if (chessMove.From.Color == Color.White)
                {
                    if (chessMove.From.Position == new ChessPosition(0, 0))
                    {
                        WhiteLongCastling = false;
                    }
                    else if (chessMove.From.Position == new ChessPosition(7, 0))
                    {
                        WhiteShortCastling = false;
                    }
                    else if (chessMove.To.Position == new ChessPosition(0,7))
                    {
                        BlackLongCastling = false;
                    }
                    else if (chessMove.To.Position == new ChessPosition(7, 7))
                    {
                        BlackShortCastling = false;
                    }
                }
                else if (chessMove.From.Color == Color.Black)
                {
                    if (chessMove.From.Position == new ChessPosition(0, 7))
                    {
                        BlackLongCastling = false;
                    }
                    else if (chessMove.From.Position == new ChessPosition(7, 7))
                    {
                        BlackShortCastling = false;
                    }
                    else if (chessMove.To.Position == new ChessPosition(0, 0))
                    {
                        WhiteLongCastling = false;
                    }
                    else if (chessMove.To.Position == new ChessPosition(7, 0))
                    {
                        WhiteShortCastling = false;
                    }
                }
            }
            if (chessMove.From.EnumFigure == EnumFigure.King)
            {
                if (Math.Abs(chessMove.From.Position.X - chessMove.To.Position.X) == 2)
                {
                    if (chessMove.From.Position.X - chessMove.To.Position.X > 0)
                    {
                        Figures[0, chessMove.From.Position.Y] = null;
                        Figures[3, chessMove.From.Position.Y] = FigureCreater.CreateFigure(EnumFigure.Rook, chessMove.From.Color, new ChessPosition(3, chessMove.From.Position.Y));
                    }
                    else
                    {
                        Figures[7, chessMove.From.Position.Y] = null;
                        Figures[5, chessMove.From.Position.Y] = FigureCreater.CreateFigure(EnumFigure.Rook, chessMove.From.Color, new ChessPosition(5, chessMove.From.Position.Y));
                    }
                }

                if (chessMove.From.Color == Color.White)
                {
                    WhiteLongCastling = false;
                    WhiteShortCastling = false;
                }
                else if (chessMove.From.Color == Color.Black)
                {
                    BlackLongCastling = false;
                    BlackShortCastling = false;
                }
            }
            Turn = Turn.Flip();
            FullmoveNumber++;
            if (chessMove.Eat is null && chessMove.From.EnumFigure != EnumFigure.Pawn)
            {
                HalfmoveClock++;
            }
            else
            {
                HalfmoveClock = 0;
            }


            StringBuilder stringBuilder = new StringBuilder();
            for (int y = 7; y >= 0; y--)
            {
                int count = 0;
                for (int x = 0; x < 8; x++)
                {
                    if (Figures[x, y] is null)
                    {
                        count++;
                    }
                    else
                    {
                        if (count != 0)
                        {
                            stringBuilder.Append(count);
                            count = 0;
                        }
                        stringBuilder.Append(Figures[x, y].Char);
                    }

                }
                if (count != 0)
                {
                    stringBuilder.Append(count);
                }
                if (y != 0)
                {
                    stringBuilder.Append('/');
                }
            }
            stringBuilder.Append(' ');
            stringBuilder.Append(Turn == Color.White ? 'w' : 'b');
            stringBuilder.Append(' ');

            if (WhiteShortCastling)
            {
                stringBuilder.Append('K');
            }
            if (WhiteLongCastling)
            {
                stringBuilder.Append('Q');
            }
            if (BlackShortCastling)
            {
                stringBuilder.Append('k');
            }
            if (BlackLongCastling)
            {
                stringBuilder.Append('q');
            }

            if (!(WhiteLongCastling || WhiteShortCastling || BlackLongCastling || BlackShortCastling))
            {
                stringBuilder.Append('-');
            }

            stringBuilder.Append(' ');

            if (chessMove.From.EnumFigure == EnumFigure.Pawn && Math.Abs(chessMove.From.Position.Y - chessMove.To.Position.Y) == 2)
            {
                stringBuilder.Append(new ChessPosition(chessMove.From.Position.X, chessMove.From.Position.Y + (chessMove.From.Color == Color.White ? +1 : -1)).ToString());
            }
            else
            {
                stringBuilder.Append('-');
            }

            stringBuilder.Append($" {HalfmoveClock} {FullmoveNumber}");

            Fen = stringBuilder.ToString();
            Moves.Add(Fen);
        }

        #region Ctor
        public Board(string fen= "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        {
            Moves.Add(fen);
            Fen = fen;
            int y = 7;
            int x = 0;
            var tmp = fen.Split();
            for (int i = 0; i < tmp[0].Length; i++)
            {
                if (tmp[0][i] == '/')
                {
                    y--;
                    x = 0;
                    continue;
                }
                EnumFigure figure = Enum.GetValues<EnumFigure>().FirstOrDefault(f => (char)f == char.ToLower(tmp[0][i]));
                if (figure == default)
                {
                    x += int.Parse(tmp[0][i].ToString());
                }
                else
                {
                    Figures[x, y] = FigureCreater.CreateFigure(figure, char.IsLower(tmp[0][i]) ? Color.Black : Color.White, new ChessPosition(x, y));
                    x++;
                }
            }
            Turn = tmp[1][0] == 'w' ? Color.White : Color.Black;

            if (tmp[2][0] != '-')
            {
                for (int i = 0; i < tmp[2].Length; i++)
                {
                    switch (tmp[2][i])
                    {
                        case 'Q':
                            WhiteLongCastling = true;
                            break;

                        case 'q':
                            BlackLongCastling = true;
                            break;

                        case 'K':
                            WhiteShortCastling = true;
                            break;

                        case 'k':
                            BlackShortCastling = true;
                            break;
                    }
                }
            }

            if (tmp[3][0] != '-')
            {
                var to = new ChessPosition(tmp[3]);
                Moves.Add(Fen);
            }

            HalfmoveClock = int.Parse(tmp[4]);
            FullmoveNumber = int.Parse(tmp[5]);
        }
        public Board(Board board, ChessMove move)
        {
            WhiteLongCastling = board.WhiteLongCastling;
            WhiteShortCastling = board.WhiteShortCastling;
            BlackLongCastling = board.BlackLongCastling;
            BlackShortCastling = board.BlackShortCastling;
            foreach (var figure in board.Figures.Cast<ChessFigure>().Where(x => x is not null))
            {
                Figures[figure.Position.X, figure.Position.Y] = figure;
            }
            Turn = board.Turn;
            Moves = new List<string>(board.Moves);
            Move(move);
        }
        #endregion
    }
}