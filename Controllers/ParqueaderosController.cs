using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers;

[Authorize(Roles = "SuperUsuario,Administrador")]
public class ParqueaderosController : Controller
{
    private readonly IConfiguration _configuration;

    public ParqueaderosController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Crear()
    {
        ViewBag.MapsToken = _configuration["APITokens:maps"];
        return View();
    }

    public IActionResult Editar(int id)
    {
        ViewBag.Id = id;
        ViewBag.MapsToken = _configuration["APITokens:maps"];
        return View();
    }

    public IActionResult Eliminar(int id)
    {
        ViewBag.Id = id;
        return View();
    }
} 