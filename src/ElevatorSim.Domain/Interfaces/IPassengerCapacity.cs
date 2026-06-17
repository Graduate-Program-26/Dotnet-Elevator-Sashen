namespace ElevatorSim.Domain.Interfaces;
public interface IPassengerCapacity
{
    int AvailableCapacity { get; }
    bool IsAtCapacity { get; }
}