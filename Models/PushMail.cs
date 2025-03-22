using System.ComponentModel.DataAnnotations;

namespace KoaLaDessertWeb.Models
{
    public class PushMail
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 電子信箱
        /// </summary>
        [Required]
        [EmailAddress]
        public string Mail { get; set; }
    }
}