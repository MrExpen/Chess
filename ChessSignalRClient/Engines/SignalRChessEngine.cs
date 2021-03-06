using ChessLib.Data;
using ChessLib.Exceptions;
using ChessLib.Figures;
using ChessLib.Http.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Engines
{
    public class SignalRChessEngine : LocalChessEngine, IChessEngine, IDisposable, IOnlineChess
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
                _hubConnection?.DisposeAsync().GetAwaiter().GetResult();
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{Url}/ChessHub")
                    .Build();
                _hubConnection.StartAsync().Wait();
                _hubConnection.On<int, string, string, string>("Move", ChessHub_OnMove);
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
        {
            if (!MatchId.HasValue)
            {
                throw new YouAreNotInMatchException();
            }
            if (!MyTurn)
            {
                return false;
            }
            return base.Move(from, to, func);
        }
        public override event Action<object, TurnChangedEventArgs> OnTurnChanged;
        #endregion

        #region EventHandlers
        private void Base_OnTurnChanged(object arg1, TurnChangedEventArgs arg2)
        {
            _hubConnection.InvokeAsync("Move", MatchId.Value, Name, arg2.From.ToString(), arg2.To.ToString(), ((char)arg2.BecomeTo).ToString()).Wait();
            OnTurnChanged?.Invoke(this, arg2);
        }
        private void ChessHub_OnMove(int matchId, string from, string to, string figure)
        {
            if (MatchId.HasValue && matchId == MatchId.Value)
            {
                (bool Success, bool IsEat, EnumFigure BocomeTo) result;
                lock (_lock)
                {
                    result = Board.Move(new ChessPosition(from), new ChessPosition(to), (col) => figure.Length > 0 ? (EnumFigure)figure[0] : EnumFigure.None);
                    if (result.Success)
                    {
                        Moves.Add(Board.Fen);
                    }
                }
                OnTurnChanged?.Invoke(this, new TurnChangedEventArgs { IsEat = result.IsEat, From = new ChessPosition(from), To = new ChessPosition(to), TurnNow = Turn, IsChecked = Board.IsChecked(Turn), BecomeTo = result.BocomeTo, FenNow = Fen });
            }
        }
        #endregion

        #region SignalR
        private HubConnection _hubConnection;
        #endregion

        #region Http
        public int? CreateMatch(string White, string Black)
            => Utils.HttpApi.CreateMatch(Url, White, Black);
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
        public SignalRChessEngine(string name, string url) : this()
        {
            Name = name;
            Url = url;
        }
        #endregion

        public void Dispose()
        {
            _hubConnection?.DisposeAsync().GetAwaiter().GetResult();
            base.OnTurnChanged -= Base_OnTurnChanged;
        }
    }
}
