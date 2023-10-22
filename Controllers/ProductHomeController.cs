using Microsoft.AspNetCore.Mvc;

namespace products.Controllers
{
    public class ProductHomeController : Controller
    {
        private IWebHostEnvironment _enviro = null;
        public ProductHomeController(IWebHostEnvironment p)
        {
            _enviro = p;
        }
        public ActionResult demoFileUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult demoFileUpload(IFormFile photo)
        {
            string path = _enviro.WebRootPath + "/imgs/001.jpg";
            photo.CopyTo(new FileStream(path, FileMode.Create));
            return View();
        }

        public IActionResult showCountBySession()
        {
            int count = 0;
            //判斷有沒有session
            if (HttpContext.Session.Keys.Contains("COUNT"))
                count = (int)HttpContext.Session.GetInt32("COUNT");
            count++;
            HttpContext.Session.SetInt32("COUNT", count);
            ViewBag.COUNT = count;
            return View();
        }
    }
}
