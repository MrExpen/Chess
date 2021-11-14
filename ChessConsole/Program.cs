using System;
using System.Linq;
using ChessLib;
using ChessLib.Http;
using ChessLib.Figures;
using System.Threading;

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
                
                if (!chess.MyTurn)
                {
                    Console.WriteLine("Not your turn.");
                    while (!chess.MyTurn)
                    {
                        Thread.Sleep(400);
                    }
                    Console.Clear();
                    continue;
                }
                var move = Console.ReadLine();
                Console.Clear();
                bool reslut = false;
                try
                {
                    if (move.Length > 4)
                    {
                        reslut = chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)), (EnumFigure)move[4]);
                    }
                    else
                    {
                        reslut = chess.Move(new ChessPosition(move.Substring(0, 2)), new ChessPosition(move.Substring(2, 2)));
                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Invalid input");
                    continue;
                }
                if (!reslut)
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }
}
