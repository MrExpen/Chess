namespace ChessHttpServer.Respones
{
    public class CreateMatchResult : BaseResponse
    {
        public string WhiteName { get; set; }
        public string BlackName { get; set; }
        public string Fen { get; set; }
        public int MatchId { get; set; }
    }
}
