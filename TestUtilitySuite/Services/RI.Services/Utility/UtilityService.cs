using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RI.AppFramework;
using RI.AppFramework.EntityModel;

namespace RI.Services.Utility
{
    public class UtilityService : IUtilityService
    {
        RechargeDbContext _db = null;
        public UtilityService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }
        public async Task<bool> WriteTransaction(TestUtilityHeader header, List<TestUtilityLoadTestDetail> details)
        {
            try
            {
                _db.TestUtilityHeader.Add(header);
                await _db.SaveChangesAsync();
                details.ForEach(x => x.HdrId = header.Id);
                _db.TestUtilityLoadTestDetail.AddRange(details);
               int count= await _db.SaveChangesAsync();
                return count > 0;
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}
