#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KoaLaDessertWeb.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<IdentityUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }


        /// <summary>
        /// 顯示登出頁面（GET 請求）
        /// </summary>
        public void OnGet()
        {
            // 僅顯示登出確認頁面，無需額外邏輯
        }


        /// <summary>
        /// 登出後導向
        /// </summary>
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // 過濾錯誤的 returnUrl
            if (returnUrl != null && returnUrl.Contains("/Identity/Account/Logout") && Url.IsLocalUrl(returnUrl))
            {
                // _logger.LogInformation($"Redirecting to provided returnUrl: {returnUrl}");
                return LocalRedirect(returnUrl);
            }
            // 預設導向首頁
            // _logger.LogInformation("Redirecting to home page.");
            return LocalRedirect("~/");
        }

    }
}
