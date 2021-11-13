using ChessHttpServer.Respones;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ChessLib;
using ChessHttpServer.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace ChessHttpServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChessController : ControllerBase
    {
        [Route("move")]
        [HttpGet]
        public MoveResponse Move([FromQuery] string name, [FromQuery] int? matchId, [FromQuery] string from, [FromQuery] string to, [FromQuery] string dest)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || !matchId.HasValue)
            {
                return new MoveResponse { Success = false, Error = "Missing parameters" };
            }
            using (var db = new ApplicationDbContext())
            {
                var Match = db.ChessMatchs.Find(matchId.Value);
                if (Match is null)
                {
                    return new MoveResponse { Success = false, Error = "Match not found" };
                }
                LocalChessEngine engine = new LocalChessEngine(Match.Fens.Select(x => x.Data));
                if (!(engine.Turn == Color.White ? name == Match.WhiteName : name == Match.BlackName))
                {
                    return new MoveResponse { Success = false, Error = "Not your turn" };
                }
                var result = engine.Move(new ChessPosition(from), new ChessPosition(to), string.IsNullOrEmpty(dest) ? ChessLib.Figures.EnumFigure.None : (ChessLib.Figures.EnumFigure)dest[0]);
                if (result)
                {
                    Match.Fens.Add(new(engine.Fen));
                    db.SaveChanges();
                }
                return new MoveResponse { Success = result, Fen = engine.Fen };
            }
        }

        [Route("creatematch")]
        [HttpGet]
        public CreateMatchResult CreateMatch([FromQuery] string whiteName, [FromQuery] string blackName)
        {
            var fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var ChessMatch = new ChessMatch()
                    {
                        BlackName = blackName,
                        WhiteName = whiteName
                    };
                    ChessMatch.Fens.Add(new FenStringData(fen));
                    db.ChessMatchs.Add(ChessMatch);
                    db.SaveChanges();

                    return new CreateMatchResult
                    {
                        Success = true,
                        Fen = fen,
                        WhiteName = whiteName,
                        BlackName = blackName,
                        MatchId = ChessMatch.Id
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateMatchResult { Success = false, Error = e.ToString() };
            }
        }

        [Route("getlastfen")]
        [HttpGet]
        public GetLastFenResponse GetLastFen([FromQuery] int? matchId)
        {
            if (!matchId.HasValue)
            {
                return new GetLastFenResponse
                {
                    Success = false,
                    Error = "matchId not set"
                };
            }
            using (var db = new ApplicationDbContext())
            {
                var Match = db.ChessMatchs.Find(matchId.Value);
                if (Match is null)
                {
                    return new GetLastFenResponse { Success = false, Error = "Match not found" };
                }
                return new GetLastFenResponse { Success = true, Fen = Match.Fens.Last().Data };
            }
        }

        [Route("getallfens")]
        [HttpGet]
        public GetAllFensRespons GetAllFens([FromQuery] int? matchId)
        {
            if (!matchId.HasValue)
            {
                return new GetAllFensRespons
                {
                    Success = false,
                    Error = "matchId not set"
                };
            }
            using (var db = new ApplicationDbContext())
            {
                var Match = db.ChessMatchs.Find(matchId.Value);
                if (Match is null)
                {
                    return new GetAllFensRespons { Success = false, Error = "Match not found" };
                }
                return new GetAllFensRespons { Success = true, Fens = Match.Fens.Select(x => x.Data) };
            }
        }
    }
}
