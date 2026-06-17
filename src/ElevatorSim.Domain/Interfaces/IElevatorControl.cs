namespace ElevatorSim.Domain.Interfaces;

public interface IElevatorControl
{
    Task MoveToFloorAsync(int targetFloor, CancellationToken cancellationToken);
    Task BoardPassengersAsync(int numberOfPassengers, CancellationToken cancellationToken);
    Task DisembarkAllPassengersAsync(CancellationToken cancellationToken);
}