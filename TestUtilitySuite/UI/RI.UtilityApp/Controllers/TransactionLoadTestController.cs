using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RI.AppFramework.Models;
using RI.Services.Partner;
using RI.UtilityApp.Models;

namespace RI.UtilityApp.Controllers
{
    public class TransactionLoadTestController : Controller
    {
        IPartnerService _partnerService;
        IPosService _posService;
        IProductService _productService;
        public TransactionLoadTestController(IPartnerService partnerService, IPosService posService, IProductService productService)
        {
            _partnerService = partnerService;
            _posService = posService;
            _productService = productService;
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
                var posUsers = await _posService.GetAllPosUsers(model.PartnerId, merchantId);
                var posunitId = posassignments.Select(x => x.POSId).ToList();
                var posUnits = await _posService.GetAllPosUnit(model.PartnerId, posunitId);
                var products = await _productService.GetAllProducts(model.PartnerId);
                var productassignments = await _productService.GetAgentProductAssignment(model.PartnerId, merchantId);
                DoProcess(posassignments, posUsers, posUnits, products,productassignments);
            }
            return View(model);
        }

        private void DoProcess(List<PosAssignment> posassignments, List<PosUser> posUsers, List<PosUnits> posUnits, List<Product> products, List<ProductAgentAssignment> productassignments)
        { 
        }

        private async Task<string> GetToken(LoginModel user)
        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                string endpoint = "https://riapimanagement.azure-api.net/api/pos-login";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string token = await Response.Content.ReadAsStringAsync();

                        return token;
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
            }

        }
    }
