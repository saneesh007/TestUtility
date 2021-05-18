using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class TransactionLoadTestModel
    {
        public List<SelectListItem> Partners { get; set; }
        [Display(Name = "Partner")]
        public int PartnerId { get; set; }
        public int NumberOfTerminals { get; set; }
        public int NumberOfTransactionPerTerminal { get; set; }
    }
}