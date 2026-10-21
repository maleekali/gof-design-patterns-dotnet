# Abstract Factory Pattern in C# and .NET

The **Abstract Factory** is a creational design pattern that provides an interface for creating families of related or dependent objects without specifying their concrete classes.

This repository explores the pattern through practical C#/.NET implementations, progressing from a minimal example to applications involving caching, file processing, Redis, and message brokers.

> The goal is not simply to implement Abstract Factory, but to understand when it provides real value, what complexity it introduces, and when a simpler design is preferable.

## Table of Contents

- [Intent](#intent)
- [The Problem](#the-problem)
- [Core Idea](#core-idea)
- [Pattern Structure](#pattern-structure)
- [C# Example](#c-example)
- [How It Works](#how-it-works)
- [When to Use It](#when-to-use-it)
- [When to Avoid It](#when-to-avoid-it)
- [Abstract Factory vs. Factory Method](#abstract-factory-vs-factory-method)
- [Relationship with Dependency Injection](#relationship-with-dependency-injection)
- [Learning Projects](#learning-projects)
- [Testing Strategy](#testing-strategy)
- [Advantages and Trade-offs](#advantages-and-trade-offs)
- [References](#references)

## Intent

Provide an interface for creating families of related or dependent objects without specifying their concrete classes.

The central idea is to separate the creation of a product family from the application code that consumes those products.

## The Problem

Imagine an application that supports multiple operating systems, each requiring its own set of UI components.

A Windows interface might require:

- `WindowsButton`
- `WindowsCheckbox`

A macOS interface might require:

- `MacButton`
- `MacCheckbox`

The application must use compatible components belonging to the selected UI family.

Without a suitable abstraction, the application may create these objects directly or repeatedly check which platform is active.

```csharp
if (platform == "Windows")
{
    button = new WindowsButton();
    checkbox = new WindowsCheckbox();
}
else if (platform == "Mac")
{
    button = new MacButton();
    checkbox = new MacCheckbox();
}
```

This approach can become difficult to maintain when object creation logic is duplicated across multiple parts of an application.

Adding another platform may require changes in several locations. It also becomes easier to accidentally combine components from different families.

Abstract Factory addresses this problem by encapsulating the creation of related products behind a common factory interface.

## Core Idea

Instead of asking individual classes to create concrete products, the application receives a factory that creates the appropriate family of products.

```text
                  Application
                       |
                       v
                   IGUIFactory
                       |
             +---------+---------+
             |                   |
             v                   v
      WindowsFactory          MacFactory
             |                   |
       +-----+-----+       +-----+-----+
       |           |       |           |
       v           v       v           v
 WindowsButton  WindowsCheckbox  MacButton  MacCheckbox
```

The application interacts with product interfaces and does not need to know the concrete classes being instantiated.

The selected factory determines which family of products is created.

## Pattern Structure

Abstract Factory typically involves the following participants:

| Participant | Responsibility |
|---|---|
| **Abstract Factory** | Declares creation methods for related products. |
| **Concrete Factory** | Implements the creation methods for a specific product family. |
| **Abstract Product** | Defines the contract for a product type. |
| **Concrete Product** | Implements a product contract for a specific family. |
| **Client** | Uses the factory and product interfaces without depending on concrete product classes. |

A factory may create two or more product types. These products do not need to share the same interface, inheritance hierarchy, or methods.

Their relationship comes from belonging to a family that must be created or used together.

## C# Example

### 1. Define the abstract products

```csharp
public interface IButton
{
    void Render();
}

public interface ICheckbox
{
    void Render();
}
```

Each interface defines the contract for a different product type.

### 2. Define the abstract factory

```csharp
public interface IGUIFactory
{
    IButton CreateButton();

    ICheckbox CreateCheckbox();
}
```

The factory interface describes which products can be created without specifying their concrete implementations.

### 3. Implement a concrete product family

```csharp
public sealed class WindowsButton : IButton
{
    public void Render()
    {
        Console.WriteLine("Rendering a Windows button.");
    }
}

public sealed class WindowsCheckbox : ICheckbox
{
    public void Render()
    {
        Console.WriteLine("Rendering a Windows checkbox.");
    }
}
```

Now implement the corresponding factory:

```csharp
public sealed class WindowsFactory : IGUIFactory
{
    public IButton CreateButton()
        => new WindowsButton();

    public ICheckbox CreateCheckbox()
        => new WindowsCheckbox();
}
```

A `MacFactory` would create `MacButton` and `MacCheckbox` instead.

### 4. Write the client

```csharp
public sealed class SettingsWindow
{
    private readonly IGUIFactory _factory;

    public SettingsWindow(IGUIFactory factory)
    {
        _factory = factory;
    }

    public void Render()
    {
        IButton button = _factory.CreateButton();
        ICheckbox checkbox = _factory.CreateCheckbox();

        button.Render();
        checkbox.Render();
    }
}
```

The client depends on `IGUIFactory`, `IButton`, and `ICheckbox`, not on the concrete Windows or macOS classes.

### 5. Select a factory

```csharp
IGUIFactory factory = new WindowsFactory();

var window = new SettingsWindow(factory);

window.Render();
```

To use another product family, provide a different factory:

```csharp
IGUIFactory factory = new MacFactory();

var window = new SettingsWindow(factory);

window.Render();
```

The `SettingsWindow` implementation remains unchanged.

This example demonstrates the essential structure of the pattern. In a production application, factory selection would typically come from configuration, dependency injection, or another explicit composition mechanism.

## How It Works

The pattern separates two responsibilities:

**Product creation**

Concrete factories know which concrete products belong to their families and are responsible for creating them.

**Product consumption**

Client code works with abstract interfaces and uses the returned products without needing to know their concrete types.

This separation makes it possible to switch between product families without distributing concrete construction logic throughout the application.

It does not automatically guarantee that every family is correct or compatible. That responsibility belongs to the concrete factory implementations and their tests.

## When to Use It

Abstract Factory is worth considering when:

- An application must support multiple families of related products.
- Products from the same family are expected to work together.
- The active family can change depending on configuration or application requirements.
- Concrete product creation should remain outside client code.
- Adding another product family should not require rewriting the main application logic.

Potential applications include UI component families, file-processing pipelines, provider-specific integrations, and groups of infrastructure components that must be configured consistently.

## When to Avoid It

Abstract Factory is not automatically the best solution for every object creation problem.

Avoid introducing it when:

- There is only one product type and a simple factory or direct dependency injection is sufficient.
- Only one concrete product family exists and there is no meaningful need for family substitution.
- The products are not actually related and are being grouped only to justify the pattern.
- The abstraction adds more interfaces and classes than the problem warrants.
- Product creation is simple, localized, and unlikely to change.

**A design pattern should reduce meaningful complexity, not merely increase the number of abstractions.**

## Abstract Factory vs. Factory Method

Both are creational patterns, but they address different design concerns.

| Aspect | Factory Method | Abstract Factory |
|---|---|---|
| Primary concern | Creating a product through an overridable creation method | Creating families of related products |
| Typical scope | One product type per creation method | Multiple related product types |
| Main abstraction | A method responsible for product creation | A factory interface declaring multiple creation methods |
| Typical example | Creating a particular document exporter | Creating a provider-specific set of related components |

The distinction is about design intent rather than the number of methods alone. Abstract Factory implementations may themselves use Factory Methods internally.

## Relationship with Dependency Injection

Abstract Factory and Dependency Injection are complementary, not interchangeable.

Dependency Injection supplies dependencies from outside a class. An Abstract Factory provides an abstraction for creating a family of products.

For example:

```csharp
services.AddScoped<IGUIFactory, WindowsFactory>();
services.AddScoped<SettingsWindow>();
```

The dependency injection container can supply the selected factory to `SettingsWindow`.

The client can then ask that factory to create the required products.

Dependency Injection does not require every dependency to be an interface, and registering a concrete class in DI does not by itself implement Abstract Factory.

## Learning Projects

The pattern is explored through three progressively more demanding implementations.

### Beginner — Notification Center

**Focus:** Abstract Factory, Dependency Injection, `IMemoryCache`

A notification application supporting email and SMS.

Each notification family provides a sender and a compatible template renderer. Templates are loaded from local files and cached in memory.

**Learning objectives:**

- Identify abstract products and concrete product families.
- Separate notification creation from notification handling.
- Introduce interfaces only where they provide useful abstractions.
- Understand when a simple factory is sufficient.
- Test factory selection and client behavior.

[Open the Beginner project](./Beginner/README.md)

### Intermediate — Import Preview Service

**Focus:** Abstract Factory, Redis, file processing

An import-preview application supporting CSV and JSON inputs.

Each format family provides parsing, validation, and report-generation components. Redis caches generated previews using a content-based key and a processing-version identifier.

**Learning objectives:**

- Apply the pattern to a realistic processing pipeline.
- Separate format-specific behavior from the orchestration logic.
- Handle cache expiration, invalid input, and processing failures.
- Test cache hits, cache misses, and format-specific behavior.

[Open the Intermediate project](./Intermediate/README.md)

### Advanced — External Feed Ingestion Platform

**Focus:** Abstract Factory, Redis, message brokers, external files

A background service that retrieves external data files, validates records, creates standard integration events, and publishes them through a selected message broker.

Potential broker implementations include RabbitMQ and Kafka.

Each broker family may provide a publisher, an encoder, and other broker-specific components when their creation and configuration genuinely belong together.

Redis tracks processing state and supports idempotency strategies.

**Learning objectives:**

- Design abstractions around infrastructure boundaries.
- Select compatible provider-specific components.
- Handle retries, duplicate processing, and partial failures.
- Understand why Redis state and broker publication are not automatically one atomic transaction.
- Evaluate the operational cost of introducing this level of abstraction.

[Open the Advanced project](./Advanced/README.md)

The three projects are intended to illustrate the same design principle at increasing levels of complexity. External services are introduced only when they contribute to the problem being solved.

## Testing Strategy

The tests should verify both behavior and architectural expectations.

Recommended test cases include:

- Each concrete factory returns products from the expected family.
- Client code works through abstract interfaces.
- Switching factories does not require modifying the client.
- A new family can be introduced without changing existing client behavior.
- Invalid configuration is rejected or handled explicitly.
- Integration-dependent behavior is tested separately from core business logic.

For the advanced implementation, add tests for provider-specific configuration, retry behavior, and failure scenarios.

Not every test needs a real external service. Factory and client behavior can often be verified with lightweight test doubles, while integration tests validate actual infrastructure interactions.

## Advantages and Trade-offs

### Advantages

- Centralizes the creation of related products.
- Reduces direct dependencies on concrete product classes.
- Makes switching between product families easier.
- Supports consistent construction of compatible components.
- Can improve testability and maintainability when family variation is a real requirement.

### Trade-offs

- Introduces additional interfaces and concrete factory classes.
- Adding a new product type may require changing the abstract factory and all concrete factories.
- Factory selection and configuration require their own design.
- Can be unnecessary overhead when product creation is simple.
- Does not automatically solve infrastructure failures, object lifetime management, or distributed consistency.

The pattern is most valuable when the need to switch between complete product families is a meaningful part of the application.

## References

- Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides, *Design Patterns: Elements of Reusable Object-Oriented Software*.
- [Microsoft Learn — Dependency injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

The implementations in this repository are educational examples inspired by the GoF pattern and adapted to modern C#/.NET development.

---

**Key takeaway:** Abstract Factory is about creating compatible families of related objects, not simply replacing every concrete class with an interface or adding a factory around every `new` expression.