using ElevatorSim.Application.Strategies;
using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;
using NSubstitute;

namespace ElevatorSim.Tests.Application;

public sealed class NearestAvailableDispatchStrategyTests
{
    private readonly NearestAvailableDispatchStrategy _strategy = new();

    [Fact]
    public void SelectElevator_FromMultipleElevators_ReturnsClosestToRequestedFloor()
    {
        IElevator elevatorOnFloor2 = CreateAvailableElevatorSubstitute(id: 1, currentFloor: 2);
        IElevator elevatorOnFloor8 = CreateAvailableElevatorSubstitute(id: 2, currentFloor: 8);

        IElevator? selected = _strategy.SelectElevator(
            availableElevators: [elevatorOnFloor2, elevatorOnFloor8],
            requestedFloor: 7);

        Assert.NotNull(selected);
        Assert.Equal(2, selected.Id);
    }

    [Fact]
    public void SelectElevator_WhenAllElevatorsAtCapacity_ReturnsNull()
    {
        IElevator fullElevator = CreateUnavailableElevatorSubstitute(id: 1, currentFloor: 3);

        IElevator? selected = _strategy.SelectElevator(
            availableElevators: [fullElevator],
            requestedFloor: 5);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectElevator_WithEmptyElevatorList_ReturnsNull()
    {
        IElevator? selected = _strategy.SelectElevator(
            availableElevators: [],
            requestedFloor: 5);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectElevator_WithEquidistantElevators_PrefersLessLoaded()
    {
        IElevator elevatorWithThreePassengers = CreateAvailableElevatorSubstitute(id: 1, currentFloor: 3, passengerCount: 3);
        IElevator elevatorWithOnePassenger = CreateAvailableElevatorSubstitute(id: 2, currentFloor: 7, passengerCount: 1);

        IElevator? selected = _strategy.SelectElevator(
            availableElevators: [elevatorWithThreePassengers, elevatorWithOnePassenger],
            requestedFloor: 5); // Both are 2 floors away

        Assert.Equal(2, selected?.Id); // Less loaded elevator preferred
    }

    private static IElevator CreateAvailableElevatorSubstitute(int id, int currentFloor, int passengerCount = 0)
    {
        IElevator substitute = Substitute.For<IElevator>();
        substitute.Id.Returns(id);
        substitute.CurrentFloor.Returns(currentFloor);
        substitute.CurrentPassengerCount.Returns(passengerCount);
        substitute.IsAvailable.Returns(true);
        return substitute;
    }

    private static IElevator CreateUnavailableElevatorSubstitute(int id, int currentFloor)
    {
        IElevator substitute = Substitute.For<IElevator>();
        substitute.Id.Returns(id);
        substitute.CurrentFloor.Returns(currentFloor);
        substitute.IsAvailable.Returns(false);
        return substitute;
    }
}