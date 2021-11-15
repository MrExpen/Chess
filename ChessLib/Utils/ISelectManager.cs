using ChessLib.Data;
using System.Collections.Generic;

namespace ChessLib.Utils
{
    public interface ISelectManager
    {
        ChessPosition? Selected { get; set; }
        IEnumerable<ChessPosition> MovesForSelected { get; }
    }
}
