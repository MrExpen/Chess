using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChessHttpServer.Data
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<ChessMatch> Matches { get; set; }
    }
}
