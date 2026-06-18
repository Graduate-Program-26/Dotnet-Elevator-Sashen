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
git clone https://github.com/your_username/dotnet-elevator-console.git
cd dotnet-elevator-console
dotnet build
dotnet run --project src/ElevatorSim.Console
```

### Run Tests
```bash
dotnet test
```
Runs the xUnit suite (Application validators, dispatch strategy, Domain elevator behavior). A passing run currently shows `Passed! - Failed: 0, Passed: 16, Skipped: 0, Total: 16`. The same command runs in CI on every push/PR via `.github/workflows/ci.yml`.

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
- Passenger model is simplified: there's no separate destination floor. The elevator travels to the called floor, boards the passengers, briefly holds them so the status table can show it, then disembarks and returns to idle.
- `FreightElevator` and `HighSpeedElevator` are fully implemented elevator types, selectable by extending the factory call.
- All state is in-memory, nothing persists between sessions.