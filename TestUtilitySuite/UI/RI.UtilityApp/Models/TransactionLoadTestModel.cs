using Microsoft.AspNetCore.Mvc.Rendering;
using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
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
        [Display(Name = "Number Of Terminals")]
        public int NumberOfTerminals { get; set; }
        [Display(Name = "Number Of Transaction Per Terminal")]
        public int NumberOfTransactionPerTerminal { get; set; }
        public PaginatedList<TestUtilityHeader> TestUtilityHeader { get; set; }
    }
}