using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class Agent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? SolutionPartnerId { get; set; }
        public int? ActiveStatus { get; set; }
    }
}
