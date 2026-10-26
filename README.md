# Machine Tracking System

A C# .NET console application that simulates monitoring and managing machines in a warehouse. Machines can be added, inspected, operated and repaired, while a background thread randomly causes machine failures and raises alerts.

> Originally developed as part of a university group project. This repository contains my portion of that project.

## Features

- Add machines of four types: Conveyor Belt, Robotic Arm, Forklift and Sorting Machine
- View all machines or look up a single machine by ID
- Remove machines
- Perform machine-specific operations (blocked while a machine is in a failed state)
- Repair all failed machines at once
- Background monitoring thread that randomly fails operational machines and fires alerts

## Concepts Demonstrated

- Abstraction and inheritance: an abstract `Machine` base class with four concrete subclasses
- Polymorphism: each machine type overrides `PerformOperation()`
- Delegates and events: `MachineFailed` and `MachineRecovered` events notify the UI
- Multithreading: a `MonitoringSystem` runs on its own thread, with a lock protecting the machine list
- Custom exceptions: `MachineNotFoundException`
- Exception handling: `try/catch/finally` around user input
- Collections: `List<Machine>` with lookup by ID

## Getting Started

### Requirements

- Windows
- Visual Studio with the .NET desktop development workload

### Run

1. Clone the repository:
   ```bash
   git clone https://github.com/MoneyAndHouse/warehouse-machine-tracking-app.git
   ```
2. Open the solution file (`PRG281_Project.slnx`) in Visual Studio.
3. Press F5 to build and run.

## Usage

```
1. View All Machines
2. View Machine Details
3. Add Machine
4. Remove Machine
5. Repair All Machines
6. Perform Machine Operation
7. Exit
```

Machine IDs are generated automatically (`M001`, `M002`, ...) and are not case-sensitive when searching.

Every 5 seconds, each operational machine has a 10% chance of failing. When that happens, an alert is printed with the machine's ID and type. Failed machines can't perform operations until repaired via option 5.

## Possible Improvements

- Save machine data to a file or database
- Separate class files