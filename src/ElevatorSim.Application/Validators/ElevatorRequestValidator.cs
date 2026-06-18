using ElevatorSim.Domain.ValueObjects;
using FluentValidation;

namespace ElevatorSim.Application.Validators;
public sealed class ElevatorRequestValidator : AbstractValidator<ElevatorRequest>
{
    public ElevatorRequestValidator(BuildingConfiguration buildingConfiguration)
    {
        RuleFor(request => request.RequestedFloor)
            .GreaterThanOrEqualTo(BuildingConfiguration.GroundFloor)
            .WithMessage($"Floor must be at least {BuildingConfiguration.GroundFloor}.")
            .LessThanOrEqualTo(buildingConfiguration.TotalFloors)
            .WithMessage($"Floor cannot exceed {buildingConfiguration.TotalFloors} (the top floor of this building).");

        RuleFor(request => request.PassengerCount)
            .GreaterThan(0)
            .WithMessage("Passenger count must be at least 1.")
            .LessThanOrEqualTo(buildingConfiguration.ElevatorPassengerCapacity)
            .WithMessage($"Cannot request more than {buildingConfiguration.ElevatorPassengerCapacity} passengers (elevator maximum capacity).");
    }
}