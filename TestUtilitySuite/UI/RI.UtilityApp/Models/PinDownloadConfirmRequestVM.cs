using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class PinDownloadConfirmRequestVM
    {
        public int pos_id { get; set; }
        public int pos_assignment_id { get; set; }
        public int merchant_id { get; set; }
        public int download_id { get; set; }
        public string token { get; set; }
        public List<PinDetails> txn { get; set; }
        public int pos_user_id { get; set; }
        public PinDownloadConfirmRequestVM()
        {
            this.txn = new List<PinDetails>();
        }
    }
    public class PinDetails
    {
        public Nullable<long> sale_txn_no { get; set; }
        public Nullable<int> download_pin_id { get; set; }
        public Nullable<int> product_id { get; set; }
        public string serial_no { get; set; }
        public Nullable<DateTime> business_date { get; set; }
        public Nullable<int> shift_no { get; set; }
        public Nullable<int> user_id { get; set; }
        public string client_txn_no { get; set; }
        public string client_local_date { get; set; }
    }
}
