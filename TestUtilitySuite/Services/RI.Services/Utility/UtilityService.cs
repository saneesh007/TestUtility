using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using RI.AppFramework;
using RI.AppFramework.EntityModel;
using Microsoft.EntityFrameworkCore;
using RI.AppFramework.Models;
using Newtonsoft.Json;

namespace RI.Services.Utility
{
    public class UtilityService : IUtilityService
    {
        RechargeDbContext _db = null;
        public UtilityService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }

        public async Task<PaginatedList<TestUtilityHeaderVM>> GetTransaction(int pageIndex, int pageSize, string searchText = "")
        {
            try
            {
                var data = await PaginatedList<TestUtilityHeader>.CreateAsync(_db.TestUtilityHeader.AsNoTracking(), pageIndex, pageSize);
                var partnerIds = data.Select(x => x.PartnerId).ToList();
                if (partnerIds.Count() > 0)
                {
                    var partner = _db.Agents.AsNoTracking().Where(x => partnerIds.Contains(x.Id)).ToList();
                    var result = data.Select(x => new TestUtilityHeaderVM()
                    {
                        Id = x.Id,
                        Batch = x.Batch,
                        Date = x.Date,
                        EndTime = x.EndTime,
                        FailureCount = x.FailureCount,
                        NumberOfTerminals = x.NumberOfTerminals,
                        NumberOfTransactionPerTerminal = x.NumberOfTransactionPerTerminal,
                        Partner = partner.FirstOrDefault(t => t.Id == x.PartnerId).Name,
                        PartnerId = x.PartnerId,
                        ProcessTypeId = x.ProcessTypeId,
                        StartTime = x.StartTime,
                        SuccessCount = x.SuccessCount
                    }).ToList();

                    return new PaginatedList<TestUtilityHeaderVM>(result, data.TotalCount, pageIndex, pageSize);
                }
                return new PaginatedList<TestUtilityHeaderVM>(new List<TestUtilityHeaderVM>(), 0, 0, pageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TestUtilityHeaderVM> GetTransaction(int id)
        {
            try
            {
                var item = await _db.TestUtilityHeader.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
                var serializedParent = JsonConvert.SerializeObject(item);
                TestUtilityHeaderVM result = JsonConvert.DeserializeObject<TestUtilityHeaderVM>(serializedParent);
                result.Partner = _db.Agents.AsNoTracking().FirstOrDefault(x => x.Id == result.PartnerId).Name;
                result.LoadTestDetail = new List<TestUtilityLoadTestDetailVM>();
                var details = _db.TestUtilityLoadTestDetail.AsNoTracking().Where(x => x.HdrId == id).ToList();
                if (details.Count() > 0)
                {
                    var posId = details.Select(x => x.PosId).Distinct().ToList();
                    List<PosUnits> posUnits = new List<PosUnits>();
                    var cashierId = details.Select(x => x.PosUserId).Distinct().ToList();
                    List<PosUser> posUsers = new List<PosUser>();
                    var productId = details.Select(x => x.ProductId).Distinct().ToList();
                    List<Product> products = new List<Product>();
                    var merchantId = details.Select(x => x.MerchantId).Distinct().ToList();
                    List<Agent> agents = new List<Agent>();
                    if (posId.Count > 0)
                    {
                        posUnits = _db.PosUnits.Where(x => posId.Contains(x.Id)).ToList();
                    }
                    if (cashierId.Count > 0)
                    {
                        posUsers = _db.PosUsers.Where(x => cashierId.Contains(x.Id)).ToList();
                    }
                    if (productId.Count > 0)
                    {
                        products = _db.Products.Where(x => productId.Contains(x.Id)).ToList();
                    }
                    if (merchantId.Count > 0)
                    {
                        agents = _db.Agents.Where(x => merchantId.Contains(x.Id)).ToList();
                    }
                    result.LoadTestDetail = details.Select(x => new TestUtilityLoadTestDetailVM()
                    {
                        ConfirmationReponseTime = x.ConfirmationReponseTime,
                        DownloadedPinId = x.DownloadedPinId,
                        DownloadResponseTime = x.DownloadResponseTime,
                        FaceValue = products.FirstOrDefault(t => t.Id == x.ProductId).FaceValue,
                        HdrId = x.HdrId,
                        Id = x.Id,
                        IsConfirmed = x.IsConfirmed,
                        IsDownloadCompleted = x.IsDownloadCompleted,
                        Merchant = agents.FirstOrDefault(t => t.Id == x.MerchantId).Name,
                        MerchantId = x.MerchantId,
                        PinDownloadId = x.PinDownloadId,
                        PosAssignmentId = x.PosAssignmentId,
                        PosCashier = posUsers.FirstOrDefault(t => t.Id == x.PosUserId).Name,
                        PosId = x.PosId,
                        PosUnit = posUnits.FirstOrDefault(t => t.Id == x.PosId).POSNo,
                        PosUserId = x.PosUserId,
                        Product = products.FirstOrDefault(t => t.Id == x.ProductId).Name,
                        ProductId = x.ProductId,
                        ResponseCode = x.ResponseCode,
                        TxnNo = x.TxnNo
                    }).ToList();
                }
                return result;
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
                var lastBatch = _db.TestUtilityHeader.AsNoTracking().Where(x => x.PartnerId == header.PartnerId).OrderByDescending(x => x.Id).FirstOrDefault();

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
                _db.TestUtilityHeader.Add(header);
                int count = await _db.SaveChangesAsync();
                return header;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> WriteTransaction(List<TestUtilityLoadTestDetail> details)
        {
            try
            {
                _db.TestUtilityLoadTestDetail.AddRange(details);
                int count = await _db.SaveChangesAsync();
                if (count > 0)
                {
                    TestUtilityHeader header = _db.TestUtilityHeader.FirstOrDefault(x => x.Id == details.FirstOrDefault().HdrId);
                    header.SuccessCount = details.Count(x => x.IsConfirmed);
                    header.FailureCount = details.Count(x => !x.IsConfirmed);
                    _db.Update<TestUtilityHeader>(header);
                    _db.SaveChanges();
                }
                return count > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
