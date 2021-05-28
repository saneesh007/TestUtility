using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;
using System.Data.SqlClient;

namespace RI.Services.Partner
{
    public class ProductService : IProductService
    {
        RechargeDbContext _db = null;
        public ProductService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }

        public async Task<List<ProductAgentAssignment>> GetAgentProductAssignment(int solutionPartnerId, List<int> agentId)
        {
            List<ProductAgentAssignment> list = new List<ProductAgentAssignment>();
            try
            {

                list = await (from p in _db.Products.AsNoTracking()
                              join pa in _db.ProductAgentAssignment.AsNoTracking() on p.Id equals pa.ProductId
                              where p.ActiveStatus == 1 && p.SolutionPartnerId == solutionPartnerId
                              select pa).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
        public async Task<List<Product>> GetAllProducts(int partnerId)
        {
            List<Product> list = new List<Product>();
            try
            {
                string sql = @"select * from (select distinct p.*   from Agents ag
join ProductAgentAssignment paa on ag.Id =paa.AgentId
join Products p on paa.ProductId=p.Id
join PosAssignments poa on ag.Id = poa.MerchantId
join PosUnits po on poa.POSId=po.Id
join PosUsers pu on ag.Id = pu.MerchantId
cross apply (select top 1 * from PinStock t where t.AgentId=ag.ParentId and  t.ProductId=p.Id and t.Status =2) x
where ag.SolutionPartnerId= @partnerId
and poa.Status=1 
and p.ActiveStatus=1 
and po.ActiveStatus=1 
and ag.ActiveStatus=1 
and pu.ActiveStatus=1)x
ORDER BY x.Name";
                var parameters = new[]
                {
                    new SqlParameter { ParameterName = "@partnerId", Value = partnerId }
                };
                list = await _db.Set<Product>().FromSql(sql, parameters.ToArray<object>()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
