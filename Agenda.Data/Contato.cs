using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda.Data
{
    [Table("Contato")]
    public class Contato
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(15)]
        public string Fone { get; set; }

        [StringLength(255)]
        public string UrlFoto { get; set; }

        [StringLength(255)]
        public string UrlThumb { get; set; }
    }
}
