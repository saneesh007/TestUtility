using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
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
                list = await _db.Set<Agent>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<Agent>> GetAllMerchant(int partnerId, List<int> agentId)
        {
            List<Agent> list = new List<Agent>();
            try
            {
                list = await _db.Agents.AsNoTracking().Where(x => x.SolutionPartnerId == partnerId && agentId.Contains(x.Id)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
