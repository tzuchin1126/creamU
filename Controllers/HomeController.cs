using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using products.Models;
using products.ViewModel;
using System.Diagnostics;

namespace products.Controllers
{
    public class HomeController : Controller
    {
        // 宣告私有變數用於儲存環境設定
        private IWebHostEnvironment _enviro;
        private IWebHostEnvironment _images;
        
        public HomeController(IWebHostEnvironment p, IWebHostEnvironment images) // 建構子，注入環境設定
        {
            _enviro = p;
            _images = images;
        }


        public ActionResult ProductIndex(CKeywordViewModel vm, string keyword = "", string productStatus = "all", decimal minPrice = 0, decimal maxPrice = decimal.MaxValue, int page = 1)
        {
            // 分頁設定
            int pageSize = 10;
            IQueryable<Product> datas = SearchProducts(keyword, minPrice, maxPrice);

            // 根據選擇的商品狀態篩選商品
            if (!string.IsNullOrEmpty(productStatus) && productStatus != "all")
                datas = datas.Where(p => p.ProductStatus == productStatus);

            // 根據用戶輸入的關鍵字進行篩選
            if (!string.IsNullOrEmpty(vm.txtKeyword))
                datas = datas.Where(p => p.ProductName.Contains(vm.txtKeyword));

            // 商品按照發布日期降序排列
            datas = datas.OrderByDescending(p => p.ReleaseDate);

            // 執行分頁以顯示特定頁面的商品
            int recordCount = datas.Count();
            int totalPages = (int)Math.Ceiling((double)recordCount / pageSize);
            datas = datas.Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.Page = page; // 當前頁數
            ViewBag.pageNum = totalPages; // 總頁數
            return View(datas.ToList());
        }

        private IQueryable<Product> SearchProducts(string keyword, decimal minPrice, decimal maxPrice)
        {
            try
            {
                mydbContext db = new mydbContext();
                IQueryable<Product> datas = db.Products;

                if (!string.IsNullOrEmpty(keyword))
                    datas = datas.Where(p => p.ProductName.Contains(keyword));

                datas = datas.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                return datas;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "資料庫存取異常，請稍後再試或聯絡系統管理員。");
                throw; // 可以選擇重新拋出異常，讓上層處理異常
            }
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("List"); // 如果未提供商品 ID，則重定向到列表頁面

            mydbContext db = new mydbContext();
            Product prod = db.Products.FirstOrDefault(p => p.ProductId == id);


            // 創建商品狀態的下拉選項數據
            var productStatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "上架中", Text = "上架中" },
                new SelectListItem { Value = "缺貨中", Text = "缺貨中" },
                new SelectListItem { Value = "未上架", Text = "未上架" },
                new SelectListItem { Value = "已停產", Text = "已停產" }
            };
            ViewBag.Categories = productStatusOptions;
            return View(prod);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] //用於防止跨站點請求偽造攻擊（CSRF）

        public ActionResult Edit(CProductWraper x, IFormFile file)
        {
            if (x == null)
                return RedirectToAction("List");

            mydbContext db = new mydbContext();
            Product prod = db.Products.FirstOrDefault(p => p.ProductId == x.ProductId);

            if (prod != null)
            {
                if (x.photo != null) //檢查是否上傳了新的圖片
                {
                    string photoName = Guid.NewGuid().ToString() + ".jpg";
                    string imagePath = Path.Combine(_enviro.WebRootPath, "imgs", photoName);
                    x.photo.CopyTo(new FileStream(imagePath, FileMode.Create));
                    prod.ProductImage = photoName;
                }
                prod.UpdatedDate = DateTime.Now.ToString("yyyy-MM-dd");
                prod.ProductName = x.ProductName;
                prod.Descript = x.Descript;
                prod.ProductStatus = x.ProductStatus;
                prod.Price = x.Price;
                prod.ProductStock = x.ProductStock;
                prod.Type = x.Type;
                db.SaveChanges();
            }
            return RedirectToAction("ProductIndex");
        }



        public ActionResult Create()
        {
            SetProductStatusOptions();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product t, IFormFile photo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // 驗證表單數據的有效性
                    if (photo == null)
                        ModelState.AddModelError("photo", "請選擇商品圖片");
                    else if (!IsImageValid(photo))
                        ModelState.AddModelError("photo", "圖片格式不正確，只支援 JPG、PNG、GIF 格式。");

                    if (IsProductDuplicate(t.ProductName))
                        ModelState.AddModelError("ProductName", "商品名稱已存在。");
                    else if (t.ProductName.Length < 3)
                        ModelState.AddModelError("ProductName", "商品名稱必須至少包含3個字元。");
                    else if (t.ProductName.Length > 60)
                        ModelState.AddModelError("ProductName", "商品名稱不能超過60個字元。");

                    if (t.ProductStock <= 0)
                        ModelState.AddModelError("ProductStock", "庫存數量必須大於零。");

                    if (t.Price <= 0)
                        ModelState.AddModelError("Price", "價格必須大於零。");

                    
                    if (ModelState.IsValid)
                    {
                        t.ReleaseDate = DateTime.Now.ToString("yyyy-MM-dd"); // 設定 ReleaseDate 為當下日期，紀錄修改商品資訊的日期

                        if (photo != null) // 上傳圖片
                        {
                            string photoName = Guid.NewGuid().ToString() + ".jpg";
                            string imagePath = Path.Combine(_images.WebRootPath, "imgs", photoName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                                photo.CopyTo(stream);
                            t.ProductImage = photoName;
                        }

                        // 儲存商品資料到資料庫
                        mydbContext db = new mydbContext();
                        db.Products.Add(t);
                        db.SaveChanges();

                        return RedirectToAction("ProductIndex");
                    }
                }
                SetProductStatusOptions();
                return View(t);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "資料庫存取異常，請稍後再試或聯絡系統管理員。");
                return View(t);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                mydbContext db = new mydbContext();
                Product prod = db.Products.FirstOrDefault(p => p.ProductId == id);
                if (prod != null)
                {
                    db.Products.Remove(prod);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("ProductIndex");
        }




        // 商品預覽
        public ActionResult ProductDetail(int id) 
        {
            mydbContext db = new mydbContext();
            Product product = db.Products.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound(); //如果找不到對應的商品，可以返回一個404頁面或其他適當的處理方式

            
            var jsonResult = new // 商品資料轉換 JSON 
            {
                imageUrl = product.ProductImage,
                productName = product.ProductName,
                price = product.Price,
                description = product.Descript,
                productStock = product.ProductStock
            };
            return Json(jsonResult);
        }

        private void SetProductStatusOptions()
        {
            var productStatusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "上架中", Text = "上架中" },
                new SelectListItem { Value = "缺貨中", Text = "缺貨中" },
                new SelectListItem { Value = "暫時下架", Text = "暫時下架" },
                new SelectListItem { Value = "已停產", Text = "已停產" }
            };
            ViewBag.Categories = productStatusOptions;
        }

        
        private bool IsProductDuplicate(string productName) 
        {
            mydbContext db = new mydbContext();
            return db.Products.Any(p => p.ProductName == productName);
        }
        
        private bool IsImageValid(IFormFile photo) 
        {
            string[] supportedFormats = { "jpg", "jpeg", "png", "gif" }; 
            var fileExtension = Path.GetExtension(photo.FileName).Substring(1).ToLower(); 
            return supportedFormats.Contains(fileExtension); 
        }
    }
}