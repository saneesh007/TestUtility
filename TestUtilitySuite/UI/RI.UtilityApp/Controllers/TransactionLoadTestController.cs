using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
using RI.Services.Partner;
using RI.Services.Utility;
using RI.UtilityApp.Models;

namespace RI.UtilityApp.Controllers
{
    public class TransactionLoadTestController : Controller
    {
        IPartnerService _partnerService;
        IPosService _posService;
        IProductService _productService;
        IUtilityService _utilityService;
        public TransactionLoadTestController(IPartnerService partnerService, IPosService posService, IProductService productService, IUtilityService utilityService)
        {
            _partnerService = partnerService;
            _posService = posService;
            _productService = productService;
            _utilityService = utilityService;
        }
        // GET: /<controller>/

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            TransactionLoadTestModel model = new TransactionLoadTestModel();
            model.Partners = (await _partnerService.GetAllActivePartners())
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Index(TransactionLoadTestModel model)
        {
            model.Partners = (await _partnerService.GetAllActivePartners())
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            if (ModelState.IsValid)
            {
                var posassignments = await _posService.GetAllPosAssignment(model.PartnerId, model.NumberOfTerminals);
                var merchantId = posassignments.Select(x => x.MerchantId).Distinct().ToList();
                var merchants = await _partnerService.GetAllMerchant(model.PartnerId, merchantId);
                var posUsers = await _posService.GetAllPosUsers(model.PartnerId, merchantId);
                var posunitId = posassignments.Select(x => x.POSId).ToList();
                var posUnits = await _posService.GetAllPosUnit(model.PartnerId, posunitId);
                var products = await _productService.GetAllProducts(model.PartnerId);
                var productassignments = await _productService.GetAgentProductAssignment(model.PartnerId, merchantId);
                TransactionModel transactionModel = new TransactionModel();
                transactionModel.PosAssignments = posassignments;
                transactionModel.Merchants = merchants;
                transactionModel.PosUnits = posUnits;
                transactionModel.PosUsers = posUsers;
                transactionModel.ProductAgentAssignments = productassignments;
                transactionModel.Products = products;
                transactionModel.Request = model;
                await DoProcess(transactionModel);
            }
            return View(model);
        }

        private async Task DoProcess(TransactionModel transactionModel)
        {
            try
            {
                TestUtilityHeader testUtilityHeader = new TestUtilityHeader()
                {
                    Batch = "12312",
                    Date = DateTime.UtcNow,
                    NumberOfTerminals = transactionModel.Request.NumberOfTerminals,
                    NumberOfTransactionPerTerminal = transactionModel.Request.NumberOfTransactionPerTerminal,
                    PartnerId = transactionModel.Request.PartnerId,
                    StartTime = DateTime.UtcNow,
                    ProcessTypeId = 1,
                };
                List<TestUtilityLoadTestDetail> testUtilityLoadTestDetail = new List<TestUtilityLoadTestDetail>();
                //foreach (var item in posassignments)
                //{
                //    for (int i = 0; i < model.NumberOfTransactionPerTerminal; i++)
                //    {
                //        Thread t = new Thread(TransactionLoadTest);
                //        t.Start();
                //    }
                //} 

                var posassignment = transactionModel.PosAssignments.FirstOrDefault();
                var merchant = transactionModel.Merchants.FirstOrDefault(x => x.Id == posassignment.MerchantId);
                var posUnits = transactionModel.PosUnits.FirstOrDefault(x => x.Id == posassignment.POSId);
                var posuser = transactionModel.PosUsers.FirstOrDefault(x => x.MerchantId == posassignment.MerchantId);
                LoginModel user = new LoginModel()
                {
                    merchant_id = merchant.Id,
                    password = posuser.Password,
                    pos_assignment_id = posassignment.Id,
                    pos_id = posassignment.POSId,
                    user_type = "pos-users",
                    pos_version = "",
                    username = posuser.UserId
                };

                Token token = await GetToken(user);
                BusinessdayRequestVM businessdayRequest = new BusinessdayRequestVM()
                {
                    merchant_id = merchant.Id,
                    token = token.token,
                    user_id = posuser.Id,
                    pos_id = posassignment.POSId,
                };
                BusinessDayResponseVM businessDay = await GetActiveBusinessDay(businessdayRequest);
                var q = (from c in transactionModel.ProductAgentAssignments
                         join p in transactionModel.Products on c.ProductId equals p.Id
                         where c.AgentId == merchant.Id
                         select p
                               ).ToList();
                var rand = new Random();
                int count = rand.Next(1, q.Count() - 1);
                var product = q.Skip(count).Take(1).FirstOrDefault();
                PindownloadRequestVM downloadRequest = new PindownloadRequestVM()
                {
                    business_date = businessDay.business_date,
                    merchant_id = merchant.Id,
                    mode = 1,
                    pos_assignment_id = posassignment.Id,
                    pos_id = posassignment.POSId,
                    pos_user_id = posuser.Id,
                    product_group_id = product.ProductGroupId,
                    product_id = product.Id,
                    qty = 1,
                    request_id = 0,
                    service_provider_id = product.ServiceProviderId,
                    shift_no = businessDay.ShiftNo,
                    token = token.token
                };
                PinDownloadReponseVM downlaod = await PinDownload(downloadRequest);
                PinDownloadConfirmRequestVM confirmationRequest = new PinDownloadConfirmRequestVM()
                {
                    download_id = downlaod.download_Id ?? 0,
                    merchant_id = merchant.Id,
                    pos_assignment_id = posassignment.Id,
                    pos_id = posassignment.POSId,
                    pos_user_id = posuser.Id,
                    token = token.token,
                    txn = downlaod.Pin.Select(x =>
                        new PinDetails()
                        {
                            business_date = businessDay.business_date,
                            download_pin_id = x.download_pin_Id,
                            product_id = x.product_id,
                            sale_txn_no = Convert.ToInt64("50" + DateTime.UtcNow.ToString("yyyyMMddHHmm") + posassignment.Id),
                            serial_no = x.serial_no,
                            shift_no = businessDay.ShiftNo,
                            user_id = posUnits.Id
                        }
                    ).ToList()
                };
                ResponseVM response = await PinDownloadConfirmation(confirmationRequest);
                testUtilityHeader.EndTime = DateTime.UtcNow;
                testUtilityLoadTestDetail.Add(new TestUtilityLoadTestDetail()
                {
                    ConfirmationReponseTime = Convert.ToInt32(response.Response_time),
                    DownloadedPinId = downlaod.Pin.FirstOrDefault().download_pin_Id,
                    DownloadResponseTime = Convert.ToInt32(downlaod.Response_time.Replace("ms","")),
                    IsConfirmed = response.response_code == "00",
                    IsDownloadCompleted = downlaod.response_code == "00",
                    MerchantId = merchant.Id,
                    PinDownloadId = downlaod.download_Id ?? 0,
                    PosAssignmentId = posassignment.Id,
                    PosId = posassignment.POSId,
                    ResponseCode = response.response_code,
                    ProductId = product.Id,
                    TxnNo = confirmationRequest.txn.FirstOrDefault().sale_txn_no?.ToString()
                });
                await _utilityService.WriteTransaction(testUtilityHeader, testUtilityLoadTestDetail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TransactionLoadTest()
        {

        }

        private async Task<Token> GetToken(LoginModel user)
        {
            try
            {
                Token result = new Token();
                using (HttpClient client = new HttpClient())
                {
                    StringContent request = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    string endpoint = "https://riapimanagement.azure-api.net/api/pos-login";
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");
                    using (var Response = await client.PostAsync(endpoint, request))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string content = await Response.Content.ReadAsStringAsync();
                            string token = JsonConvert.DeserializeObject(content).ToString();
                            result = JsonConvert.DeserializeObject<Token>(token);
                            return result;
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task<BusinessDayResponseVM> GetActiveBusinessDay(BusinessdayRequestVM request)
        {
            BusinessDayResponseVM result = new BusinessDayResponseVM();
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                string endpoint = "https://riapimanagement.azure-api.net/api/pos-get-active-business-day-statusV2";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string response = await Response.Content.ReadAsStringAsync();
                        string businessday = JsonConvert.DeserializeObject(response).ToString();
                        result = JsonConvert.DeserializeObject<BusinessDayResponseVM>(businessday);
                        return result;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }
        private async Task<PinDownloadReponseVM> PinDownload(PindownloadRequestVM model)
        {
            PinDownloadReponseVM result = new PinDownloadReponseVM();
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                string endpoint = "https://riapimanagement.azure-api.net/api/pos-download-pinV2";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string response = await Response.Content.ReadAsStringAsync();
                        var responsestring = JsonConvert.DeserializeObject(response).ToString();
                        result = JsonConvert.DeserializeObject<PinDownloadReponseVM>(responsestring);
                        return result;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }
        private async Task<ResponseVM> PinDownloadConfirmation(PinDownloadConfirmRequestVM model)
        {
            ResponseVM result = new ResponseVM();
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                string endpoint = "https://riapimanagement.azure-api.net/api/pos-confirm-pin-ConfirmationV2";
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string confirmation = await Response.Content.ReadAsStringAsync();
                        var confirmationresponse = JsonConvert.DeserializeObject(confirmation).ToString();
                        result = JsonConvert.DeserializeObject<ResponseVM>(confirmationresponse);
                        return result;
                    }
                    else
                    {
                        return result;
                    }
                }
            }
        }
    }
}
