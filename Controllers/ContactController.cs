using System.ComponentModel.DataAnnotations;
using KoaLaDessertWeb.Models;
using KoaLaDessertWeb.Tools.DBContext;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly UserManager<IdentityUser> _userManager;

        public ContactController(SqlDbContext modelsContext, UserManager<IdentityUser> userManager)
        {
            _modelsContext = modelsContext;
            _userManager = userManager;
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
                            .OrderByDescending(m => m.MessageTime)
                            .Select(m => new
                            {
                                m.Id,
                                m.UserName,
                                m.RoleList,
                                m.MessageContent,
                                MessageTime = m.MessageTime.ToOffset(TimeSpan.FromHours(8))
                            })
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
                    .Select(m => new
                    {
                        m.Id,
                        m.UserName,
                        m.RoleList,
                        m.MessageContent,
                        MessageTime = m.MessageTime.ToOffset(TimeSpan.FromHours(8))
                    })
                    .ToList();

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
        /// UnauthorizedRole = 角色驗證失敗 <br />
        /// </remarks>
        [HttpPost("AddMessage")]
        public async Task<IActionResult> AddMessage([FromBody] AddMessageInputModel data)
        {
            string funcFrom = "Controllers.ContactController.AddMessage";
            try
            {
                string message = "";
                if (ModelState.IsValid)
                {
                    // 驗證使用者名稱（防止前端偽造）
                    string expectedUserName = User.Identity.IsAuthenticated ? User.Identity.Name : "遊客";
                    if (data.Role != expectedUserName)
                    {
                        message = "UnauthorizedRole";
                        _loggerForNormal.Write($"Expected UserName: {expectedUserName}, Received: {data.Role}", funcFrom);
                        return Ok(new { state = "Normal", message = message, result = "角色驗證失敗" });
                    }

                    // 取得使用者角色清單
                    var roleList = new List<string>();
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await _userManager.FindByNameAsync(expectedUserName);
                        if (user != null)
                        {
                            roleList = (await _userManager.GetRolesAsync(user)).ToList();
                        }
                    }

                    // 創建輸出模型
                    var outputData = new Message
                    {
                        UserName = expectedUserName,
                        RoleList = roleList,
                        MessageContent = data.MessageContent,
                        MessageTime = DateTimeOffset.UtcNow
                    };
                    // 資料寫入資料庫並儲存
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
                    return Ok(new { state = "Normal", message = message, result = "輸入資料結構錯誤" });
                }
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }


        /// <summary>
        /// 獲取當前使用者資訊
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// </remarks>
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            string funcFrom = "Controllers.ContactController.GetCurrentUser";
            try
            {
                // 取得使用者名稱
                string userName = User.Identity.IsAuthenticated ? User.Identity.Name : "遊客";
                // 取得使用者角色清單
                var userRoleList = new List<string>();
                if (User.Identity.IsAuthenticated)
                {
                    var user = await _userManager.FindByNameAsync(userName);
                    if (user != null)
                    {
                        userRoleList = (await _userManager.GetRolesAsync(user)).ToList();
                    }
                }

                string message = "Success";
                _loggerForNormal.Write(message, funcFrom);
                return Ok(new { state = "Normal", message = message, results = userName, userRoleList });
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }


        /// <summary>
        /// 刪除留言
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// DataStructureFail = 輸入資料結構錯誤 <br />
        /// UnauthorizedRole = 無權限 <br />
        /// NotFound = 留言不存在 <br />
        /// </remarks>
        [HttpDelete("DeleteMessage")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> DeleteMessage([FromBody] int Id)
        {
            string funcFrom = "Controllers.ContactController.DeleteMessage";
            try
            {
                string message = "";
                if (ModelState.IsValid)
                {
                    // 檢查使用者角色
                    if (!User.Identity.IsAuthenticated)
                    {
                        message = "UnauthorizedRole";
                        _loggerForNormal.Write("User not authenticated", funcFrom);
                        return Ok(new { state = "Normal", message = message, result = "無權限" });
                    }

                    var user = await _userManager.FindByNameAsync(User.Identity.Name);
                    if (user == null)
                    {
                        message = "UnauthorizedRole";
                        _loggerForNormal.Write("User not found", funcFrom);
                        return Ok(new { state = "Normal", message = message, result = "無權限" });
                    }

                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Contains("Admin") && !roles.Contains("SuperAdmin"))
                    {
                        message = "UnauthorizedRole";
                        _loggerForNormal.Write($"User {User.Identity.Name} does not have required role (Admin or SuperAdmin)", funcFrom);
                        return Ok(new { state = "Normal", message = message, result = "無權限" });
                    }

                    // 查找留言
                    var messageToDelete = await _modelsContext.Messages.FindAsync(Id);
                    if (messageToDelete == null)
                    {
                        message = "NotFound";
                        _loggerForNormal.Write($"Message with Id {Id} not found", funcFrom);
                        return Ok(new { state = "Normal", message = message, result = "留言不存在" });
                    }

                    // 刪除留言
                    _modelsContext.Messages.Remove(messageToDelete);
                    await _modelsContext.SaveChangesAsync();

                    message = "Success";
                    _loggerForNormal.Write(message, funcFrom);
                    return Ok(new { state = "Normal", message = message, results = "刪除成功" });
                }
                else
                {
                    message = "DataStructureFail";
                    _loggerForNormal.Write(message, funcFrom);
                    return Ok(new { state = "Normal", message = message, result = "輸入資料結構錯誤" });
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
            public string Role { get; set; } // 使用者名稱（例如 so2222g@gmail.com 或 遊客）
            public string MessageContent { get; set; } // 留言內容
        }
    }
}