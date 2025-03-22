
using KoaLaDessertWeb.Models;
using KoaLaDessertWeb.Tools.DBContext;
using System.ComponentModel.DataAnnotations;
using KoaLaDessertWeb.Tools.Logger;
using KoaLaDessertWeb.Tools.Logger.LogType;

namespace KoaLaDessertWeb.Controllers
{
    /// <summary>
    /// 商品目錄
    /// </summary>
    [ApiExplorerSettings(GroupName = "HomeManagement")]
    [Route("KoaLaDessertWeb/[controller]")]
    public class ProductsController : Controller
    {
        private Logger _loggerForNormal = new Logger(new WebNormal(), "Log"); // 一般紀錄
        private Logger _loggerForError = new Logger(new WebError(), "Log"); // 錯誤紀錄
        private readonly SqlDbContext _modelsContext;

        public ProductsController(SqlDbContext modelsContext)
        {
            _modelsContext = modelsContext;
        }

        /// <summary>
        /// 提供商品目錄頁面
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// </remarks>
        [HttpGet("Index")]
        public IActionResult Index()
        {
            string htmlPath = "~/Views/Home/Products.cshtml";
            return View(htmlPath);
        }


        /// <summary>
        /// 取得商品清單，可根據標籤篩選
        /// </summary>
        /// <param name="tag">篩選用的標籤名稱，預設為 null 表示顯示所有商品</param>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// </remarks>
        [HttpGet("GetProducts")]
        public IActionResult GetProducts(string? tag)
        {
            string funcFrom = "Controllers.ProductsController.GetProducts";
            try
            {
                string message = "";
                // 查詢商品並預載入標籤資料
                var products = _modelsContext.Products
                    .Include(p => p.ProductTags)
                    .ThenInclude(pt => pt.Tag)
                    .AsQueryable();

                // 如果有指定 tag 且不是 "所有商品"，進行篩選
                if (!string.IsNullOrEmpty(tag) && tag != "所有商品")
                {
                    products = products.Where(p => p.ProductTags.Any(pt => pt.Tag.Name == tag));
                }

                var productList = products.ToList();
                message = "Success";
                _loggerForNormal.Write($"Retrieved {productList.Count} products", funcFrom);
                return Ok(new { state = "Normal", message = message, results = productList });
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }

        /// <summary>
        /// 取得所有標籤清單
        /// </summary>
        /// <remarks>
        /// Message: <br />
        /// Success = 成功 <br />
        /// </remarks>
        [HttpGet("GetTags")]
        public IActionResult GetTags()
        {
            string funcFrom = "Controllers.ProductsController.GetTags";
            try
            {
                string message = "";
                var tags = _modelsContext.Tags.ToList();
                message = "Success";
                _loggerForNormal.Write(message, funcFrom);
                return Ok(new { state = "Normal", message = message, results = tags });
            }
            catch (Exception ex)
            {
                _loggerForError.Write(ex.Message, funcFrom);
                return Ok(new { state = "Error", message = ex.Message, results = new { } });
            }
        }

    }
}
