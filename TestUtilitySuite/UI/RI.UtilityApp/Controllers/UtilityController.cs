using System; 
using Microsoft.AspNetCore.Mvc;
using RI.AppFramework.EntityModel;
using RI.Services.Utility;

namespace RI.UtilityApp.Controllers
{
    [Route("api/[controller]")]
    public class UtilityController : Controller
    {
        IUtilityService _utilityService;
        public UtilityController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }
        [HttpPost]
        [Route("api/WriteTransactionLoad")]
        public void WriteTransactionLoad([FromBody]TestUtilityHeader testUtilityHeader)
        {
            try
            {
                var result = _utilityService.WriteTransaction(testUtilityHeader, testUtilityHeader.TestUtilityLoadTestDetail).Result;
                //  return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //  return false;
        }
        [HttpPost]
        [Route("api/load")]
        public string load([FromBody]string testUtilityHeader)
        {
            return testUtilityHeader;
            //  return false;
        }
        [HttpPost]
        public string Post([FromBody]string value)
        {
            return value;
        }
        public string Get()
        {
            return "Hai";
        }
    }

}