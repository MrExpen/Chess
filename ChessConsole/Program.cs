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
            IChessEngine chess = new LocalChessEngine();
            while (chess.InGame)
            {
                Console.WriteLine(chess.Fen);
                Console.WriteLine(string.Join(" ", chess.Figures.Where(f => f.Color == chess.Turn).SelectMany(x => chess.GetMoves(x).Select(y => $"{x.Position}{y}"))));
                for (int y = 7; y >= 0; y--)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        Console.Write(chess.GetFigure(x, y)?.Char ?? '.');
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
