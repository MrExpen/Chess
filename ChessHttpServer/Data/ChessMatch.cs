using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChessHttpServer.Data
{
    public class ChessMatch
    {
        [Key]
        public int Id { get; set; }
        public string WhiteName { get; set; }
        public string BlackName { get; set; }
        public virtual List<FenStringData> Fens { get; set; } 

        public ChessMatch()
        {
            Fens = new List<FenStringData>();
        }
    }
}