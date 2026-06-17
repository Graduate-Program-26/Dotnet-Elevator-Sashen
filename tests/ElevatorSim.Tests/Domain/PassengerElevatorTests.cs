using ElevatorSim.Domain.Entities;
using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Exceptions;

namespace ElevatorSim.Tests.Domain;

public sealed class PassengerElevatorTests
{
    [Fact]
    public void Constructor_WithValidArguments_SetsInitialStateCorrectly()
    {
        var elevator = new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 10);

        Assert.Equal(1, elevator.CurrentFloor);
        Assert.Equal(0, elevator.CurrentPassengerCount);
        Assert.Equal(ElevatorDirection.Stationary, elevator.Direction);
        Assert.Equal(ElevatorStatus.Idle, elevator.Status);
        Assert.True(elevator.IsAvailable);
        Assert.Equal(ElevatorType.Passenger, elevator.Type);
    }

    [Fact]
    public void Constructor_WithZeroCapacity_ThrowsArgumentOutOfRangeException()
    {
        var act = () => new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 0);

        Assert.Throws<ArgumentOutOfRangeException>(act);
    }

    [Fact]
    public async Task BoardPassengersAsync_WhenUnderCapacity_IncreasesPassengerCount()
    {
        var elevator = new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 10);

        await elevator.BoardPassengersAsync(passengerCount: 5, CancellationToken.None);

        Assert.Equal(5, elevator.CurrentPassengerCount);
    }

    [Fact]
    public async Task BoardPassengersAsync_WhenExceedingCapacity_ThrowsCapacityExceededException()
    {
        var elevator = new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 5);

        var act = async () =>
            await elevator.BoardPassengersAsync(passengerCount: 6, CancellationToken.None);

        await Assert.ThrowsAsync<CapacityExceededException>(act);
    }

    [Fact]
    public async Task BoardPassengersAsync_AtExactCapacity_SetsStatusToAtCapacity()
    {
        var elevator = new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 5);

        await elevator.BoardPassengersAsync(passengerCount: 5, CancellationToken.None);

        Assert.False(elevator.IsAvailable);
    }

    [Fact]
    public async Task MoveToFloorAsync_WhenTargetFloorIsHigher_SetsDirectionToUp()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var elevator = new PassengerElevator(id: 1, initialFloor: 1, maximumPassengerCapacity: 10);
        ElevatorDirection? capturedDirection = null;

        elevator.FloorReached += (_, floor) =>
        {
            if (capturedDirection is null)
                capturedDirection = elevator.Direction;
        };

        await elevator.MoveToFloorAsync(targetFloor: 3, cts.Token);

        Assert.Equal(3, elevator.CurrentFloor);
        Assert.Equal(ElevatorDirection.Up, capturedDirection);
    }

    [Fact]
    public async Task MoveToFloorAsync_ToSameFloor_DoesNotChangeStatus()
    {
        var elevator = new PassengerElevator(id: 1, initialFloor: 3, maximumPassengerCapacity: 10);

        await elevator.MoveToFloorAsync(targetFloor: 3, CancellationToken.None);

        Assert.Equal(ElevatorStatus.Idle, elevator.Status);
        Assert.Equal(ElevatorDirection.Stationary, elevator.Direction);
    }
}