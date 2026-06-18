using ElevatorSim.Application.Interfaces;
using ElevatorSim.Application.Services;
using ElevatorSim.Application.Strategies;
using ElevatorSim.Application.Validators;
using ElevatorSim.Domain.Enums;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Domain.ValueObjects;
using ElevatorSim.Infrastructure.Factories;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSim.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElevatorSimServices(
        this IServiceCollection services,
        BuildingConfiguration buildingConfiguration)
    {
        services.AddSingleton(buildingConfiguration);

        services.AddScoped<IValidator<ElevatorRequest>, ElevatorRequestValidator>(
            sp => new ElevatorRequestValidator(buildingConfiguration));

        services.AddSingleton<IElevatorFactory, ElevatorFactory>();

        services.AddSingleton<IEnumerable<IElevatorControl>>(sp =>
        {
            IElevatorFactory factory = sp.GetRequiredService<IElevatorFactory>();
            return Enumerable.Range(1, buildingConfiguration.TotalElevators)
                .Select(elevatorId => factory.CreateElevator(
                    ElevatorType.Passenger,
                    id: elevatorId,
                    initialFloor: BuildingConfiguration.GroundFloor,
                    capacity: buildingConfiguration.ElevatorPassengerCapacity))
                .ToList();
        });

        services.AddSingleton<IDispatchStrategy, NearestAvailableDispatchStrategy>();
        services.AddSingleton<IElevatorController, ElevatorController>();

        return services;
    }
}