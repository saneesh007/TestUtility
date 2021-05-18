using Microsoft.EntityFrameworkCore;
using RI.AppFramework;
using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
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

        public async Task<List<PosAssignment>> GetAllPosAssignment(int solutionPartnerId, List<int> agentId)
        {
            List<PosAssignment> list = new List<PosAssignment>();
            try
            {
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<PosAssignment>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }

        public async Task<List<PosUnits>> GetAllPosUnit(int solutionPartnerId, List<int> agentId)
        {
            List<PosUnits> list = new List<PosUnits>();
            try
            {
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<PosUnits>().FromSql(sql).ToListAsync();
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
                string sql = "select * from agents where ISNULL(parentid,0)=0";
                //List<SqlParameter> parms = new List<SqlParameter>
                //{
                //    new SqlParameter { ParameterName = "@ProductID", Value = 706 }
                //};
                list = await _db.Set<PosUser>().FromSql(sql).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return list;
        }
    }
}
