global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using KoaLaDessertWeb.Tools.DBContext;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using KoaLaDessertWeb.Tools.Identity;


var builder = WebApplication.CreateBuilder(args);

#region 資料庫連線設定
string server = ".\\SQL2022";
string database = "koaladessertweb";
bool trusted_Connection = true;
bool trustServerCertificate = true;
bool multipleActiveResultSets = true;
string userID = "sa";
string password = "1qaz@wsx";
bool integratedSecurity = false;

var dbcon = $"Server={server};" +
            $"Database={database};" +
            $"Trusted_Connection={trusted_Connection};" +
            $"TrustServerCertificate={trustServerCertificate};" +
            $"MultipleActiveResultSets={multipleActiveResultSets};" +
            $"User ID={userID};" +
            $"Password={password};" +
            $"Integrated Security={integratedSecurity};";

// 使用Sql Server
builder.Services.AddDbContext<SqlDbContext>(options =>
{
    // 使用上面的設定
    // options.UseSqlServer(dbcon);

    // 使用appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbcon"));

}, ServiceLifetime.Scoped);


// 此程式碼為指令自動產生預設的簡單配置，如果以使用自訂配置則需要註解或者刪除，避免重複註冊導致專案無法啟用
// builder.Services.AddDefaultIdentity<IdentityUser>(options => 
// options.SignIn.RequireConfirmedAccount = true)
// .AddEntityFrameworkStores<SqlDbContext>();

#endregion

#region 註冊服務: 探索和測試 API
// 註冊服務: 探索和測試 API (三選一) AddControllers()、AddControllersWithViews()、AddMvc()
// 這行註冊 MVC 控制器服務，讓你的應用程式支援 Web API 控制器。
builder.Services.AddMvc()
    .AddJsonOptions(options =>
    {
        //原本是 JsonNamingPolicy.CamelCase，強制頭文字轉小寫，我偏好維持原樣，設為null
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //允許基本拉丁英文及中日韓文字維持原字元
        options.JsonSerializerOptions.Encoder =
            JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
        // 添加循環參考處理
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
#endregion

#region Swagger服務
// 確保有註冊服務: 探索和測試 API
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("HomeManagement", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HomeManagement",
        Version = "v1",
        Description = "主頁管理API",
    });

    options.SwaggerDoc("SystemManagement", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SystemManagement",
        Version = "v1",
        Description = "系統管理API",
    });
    // 加入xml檔案到swagger
    options.MapType<DateTime>(() => new OpenApiSchema { Type = "DateTime", Format = "date-time", Example = new OpenApiDateTime(DateTimeOffset.Now) });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});
#endregion

#region Identity 角色認證服務
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true; // 是否要求密碼包含數字。
    options.Password.RequiredLength = 6; // 密碼的最小長度。
    options.Password.RequireNonAlphanumeric = false; // 是否要求非字母數字字符（例如 @、#、!）。
    options.Password.RequireUppercase = false; // 是否要求大寫字母
})
    .AddEntityFrameworkStores<SqlDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // 明確添加 Identity UI 服務
// 使用 Identity UI
builder.Services.AddRazorPages();
// 使用 授權功能
builder.Services.AddAuthorization();

// 註冊 Identity
builder.Services.AddScoped<Identity>();

#endregion


var app = builder.Build();


#region 在非開發模式下執行
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // 強制執行 HTTPS
    app.UseHsts();
}
#endregion

#region 專案啟用模式為開發模式時啟用
if (app.Environment.IsDevelopment())
{
    // 一次性初始化任務
    using (var scope = app.Services.CreateScope())
    {
        // 測試資料庫連線
        var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
        var connectionString = builder.Configuration.GetConnectionString("dbcon");
        dbContext.TestDatabaseConnection(connectionString);

        // 初始化 Identity 角色和使用者
        var initializer = scope.ServiceProvider.GetRequiredService<Identity>();
        await initializer.InitializeAsync();
    }

    // 啟用 Swagger 和 Swagger UI
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/HomeManagement/swagger.json", "主頁管理API v1");
        options.SwaggerEndpoint("/swagger/SystemManagement/swagger.json", "使用者管理API v1");

        // 設定 Swagger UI 的路由前綴
        options.RoutePrefix = "swagger";
        // 設定 Swagger UI 的 API 文件展開方式 {None：完全折疊} {List：展開 API 類別名稱} {Full：全部展開}
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
    // 啟用 CORS 跨源共用
    app.UseCors();
}
#endregion



app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.UseRouting();


// 啟用靜態文件服務，讓應用程式從 wwwroot 資料夾提供靜態資源（如圖片、CSS、JS）。
app.UseStaticFiles();



// 重新定向路由
app.MapGet("/", context =>
{
    context.Response.Redirect("/KoaLaDessertWeb/Home/Index");
    return Task.CompletedTask;
});
// 註冊Web服務 API 路由
app.MapControllers();
// 註冊Web服務: 映射到 MVC 控制器和動作
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}");
// 註冊Web服務 Area 路由
app.MapAreaControllerRoute(
    name: "SuperAdminArea",
    areaName: "SuperAdmin",
    pattern: "SuperAdmin/{controller}/{action}/{id?}");
app.MapAreaControllerRoute(
    name: "AdminArea",
    areaName: "Admin",
    pattern: "Admin/{controller}/{action}/{id?}");
app.MapAreaControllerRoute(
    name: "UserArea",
    areaName: "User",
    pattern: "User/{controller}/{action}/{id?}");
app.MapAreaControllerRoute(
    name: "GeneralArea",
    areaName: "General",
    pattern: "General/{controller}/{action}/{id?}");

// 使用 Identity UI
app.MapRazorPages();




app.Run();
