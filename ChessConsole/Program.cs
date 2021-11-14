using System;
using System.Linq;
using ChessLib;
using ChessLib.Http;
using ChessLib.Figures;

namespace ChessConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpChessEngine chess = new HttpChessEngine(args[0], "https://chess.mrexpen.ru:4432/");
            chess.JoinMatch(1);
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
                bool reslut = false;
                if (move.Length > 4)
                {
                    reslut = chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)), (EnumFigure)move[4]);
                }
                else
                {
                    reslut = chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)));
                }
                if (!reslut)
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }
}
