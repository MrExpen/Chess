using System.Collections.Generic;

namespace ChessLib.Http.Responses
{
    public class GetAllFensRespons : BaseResponse
    {
        public IEnumerable<string> Fens { get; set; }
    }
}
