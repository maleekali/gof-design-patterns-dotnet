# GoF Design Patterns in C# and .NET

A practical learning repository covering all 23 Gang of Four (GoF) design patterns, with beginner, intermediate, and advanced projects built with C# and .NET.

The goal is not just to implement design patterns, but to understand **why they exist, when to use them, when to avoid them, and how they fit into real-world software architecture.**

## 🎯 Objectives

- Implement all 23 classic GoF design patterns in C# and .NET.
- Explore each pattern through three progressively challenging projects.
- Understand the problem each pattern solves and the trade-offs it introduces.
- Practice clean code, dependency injection, SOLID principles, and testable design.
- Explore practical integrations with caching, Redis, message brokers, file processing, and external services where appropriate.
- Compare similar patterns and understand when a simpler solution is sufficient.

## 🧩 The 23 GoF Design Patterns

The patterns are organized into three classic categories.

### 1. Creational Patterns

Patterns concerned with object creation.

| Pattern | Main Learning Focus |
|---|---|
| [Abstract Factory](src/Creational/AbstractFactory/) | Creating families of related objects |
| [Builder](src/Creational/Builder/) | Constructing complex objects step by step |
| [Factory Method](src/Creational/FactoryMethod/) | Delegating object creation to specialized implementations |
| [Prototype](src/Creational/Prototype/) | Creating objects by copying existing instances |
| [Singleton](src/Creational/Singleton/) | Controlling a type's instance lifetime |

### 2. Structural Patterns

Patterns concerned with how classes and objects are composed.

| Pattern | Main Learning Focus |
|---|---|
| [Adapter](src/Structural/Adapter/) | Making incompatible interfaces work together |
| [Bridge](src/Structural/Bridge/) | Separating abstractions from their implementations |
| [Composite](src/Structural/Composite/) | Representing tree-like object structures |
| [Decorator](src/Structural/Decorator/) | Adding responsibilities without modifying existing classes |
| [Facade](src/Structural/Facade/) | Providing a simplified interface to a subsystem |
| [Flyweight](src/Structural/Flyweight/) | Sharing common state to reduce memory usage |
| [Proxy](src/Structural/Proxy/) | Controlling access to another object |

### 3. Behavioral Patterns

Patterns concerned with communication, responsibilities, and object behavior.

| Pattern | Main Learning Focus |
|---|---|
| [Chain of Responsibility](src/Behavioral/ChainOfResponsibility/) | Passing requests through a chain of handlers |
| [Command](src/Behavioral/Command/) | Encapsulating requests as objects |
| [Interpreter](src/Behavioral/Interpreter/) | Representing and evaluating a simple language |
| [Iterator](src/Behavioral/Iterator/) | Traversing collections without exposing their internals |
| [Mediator](src/Behavioral/Mediator/) | Centralizing communication between collaborating objects |
| [Memento](src/Behavioral/Memento/) | Capturing and restoring object state |
| [Observer](src/Behavioral/Observer/) | Notifying subscribers when state changes |
| [State](src/Behavioral/State/) | Encapsulating behavior associated with an object's state |
| [Strategy](src/Behavioral/Strategy/) | Encapsulating interchangeable algorithms |
| [Template Method](src/Behavioral/TemplateMethod/) | Defining an algorithm's structure while allowing selected steps to vary |
| [Visitor](src/Behavioral/Visitor/) | Adding operations to object structures without placing every operation in the element classes |

## 📈 Three Learning Levels

Each pattern is explored through three independent implementations.

### 🟢 Beginner — Understand the Pattern

Focus on the essential structure and behavior of the pattern.

- A small, self-contained application.
- Minimal dependencies and no external infrastructure.
- Clear interfaces, concrete implementations, and client code.
- Unit tests demonstrating the intended behavior.
- An explanation of the problem and why the pattern fits.

### 🟡 Intermediate — Apply the Pattern

Explore the pattern in a more realistic application.

- A practical use case with clear requirements.
- Dependency injection and separation of responsibilities.
- Integration tests where appropriate.
- Configuration, error handling, and maintainable project structure.
- External technologies such as Redis or file processing when they contribute to the problem being solved.

### 🔴 Advanced — Explore Real-World Trade-offs

Investigate more demanding engineering scenarios.

- Failure handling, concurrency, or performance where relevant.
- Integration with external systems and infrastructure.
- Testing of edge cases and failure scenarios.
- Architectural decisions and alternative approaches.
- Explicit discussion of the pattern's limitations and maintenance cost.

Not every advanced example requires distributed infrastructure. The goal is to solve a genuinely harder problem, not to add technologies merely to make a project look sophisticated.

## 🛠️ Technology Stack

The technology choices may vary depending on the problem each example solves.

- **Language:** C#
- **Platform:** .NET
- **Testing:** xUnit
- **Dependency Injection:** Microsoft.Extensions.DependencyInjection
- **Caching:** IMemoryCache and Redis, where appropriate
- **Messaging:** RabbitMQ, Kafka, or other brokers when relevant
- **Infrastructure:** Docker and Docker Compose for examples that require external services
- **Automation:** GitHub Actions for build and test workflows

Specific framework versions and additional dependencies will be documented in each project.

## 🗂️ Repository Structure

```text
gof-design-patterns-dotnet/
├── README.md
├── docs/
│   ├── pattern-catalog.md
│   ├── decision-guide.md
│   ├── learning-roadmap.md
│   └── templates/
├── src/
│   ├── Creational/
│   │   ├── AbstractFactory/
│   │   │   ├── README.md
│   │   │   ├── Beginner/
│   │   │   ├── Intermediate/
│   │   │   └── Advanced/
│   │   ├── Builder/
│   │   ├── FactoryMethod/
│   │   ├── Prototype/
│   │   └── Singleton/
│   ├── Structural/
│   └── Behavioral/
├── tests/
└── .github/
    └── workflows/
        └── ci.yml
```

The directory structure is a planned convention. Additional projects and infrastructure will be introduced only when they are justified by the example.

## 📚 What Each Pattern Includes

Each pattern will have its own documentation covering:

1. **Intent** — What the pattern is designed to accomplish.
2. **Problem** — The design problem before introducing the pattern.
3. **Structure** — The main participants and their relationships.
4. **Implementation** — A practical C# implementation.
5. **Real-World Scenario** — A meaningful use case.
6. **Why This Pattern?** — Why it fits the problem.
7. **Alternatives** — Simpler designs or other applicable patterns.
8. **Trade-offs** — Complexity, flexibility, and maintenance costs.
9. **Testing** — Unit tests and integration tests where appropriate.
10. **Further Reading** — Relevant references and comparisons with related patterns.

## 🧪 Testing and Code Quality

Examples are intended to be runnable and testable, rather than merely illustrative snippets.

The repository aims to include:

- Unit tests for core behavior.
- Integration tests for external infrastructure where appropriate.
- Automated build and test checks through GitHub Actions.
- Consistent formatting and configuration.
- Clear setup instructions.
- No committed credentials, tokens, or real secrets.

## 🧠 Choosing the Right Pattern

A design pattern is not automatically a better solution.

For each implementation, this repository asks:

- What problem does the pattern solve?
- Would a simpler design be sufficient?
- What additional abstractions does it introduce?
- How does it affect testing and future changes?
- What are the costs of introducing it too early?
- When should the pattern be avoided?

The objective is to develop sound design judgment, not to force every pattern into every application.

## 🗺️ Project Roadmap

- [ ] Complete the three learning levels for Abstract Factory.
- [ ] Complete the remaining Creational patterns.
- [ ] Complete the Structural patterns.
- [ ] Complete the Behavioral patterns.
- [ ] Add pattern comparison guides.
- [ ] Add architecture diagrams and decision records.
- [ ] Establish automated build and test workflows.
- [ ] Review all examples for correctness, consistency, and practical value.

Progress will be tracked as implementations and tests are completed.

## 🚀 Getting Started

Prerequisites depend on the example you want to run. Generally, you will need:

- A compatible .NET SDK.
- Git.
- Docker and Docker Compose for examples that use external services.

To explore a pattern:

1. Open its directory under `src/`.
2. Read the pattern-specific README.
3. Follow the setup instructions for the chosen learning level.
4. Run the example and its tests.
5. Review the documented trade-offs and alternative designs.

Each implementation will provide its own commands and configuration requirements.

## 📖 References

The primary conceptual reference is:

- *Design Patterns: Elements of Reusable Object-Oriented Software* — Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides (the Gang of Four).

The examples are educational implementations intended to demonstrate the patterns in modern C# and .NET. They are not official implementations from the book and may adapt the original structures to fit contemporary application design.

## 📄 License

This project is intended for educational purposes. The repository license will define the terms under which its code and documentation may be reused.

---

**Learning design patterns is not about writing more classes. It is about understanding the design problem, choosing an appropriate solution, and knowing when not to use a pattern.**
