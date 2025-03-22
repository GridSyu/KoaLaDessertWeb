using KoaLaDessertWeb.Tools.DBContext;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;

namespace KoaLaDessertWeb.Controllers
{
    /// <summary>
    /// 品牌介紹
    /// </summary>
    [ApiExplorerSettings(GroupName = "HomeManagement")]
    [Route("KoaLaDessertWeb/[controller]")]
    public class BrandController : Controller
    {
        private Logger _loggerForNormal = new Logger(new WebNormal(), "Log"); // 一般紀錄
        private Logger _loggerForError = new Logger(new WebError(), "Log"); // 錯誤紀錄

        private readonly SqlDbContext _modelsContext;

        public BrandController(SqlDbContext modelsContext)
        {
            _modelsContext = modelsContext;
        }

        /// <summary>
        /// 提供品牌介紹頁面
        /// </summary>
        [HttpGet("Index")]
        public IActionResult Index()
        {
            string htmlPath = "~/Views/Home/Brand.cshtml";
            return View(htmlPath);
        }
    }
}