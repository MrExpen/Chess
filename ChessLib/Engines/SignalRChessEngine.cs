using ChessLib.Data;
using ChessLib.Exceptions;
using ChessLib.Figures;
using ChessLib.Http.Responses;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Engines
{
    public class SignalRChessEngine : LocalChessEngine, IChessEngine, IDisposable
    {
        private RestClient _restClient = new RestClient();
        public string Name { get; set; }

        private string _url;
        public string Url 
        {
            get => _url;
            set
            {
                _url = value.EndsWith("/") ? value.Substring(0, value.Length - 1) : value;
                _hubConnection?.Dispose();
                _hubConnection = new HubConnection(_url);
                _chessHub = _hubConnection.CreateHubProxy("/ChessHub");
                _hubConnection.Start();
                _chessHub.On<int, TurnChangedEventArgs>("Move", ChessHub_OnMove);
            }
        }
        public int? MatchId { get; private set; }
        public Color? MyColor { get; private set; }
        public bool MyTurn
            => MatchId.HasValue ? MyColor.Value == Turn : throw new YouAreNotInMatchException();

        #region Overrides
        public override ChessFigure GetFigure(int x, int y)
            => MatchId.HasValue ? base.GetFigure(x, y) : throw new YouAreNotInMatchException();
        public override IEnumerable<ChessPosition> GetMoves(int x, int y)
            => MatchId.HasValue ? base.GetMoves(x, y) : throw new YouAreNotInMatchException();
        public override IEnumerable<ChessFigure> Figures
            => MatchId.HasValue ? base.Figures : throw new YouAreNotInMatchException();
        public override bool Move(ChessPosition from, ChessPosition to, Func<Color, EnumFigure> func)
            => MatchId.HasValue ? base.Move(from, to, func) : throw new YouAreNotInMatchException();
        public override event Action<object, TurnChangedEventArgs> OnTurnChanged;
        #endregion

        //TODO
        #region EventHandlers
        private void Base_OnTurnChanged(object arg1, TurnChangedEventArgs arg2)
        {
            OnTurnChanged.Invoke(this, arg2);
        }
        private void ChessHub_OnMove(int matchId, TurnChangedEventArgs args)
        {
            if (MatchId.HasValue && matchId == MatchId.Value)
            {
                lock (_lock)
                {
                    Moves.Add(args.FenNow);
                    Board = new Board(args.FenNow);
                }
                OnTurnChanged.Invoke(this, args);
            }
        }
        #endregion

        #region SignalR
        private HubConnection _hubConnection;
        private IHubProxy _chessHub;
        #endregion

        #region Http
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
                MyColor = MyColor = Name == response.WhiteName ? Color.White : Name == response.BlackName ? Color.Black : Color.None;
                lock (_lock)
                {
                    Moves = new List<string>(response.Fens);
                    Board = new Board(Moves.Last());
                }
            }
            return response.Success;
        }
        #endregion

        #region Ctor
        public SignalRChessEngine() : base()
        {
            base.OnTurnChanged += Base_OnTurnChanged;
        }
        #endregion

        public void Dispose()
        {
            _hubConnection?.Dispose();
            base.OnTurnChanged -= Base_OnTurnChanged;
        }
    }
}
