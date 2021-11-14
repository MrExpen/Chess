using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Http
{
    public class HttpChessWithSelect : HttpChessEngine, IChessWithSelect
    {
        private ChessPosition? _selected = null;
        public ChessPosition? Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                if (Selected.HasValue)
                {
                    if (Board.Figures[Selected.Value.X, Selected.Value.Y] is not null && Board.Figures[Selected.Value.X, Selected.Value.Y].Color == Turn && MyTurn)
                    {
                        MovesForSelected = GetMoves(Selected.Value);
                    }
                    else
                    {
                        Selected = null;
                    }
                }
                else
                {
                    MovesForSelected = new List<ChessPosition>();
                }
            }
        }
        public IEnumerable<ChessPosition> MovesForSelected { get; private set; } = new List<ChessPosition>();
        public HttpChessWithSelect(string name, string url) : base(name, url)
        {
        }
    }
}
