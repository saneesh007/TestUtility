using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RI.AppFramework.EntityModel
{
    public class TestUtilityHeader
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required] 
        public int ProcessTypeId { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Batch Required", MinimumLength = 10)]
        public string Batch { get; set; }
        [Required] 
        public int PartnerId { get; set; } 
        [Display(Name = "Number Of Terminals")]
        public int NumberOfTerminals { get; set; }
        [Display(Name = "Number Of Transaction Per Terminal")]
        public int NumberOfTransactionPerTerminal { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [Display(Name = "End Time")]
        public DateTime? EndTime  { get; set; }
        [Display(Name = "Success Count")]
        public int SuccessCount { get; set; }
        [Display(Name = "Failure Count")]
        public int FailureCount { get; set; }
        [NotMapped]
        public List<TestUtilityLoadTestDetail> TestUtilityLoadTestDetail { get; set; }
    }
}
