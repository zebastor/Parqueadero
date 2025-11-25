using Microsoft.AspNetCore.Mvc;

namespace Parqueadero.Controllers;

public class VehiculoController : Controller
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
