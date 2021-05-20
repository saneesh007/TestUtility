using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Partner
{
    public interface IPosService
    {
        Task<List<PosUser>> GetAllPosUsers(int solutionPartnerId, List<int> agentId);
        Task<List<PosUnits>> GetAllPosUnit(int solutionPartnerId, List<int> posassignmentId);
        Task<List<PosAssignment>> GetAllPosAssignment(int solutionPartnerId, int terminalCount);
    }
}
