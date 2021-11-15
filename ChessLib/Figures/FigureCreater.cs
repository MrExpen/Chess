using ChessLib.Data;
using System;
using System.Drawing;

namespace ChessLib.Figures
{
    public static class FigureCreater
    {
        public static ChessFigure CreateFigure(EnumFigure enumFigure, Data.Color color, ChessPosition position)
        {
            switch (enumFigure)
            {
                case EnumFigure.Knight:
                    return new Knight(color, position);
                case EnumFigure.Rook:
                    return new Rook(color, position);
                case EnumFigure.Bishop:
                    return new Bishop(color, position);
                case EnumFigure.King:
                    return new King(color, position);
                case EnumFigure.Queen:
                    return new Queen(color, position);
                case EnumFigure.Pawn:
                    return new Pawn(color, position);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
