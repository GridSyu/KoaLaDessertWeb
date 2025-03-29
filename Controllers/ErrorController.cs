using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    [AllowAnonymous]
    public IActionResult HandleError(int? statusCode = null)
    {
        if (statusCode == 403)
        {
            return View("AccessDenied");
        }
        return View("Error");
    }
}