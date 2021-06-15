using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ServiceProviderId { get; set; }
        public int ProductGroupId { get; set; }
        public int SolutionPartnerId { get; set; }
        public decimal FaceValue { get; set; }
        public int ActiveStatus { get; set; }
    }
}
