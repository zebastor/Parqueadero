using Microsoft.EntityFrameworkCore;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class ParqueaderoServicio : GenericoServicio<Models.Parqueadero>, IParqueaderoServicio
{
    private readonly IPisoServicio _pisosServicio;
    private readonly IZonaServicio _zonaServicio; // <-- 1. Nueva dependencia

    // 2. Actualizamos el constructor
    public ParqueaderoServicio(
        IParqueaderoRepositorio parqueaderoRepository,
        IPisoServicio pisosServicio,
        IZonaServicio zonaServicio) : base(parqueaderoRepository)
    {
        _pisosServicio = pisosServicio;
        _zonaServicio = zonaServicio;
    }

    public async Task<IEnumerable<Parqueadero.Models.Parqueadero>> ObtenerTodoPorEmpresa(int empresaId)
        => await ((IParqueaderoRepositorio)_repositorio).ObtenerTodoPorEmpresa(empresaId).ToListAsync();

    public override async Task<Parqueadero.Models.Parqueadero> Insertar(Parqueadero.Models.Parqueadero parqueadero)
    {
        // A. Insertamos el Parqueadero
        var nuevoParqueadero = await _repositorio.Insertar(parqueadero);

        // B. Obtenemos cantidades (validando mínimos)
        int numPisos = parqueadero.CantidadPisos > 0 ? parqueadero.CantidadPisos : 1;
        int numZonas = parqueadero.CantidadZonas > 0 ? parqueadero.CantidadZonas : 10;

        // C. Bucle de Pisos
        for (int i = 1; i <= numPisos; i++)
        {
            var nuevoPiso = new Piso
            {
                ParqueaderoId = nuevoParqueadero.Id,
                Numero = i
            };

            // Guardamos el piso para obtener su ID
            var pisoCreado = await _pisosServicio.Insertar(nuevoPiso);

            // D. Bucle de Zonas (dentro del piso)
            for (int z = 1; z <= numZonas; z++)
            {
                var nuevaZona = new Zona
                {
                    PisoId = pisoCreado.Id,
                    Nombre = $"Zona {z}",
                    Codigo = $"{i}-{z:00}", // Genera códigos tipo 1-01, 1-02, 2-01...
                    Estado = EstadoPlaza.Libre,
                    TipoVehiculo = (TipoVehiculo)1 // Default: Auto (1). Ajusta según tu Enum si es necesario.
                };
                await _zonaServicio.Insertar(nuevaZona);
            }
        }

        return nuevoParqueadero;
    }
}