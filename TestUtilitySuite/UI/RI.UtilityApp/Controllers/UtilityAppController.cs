using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
using RI.Services.Utility;

namespace RI.UtilityApp.Controllers
{
    [Produces("application/json")]
    public class UtilityAppController : Controller
    {
        IUtilityService _utilityService;
        public UtilityAppController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }
        [HttpPost]
        [Route("api/UtilityApp/WriteTransactionLoad")]
        public async Task WriteTransactionLoad([FromBody]List<TestUtilityLoadTestDetail> details)
        {
            try
            {
                var result = await _utilityService.WriteTransaction(details);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //  return false;
        }
        [HttpPost]
        [Route("api/UtilityApp/RegisterTransaction")]
        public async Task<TestUtilityHeader> RegisterTransaction([FromBody]TestUtilityHeader testUtilityHeader)
        {
            try
            {
                var result = await _utilityService.RegisterTransaction(testUtilityHeader);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //  return false;
        }
        [HttpGet]
        [Route("api/UtilityApp/GetTransaction/{pageIndex}/{pageSize}")]
        public async Task<PaginatedList<TestUtilityHeader>> GetTransaction(int pageIndex, int pageSize)
        {
            try
            {
                var result = await _utilityService.GetTransaction(pageIndex, pageSize);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //  return false;
        }
    }
}
