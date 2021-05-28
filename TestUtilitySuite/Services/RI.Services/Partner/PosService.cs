using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Partner
{
    public class PosService : IPosService
    {
        RechargeDbContext _db = null;
        public PosService(RechargeDbContext rechargeDbContext)
        {
            _db = rechargeDbContext;
        }

        public async Task<List<PosAssignment>> GetAllPosAssignment(int solutionPartnerId, int terminalCount)
        {
            List<PosAssignment> list = new List<PosAssignment>();
            try
            {
                string sql = @"select * from (select distinct poa.*   from Agents ag
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
ORDER BY NEWID()  OFFSET 0 ROWS
FETCH NEXT @count ROWS ONLY";
                var parameters = new[]
                {
                    new SqlParameter { ParameterName = "@partnerId", Value = solutionPartnerId },
                    new SqlParameter { ParameterName = "@count", Value = terminalCount }
                };
                list = await _db.Set<PosAssignment>().FromSql(sql, parameters.ToArray<object>()).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<PosUnits>> GetAllPosUnit(int solutionPartnerId, List<int> posassignmentId)
        {
            List<PosUnits> list = new List<PosUnits>();
            try
            {
                var query = _db.PosUnits.Where(x => posassignmentId.Contains(x.Id) && x.ActiveStatus == 1);
                list = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<PosUser>> GetAllPosUsers(int solutionPartnerId, List<int> agentId)
        {
            List<PosUser> list = new List<PosUser>();
            try
            {
                var query = _db.PosUsers.Where(x => agentId.Contains(x.MerchantId) && x.ActiveStatus == 1 && x.Type == 1);
                list = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
