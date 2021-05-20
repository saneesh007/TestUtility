using RI.AppFramework.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RI.Services.Partner
{
    public interface IProductService
    {
        Task<List<ProductAgentAssignment>> GetAgentProductAssignment(int solutionPartnerId, List<int> agentId);
        Task<List<Product>> GetAllProducts(int partnerId);
    }
}
