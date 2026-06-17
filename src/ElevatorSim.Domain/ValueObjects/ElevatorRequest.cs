namespace ElevatorSim.Domain.ValueObjects;

public sealed record ElevatorRequest(
    int RequestedFloor,
    int PassengerCount,
    DateTimeOffset RequestAt
)
{
    public static ElevatorRequest Create(int requestedFloor, int passengerCount) =>
        new(requestedFloor, passengerCount, DateTimeOffset.UtcNow);
}