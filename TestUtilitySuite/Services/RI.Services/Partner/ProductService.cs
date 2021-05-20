using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;

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
                list = await (from p in _db.Products.AsNoTracking()
                              where p.ActiveStatus == 1 && p.SolutionPartnerId == partnerId
                              select p).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
