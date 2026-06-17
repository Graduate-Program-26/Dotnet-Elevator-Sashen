namespace ElevatorSim.Domain.ValueObjects;

public sealed record BuildingConfiguration
{
    public int TotalFloors { get; }
    public int TotalElevators { get; }
    public int ElevatorPassengerCapacity { get; }
    public const int GroundFloor = 1;
    public BuildingConfiguration(int totalFloors, int totalElevators, int elevatorPassengerCapacity)
    {
        if (totalFloors < 2)
            throw new ArgumentOutOfRangeException(nameof(totalFloors), "A building requires at least 2 floors.");
        if (totalElevators < 1)
            throw new ArgumentOutOfRangeException(nameof(totalElevators), "A building requires at least one elevator.");
        if (elevatorPassengerCapacity < 1)
            throw new ArgumentOutOfRangeException(nameof(elevatorPassengerCapacity), "Elevator capacity must be at least 1.");

        TotalFloors = totalFloors;
        TotalElevators = totalElevators;
        ElevatorPassengerCapacity = elevatorPassengerCapacity;
    }

    public bool IsValidFloor(int floorNumber) => floorNumber >= GroundFloor && floorNumber <= TotalFloors;
}