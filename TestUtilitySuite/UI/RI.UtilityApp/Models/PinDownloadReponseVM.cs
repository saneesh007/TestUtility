using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class PinDownloadReponseVM
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string Response_time { get; set; }
        public int provider_id { get; set; }
        public int group_id { get; set; }
        public int? download_Id { get; set; }
        public string token { get; set; }
        public int total_records { get; set; }
        public List<Pin> Pin { get; set; }
        public PinDownloadReponseVM()
        {
            Pin = new List<Pin>();
        }
    }
    public class Pin
    {
        public int product_id { get; set; }
        public int download_pin_Id { get; set; }
        public string serial_no { get; set; }
        public string encrypted_pin_no { get; set; }
        public Nullable<decimal> face_val { get; set; }
        public Nullable<decimal> tax1_perc { get; set; }
        public Nullable<decimal> tax1_amt { get; set; }
        public Nullable<decimal> tax2_perc { get; set; }
        public Nullable<decimal> tax2_amt { get; set; }
        public Nullable<decimal> comm_perc { get; set; }
        public Nullable<decimal> comm_amt { get; set; }
        public Nullable<decimal> service_charge_perc { get; set; }
        public Nullable<decimal> service_charge_amt { get; set; }
        public DateTime pin_expiry { get; set; }
        public DateTime server_time { get; set; }
    }
}
