namespace ElevatorSim.Domain.Exceptions;
public sealed class InvalidFloorException : ElevatorSimException
{
    public int RequestedFloor { get; }
    public int MinFloor { get; }
    public int MaxFloor { get; }
    public InvalidFloorException(int requestedFloor, int minFloor, int maxFloor)
        : base($"Floor {requestedFloor} is not valid. This building has {minFloor} to {maxFloor}.")
    {
        RequestedFloor = requestedFloor;
        MinFloor = minFloor;
        MaxFloor = maxFloor;
    }

}