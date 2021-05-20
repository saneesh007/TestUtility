using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class LoginModel
    {
        public int merchant_id { get; set; }
        public string password { get; set; }
        public int pos_assignment_id { get; set; }
        public int pos_id { get; set; }
        public string pos_version { get; set; }
        public string user_type { get; set; } = "pos-users";
        public string username { get; set; }
    }
}
