namespace ChessLib.Data
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
