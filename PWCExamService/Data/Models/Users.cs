using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWCExamService.Data
{
    public class Users
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Username { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        public string Password { get; set; }
    }
}
