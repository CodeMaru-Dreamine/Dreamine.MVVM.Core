<!--!
\file README.md
\brief Dreamine.MVVM.Core - Lightweight MVVM infrastructure for WPF applications.
\details Architecture, installation, quick-start, and component reference.
\author Dreamine
\date 2026-03-08
\version 1.0.0
-->

# Dreamine.MVVM.Core

**Dreamine.MVVM.Core** is a **lightweight MVVM infrastructure** designed for modern .NET desktop applications.

It provides the essential building blocks required to implement MVVM while keeping the architecture **explicit, predictable, and easy to maintain**.

Dreamine focuses on **clarity and developer control** rather than heavy framework abstraction.

[➡️ 한국어 문서 보기](README_KO.md)

---

## What this library solves

MVVM frameworks often introduce complexity such as:

- excessive configuration
- heavy dependency injection frameworks
- hidden View ↔ ViewModel wiring
- large boilerplate code

Dreamine.MVVM.Core solves these problems by providing a **minimal but powerful MVVM core**.

---

## Key Features

- **ViewModelBase** for MVVM property notification
- **RelayCommand** for ICommand implementation
- **DMContainer** lightweight dependency injection container
- **DreamineAppBuilder** application bootstrapper
- Explicit View ↔ ViewModel architecture
- Minimal boilerplate code

---

## Requirements

- **.NET**: `net8.0-windows`
- **UI Framework**: WPF

---

## Installation

### Option A) Project Reference

Add a project reference to your application:

```xml
<ItemGroup>
  <ProjectReference Include="..\Dreamine.MVVM.Core\Dreamine.MVVM.Core.csproj" />
</ItemGroup>
```

### Option B) NuGet (future)

NuGet packaging is planned in future releases.

---

## Project Structure

```
Dreamine.MVVM.Core
├── ViewModelBase.cs
├── RelayCommand.cs
├── DMContainer.cs
└── DreamineAppBuilder.cs
```

---

## Architecture

```
Application
│
├── DreamineAppBuilder
│
├── DMContainer
│
├── View
│    ↔ ViewModelBase
│
└── RelayCommand
```

---

## Quick Start

### 1) Initialize Dreamine

```csharp
DreamineAppBuilder.Initialize(
    Assembly.GetExecutingAssembly());
```

---

### 2) Create a ViewModel

```csharp
public class MainViewModel : ViewModelBase
{
    private string _title;

    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
}
```

---

### 3) Add Command

```csharp
public RelayCommand SaveCommand { get; }

public MainViewModel()
{
    SaveCommand = new RelayCommand(Save);
}

private void Save()
{
    Console.WriteLine("Saved");
}
```

---

## Component Reference

### ViewModelBase

Base class for ViewModels.

Provides:

- INotifyPropertyChanged implementation
- SetProperty helper

---

### RelayCommand

Basic ICommand implementation used for UI actions.

---

### DMContainer

Lightweight dependency injection container.

Supports:

- singleton registration
- factory registration
- constructor injection

Example:

```csharp
DMContainer.Register<IMyService>(() => new MyService());
var service = DMContainer.Resolve<IMyService>();
```

---

### DreamineAppBuilder

Initializes the Dreamine MVVM environment.

Responsibilities:

- container initialization
- viewmodel discovery
- application bootstrapping

---

## Comparison

| Framework | Complexity | Boilerplate | Control |
|----------|------------|-------------|--------|
| Prism | High | High | Medium |
| MVVMLight | Medium | Medium | Medium |
| CommunityToolkit | Medium | Medium | Medium |
| ReactiveUI | Very High | High | Low |
| Dreamine | Low | Low | High |

Dreamine prioritizes **clarity and developer control**.

---

## Roadmap

Future modules:

```
Dreamine.Container
Dreamine.MVVM.Generator
Dreamine.Hybrid
Dreamine.Threading
Dreamine.Actuator
```

---

## License

MIT License
