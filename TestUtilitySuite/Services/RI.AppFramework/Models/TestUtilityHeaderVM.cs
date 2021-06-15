using RI.AppFramework.EntityModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RI.AppFramework.Models
{
    public class TestUtilityHeaderVM : TestUtilityHeader
    {
        public string Partner { get; set; }
        public List<TestUtilityLoadTestDetailVM> LoadTestDetail { get; set; }
    }
}
