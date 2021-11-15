using System;

namespace ChessLib.Data
{
    public struct ChessPosition
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsOnBoard => X >= 0 && X < 8 && Y >= 0 && Y < 8;

        #region Overrides
        public override string ToString()
        {
            return $"{(char)(X + 'a')}{(char)(Y + '1')}";
        }
        public override bool Equals(object obj)
        {
            if (obj is ChessPosition position)
            {
                return position.X == X && position.Y == Y;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        #endregion

        #region Operators
        public static ChessPosition operator +(ChessPosition left, ChessPosition right)
            => new ChessPosition(left.X + right.X, left.Y + right.Y);
        public static bool operator ==(ChessPosition left, ChessPosition right)
            => left.Equals(right);
        public static bool operator !=(ChessPosition left, ChessPosition right)
            => !left.Equals(right);
        #endregion

        #region Ctor
        public ChessPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public ChessPosition(string cell)
        {
            if (cell.Length != 2)
            {
                throw new ArgumentException();
            }
            if (!(cell[0] >= 'a' && cell[0] <= 'h'))
            {
                throw new ArgumentException();
            }
            if (!(cell[1] >= '1' && cell[1] <= '8'))
            {
                throw new ArgumentException();
            }
            X = cell[0] - 'a';
            Y = cell[1] - '1';
        }
        #endregion
    }
}
