using ChessLib.Data;
using ChessLib.Exceptions;
using ChessLib.Figures;
using ChessLib.Http.Responses;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ChessLib.Engines
{
    [Obsolete("Unstable")]
    public class HttpChessEngine : LocalChessEngine, IChessEngine, IDisposable
    {
        private object _lock = new object();
        private RestClient _restClient { get; set; }
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
        private Timer Timer { get; set; }
        protected override bool IsTie2
        {
            get
            {
                lock (_lock)
                {
                     return base.IsTie2;
                }
            }
        }

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
                lock (_lock)
                {
                    base.Move(from, to, figure);
                }
                if (!InGame)
                {
                    Timer.Stop();
                }
                else
                {
                    Timer.Start();
                }
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
                lock (_lock)
                {
                    Moves = new List<string>(response.Fens);
                    Board = new Board(Moves.Last());
                    MyColor = MyColor = Name == response.WhiteName ? Color.White : Name == response.BlackName ? Color.Black : Color.None;
                    if (!MyTurn || MyColor == Color.None)
                    {
                        Timer.Start();
                    }
                }
            }
            return response.Success;
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
            Timer = new Timer(1500);
            Timer.Elapsed += Timer_Elapsed;
        }
        #endregion

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (MatchId.HasValue)
            {
                try
                {
                    var newFen = GetLastFen();
                    if (newFen != Fen)
                    {
                        lock ( _lock)
                        {
                            Moves.Add(newFen);
                            Board = new Board(newFen);
                        }
                    }
                    if (MyTurn)
                    {
                        Timer.Stop();
                    }
                }
                catch (Exception) { }
            }
        }

        public void Dispose()
        {
            Timer.Dispose();
        }
    }
}
