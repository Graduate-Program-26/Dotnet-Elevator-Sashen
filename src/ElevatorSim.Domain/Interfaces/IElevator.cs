using ElevatorSim.Domain.Enums;

namespace ElevatorSim.Domain.Interfaces;

public interface IElevator
{
    int Id { get; }
    int CurrentFloor { get; }
    ElevatorDirection Direction { get; }
    ElevatorStatus Status { get; }
    int CurrentPassengerCount { get; }
    int MaximumPassengerCapacity { get; }
    bool IsAvailable { get; }
    ElevatorType Type { get; }
}