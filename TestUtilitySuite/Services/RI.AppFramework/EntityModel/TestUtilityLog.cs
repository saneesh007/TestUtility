using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RI.AppFramework.EntityModel
{
    public class TestUtilityLog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Process Name Required")]
        public string ProcessName { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Batch Required", MinimumLength = 10)]
        public string Batch { get; set; }
        [Required]
        [StringLength(450, ErrorMessage = "Partner Id Required")]
        public int PartnerId { get; set; }
    }
}
