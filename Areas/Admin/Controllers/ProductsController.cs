using KoaLaDessertWeb.Models;
using KoaLaDessertWeb.Tools.DBContext;
using System.ComponentModel.DataAnnotations;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;
using Microsoft.AspNetCore.Authorization;
namespace KoaLaDessertWeb.Areas.Admin.Controllers;

/// <summary>
/// 後台_商品目錄控制器
/// </summary>
[Authorize(Roles = "Admin")]
[Area("Admin")]
[ApiExplorerSettings(GroupName = "HomeManagement")]
[Route("KoaLaDessertWeb/[area]/[controller]")]
public class ProductsController : Controller
{
    private Logger _loggerForNormal = new Logger(new WebNormal(), "Log"); // 一般紀錄
    private Logger _loggerForError = new Logger(new WebError(), "Log"); // 錯誤紀錄
    private readonly SqlDbContext _modelsContext;

    public ProductsController(SqlDbContext modelsContext)
    {
        _modelsContext = modelsContext;
    }

    [HttpGet("Index")]
    public IActionResult Index()
    {
        var products = _modelsContext.Products
        .Include(p => p.ProductTags)
        .ThenInclude(p => p.Tag)
        .ToList();
        return View(products);
    }



}