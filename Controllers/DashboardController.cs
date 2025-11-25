using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Parqueadero.Models;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IAutenticacionServicio _autenticacionServicio;
    private readonly IConfiguration _configuration;
    public DashboardController(IAutenticacionServicio autenticacionServicio, IConfiguration configuration)
    {
        _autenticacionServicio = autenticacionServicio;
        _configuration = configuration; 
    }

    public async Task<IActionResult> Index()
    {
        var email = User.FindFirst("Correo")?.Value;
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login", "Autenticacion");

        var usuario = await _autenticacionServicio.ObtenerUsuarioPorEmail(email);

        if (usuario == null)
            return RedirectToAction("Login", "Autenticacion");

        return usuario.Rol switch
        {
            Rol.SuperUsuario => RedirectToAction("SuperAdmin"),
            Rol.Administrador => RedirectToAction("Admin"),
            Rol.Trabajador => RedirectToAction("Trabajador"),
            Rol.Cliente => RedirectToAction("Cliente"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [Authorize(Roles = "SuperUsuario")]
    public IActionResult SuperAdmin()
    {
        return View();
    }

    [Authorize(Roles = "Administrador")]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Roles = "Trabajador")]
    public IActionResult Trabajador()
    {
        return View();
    }
    [Authorize(Roles = "Cliente")]
    public IActionResult Cliente()
    {
        return View();
    }
    [Authorize(Roles = "Cliente")]
    public IActionResult VerMapa()
    {
        // Pasamos la clave de API (leída desde appsettings.json) a la vista
        ViewBag.MapsApiKey = _configuration.GetValue<string>("APITokens:maps");
        return View();
    }
} 