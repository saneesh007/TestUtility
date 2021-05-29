using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RI.AppFramework.EntityModel;
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
        public void WriteTransactionLoad([FromBody]List<TestUtilityLoadTestDetail> details)
        {
            try
            {
                var result = _utilityService.WriteTransaction(details).Result;
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
    }
}
