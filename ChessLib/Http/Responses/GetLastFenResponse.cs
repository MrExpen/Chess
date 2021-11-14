namespace ChessLib.Http.Responses
{
    public class GetLastFenResponse : BaseResponse
    {
        public string Fen { get; set; }
        public string WhiteName { get; set; }
        public string BlackName { get; set; }
    }
}
