using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLib.Data
{
    public class TurnChangedEventArgs : EventArgs
    {
        public bool IsEat { get; set; }
        public bool IsChecked { get; set; }
        public Color TurnNow { get; set; }
        public ChessPosition From { get; set; }
        public ChessPosition To { get; set; }
    }
}
