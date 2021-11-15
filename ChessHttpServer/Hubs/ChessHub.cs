﻿using ChessHttpServer.Data;
using ChessLib.Engines;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using ChessLib.Data;
using ChessLib.Figures;

namespace ChessHttpServer.Hubs
{
    public class ChessHub : Hub
    {
        public async Task Move(int MatchId, string from, string to, string figure)
        {
            using (var db = new ApplicationDbContext())
            {
                var Match = await db.ChessMatchs.FindAsync(MatchId);
                if (Match is null)
                {
                    return;
                }
                    
                var engine = new LocalChessEngine(Match.Fens.Select(x => x.Data));
                engine.OnTurnChanged += async (sender, args) =>
                {
                    Match.Fens.Add(new FenStringData(args.FenNow));
                    await Clients.Others.SendAsync("Move", MatchId, args);
                };
                engine.Move(new ChessPosition(from), new ChessPosition(to), figure.Length > 0 ? (EnumFigure)figure[0] : EnumFigure.None);
            }
        }
    }
}