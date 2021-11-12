using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public class ChessMove
    {
        public ChessFigure From { get; private set; }
        public ChessFigure To { get; private set; }
        public ChessFigure Eat { get; private set; }

        #region Overrides
        public override string ToString()
        {
            return $"{From} {To} {Eat}";
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is ChessMove move)
            {
                return From == move.From && To == move.To && Eat == move.Eat;
            }
            return false;
        }
        #endregion

        #region Operators
        public static bool operator ==(ChessMove left, ChessMove right)
            => left.Equals(right);
        public static bool operator !=(ChessMove left, ChessMove right)
            => !left.Equals(right);

        #endregion

        #region Ctor
        public ChessMove(ChessFigure from, ChessFigure to, ChessFigure eat = null)
        {
            From = from;
            To = to;
            Eat = eat;
        }
        #endregion

    }
}
