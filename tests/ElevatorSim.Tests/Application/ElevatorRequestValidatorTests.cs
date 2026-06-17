using ElevatorSim.Application.Validators;
using ElevatorSim.Domain.ValueObjects;
using FluentValidation.Results;

namespace ElevatorSim.Tests.Application;

public sealed class ElevatorRequestValidatorTests
{
    private readonly BuildingConfiguration _tenFloorBuilding = new(
        totalFloors: 10,
        totalElevators: 3,
        elevatorPassengerCapacity: 8);

    [Fact]
    public void Validate_WithValidRequest_PassesValidation()
    {
        var validator = new ElevatorRequestValidator(_tenFloorBuilding);
        ElevatorRequest validRequest = ElevatorRequest.CreateNow(requestedFloor: 5, passengerCount: 3);
        ValidationResult result = validator.Validate(validRequest);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]   
    [InlineData(11)] 
    [InlineData(-5)]
    public void Validate_WithInvalidFloor_FailsValidation(int invalidFloor)
    {
        var validator = new ElevatorRequestValidator(_tenFloorBuilding);
        ElevatorRequest requestWithBadFloor = ElevatorRequest.CreateNow(invalidFloor, passengerCount: 1);
        ValidationResult result = validator.Validate(requestWithBadFloor);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(ElevatorRequest.RequestedFloor));
    }

    [Fact]
    public void Validate_WithPassengerCountExceedingCapacity_FailsValidation()
    {
        var validator = new ElevatorRequestValidator(_tenFloorBuilding);
        ElevatorRequest requestWithTooManyPassengers = ElevatorRequest.CreateNow(
            requestedFloor: 5, passengerCount: 9); 

        ValidationResult result = validator.Validate(requestWithTooManyPassengers);
        Assert.False(result.IsValid);
    }
}