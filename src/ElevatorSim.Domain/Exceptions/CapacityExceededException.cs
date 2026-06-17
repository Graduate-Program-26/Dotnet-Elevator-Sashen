namespace ElevatorSim.Domain.Exceptions;
public sealed class CapacityExceededException : ElevatorSimException
{
    public int ElevatorId { get; }
    public int MaximumCapacity { get; }
    public int AttemptedTotal { get; }

    public CapacityExceededException(int elevatorId, int maximumCapacity, int attemptTotal)
        : base($"Elevator {elevatorId} cannot carry {attemptTotal} passengers. Maximum capacity is {maximumCapacity}.")
    {
        ElevatorId = elevatorId;
        MaximumCapacity = maximumCapacity;
        AttemptedTotal = attemptTotal;
    }
}