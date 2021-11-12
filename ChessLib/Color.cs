using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public enum Color : byte
    {
        None,
        White,
        Black
    }

    public static class ColorExtensions
    {
        public static Color Flip(this Color color)
        {
            if (color == Color.White)
                return Color.Black;
            if (color == Color.Black)
                return Color.White;
            return Color.None;
        }
    }
}
