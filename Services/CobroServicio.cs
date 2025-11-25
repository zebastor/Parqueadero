using Parqueadero.Factories.FacturaFactory;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Parqueadero.Strategies.Descuento;
using Parqueadero.Strategies.Tarifa;
// Asegúrate de que esta referencia exista, la necesitaremos
using Microsoft.EntityFrameworkCore;

namespace Parqueadero.Services;

public class CobroServicio : ICobroServicio
{
    private readonly IReservaServicio _reservaServicio;
    private readonly IDescuentoServicio _descuentoServicio;
    private readonly ITarifaServicio _tarifaServicio;
    // --- INICIO DE MODIFICACIÓN ---
    private readonly IParqueaderoServicio _parqueaderoServicio; // <-- Línea añadida
    // --- FIN DE MODIFICACIÓN ---
    private readonly IEnumerable<IDescuentoStrategy> _descuentoStrategy;
    private readonly IEnumerable<ITarifaStrategy> _tarifaStrategy;
    private readonly ICobroRepositorio _cobroRepositorio;

    // Constructor modificado
    public CobroServicio(
        IReservaServicio reservaServicio,
        IDescuentoServicio descuentoServicio,
        ITarifaServicio tarifaServicio,
        IParqueaderoServicio parqueaderoServicio, // <-- Línea añadida
        IEnumerable<IDescuentoStrategy> descuentoStrategy,
        IEnumerable<ITarifaStrategy> tarifaStrategy,
        ICobroRepositorio cobroRepositorio)
    {
        _reservaServicio = reservaServicio;
        _descuentoServicio = descuentoServicio;
        _tarifaServicio = tarifaServicio;
        _parqueaderoServicio = parqueaderoServicio; // <-- Línea añadida
        _descuentoStrategy = descuentoStrategy;
        _tarifaStrategy = tarifaStrategy;
        _cobroRepositorio = cobroRepositorio;
    }

    public async Task<Cobro> GenerarCobro(int idReserva, string? codigoDescuento)
    {
        var reserva = await _reservaServicio.ObtenerPorId(idReserva) ?? throw new ArgumentException("Reserva no encontrada");

        if (reserva.Zona?.Piso == null)
            throw new ArgumentException("La reserva no tiene una zona o piso válido. No se puede encontrar el parqueadero.");

        int parqueaderoId = reserva.Zona.Piso.ParqueaderoId;
        var tarifa = await _tarifaServicio.ObtenerPorParqueaderoYTipo(parqueaderoId, reserva.Vehiculo!.TipoVehiculo)
            ?? throw new ArgumentException("Tarifa no encontrada");

        Cobro cobro;

        var parqueadero = await _parqueaderoServicio.ObtenerPorId(tarifa.ParqueaderoId);
        if (parqueadero == null)
            throw new ArgumentException("Parqueadero asociado a la tarifa no encontrado");

        var empresaId = parqueadero.EmpresaId;

        var planEspecialUsuario = reserva.Usuario!.UsuarioEmpresas
            .FirstOrDefault(ue => ue.EmpresaId == empresaId && ue.PlanEspecialId != null)?
            .PlanEspecial;

        // --- INICIO DE CORRECCIÓN (Arreglo del DbContext) ---

        // 1. Calculamos la tarifa
        var tarifaStrategy = _tarifaStrategy.FirstOrDefault(ts => ts is TarifaPorHoraStrategy);

        // 2. Simulamos la hora de salida ANTES de llamar a la BD.
        // Esto nos permite calcular el costo sin necesidad de una llamada 'await' extra.
        reserva.HoraSalida = DateTime.UtcNow;

        var subtotal = tarifaStrategy!.Calcular(tarifa, reserva); // Usamos la reserva ya actualizada

        Descuento? descuentoAplicado = null;

        // 3. Aplicamos el Plan Especial (si existe)
        if (planEspecialUsuario is not null)
        {
            subtotal = subtotal - planEspecialUsuario.Costo;
            if (subtotal < 0) subtotal = 0;
        }
        // 4. Si no hay plan, aplicamos un código de descuento
        else if (codigoDescuento is not null)
        {
            var descuento = await _descuentoServicio.BuscarPorCodigo(codigoDescuento) ?? throw new ArgumentException("Descuento no encontrado");
            var descuentoStrategy = _descuentoStrategy.FirstOrDefault(ds => ds is CodigoDescuentoStrategy);

            if (descuentoStrategy is null) throw new ArgumentException("Strategy no encontrado");
            if (!descuentoStrategy.EsValido(descuento)) throw new ArgumentException("Descuento no valido");

            subtotal = descuentoStrategy!.Aplicar(descuento, subtotal);
            descuentoAplicado = descuento;
        }

        // 5. Creamos el cobro
        cobro = new Cobro
        {
            ReservaId = idReserva,
            TarifaId = tarifa.Id,
            DescuentoId = descuentoAplicado?.Id,
            Total = subtotal,
            FechaCobro = DateTime.UtcNow
        };

        // 6. Guardamos el cobro
        var cobroCreado = await _cobroRepositorio.Insertar(cobro);

        // 7. AHORA SÍ, finalizamos la reserva (lo cual también guarda la HoraSalida)
        await _reservaServicio.FinalizarReserva(idReserva);


        return cobroCreado;
    }
}