using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using products.Models;
using products.ViewModel;
using System.Text.Json;

namespace products.Controllers
{
	public class IndexController : Controller
	{
        //檢視購物車
        //public IActionResult CartView()
        //{
        //	// 檢查 Session 中是否包含購買產品的清單
        //	if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
        //		return RedirectToAction("Index"); // 若無，重新導向至首頁
        //	string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
        //	List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
        //	if (cart == null)
        //		return RedirectToAction("Index");// 若購物車為空，重新導向至首頁
        //	return View(cart); // 若購物車為空，重新導向至首頁
        //}

        //檢視購物車
        public IActionResult CartView()
        {
            if (!HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))
                return RedirectToAction("Index");

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);

            if (cart == null)
                return RedirectToAction("Index");

            // 使用字典來合併相同產品的數量
            Dictionary<int, CShoppingCartItem> mergedCart = new Dictionary<int, CShoppingCartItem>();

            foreach (var item in cart)
            {
                if (mergedCart.ContainsKey(item.productId))
                {
                    // 如果已經存在相同的產品，將數量相加
                    mergedCart[item.productId].count += item.count;
                }
                else
                {
                    // 否則將產品加入字典中
                    mergedCart.Add(item.productId, item);
                }
            }

            // 將合併後的購物車更新回 Session 中
            json = JsonSerializer.Serialize(mergedCart.Values.ToList());
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

            return View(mergedCart.Values.ToList());
        }


        //訂單購物
        public IActionResult CreateOrder()
		{
            return View();
        }


        // 移除购物车中的商品
        public IActionResult RemoveFromCart(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return RedirectToAction("CartView"); // 如果productId为空，返回购物车视图
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (json != null)
            {
                List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
                CShoppingCartItem itemToRemove = cart.FirstOrDefault(item => item.productId.ToString() == productId);// 查找要移除的商品
                if (itemToRemove != null)
                {
                    cart.Remove(itemToRemove); // 从购物车中移除商品
                    json = JsonSerializer.Serialize(cart);
                    HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json); // 更新Session中的购物车信息
                }
            }
            return RedirectToAction("CartView");
        }


        public IActionResult Index(CKeywordViewModel vm, int page = 1)
        {
            string keyword = vm.txtKeyword;
            mydbContext db = new mydbContext();

            IQueryable<Product> datas = db.Products;

            if (!string.IsNullOrEmpty(keyword))
                datas = datas.Where(p => p.ProductName.Contains(keyword));

            datas = datas.Where(p =>
                p.ProductStatus != "未上架" &&
                p.ProductStatus != "缺貨中" &&
                p.ProductStatus != "已停產");

            datas = PaginateProducts(datas, page, 8);

            ViewBag.Page = page;
            ViewBag.Keyword = keyword;

            return View(datas.ToList());
        }




        // 加入購物車
        public ActionResult AddToCart(int? id)
		{
			//if (id == null)
			//	return RedirectToAction("index");
			//ViewBag.ProductId = id;
			//return View();

			if (id == null)
				return RedirectToAction("index");

			mydbContext db = new mydbContext();
			Product prod = db.Products.FirstOrDefault(t => t.ProductId == id);
			if (prod != null)
			{
				ViewBag.ProductId = prod.ProductId;
				ViewBag.ProductImage = prod.ProductImage;
				ViewBag.ProductName = prod.ProductName;
				ViewBag.Descript = prod.Descript; //描述
				ViewBag.Price = prod.Price;
				return View();
			}

			return RedirectToAction("index");
		}
		[HttpPost]
		public ActionResult AddToCart(AddToCartViewModel vm)
		{
            mydbContext db = new mydbContext();
			Product prod = db.Products.FirstOrDefault(t => t.ProductId == vm.txtFid);
            if (prod != null) //如果不是null代表有東西放進購物車
            {
				string json = "" ; //取得json字串
                List<CShoppingCartItem> cart = null; //需要有購物車,先等於null
                if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_PRODUCTS_LIST))  // 檢查 Session 中是否已經有購物車資料
                {
					json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST); //利用字串把session裡面get出來
                    cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json); //用Deserialize
                }
                else  // 如果 Session 中沒有購物車資料，則初始化一個新的購物車
                    cart = new List<CShoppingCartItem>();

                // 將畫面上的資料記錄到購物車項目中
                CShoppingCartItem item = new CShoppingCartItem();
				item.price = (decimal)prod.Price;
				item.productId = vm.txtFid;
				item.count = vm.txtCount;
				item.product = prod;
				cart.Add(item);
               
                json = JsonSerializer.Serialize(cart);  // 將更新後的購物車資料轉換為 JSON 字串並存回 Session 中
                HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
			}
            return RedirectToAction("Index");
        }

		private IQueryable<Product> PaginateProducts(IQueryable<Product> products, int page, int pageSize)
		{
			int recordCount = products.Count();
			int totalPages = (int)Math.Ceiling((double)recordCount / pageSize);
			products = products.Skip((page - 1) * pageSize).Take(pageSize);
			ViewBag.Page = page; // 當前頁數
			ViewBag.pageNum = totalPages; // 總頁
			return products;
		}


		// 商品預覽
		public ActionResult ProductDetail(int id) 
		{
			mydbContext db = new mydbContext();
			Product product = db.Products.FirstOrDefault(p => p.ProductId == id);
			if (product == null)
				return NotFound(); //如果找不到對應的商品，可以返回一個404頁面
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


		public ActionResult about()
		{
			return View();
		}
	}
}
