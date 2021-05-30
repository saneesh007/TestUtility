using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using RI.AppFramework;
using RI.AppFramework.EntityModel;
using Microsoft.EntityFrameworkCore;
using RI.AppFramework.Models;

namespace RI.Services.Utility
{
    public class UtilityService : IUtilityService
    {
        RechargeDbContext _db = null;
        public UtilityService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }

        public async Task<PaginatedList<TestUtilityHeader>> GetTransaction(int pageIndex, int pageSize)
        {
            try
            {
                return await PaginatedList<TestUtilityHeader>.CreateAsync(_db.TestUtilityHeader.AsNoTracking(), pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TestUtilityHeader> RegisterTransaction(TestUtilityHeader header)
        {
            try
            {
                var lastBatch = _db.TestUtilityHeader.AsNoTracking().Where(x=>x.PartnerId==header.PartnerId).OrderByDescending(x => x.Id).FirstOrDefault();

                if (lastBatch == null)
                {
                    header.Batch = "89" + DateTime.UtcNow.ToString("yy") + header.PartnerId.ToString().PadLeft(3, '0') + "00001";

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(lastBatch.Batch))
                    {
                        string number = lastBatch.Batch.ToString();
                        string lastFive = number.Substring(number.Length - 5);
                        int newNumber = Convert.ToInt32(lastFive) + 1;
                        header.Batch = "89" + DateTime.UtcNow.ToString("yy") + header.PartnerId.ToString().PadLeft(3, '0') + newNumber.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        header.Batch = "89" + DateTime.UtcNow.ToString("yy") + header.PartnerId.ToString().PadLeft(3, '0') + "00001";
                    }
                }
                if (true)
                {
                    _db.TestUtilityHeader.Add(header);
                    int count = await _db.SaveChangesAsync(); 
                }
                return header;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> WriteTransaction(List<TestUtilityLoadTestDetail> details)
        {
            try
            {
                _db.TestUtilityLoadTestDetail.AddRange(details);
                int count = await _db.SaveChangesAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
