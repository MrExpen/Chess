using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib
{
    public interface IChessWithSelect : IChessEngine
    {
        public ChessPosition? Selected { get; set; }
        public IEnumerable<ChessPosition> MovesForSelected { get; }
    }
}
