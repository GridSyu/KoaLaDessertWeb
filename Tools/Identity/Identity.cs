using KoaLaDessertWeb.Tools.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace KoaLaDessertWeb.Tools.Identity
{
    public class Identity
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public Identity(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Identity 初始化配置
        /// </summary>
        public async Task InitializeAsync()
        {
            // 讀取 [appsettings] 檔案內的 [IdentitySettings] 標籤配置
            var settings = _configuration.GetSection("IdentitySettings").Get<IdentitySettings>();
            if (settings == null)
            {
                throw new InvalidOperationException("IdentitySettings 未在 appsettings.json 中正確配置。");
            }

            // 角色和預設管理員的建立
            await EnsureRolesAsync(settings.Roles);
            await EnsureAdminUserAsync(settings.AdminUser);

        }
        /// <summary>
        /// 角色異步處理
        /// </summary>
        private async Task EnsureRolesAsync(string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"無法創建角色 {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        /// <summary>
        /// 預設管理員的建立方法
        /// </summary>
        private async Task EnsureAdminUserAsync(AdminUserSettings adminSettings)
        {
            var adminUser = await _userManager.FindByEmailAsync(adminSettings.Email);
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminSettings.Email,
                    Email = adminSettings.Email,
                    EmailConfirmed = true // 假設不需要郵件驗證
                };
                var result = await _userManager.CreateAsync(user, adminSettings.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, adminSettings.Role);
                }
                else
                {
                    throw new Exception($"無法創建管理員使用者: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}