using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class TransactionModel
    {
        public List<PosAssignment> PosAssignments { get; set; }
        public List<PosUnits> PosUnits { get; set; }
        public List<PosUser> PosUsers { get; set; }
        public List<Agent> Merchants { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductAgentAssignment> ProductAgentAssignments { get; set; }
        public TransactionLoadTestModel Request { get; set; }
        public TestUtilityHeader TestUtilityHeader { get; set; }
        public string URL { get; set; }
    }
}
