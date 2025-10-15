namespace price_conversion_web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using price_conversion_web.Models;

    public class PurchaseController : Controller
    {
        // GET: PurchaseController
        public ActionResult Index()
        {
            return View();
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
