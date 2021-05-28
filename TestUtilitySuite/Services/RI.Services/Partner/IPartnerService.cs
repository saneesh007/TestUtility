using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Partner
{
    public interface IPartnerService
    {
        Task<List<Agent>> GetAllActivePartners();
        Task<List<Agent>> GetAllMerchant(int partnerId, List<int> agentId);
    }
}
