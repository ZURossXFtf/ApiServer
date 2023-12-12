using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab3
{
    public class Registration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public int? Weight { get; set; }
        public int? Cost { get; set; }
        [Required]
        public DateTime? DateConfirm {  get; set; }
        public int AssortimentId {  get; set; }
        public Registration? Registrations { get; set; }
    }
}
