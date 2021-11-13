using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChessHttpServer.Data
{
    public class ChessMatch
    {
        [Key]
        public int Id { get; set; }
        public virtual User White { get; set; }
        public virtual User Black { get; set; }
        public virtual List<FenStringData> Fens { get; set; }
    }
}