namespace ElevatorSim.Domain.Interfaces;

public interface IElevatorControl
{
    int Id { get; }
    int CurrentPassengerCount { get; }
    int MaximumPassengerCapacity { get; }
    Task MoveToFloorAsync(int targetFloor, CancellationToken cancellationToken);
    Task BoardPassengersAsync(int numberOfPassengers, CancellationToken cancellationToken);
    Task DisembarkAllPassengersAsync(CancellationToken cancellationToken);
}