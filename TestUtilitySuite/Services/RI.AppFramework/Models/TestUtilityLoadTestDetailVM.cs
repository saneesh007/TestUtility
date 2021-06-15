using RI.AppFramework.EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class TestUtilityLoadTestDetailVM : TestUtilityLoadTestDetail
    {
        public string Merchant { get; set; }
        public string PosCashier { get; set; }
        public string PosUnit { get; set; }
        public string Product { get; set; }
        public decimal FaceValue { get; set; }
    }
}
