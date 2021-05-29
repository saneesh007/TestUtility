using RI.AppFramework.EntityModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Utility
{
    public interface IUtilityService
    {
        Task<TestUtilityHeader> RegisterTransaction(TestUtilityHeader header);
        Task<bool> WriteTransaction(List<TestUtilityLoadTestDetail> details);
    }
}
