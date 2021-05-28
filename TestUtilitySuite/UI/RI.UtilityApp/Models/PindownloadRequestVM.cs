using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class PindownloadRequestVM
    {
        public int pos_id { get; set; }
        public int pos_assignment_id { get; set; }
        public int merchant_id { get; set; }
        public int request_id { get; set; }
        public int service_provider_id { get; set; }
        public int product_group_id { get; set; }
        public int product_id { get; set; }
        public int qty { get; set; }
        public int pos_user_id { get; set; }
        public int shift_no { get; set; }
        public DateTime business_date { get; set; }
        public int mode { get; set; }
        public string token { get; set; }
        public string TimezoneTag { get; set; }
        public string ClientTxnNo { get; set; }
        public string ClientLocalTime { get; set; }
    }
}