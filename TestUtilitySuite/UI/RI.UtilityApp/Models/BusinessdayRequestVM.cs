using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class BusinessdayRequestVM
    {
        public int pos_id { get; set; }
        public int merchant_id { get; set; }
        public int user_id { get; set; }
        public string token { get; set; }
    }
}
