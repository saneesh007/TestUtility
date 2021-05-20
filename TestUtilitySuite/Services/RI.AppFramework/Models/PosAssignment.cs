using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class PosAssignment
    {
        public int Id { get; set; }
        public int POSId { get; set; }
        public int MerchantId { get; set; }
        public int Status { get; set; }
    }
}
