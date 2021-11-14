using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessSFML
{
    internal enum GameState : byte
    {
        None,
        InPause,
        InGame,
        CreateOnline,
        JoinOnline,
        NewOnline,
        ShowResults
    }
}
