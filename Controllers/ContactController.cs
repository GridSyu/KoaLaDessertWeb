using System.ComponentModel.DataAnnotations;
using KoaLaDessertWeb.Models;
using KoaLaDessertWeb.Tools.DBContext;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;
using Microsoft.AspNetCore.Identity;

namespace KoaLaDessertWeb.Controllers
{
    /// <summary>
    /// 告訴我們
    /// </summary>
    [ApiExplorerSettings(GroupName = "HomeManagement")]
    [Route("KoaLaDessertWeb/[controller]")]
    public class ContactController : Controller
    {
        private Logger _loggerForNormal = new Logger(new WebNormal(), "Log"); // 一般紀錄
        private Logger _loggerForError = new Logger(new WebError(), "Log"); // 錯誤紀錄
        private readonly SqlDbContext _modelsContext;

        public ContactController(SqlDbContext modelsContext)
        {
            _modelsContext = modelsContext;
        }

        /// <summary>
        /// 提供頁面
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// </remarks>
        [HttpGet("Index")]
        public IActionResult Index()
        {
            string htmlPath = "~/Views/Home/Contact.cshtml";
            var messages = _modelsContext.Messages
                            .OrderByDescending(m => m.MessageTime) // 按時間降序排列
                            .ToList();
            return View(htmlPath, messages);
        }


        /// <summary>
        /// 獲取所有留言
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// </remarks>
        [HttpGet("GatMessages")]
        public IActionResult GatMessages()
        {
            string funcFrom = "Controllers.ContactController.GatMessages";
            try
            {
                string message = "";
                var messages = _modelsContext.Messages
                .OrderByDescending(m => m.MessageTime)
                .ToList();

                // 將UTC時間取出時 依照時區顯示
                foreach (var item in messages)
                {
                    item.MessageTime = item.MessageTime.ToOffset(TimeSpan.FromHours(8));
                }

                message = "Success";
                _loggerForNormal.Write(message, funcFrom);
                return Ok(new { state = "Normal", message = message, results = messages });
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }


        /// <summary>
        /// 新增留言
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// DataStructureFail = 輸入資料結構錯誤 <br />
        /// </remarks>
        [HttpPost("AddMessage")]
        public IActionResult AddMessage([FromBody] AddMessageInputModel data)
        {
            string funcFrom = "Controllers.ContactController.AddMessage";
            try
            {
                string message = "";
                if(ModelState.IsValid)
                {
                    // 創建輸出模型
                    var outputData = new Message
                    {
                        Role = data.Role,
                        MessageContent = data.MessageContent,
                        MessageTime = DateTimeOffset.UtcNow
                    };
                    //資料寫入資料庫並儲存
                    _modelsContext.Messages.Add(outputData);
                    _modelsContext.SaveChanges();


                    message = "Success";
                    _loggerForNormal.Write(message, funcFrom);
                    return Ok(new { state = "Normal", message = message, results = outputData.Id });
                }
                else
                {
                    message = "DataStructureFail";
                    _loggerForNormal.Write(message, funcFrom);
                    return Ok(new { state = "normal", message = message, result = "輸入資料結構錯誤" });
                }
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }


        /// <summary>
        /// 新增留言 輸入模型
        /// </summary>
        public class AddMessageInputModel
        {
            public string Role { get; set; } // "遊客" 或會員帳號
            public string MessageContent { get; set; } // 留言內容
        }

    }
}

