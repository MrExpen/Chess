using System.Collections.Generic;

namespace ChessHttpServer.Respones
{
    public class GetAllFensRespons : BaseResponse
    {
        public IEnumerable<string> Fens { get; set; }
    }
}
