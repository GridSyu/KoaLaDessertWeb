using KoaLaDessertWeb.Tools.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// 後台控制
/// </summary>
[ApiExplorerSettings(GroupName = "SuperAdminManagement")]
[Route("KoaLaDessertWeb/Areas/SuperAdmin/[controller]")]
[Area("SuperAdmin")]
[Authorize(Roles = "SuperAdmin")]
public class DashboardController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SqlDbContext _modelsContext;

    public DashboardController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager , SqlDbContext modelsContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _modelsContext = modelsContext;
    }


    /// <summary>
    /// 獲取後台管理首頁資料
    /// </summary>
    /// <returns>後台功能清單</returns>
    [HttpGet("index")]
    public IActionResult Index()
    {
        var dashboardData = new
        {
            Message = "歡迎來到 SuperAdmin 後台管理！",
            AvailableFunctions = new[] { "ManageUsers" }
        };
        return Ok(dashboardData);
    }

    /// <summary>
    /// 獲取所有使用者及其角色
    /// </summary>
    /// <returns>使用者列表與對應角色</returns>
    [HttpGet("manage-users")]
    public async Task<IActionResult> ManageUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles
            });
        }

        return Ok(userRoles);
    }

    /// <summary>
    /// 獲取指定使用者的角色編輯資料
    /// </summary>
    /// <param name="id">使用者 ID</param>
    /// <returns>使用者當前角色與所有可用角色</returns>
    [HttpGet("edit-user-roles/{id}")]
    public async Task<IActionResult> GetEditUserRoles(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = "使用者不存在" });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

        var response = new
        {
            UserId = user.Id,
            Email = user.Email,
            CurrentRoles = userRoles,
            AllRoles = allRoles
        };

        return Ok(response);
    }

    /// <summary>
    /// 更新指定使用者的角色
    /// </summary>
    /// <param name="id">使用者 ID</param>
    /// <param name="roles">新的角色列表</param>
    /// <returns>更新結果</returns>
    [HttpPost("edit-user-roles/{id}")]
    public async Task<IActionResult> PostEditUserRoles(string id, [FromBody] List<string> roles)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = "使用者不存在" });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = roles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(roles).ToList();

        var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
        if (!addResult.Succeeded)
        {
            return BadRequest(new { Message = "新增角色失敗", Errors = addResult.Errors });
        }

        var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (!removeResult.Succeeded)
        {
            return BadRequest(new { Message = "移除角色失敗", Errors = removeResult.Errors });
        }

        return Ok(new { Message = "角色更新成功" });
    }

}
