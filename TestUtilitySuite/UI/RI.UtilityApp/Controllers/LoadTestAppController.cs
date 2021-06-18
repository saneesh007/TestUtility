using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RI.AppFramework.EntityModel;
using RI.AppFramework.Models;
using RI.Services.Partner;
using RI.Services.Utility;
using RI.UtilityApp.Models;
using RI.UtilityApp.Services;

namespace RI.UtilityApp.Controllers
{
    public class LoadTestAppController : Controller
    {
        IPartnerService _partnerService;
        IPosService _posService;
        IProductService _productService;
        IHttpContextAccessor _httpContextAccessor;
        IUtilityService _utilityService;
        private readonly IViewRenderService _viewRenderService;
        public LoadTestAppController(IPartnerService partnerService, IPosService posService,
            IProductService productService, IHttpContextAccessor httpContextAccessor, IUtilityService utilityService
            , IViewRenderService viewRenderService)
        {
            _partnerService = partnerService;
            _posService = posService;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
            _utilityService = utilityService;
            _viewRenderService = viewRenderService;
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
        public async Task<JsonResult> ProcesTransaction(TransactionLoadTestModel model)
        {
            TransactionModel transactionModel = new TransactionModel();
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
                transactionModel.PosAssignments = posassignments;
                transactionModel.Merchants = merchants;
                transactionModel.PosUnits = posUnits;
                transactionModel.PosUsers = posUsers;
                transactionModel.ProductAgentAssignments = productassignments;
                transactionModel.Products = products;
                transactionModel.Request = model;
                transactionModel.URL = GetBaseUrl();
                transactionModel.TestUtilityHeader = new TestUtilityHeader()
                {
                    Date = DateTime.UtcNow,
                    NumberOfTerminals = transactionModel.Request.NumberOfTerminals,
                    NumberOfTransactionPerTerminal = transactionModel.Request.NumberOfTransactionPerTerminal,
                    PartnerId = transactionModel.Request.PartnerId,
                    StartTime = DateTime.UtcNow,
                    ProcessTypeId = 1,
                };
                transactionModel.TestUtilityHeader = await RegisterTransaction(transactionModel.TestUtilityHeader, transactionModel.URL);
                if (transactionModel.TestUtilityHeader.Id != 0)
                {
                    TransactionLoadTest(transactionModel);
                }
                else
                {

                    return Json(new { status = 0, message = "Batch transaction unable to create", data = string.Empty });
                }
            }
            return Json(new { status = 1, message = "Transaction processing with batch " + transactionModel.TestUtilityHeader.Batch, data = string.Empty });
        }
        [HttpGet]
        public async Task<JsonResult> GetProcessDetail(int id)
        {
            var viewModel = await _utilityService.GetTransaction(id);
            var result = await _viewRenderService.RenderToStringAsync("LoadTestApp/_Details", viewModel);
            return Json(new { status = 1, message = "Transaction loaded", data = result });
        }
        [HttpGet]
        public async Task<JsonResult> LoadTransactionList(int? pageIndex, int? pageSize, string searchText = "")
        {
            var viewModel = await _utilityService.GetTransaction(pageIndex ?? 1, pageSize ?? 10, searchText);
            var result = await _viewRenderService.RenderToStringAsync("LoadTestApp/_TransactionList", viewModel);
            return Json(new { status = 1, message = "Transaction loaded", data = result });
        }

        [HttpGet]
        public async Task<JsonResult> AddModel()
        {
            TransactionLoadTestModel model = new TransactionLoadTestModel();
            model.Partners = (await _partnerService.GetAllActivePartners())
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            var result = await _viewRenderService.RenderToStringAsync("LoadTestApp/_AddTransaction", model);
            return Json(new { status = 1, message = "Transaction add loaded", data = result });
        }
        #region process
        private string GetBaseUrl()
        {
            var baseUrl = String.Empty;
            var request = _httpContextAccessor.HttpContext.Request;
            baseUrl = string.Format("{0}://{1}", request.Scheme, request.Host.ToUriComponent());
            if (!string.IsNullOrWhiteSpace(baseUrl) && !baseUrl.EndsWith("/"))
                baseUrl = String.Format("{0}/", baseUrl);
            return baseUrl;
        }
        private async Task TransactionLoadTest(TransactionModel transactionModel)
        {
            transactionModel.TestUtilityHeader.TestUtilityLoadTestDetail = new List<TestUtilityLoadTestDetail>();
            List<Thread> transactionThread = new List<Thread>();
            foreach (var item in transactionModel.PosAssignments)
            {
                for (int i = 0; i < transactionModel.Request.NumberOfTransactionPerTerminal; i++)
                {
                    Thread t = new Thread(() => DoProcess(transactionModel, item, i + 1));
                    transactionThread.Add(t);
                    t.Start();
                }
            }
        }
        private async Task DoProcess(TransactionModel transactionModel, PosAssignment posassignment, int iteration)
        {
            try
            {
                var merchant = transactionModel.Merchants.FirstOrDefault(x => x.Id == posassignment.MerchantId);
                var posUnits = transactionModel.PosUnits.FirstOrDefault(x => x.Id == posassignment.POSId);
                var posuser = transactionModel.PosUsers.FirstOrDefault(x => x.MerchantId == posassignment.MerchantId);
                var detail = new TestUtilityLoadTestDetail()
                {
                    HdrId = transactionModel.TestUtilityHeader.Id,
                    MerchantId = merchant.Id,
                    PosUserId = posuser.Id,
                    PosAssignmentId = posassignment.Id,
                    PosId = posassignment.POSId,
                };
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
                detail.ResponseCode = token.response_code;
                if (detail.ResponseCode == "00")
                {
                    BusinessdayRequestVM businessdayRequest = new BusinessdayRequestVM()
                    {
                        merchant_id = merchant.Id,
                        token = token.token,
                        user_id = posuser.Id,
                        pos_id = posassignment.POSId,
                    };
                    BusinessDayResponseVM businessDay = await GetActiveBusinessDay(businessdayRequest);
                    detail.ResponseCode = businessDay.response_code;
                    if (detail.ResponseCode == "00")
                    {
                        var q = (from c in transactionModel.ProductAgentAssignments
                                 join p in transactionModel.Products on c.ProductId equals p.Id
                                 where c.AgentId == merchant.Id
                                 select p
                                               ).ToList();
                        var rand = new Random();
                        int count = rand.Next(1, q.Count() - 1);
                        var product = q.Skip(count).Take(1).FirstOrDefault();
                        detail.ProductId = product.Id;
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
                        detail.ResponseCode = downlaod.response_code;
                        if (detail.ResponseCode == "00")
                        {
                            detail.DownloadedPinId = downlaod.Pin.FirstOrDefault().download_pin_Id;
                            detail.DownloadResponseTime = Convert.ToInt32(downlaod.Response_time.Replace("ms", ""));
                            detail.IsDownloadCompleted = downlaod.response_code == "00";
                            detail.PinDownloadId = downlaod.download_Id ?? 0;
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
                                        sale_txn_no = Convert.ToInt64("50" + DateTime.UtcNow.ToString("yyMMddHHmmssff") + posassignment.Id.ToString()),
                                        serial_no = x.serial_no,
                                        shift_no = businessDay.ShiftNo,
                                        user_id = posUnits.Id
                                    }
                                ).ToList()
                            };
                            detail.TxnNo = confirmationRequest.txn.FirstOrDefault().sale_txn_no?.ToString();
                            ResponseVM response = await PinDownloadConfirmation(confirmationRequest);
                            detail.IsConfirmed = response.response_code == "00";
                            detail.ResponseCode = response.response_code;
                            detail.ConfirmationReponseTime = Convert.ToInt32(response.Response_time);
                        }
                    }
                }
                transactionModel.TestUtilityHeader.EndTime = DateTime.UtcNow;
                transactionModel.TestUtilityHeader.TestUtilityLoadTestDetail.Add(detail);
                if (transactionModel.TestUtilityHeader.TestUtilityLoadTestDetail.Count == (transactionModel.Request.NumberOfTerminals * transactionModel.Request.NumberOfTransactionPerTerminal))
                {
                    await WriteTransactionLoad(transactionModel.TestUtilityHeader, transactionModel.URL);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        private async Task<bool> WriteTransactionLoad(TestUtilityHeader testResult, string url)
        {
            try
            {
                bool result = false;
                using (HttpClient client = new HttpClient())
                {
                    var data = JsonConvert.SerializeObject(testResult.TestUtilityLoadTestDetail);
                    StringContent request = new StringContent(data, Encoding.UTF8, "application/json");
                    string endpoint = url + "api/UtilityApp/WriteTransactionLoad"; //url + "api/Utility/WriteTransactionLoad";
                    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");
                    using (var Response = await client.PostAsync(endpoint, request))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string content = await Response.Content.ReadAsStringAsync();
                            string token = JsonConvert.DeserializeObject(content).ToString();
                            result = JsonConvert.DeserializeObject<bool>(token);
                            return result;
                        }
                        else
                        {
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<TestUtilityHeader> RegisterTransaction(TestUtilityHeader testResult, string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var data = JsonConvert.SerializeObject(testResult);
                    StringContent request = new StringContent(data, Encoding.UTF8, "application/json");
                    string endpoint = url + "api/UtilityApp/RegisterTransaction"; //url + "api/Utility/WriteTransactionLoad";
                    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "50185b70341b4f5aa5e1d3307a261798");
                    using (var Response = await client.PostAsync(endpoint, request))
                    {
                        if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string content = await Response.Content.ReadAsStringAsync();
                            string header = JsonConvert.DeserializeObject(content).ToString();
                            testResult = JsonConvert.DeserializeObject<TestUtilityHeader>(header);
                            return testResult;
                        }
                        else
                        {
                            return testResult;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
        #endregion
    }
}
