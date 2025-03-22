using System.ComponentModel.DataAnnotations;

namespace KoaLaDessertWeb.Models
{
    public class User
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        [Display(Name = "帳號")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "密碼")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        [Required]
        [Display(Name = "電子信箱")]
        [EmailAddress(ErrorMessage = "Email 格式設定有誤")]
        public string Email { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        [Display(Name = "出生日期")]
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        [Display(Name = "手機號碼")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Display(Name = "地址")]
        public string Address { get; set; }
    }
}