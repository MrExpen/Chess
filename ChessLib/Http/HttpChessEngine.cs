using ChessLib.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using ChessLib.Http.Responses;
using ChessLib.Exceptions;

namespace ChessLib.Http
{
    public class HttpChessEngine : LocalChessEngine, IChessEngine
    {
        private RestClient _restClient { get; set; }
        public string Name { get; set; }
        public int? MatchId { get; private set; }
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                _restClient = new RestClient();
            }
        }
        private string _url;

        public override IEnumerable<ChessPosition> GetMoves(int x, int y)
        {
            return MatchId.HasValue ? base.GetMoves(x, y) : throw new YouAreNotInMatchException();
        }
        public override ChessFigure GetFigure(int x, int y)
        {
            return MatchId.HasValue ? base.GetFigure(x, y) : throw new YouAreNotInMatchException();
        }
        public override IEnumerable<ChessFigure> Figures 
            => MatchId.HasValue ? base.Figures : throw new YouAreNotInMatchException();
        public override bool Move(ChessPosition from, ChessPosition to, EnumFigure figure = EnumFigure.None)
        {
            if (!MatchId.HasValue)
            {
                throw new YouAreNotInMatchException();
            }
            RestRequest request = new RestRequest("api/move", Method.GET);
            request.AddQueryParameter("name", Name);
            request.AddQueryParameter("matchId", Name);
            if (figure != EnumFigure.None)
            {
                request.AddQueryParameter("dist", ((char)figure).ToString());
            }
            var response = JsonConvert.DeserializeObject<MoveResponse>(_restClient.Execute(request).Content);
            if (response.Success)
            {
                base.Move(from, to, figure);
            }
            return response.Success;
        }
        
        public bool CreateMatch(string OponentName)
        {
            RestRequest request = new RestRequest("api/creatematch");
            request.AddQueryParameter("whiteName", Name);
            request.AddQueryParameter("blackName", OponentName);
            var response = JsonConvert.DeserializeObject<CreateMatchResponse>(_restClient.Execute(request).Content);
            return response.Success;
        }
        public bool JoinMatch(int matchId)
        {
            MatchId = matchId;
            RestRequest request = new RestRequest("api/creatematch");
            request.AddQueryParameter("matchId", MatchId.Value.ToString());
            var response = JsonConvert.DeserializeObject<GetAllFensRespons>(_restClient.Execute(request).Content);
            if (response.Success)
            {
                Moves = new List<string>(response.Fens);
                Board = new Board(Moves.Last());
            }
            return response.Success;
        }
        public string GetLastFen()
        {
            RestRequest request = new RestRequest("api/creatematch");
            request.AddQueryParameter("matchId", MatchId.Value.ToString());
            var response = JsonConvert.DeserializeObject<GetLastFenResponse>(_restClient.Execute(request).Content);
            return response.Success ? response.Fen : null;
        }

        #region Ctor
        public HttpChessEngine(string name, string url) : this()
        {
            Name = name;
            Url = url;
            
        }
        public HttpChessEngine()
        {
            _restClient = new RestClient();
        }
        #endregion
    }
}
