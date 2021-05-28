using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class BusinessDayResponseVM
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public DateTime business_date { get; set; }
        public int Status { get; set; }
        public DateTime Previousbusinessday { get; set; }
        public int ShiftNo { get; set; }
        public string token { get; set; }
        public int ShiftStatus { get; set; }
    }
}
