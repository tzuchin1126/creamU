using products.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace products.ViewModel
{
    public class CProductWraper
    {
        private Product _product;

        public CProductWraper()
        {
            if (_product == null)
                _product = new Product();
        }

        public Product product
        {
            get { return _product; }
            set { _product = value; }
        }

        [DisplayName("圖片")]
        [Required(ErrorMessage = "請上傳商品圖片")]
        public string? ProductImage
        {
            get { return _product.ProductImage; }
            set { _product.ProductImage = value; }
        }

        public int ProductId
        {
            get { return _product.ProductId; }
            set { _product.ProductId = value; }
        }

        [DisplayName("名稱")]
        [Required(ErrorMessage = "請輸入商品名稱")]
        public string? ProductName
        {
            get { return _product.ProductName; }
            set { _product.ProductName = value; }
        }

        [DisplayName("描述")]
        [Required(ErrorMessage = "請輸入商品敘述")]
        [MaxLength(500, ErrorMessage = "商品敘述不能超過300個字")]
        public string? Descript
        {
            get { return _product.Descript; }
            set { _product.Descript = value; }
        }

      
        [DisplayName("售價")]
        [Required(ErrorMessage = "請輸入商品售價")]
        [Range(100, int.MaxValue, ErrorMessage = "商品售價必須大於等於100元")]
        public int? Price
        {
            get { return _product.Price; }
            set { _product.Price = value; }
        }

        [DisplayName("狀態")]
        public string? ProductStatus
        {
            get { return _product.ProductStatus; }
            set { _product.ProductStatus = value; }
        }

        [DisplayName("庫存")]
        [Required(ErrorMessage = "請輸入庫存數量")]
        public int? ProductStock
        {
            get { return _product.ProductStock; }
            set { _product.ProductStock = value; }
        }

        [DisplayName("上架日期")]
        public string? ReleaseDate
        {
            get { return _product.ReleaseDate; }
            set { _product.ReleaseDate = value; }
        }

        [DisplayName("更新日期")]
        public string? UpdatedDate
        {
            get { return _product.UpdatedDate; }
            set { _product.UpdatedDate = value; }
        }

        [DisplayName("類型")]
        public string? Type
        {
            get { return _product.Type; }
            set { _product.Type = value; }
        }

        // 其他屬性...
        public IFormFile photo { get; set; }
    }
}
