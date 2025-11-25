using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Parqueadero.Controllers;

// [Authorize] // <-- YA NO EST� EL PERMISO A NIVEL DE CLASE
public class ReservaController : Controller
{
    private readonly IConfiguration _configuration;

    public ReservaController(IConfiguration configuration) : base()
    {
        _configuration = configuration;
    }

    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")] 
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")] 
    public IActionResult Crear()
    {
        ViewBag.PlaterecognizerToken = _configuration["APITokens:platerecognizer"];
        return View();
    }

    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador,Cliente")] 
    public IActionResult Editar(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")] 
    public IActionResult Eliminar(int id)
    {
        ViewBag.Id = id;
        return View();
    }

    [Authorize(Roles = "Cliente")] 
    public IActionResult ClienteCrear()
    {
        ViewBag.ClienteId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return View();
    }

    [Authorize(Roles = "Cliente")]
    public IActionResult MisReservas()
    {
        return View();
    }
    
    [Authorize(Roles = "Administrador,SuperUsuario,Trabajador")] 
      public IActionResult IngresarClientes()
    {
        return View();
    }
}