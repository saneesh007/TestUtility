using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RI.Services.Partner;
using RI.UtilityApp.Models;

namespace RI.UtilityApp.Controllers
{
    public class TransactionLoadTestController : Controller
    {
        IPartnerService _partnerService;
        public TransactionLoadTestController(IPartnerService partnerService)
        {
            _partnerService = partnerService;
        }
        // GET: /<controller>/
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
    }
}
