using System;
using System.Linq;
using ChessLib;
using ChessLib.Figures;

namespace ChessConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Chess chess = new Chess();
            while (chess.InGame)
            {
                Console.WriteLine(chess.Board.Fen);
                Console.WriteLine(string.Join(" ", chess.Board.Figures.Cast<ChessFigure>().Where(f => f?.Color == chess.Board.Turn).SelectMany(x => x.GetMovePositionsWithCheckCheck(chess.Board).Select(y => $"{x.Position}{y}"))));
                for (int y = 7; y >= 0; y--)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        Console.Write(chess.Board.Figures[x, y]?.Char ?? '.');
                    }
                    Console.WriteLine();
                }
                var move = Console.ReadLine();
                Console.Clear();
                try
                {
                    if (move.Length > 4)
                    {
                        chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)), (EnumFigure)move[4]);
                    }
                    chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)));
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }
}
