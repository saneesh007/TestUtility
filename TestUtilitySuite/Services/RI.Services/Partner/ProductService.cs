using System;
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

        public async Task<List<ProductAssignment>> GetAgentProductAssignment(int solutionPartnerId, List<int> agentId)
        {
            List<ProductAssignment> list = new List<ProductAssignment>();
            try
            {
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<ProductAssignment>().FromSql(sql).ToListAsync();
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
                string sql = "select * from Product where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<Product>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
