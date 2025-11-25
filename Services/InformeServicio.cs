using Parqueadero.Builder;
using Parqueadero.Models;
using Parqueadero.Repositories.Interfaces;
using Parqueadero.Services.Interfaces;
using Parqueadero.Strategies.Bill;

namespace Parqueadero.Services
{
    public class InformeServicio : IInformeServicio
    {
        private readonly IInformeRepositorio _informeRepositorio;
        private readonly IContentBuilder _contentBuilder;
        private readonly IBillStrategySelector _billStrategySelector;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public InformeServicio(
            IInformeRepositorio informeRepositorio,
            IContentBuilder contentBuilder,
            IBillStrategySelector billStrategySelector,
            IUsuarioRepositorio usuarioRepositorio)
        {
            _informeRepositorio = informeRepositorio;
            _contentBuilder = contentBuilder;
            _billStrategySelector = billStrategySelector;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<InformeResult> GenerarInformeAsync(InformeRequest request, int usuarioId)
        {
            // CAMBIO: Usar ObtenerPorId en lugar de ObtenerPorIdAsync
            var usuario = await _usuarioRepositorio.ObtenerPorId(usuarioId);
            if (usuario == null)
                throw new ArgumentException("Usuario no encontrado");

            if (request.FechaInicio > request.FechaFin)
                throw new ArgumentException("La fecha fin no puede ser anterior a la fecha inicio");

            string contenido = request.Tipo.ToLower() switch
            {
                "ingresos" => await GenerarInformeIngresosAsync(request, usuario),
                "ocupacion" => await GenerarInformeOcupacionAsync(request, usuario),
                "vehiculos" => await GenerarInformeVehiculosAsync(request, usuario),
                _ => throw new ArgumentException($"Tipo de informe no válido: {request.Tipo}")
            };

            var strategy = _billStrategySelector.GetStrategy(request.Formato);
            var archivoBytes = strategy.GenerarFactura(contenido);

            var resultado = new InformeResult
            {
                Contenido = contenido,
                Archivo = archivoBytes,
                ContentType = strategy.TipoContenido(),
                NombreArchivo = $"Informe_{request.Tipo}_{DateTime.Now:yyyyMMddHHmmss}.{strategy.ObtenerExtension()}"
            };

            // CAMBIO: Usar Insertar en lugar de AgregarAsync
            await GuardarInformeAsync(new Informe
            {
                Tipo = request.Tipo,
                Titulo = $"Informe de {ObtenerTituloPorTipo(request.Tipo)}",
                Descripcion = $"Informe generado para el período {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}",
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                Formato = request.Formato,
                UsuarioId = usuarioId,
                Datos = contenido,
                FechaGeneracion = DateTime.UtcNow // Ensure UTC
            });

            return resultado;
        }

        private string ObtenerTituloPorTipo(string tipo)
        {
            return tipo.ToLower() switch
            {
                "ingresos" => "Ingresos",
                "ocupacion" => "Ocupación",
                "vehiculos" => "Vehículos",
                _ => tipo
            };
        }

        private async Task<string> GenerarInformeIngresosAsync(InformeRequest request, Usuario usuario)
        {
            var cobros = await _informeRepositorio.ObtenerCobrosParaInformeAsync(request.FechaInicio, request.FechaFin, request.ParqueaderoId);
            var ingresosTotales = await _informeRepositorio.ObtenerIngresosTotalesAsync(request.FechaInicio, request.FechaFin, request.ParqueaderoId);

            var builder = _contentBuilder
                .AddTitle("INFORME DE INGRESOS - SISTEMA DE PARQUEADEROS")
                .AddParagraph("=".PadRight(50, '='))
                .AddParagraph($"Generado por: {usuario.Nombre}")
                .AddParagraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .AddParagraph($"Período analizado: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}")
                .AddParagraph("")
                .AddParagraph("RESUMEN EJECUTIVO:")
                .AddParagraph($"- Total de transacciones: {cobros.Count()}")
                .AddParagraph($"- Ingresos totales: {ingresosTotales:C}")
                .AddParagraph($"- Promedio por transacción: {(cobros.Any() ? cobros.Average(c => c.Total):0):C}")
                .AddParagraph("")
                .AddParagraph("DETALLE POR DÍA:")
                .AddParagraph("-".PadRight(40, '-'));

            var ingresosPorDia = cobros
                .GroupBy(c => c.FechaCobro.Date)
                .OrderBy(g => g.Key);

            foreach (var grupo in ingresosPorDia)
            {
                builder.AddParagraph($"{grupo.Key:dd/MM/yyyy}: {grupo.Count()} transacciones - {grupo.Sum(c => c.Total):C}");
            }

            builder.AddParagraph("")
                   .AddParagraph("RESUMEN FINAL:")
                   .AddParagraph("-".PadRight(40, '-'))
                   .AddParagraph($"INGRESOS TOTALES DEL PERÍODO: {ingresosTotales:C}");

            return builder.BuildContent();
        }

        private async Task<string> GenerarInformeOcupacionAsync(InformeRequest request, Usuario usuario)
{
    var reservas = await _informeRepositorio.ObtenerReservasParaInformeAsync(request.FechaInicio, request.FechaFin, request.ParqueaderoId);
    var totalVehiculos = await _informeRepositorio.ObtenerTotalVehiculosAsync(request.FechaInicio, request.FechaFin, request.ParqueaderoId);

    var builder = _contentBuilder
        .AddTitle("INFORME DE OCUPACIÓN - SISTEMA DE PARQUEADEROS")
        .AddParagraph("=".PadRight(50, '='))
        .AddParagraph($"Generado por: {usuario.Nombre}")
        .AddParagraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
        .AddParagraph($"Período analizado: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}")
        .AddParagraph("")
        .AddParagraph("ESTADÍSTICAS DE OCUPACIÓN:")
        .AddParagraph($"- Total de reservas: {reservas.Count()}")
        .AddParagraph($"- Vehículos únicos atendidos: {totalVehiculos}")
        .AddParagraph("")
        .AddParagraph("OCUPACIÓN DIARIA:")
        .AddParagraph("-".PadRight(40, '-'));

    // CAMBIO: Usar HoraEntrada en lugar de FechaInicio
    var ocupacionPorDia = reservas
        .GroupBy(r => r.HoraEntrada.Date)
        .OrderBy(g => g.Key);

    foreach (var grupo in ocupacionPorDia)
    {
        builder.AddParagraph($"{grupo.Key:dd/MM/yyyy}: {grupo.Count()} reservas");
    }

    return builder.BuildContent();
}

        private async Task<string> GenerarInformeVehiculosAsync(InformeRequest request, Usuario usuario)
        {
            var reservas = await _informeRepositorio.ObtenerReservasParaInformeAsync(request.FechaInicio, request.FechaFin, request.ParqueaderoId);

            var builder = _contentBuilder
                .AddTitle("INFORME DE VEHÍCULOS - SISTEMA DE PARQUEADEROS")
                .AddParagraph("=".PadRight(50, '='))
                .AddParagraph($"Generado por: {usuario.Nombre}")
                .AddParagraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .AddParagraph($"Período analizado: {request.FechaInicio:dd/MM/yyyy} - {request.FechaFin:dd/MM/yyyy}")
                .AddParagraph("")
                .AddParagraph("ESTADÍSTICAS GENERALES:")
                .AddParagraph($"- Total de vehículos únicos: {reservas.Select(r => r.VehiculoId).Distinct().Count()}")
                .AddParagraph($"- Total de reservas procesadas: {reservas.Count()}");

            // Estadísticas por tipo de vehículo
            var porTipo = reservas
                .Where(r => r.Vehiculo != null)
                .GroupBy(r => r.Vehiculo!.TipoVehiculo)
                .OrderByDescending(g => g.Count());

            builder.AddParagraph("")
                   .AddParagraph("DISTRIBUCIÓN POR TIPO DE VEHÍCULO:")
                   .AddParagraph("-".PadRight(40, '-'));

            foreach (var grupo in porTipo)
            {
                var porcentaje = (double)grupo.Count() / reservas.Count() * 100;
                builder.AddParagraph($"{grupo.Key}: {grupo.Count()} reservas ({porcentaje:F1}%)");
            }

            return builder.BuildContent();
        }

        public async Task<IEnumerable<Informe>> ObtenerHistorialInformesAsync(int usuarioId)
        {
            return await _informeRepositorio.ObtenerPorUsuarioAsync(usuarioId);
        }

        public async Task<Informe> GuardarInformeAsync(Informe informe)
        {
            return await _informeRepositorio.Insertar(informe);
        }

        public async Task<bool> EliminarInformeAsync(int id)
        {
            return await _informeRepositorio.Eliminar(id);
        }
    }
}