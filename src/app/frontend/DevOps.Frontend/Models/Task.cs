using System;
using System.ComponentModel.DataAnnotations;

namespace DevOps.Frontend.Models
{
    public sealed class Task
    {
        [MaxLength(32)]
        [MinLength(3)]
        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [MaxLength(256)]
        [MinLength(3)]
        [Required]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime Due { get; set; }

        [Key]
        public int Id { get; set; }
    }
}