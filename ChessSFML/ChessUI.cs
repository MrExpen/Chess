using ChessLib;
using System.Collections.Generic;

namespace ChessSFML
{
    internal sealed class ChessUI : Chess
    {
        public SkinProvider SkinProvider { get; set; }
        private ChessPosition? _selected = null;
        public ChessPosition? Selected 
        { 
            get => _selected; 
            set { 
                _selected = value;
                if (Selected.HasValue)
                {
                    if (Board.Figures[Selected.Value.X, Selected.Value.Y] is not null && Board.Figures[Selected.Value.X, Selected.Value.Y].Color == Turn)
                    {
                        Moves = Board.Figures[Selected.Value.X, Selected.Value.Y].GetMovePositionsWithCheckCheck(Board);
                    }
                    else
                    {
                        Selected = null;
                    }
                }
                else
                {
                    Moves = new List<ChessPosition>();
                }
            } 
        }
        public List<ChessPosition> Moves { get; private set; } = new List<ChessPosition>();
        public ChessUI(SkinProvider skinProvider, string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") : base(fen)
        {
            SkinProvider = skinProvider;
        }
    }
}
