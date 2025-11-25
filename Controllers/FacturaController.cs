using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;

namespace Parqueadero.Controllers;

[Authorize]
public class FacturaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}
