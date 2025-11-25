using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Parqueadero.Controllers;

[Authorize(Roles = "Administrador,SuperUsuario")]
public class DescuentosController : Controller
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
