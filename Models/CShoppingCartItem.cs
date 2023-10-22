using System.ComponentModel;

namespace products.Models
{
    public class CShoppingCartItem
    {
        //紀錄購買的 產品 金額 數量
        public int productId { get; set; }

        [DisplayName("數量")]
        public int count { get; set; }
        [DisplayName("單價")]
        public decimal price { get; set; }
        [DisplayName("總計")]
        public decimal 小計 { get { return this.count * this.price; } }


        //join
        public Product product { get; set; }
    }
}
