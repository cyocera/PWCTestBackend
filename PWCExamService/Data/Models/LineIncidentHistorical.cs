using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWCExamService.Data
{
    public class LineIncidentHistorical
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string lineName { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string status { get; set; }
        public DateTime date { get; set; }
    }
}
