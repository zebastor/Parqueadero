namespace Parqueadero.Strategies.Bill;

public interface IBillStrategySelector
{
    IBillStrategy GetStrategy(string tipo);
}
