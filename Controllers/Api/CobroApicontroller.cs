using Microsoft.AspNetCore.Mvc;
using Parqueadero.Models;
using Parqueadero.Requests;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Controllers.Api;

[ApiController]
[Route("api/Cobros")]
public class CobroApicontroller : ControllerBase
{
    private readonly ICobroServicio _cobroServicio;
    private readonly IReciboServicio _reciboServicio;

    public CobroApicontroller(ICobroServicio cobroServicio, IReciboServicio reciboServicio)
    {
        _cobroServicio = cobroServicio;
        _reciboServicio = reciboServicio;
    }

    [HttpPost("GenerarCobro/{idReserva}")]
    public async Task<ActionResult<Recibo>> GenerarCobro([FromRoute] int idReserva, [FromBody] CobroRequest request)
    {
        try
        {
            var cobro = await _cobroServicio.GenerarCobro(idReserva, request.CodigoDescuento);

            var recibo = await _reciboServicio.Insertar(cobro, request.EnviarCorreo);

            return Ok(recibo);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}
