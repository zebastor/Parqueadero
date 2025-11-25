using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers;

[Authorize(Roles = "SuperUsuario")]
public class EmpresasController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Crear()
    {
        return View();
    }

    public IActionResult Editar(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    public IActionResult Eliminar(int id)
    {
        ViewBag.Id = id;
        return View();
    }
} 