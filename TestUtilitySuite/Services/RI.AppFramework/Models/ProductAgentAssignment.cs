using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class ProductAgentAssignment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AgentId { get; set; }
    }
}
