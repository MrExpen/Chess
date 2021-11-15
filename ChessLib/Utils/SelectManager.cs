using ChessLib.Data;
using ChessLib.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Utils
{
    public class SelectManager : ISelectManager
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
                    var figure = _chessEngine.GetFigure(Selected.Value);
                    if (figure is not null && figure.Color == _chessEngine.Turn)
                    {
                        if (_chessEngine is IOnlineChess onlineChess)
                        {
                            if (onlineChess.MyTurn)
                            {
                                MovesForSelected = _chessEngine.GetMoves(figure);
                            }
                            else
                            {
                                MovesForSelected = Array.Empty<ChessPosition>();
                            }
                        }
                        else
                        {
                            MovesForSelected = _chessEngine.GetMoves(figure);
                        }
                        
                    }
                }
                else
                {
                    MovesForSelected = Array.Empty<ChessPosition>();
                }
            }
                
        }
        public IEnumerable<ChessPosition> MovesForSelected { get; private set; } = Array.Empty<ChessPosition>();

        private IChessEngine _chessEngine;

        #region Ctor
        public SelectManager(IChessEngine chessEngine)
        {
            _chessEngine = chessEngine;
        }
        #endregion

    }
}
