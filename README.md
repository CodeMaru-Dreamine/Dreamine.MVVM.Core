<!--!
\file README.md
\brief Dreamine.MVVM.Core - Lightweight dependency injection and infrastructure module.
\details Core container and infrastructure services for the Dreamine MVVM framework.
\author Dreamine
\date 2026-03-15
\version 1.0.0
-->

# Dreamine.MVVM.Core

**Dreamine.MVVM.Core** is the **lightweight infrastructure core** of the Dreamine MVVM framework.

It provides the minimal runtime foundation required to support explicit architecture, constructor-based resolution, and low-complexity application composition.

This package is intentionally small and focused.

[➡️ 한국어 문서 보기](README_KO.md)

---

## What this library provides

Dreamine.MVVM.Core currently focuses on:

- lightweight dependency registration
- constructor-based object resolution
- singleton and factory-based registration
- minimal infrastructure for Dreamine MVVM modules

---

## Key Features

- **DMContainer** lightweight dependency injection container
- explicit registration model
- constructor-based resolution
- simple singleton caching
- framework-light architecture

---

## Requirements

- **.NET**: `net8.0`

---

## Installation

### Option A) Project Reference

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
└── DMContainer.cs
```

---

## Quick Start

### Register a service

```csharp
DMContainer.Register<IMyService>(() => new MyService());
```

### Resolve a service

```csharp
IMyService service = DMContainer.Resolve<IMyService>();
```

### Register a singleton

```csharp
var service = new MyService();
DMContainer.RegisterSingleton<IMyService>(service);
```

---

## Component Reference

### DMContainer

`DMContainer` is the central infrastructure component of this package.

Responsibilities:

- register factory delegates
- register singleton instances
- resolve constructor dependencies
- auto-register supported application types

Example:

```csharp
DMContainer.Register<IMessageService>(() => new MessageService());

var messageService = DMContainer.Resolve<IMessageService>();
```

---

## Design Goals

Dreamine.MVVM.Core prioritizes:

- explicit behavior over hidden magic
- low dependency surface
- predictable constructor-based composition
- simple extension points for higher-level modules

---

## Related Modules

Typical composition with other Dreamine packages:

- `Dreamine.MVVM.Interfaces`
- `Dreamine.MVVM.ViewModels`
- `Dreamine.MVVM.Locators`
- `Dreamine.MVVM.Wpf`

---

## License

MIT License
