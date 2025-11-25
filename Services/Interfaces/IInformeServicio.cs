using Parqueadero.Models;

namespace Parqueadero.Services.Interfaces
{
    public interface IInformeServicio
    {
        Task<InformeResult> GenerarInformeAsync(InformeRequest request, int usuarioId);
        Task<IEnumerable<Informe>> ObtenerHistorialInformesAsync(int usuarioId);
        Task<Informe> GuardarInformeAsync(Informe informe);
        Task<bool> EliminarInformeAsync(int id);
    }
}