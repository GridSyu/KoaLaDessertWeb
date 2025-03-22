using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KoaLaDessertWeb.Models;
using KoaLaDessertWeb.Tools.DBContext;
using System.ComponentModel.DataAnnotations;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;


namespace KoaLaDessertWeb.Controllers
{
    /// <summary>
    /// 首頁
    /// </summary>
    [ApiExplorerSettings(GroupName = "HomeManagement")]
    [Route("KoaLaDessertWeb/[controller]")]
    public class HomeController : Controller
    {
        private Logger _loggerForNormal = new Logger(new WebNormal(), "Log"); // 一般紀錄
        private Logger _loggerForError = new Logger(new WebError(), "Log"); // 錯誤紀錄
        private readonly SqlDbContext _modelsContext;

        public HomeController(SqlDbContext modelsContext)
        {
            _modelsContext = modelsContext;
        }

        /// <summary>
        /// 提供首頁
        /// </summary>
        [HttpGet("Index")]
        public IActionResult Index()
        {
            string htmlPath = "~/Views/Home/Index.cshtml";
            return View(htmlPath);
        }

        /// <summary>
        /// 提供商品目錄頁面
        /// </summary>
        [HttpGet("Order")]
        public IActionResult Order()
        {
            string htmlPath = "~/Views/Home/Order.cshtml";
            return View(htmlPath);
        }

        /// <summary>
        /// 提供商品目錄頁面
        /// </summary>
        [HttpGet("Member")]
        public IActionResult Member()
        {
            string htmlPath = "~/Views/Home/Member.cshtml";
            return View(htmlPath);
        }

        /// <summary>
        /// 提供商品目錄頁面
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// </remarks>
        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            string htmlPath = "~/Views/Home/Privacy.cshtml";
            return View(htmlPath);
        }

        /// <summary>
        /// 取得用戶電子信箱
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// Subscribed = 此信箱已訂閱 <br />
        /// DataStructureFail = 輸入資料結構錯誤 <br />
        /// </remarks>
        [HttpPost("GetUsersEmailAddress")]
        public IActionResult GetUsersEmailAddress([FromBody] GetUsersEmailAddressInputmodel data)
        {
            string funcFrom = "Controllers.HomeController.GetUsersEmailAddress";
            try
            {
                string message = "";
                if(ModelState.IsValid)
                {
                    // 防止重複訂閱
                    if(_modelsContext.PushMails.FirstOrDefault(p => p.Mail == data.Email) != null)
                    {
                        message = "Subscribed";
                        _loggerForNormal.Write(message, funcFrom);
                        return Ok(new { state = "Success", message = message, results = "此信箱已訂閱" });
                    }
                    // 創建新增模型
                    var addData = new PushMail
                    {
                        Mail = data.Email
                    };
                    // 變更寫入資料庫
                    _modelsContext.PushMails.Add(addData);
                    _modelsContext.SaveChanges();

                    message = "Success";
                    _loggerForNormal.Write(message, funcFrom);
                    return Ok(new {state = "Success", message = message, results = addData.Id});
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
                return Ok(new {state = "error", message = ex.Message , results = new{ }});
            }
        }




        /// <summary>
        /// 取得用戶電子信箱 輸入模型
        /// </summary>
        public class GetUsersEmailAddressInputmodel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

    }
}
