namespace ChessLib.Http.Responses
{
    public abstract class BaseResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
