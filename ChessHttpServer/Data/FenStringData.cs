using System.ComponentModel.DataAnnotations;

namespace ChessHttpServer.Data
{
    public class FenStringData
    {
        [Key]
        public int Id { get; set; }
        public string Data { get; set; }
    }
}
