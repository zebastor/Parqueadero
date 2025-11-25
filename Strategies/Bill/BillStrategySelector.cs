using Microsoft.Extensions.DependencyInjection;

namespace Parqueadero.Strategies.Bill;

public class BillStrategySelector : IBillStrategySelector
{
    private readonly IServiceProvider _serviceProvider;

    public BillStrategySelector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBillStrategy GetStrategy(string tipo)
    {
        return tipo.ToLower() switch
        {
            "html" => _serviceProvider.GetRequiredService<HTMLBillStrategy>(),
            "pdf" => _serviceProvider.GetRequiredService<PDFBillStrategy>(),
            _ => throw new ArgumentException("Tipo de estrategia no válido")
        };
    }
}