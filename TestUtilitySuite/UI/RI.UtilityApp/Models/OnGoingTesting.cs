using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RI.UtilityApp.Models
{
    public class OnGoingTesting
    {
        public bool IsTransactionLoadTestProcessing { get; set; }
        public long TransactionLoadTestBatchNo { get; set; }
    }
}
