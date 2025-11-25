using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers;

[Authorize(Roles = "Administrador,SuperUsuario")]
public class PlanesEspecialesController : Controller
{
    private readonly IPlanEspecialServicio _planEspecialServicio;

    public PlanesEspecialesController(IPlanEspecialServicio planEspecialServicio)
    {
        _planEspecialServicio = planEspecialServicio;
    }

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
