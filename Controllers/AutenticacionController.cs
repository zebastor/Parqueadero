using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers;

public class AutenticacionController : Controller
{
    private readonly IAutenticacionServicio _autenticacionServicio;

    public AutenticacionController(IAutenticacionServicio autenticacionServicio)
    {
        _autenticacionServicio = autenticacionServicio;
    }
    
    public IActionResult Login()
    {
        return View();
    }

   
[HttpPost]
public async Task<IActionResult> Login(string email, string password)
{
    var user = await _autenticacionServicio.AutenticarUsuario(email, password);
    if (user != null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Nombre),
            new("Correo", user.Correo),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Rol.ToString()),
        };

        if (user.EmpresaId != null)
        {
            claims.Add(new Claim("EmpresaId", user.EmpresaId.ToString()!));
            
            // Asegúrate de que esta línea esté presente
            if (user.Empresa != null)
            {
                claims.Add(new Claim("EmpresaNombre", user.Empresa.Nombre));
            }
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Dashboard");
    }

    ViewBag.Error = "Usuario o contraseña incorrectos";
    return View();
}


    public IActionResult Register()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}