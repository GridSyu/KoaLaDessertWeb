using System.ComponentModel.DataAnnotations;

namespace KoaLaDessertWeb.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; } // 主鍵，建議使用 Guid 生成唯一值
        public string UserName { get; set; } // "遊客" 或會員帳號
        public List<string> RoleList { get; set; } // 使用者的身分清單
        [Required]
        public string MessageContent { get; set; } // 留言內容
        public DateTimeOffset MessageTime { get; set; } // 留言時間
    }
}