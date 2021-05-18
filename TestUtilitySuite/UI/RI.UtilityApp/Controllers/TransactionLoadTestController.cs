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
        IPosService _posService;
        public TransactionLoadTestController(IPartnerService partnerService, IPosService posService)
        {
            _partnerService = partnerService;
            _posService = posService;
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
                var posassignments= await _posService.GetAllPosAssignment(model.PartnerId, model.NumberOfTerminals); 

    }
            return View(model);
        }
    }
}
