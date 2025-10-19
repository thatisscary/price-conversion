namespace price_conversion_web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using price_conversion_web.Models;
    using price_conversion_web.Services;

    public class PurchaseController : Controller
    {
        
        public PurchaseController()
        {
            /*PurchaseDataService purchaseDataService, ILogger< PurchaseController > _purchaseContoller
            _purchaseDataService = purchaseDataService;
            this._purchaseContoller = _purchaseContoller;*/
        }

        // GET: PurchaseController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> List()
        {
            //var result = await _purchaseDataService.GetPurchasesDataAsync();
            PurchaseResult[] result = new PurchaseResult[] { new PurchaseResult() { Description = "Sample Purchase", TotalAmount = 100.00M, TransactionDate = DateTime.Now },
            new PurchaseResult { Description = "Sample Purchase", TotalAmount = 100.00M, TransactionDate = DateTime.Now }};
            return View(result);
        }

        // GET: PurchaseController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PurchaseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PurchaseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PurchaseRequest collection)
        {
            try
            {
                PurchaseResult result = new PurchaseResult(collection);
                return View(nameof(PurchaseResult),result);
            }
            catch
            {
                return View();
            }
        }

        // GET: PurchaseController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PurchaseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PurchaseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PurchaseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
