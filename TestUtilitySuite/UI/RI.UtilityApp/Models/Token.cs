using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class Token : ResponseVM
    {
        public int PosId { get; set; }
        public int PosAssignmentId { get; set; }
        public int CashierId { get; set; }
        public string token { get; set; }
        public int user_type { get; set; }
        public string pos_version { get; set; }

    }
}
