# ElevatorSim - .NET 10 Console Elevator Simulation

A console application simulating elevator dispatch in a configurable building. Built with Clean Architecture, SOLID principles, async/await concurrent movement, and full TDD unit test coverage.

## Tech Stack
- .NET 10 C# 14
- Clean Architecture (Domain / Application / Infrastructure / Console)
- Microsoft Extensions
- FluentValidation
- Serilog (Console + File sinks)
- xUnit + NSubstitute

## Getting Started

### Prerequisites
- .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0

### Build and Run
```bash
git clone https://github.com/your_username/Dotnet-Elevator-Sashen.git
cd dotnet-elevator-sim
dotnet build
dotnet run --project src/ElevatorSim.Console
```

### Run Tests
```bash
dotnet test
```

## How to Use
At startup the app prompts for: number of floors, number of elevators, and passenger capacity per elevator.

During simulation:
- `call <floor> <passengers>` - call an elevator to a floor
- `status` or `s` - confirm current elevator statuses
- `quit` or `q` - gracefully shut down

## Architecture
```
ElevatorSim.Domain          ← entities, interfaces, exceptions
ElevatorSim.Application     ← use cases, dispatch strategy, validators
ElevatorSim.Infrastructure  ← DI wiring, factory, Serilog
ElevatorSim.Console         ← entry point, ASCII renderer, input handler
ElevatorSim.Tests           ← xUnit tests
```

## Assumptions
- Passenger model is simplified: passengers board at the called floor and tthe elevator returns to idle after boarding.
- FreightElevator and HighSpeedElevator are fully implemented elevator types accessible by extending the factory call.
- All state is in-memory, no persistence between sessions.