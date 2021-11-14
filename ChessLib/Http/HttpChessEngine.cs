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
using System.Timers;
using System.Threading;

namespace ChessLib.Http
{
    public class HttpChessEngine : LocalChessEngine, IChessEngine, IDisposable
    {
        private RestClient _restClient { get; set; }
        private RestClient _restClientTimeout { get; set; }
        public string Name { get; set; }
        public int? MatchId { get; private set; }
        public Color? MyColor { get; private set; }
        public bool MyTurn 
            => MatchId.HasValue ? MyColor.Value == Turn : throw new YouAreNotInMatchException();
        public string Url
        {
            get => _url;
            set
            {
                _url = value.EndsWith("/") ? value.Substring(0, value.Length - 1) : value;
            }
        }
        private string _url;
        private Thread UpdateThread;

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
            RestRequest request = new RestRequest($"{Url}/api/chess/move", Method.GET);
            request.AddQueryParameter("name", Name);
            request.AddQueryParameter("matchId", MatchId.Value.ToString());
            request.AddQueryParameter("from", from.ToString());
            request.AddQueryParameter("to", to.ToString());
            if (figure != EnumFigure.None)
            {
                request.AddQueryParameter("dest", ((char)figure).ToString());
            }
            
            if (figure != EnumFigure.None)
            {
                request.AddQueryParameter("dist", ((char)figure).ToString());
            }
            MoveResponse response = null;
            do
            {
                response = JsonConvert.DeserializeObject<MoveResponse>(_restClient.Execute(request).Content);
            } while (response is null);
            if (response.Success)
            {
                base.Move(from, to, figure);
            }
            return response.Success;
        }
        
        public int? CreateMatch(string White, string Black)
        {
            RestRequest request = new RestRequest($"{Url}/api/chess/creatematch");
            request.AddQueryParameter("whiteName", White);
            request.AddQueryParameter("blackName", Black);
            CreateMatchResponse response = null;
            do
            {
                response = JsonConvert.DeserializeObject<CreateMatchResponse>(_restClient.Execute(request).Content);
            } while (response is null);
            return response.Success ? response.MatchId : null;
        }
        public int? CreateMatch(string OponentName)
            => CreateMatch(Name, OponentName);
        public bool JoinMatch(int matchId)
        {
            MatchId = matchId;
            RestRequest request = new RestRequest($"{Url}/api/chess/getallfens");
            request.AddQueryParameter("matchId", MatchId.Value.ToString());
            GetAllFensRespons response = null;
            do
            {
                response = JsonConvert.DeserializeObject<GetAllFensRespons>(_restClient.Execute(request).Content);
            } while (response is null);
            if (response.Success)
            {
                Moves = new List<string>(response.Fens);
                Board = new Board(Moves.Last());
                MyColor = MyColor = Name == response.WhiteName ? Color.White : Name == response.BlackName ? Color.Black : Color.None;
            }
            return response.Success;
        }
        private void StartUpdating()
        {
            UpdateThread = new Thread(async () =>
            {
                while (true)
                {
                    if (!MatchId.HasValue)
                    {
                        await Task.Delay(100);
                    }
                    else
                    {
                        RestRequest request = new RestRequest($"{Url}/api/chess/pollinggetfens");
                        request.AddQueryParameter("matchId", MatchId.Value.ToString());
                        request.AddQueryParameter("time", 25.ToString());
                        request.AddQueryParameter("lastMove", Fen.Split().Last());
                        var resp = await _restClientTimeout.ExecuteAsync(request);
                        var response = JsonConvert.DeserializeObject<GetAllFensRespons>(resp.Content);
                        if (response is not null && response.Success)
                        {
                            foreach (var fen in response.Fens)
                            {
                                if (!Moves.Contains(fen))
                                {
                                    Moves.Add(fen);
                                }
                            }
                            if (response.Fens.Count() != 0 && Moves.Count != 0)
                            {
                                Board = new Board(Moves.Last());
                            }
                        }
                    }
                }
            });
            UpdateThread.Start();
        }
        public string GetLastFen()
        {
            RestRequest request = new RestRequest($"{Url}/api/chess/getlastfen");
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
        private HttpChessEngine()
        {
            _restClient = new RestClient();
            _restClientTimeout = new RestClient();
            _restClientTimeout.Timeout = 30000;
            StartUpdating();
        }
        #endregion

        public void Dispose()
        {
            UpdateThread.Join();
        }
    }
}
