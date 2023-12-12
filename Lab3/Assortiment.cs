using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3
{
    public class Assortiment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Required]
        public string? Kod { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? Price { get; set; }
        List<Registration> Registrations { get; set; } = new();

    }
}
