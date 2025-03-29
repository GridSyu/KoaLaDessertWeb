
*** 以下是在使用 Identity 時，需要再appsettings內設定的格式： ***

"IdentitySettings": {
    "Roles": [
      "SuperAdmin",
      "Admin",
      "User"
    ],
    "AdminUser": {
      "Email": "Email String",
      "Password": "string",
      "Role": "SuperAdmin"
    }
  },
*<----------------------------------------------->*
小節：


*<----------------------------------------------->*

*** Authorization功能使用方式如下： ***
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Area("SuperAdmin")]
[Authorize(Roles = "SuperAdmin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

[Area("User")]
[Authorize(Roles = "User")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

[Area("General")]
[Authorize] // 不指定角色，所有登入使用者皆可存取
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}