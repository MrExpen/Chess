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
        Spectate,
        InPause,
        InGame,
        CreateOnline,
        JoinOnline,
        NewOnline,
        ShowResults
    }
}
