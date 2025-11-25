using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Parqueadero.Services.Interfaces;
using Parqueadero.Models;

namespace Parqueadero.Controllers
{
    [Authorize]
    public class InformesController : Controller
    {
        private readonly IInformeServicio _informeServicio;
        private readonly IUsuarioServicio _usuarioServicio;

        public InformesController(IInformeServicio informeServicio, IUsuarioServicio usuarioServicio)
        {
            _informeServicio = informeServicio;
            _usuarioServicio = usuarioServicio;
        }

        public async Task<IActionResult> Index()
        {
            var correo = User.FindFirst("Correo")?.Value;
            if (string.IsNullOrEmpty(correo)) return RedirectToAction("Login", "Autenticacion");

            var usuario = await _usuarioServicio.ObtenerUsuarioPorCorreoAsync(correo);
            if (usuario == null) return RedirectToAction("Login", "Autenticacion");

            var historial = await _informeServicio.ObtenerHistorialInformesAsync(usuario.Id);
            return View(historial);
        }

        public IActionResult Generar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generar(string Tipo, DateTime FechaInicio, DateTime FechaFin, string Formato, int? ParqueaderoId)
        {
            if (string.IsNullOrEmpty(Tipo))
            {
                ModelState.AddModelError("Tipo", "El tipo de informe es requerido");
            }

            if (FechaInicio > FechaFin)
            {
                ModelState.AddModelError("FechaFin", "La fecha fin no puede ser anterior a la fecha inicio");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var correo = User.FindFirst("Correo")?.Value;
                if (string.IsNullOrEmpty(correo)) return RedirectToAction("Login", "Autenticacion");

                var usuario = await _usuarioServicio.ObtenerUsuarioPorCorreoAsync(correo);
                if (usuario == null) return RedirectToAction("Login", "Autenticacion");

                var request = new InformeRequest
                {
                    Tipo = Tipo,
                    FechaInicio = DateTime.SpecifyKind(FechaInicio, DateTimeKind.Utc),
                    FechaFin = DateTime.SpecifyKind(FechaFin, DateTimeKind.Utc),
                    Formato = Formato,
                    ParqueaderoId = ParqueaderoId
                };

                var resultado = await _informeServicio.GenerarInformeAsync(request, usuario.Id);

                return File(resultado.Archivo, resultado.ContentType, resultado.NombreArchivo);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al generar el informe: {ex.Message}");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var resultado = await _informeServicio.EliminarInformeAsync(id);
                if (resultado)
                {
                    TempData["Success"] = "Informe eliminado correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el informe. Es posible que no exista o no tenga permisos.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar el informe: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}