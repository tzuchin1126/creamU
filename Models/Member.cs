using System.ComponentModel.DataAnnotations;

namespace products.Models
{
    public class Member
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "姓名")]
        [StringLength(10, ErrorMessage = "姓名長度不能超過10個字符。")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "電話")]
        [RegularExpression(@"^[0-9]{9,}$", ErrorMessage = "電話號碼至少需要9個數字。")]
        public string? Telephone { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "密碼")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "密碼長度需在8到15位之間 !")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email 是必填的。")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "請輸入有效的 Email 地址。")]
        public string Email { get; set; }

        public int? EmailId { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "地址")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "生日")]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "權限")]
        [Range(1, 5, ErrorMessage = "權限必須在1到5之間。")]
        public int? Level { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "加入日期")]
        [DataType(DataType.Date, ErrorMessage = "請輸入有效的日期。")]
        public DateTime? JoinDate { get; set; }

        [Required(ErrorMessage = "此欄位必填 !")]
        [Display(Name = "照片")]
        public string? Image { get; set; }


        [Display(Name = "備註")]
        public string? Notes { get; set; }
    }
}
