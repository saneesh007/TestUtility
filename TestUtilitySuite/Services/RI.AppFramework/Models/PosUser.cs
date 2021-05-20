using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class PosUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MerchantId { get; set; }
        public int Type { get; set; }
        public int ActiveStatus { get; set; }
    }
}
