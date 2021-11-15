using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Data
{
    public interface IOnlineChess
    {
        Color? MyColor { get; }
        public bool MyTurn { get; }
    }
}
