using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography;
using ElevatorSim.Application.Interfaces;
using ElevatorSim.Console.Display;
using ElevatorSim.Console.Input;
using ElevatorSim.Domain.ValueObjects;
using ElevatorSim.Infrastructure.DependencyInjection;
using ElevatorSim.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = SerilogConfiguration.CreateLogger();

try
{
    Log.Information("The Elevator Simulation is Starting up.");

    BuildingConfiguration buildingConfiguration = await BuildingConfigurationPrompt.CollectAsync();

    using var simulationCancellationSource = new CancellationTokenSource();

    ServiceProvider serviceProvider = new ServiceCollection()
        .AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger);
        })
        .AddElevatorSimServices(buildingConfiguration)
        .AddSingleton(simulationCancellationSource)
        .AddSingleton(sp => new ConsoleRenderer(
            sp.GetRequiredService<IElevatorController>(),
            buildingConfiguration))
        .AddSingleton<UserInputHandler>()
        .BuildServiceProvider();

    System.Console.CancelKeyPress += (_, eventArgs) =>
    {
        eventArgs.Cancel = true;
        simulationCancellationSource.Cancel();

    };

    ConsoleRenderer renderer = serviceProvider.GetRequiredService<ConsoleRenderer>();
    UserInputHandler inputHandler = serviceProvider.GetRequiredService<UserInputHandler>();

    await Task.WhenAll(
        renderer.RunDisplayLoopAsync(simulationCancellationSource.Token),
        inputHandler.RunInputLoopAsync(simulationCancellationSource.Token));

    Log.Information("Elevation Simulation stopped.");

}
catch (OperationCanceledException)
{
    Log.Information("Simulation cancelled by user.");
}
catch (Exception unexpectedException)
{
    Log.Fatal(unexpectedException, "Unhandled exception. Simulation terminated.");
    return 1; 
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0; 