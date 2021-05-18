using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Partner
{
    public class PartnerService : IPartnerService
    {
        RechargeDbContext _db = null;
        public PartnerService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }
        public async Task<List<Agent>> GetAllActivePartners()
        {
            List<Agent> list = new List<Agent>();
            try
            {
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<Agent>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<Agent>> GetAllMerchant(int partnerId)
        {
            List<Agent> list = new List<Agent>();
            try
            {
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<Agent>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
