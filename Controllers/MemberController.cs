using Microsoft.AspNetCore.Mvc;
using products.Models;

namespace products.Controllers
{
    public class MemberController : Controller
    {

        private readonly mydbContext _context;
        private readonly IWebHostEnvironment _images;

        public MemberController(mydbContext context, IWebHostEnvironment images)
        {
            _context = context;
            _images = images;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
