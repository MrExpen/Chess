﻿using ChessLib.Figures;
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

namespace ChessLib.Http
{
    public class HttpChessEngine : LocalChessEngine, IChessEngine, IDisposable
    {
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
        private Timer Timer { get; set; } = new Timer(1000);

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
            var response = JsonConvert.DeserializeObject<MoveResponse>(_restClient.Execute(request).Content);
            if (response.Success)
            {
                base.Move(from, to, figure);
                if (!InGame)
                {
                    Timer.Stop();
                }
            }
            return response.Success;
        }
        
        public int? CreateMatch(string White, string Black)
        {
            RestRequest request = new RestRequest($"{Url}/api/chess/creatematch");
            request.AddQueryParameter("whiteName", White);
            request.AddQueryParameter("blackName", Black);
            var response = JsonConvert.DeserializeObject<CreateMatchResponse>(_restClient.Execute(request).Content);

            return response.Success ? response.MatchId : null;
        }
        public int? CreateMatch(string OponentName)
            => CreateMatch(Name, OponentName);
        public bool JoinMatch(int matchId)
        {
            MatchId = matchId;
            RestRequest request = new RestRequest($"{Url}/api/chess/getallfens");
            request.AddQueryParameter("matchId", MatchId.Value.ToString());
            var response = JsonConvert.DeserializeObject<GetAllFensRespons>(_restClient.Execute(request).Content);
            if (response.Success)
            {
                Moves = new List<string>(response.Fens);
                Board = new Board(Moves.Last());
                MyColor = MyColor = Name == response.WhiteName ? Color.White : Name == response.BlackName ? Color.Black : Color.None;
                Timer.Start();
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
                        Moves.Add(newFen);
                        Board = new Board(newFen);
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