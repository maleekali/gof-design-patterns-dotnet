# GoF Pattern Catalog

This catalog tracks the 23 classic Gang of Four (GoF) design patterns and provides a consistent place to plan their C#/.NET examples.

**Status legend:** `Planned` means the example is not yet complete. Update the status only after the corresponding implementation, tests, and documentation exist.

## Creational Patterns

Patterns concerned with object creation.

| Pattern | Intent | Beginner project idea | Intermediate project idea | Advanced project idea | Status |
|---|---|---|---|---|---|
| [Abstract Factory](../src/Creational/AbstractFactory/README.md) | Create families of related objects without naming their concrete classes. | Light/dark UI component families | CSV/JSON import tool families | File ingestion with broker-specific component families | Planned |
| [Builder](../src/Creational/Builder/README.md) | Construct a complex object step by step. | Build a simple report with optional sections | Build configurable invoices | Generate large reports incrementally | Planned |
| [Factory Method](../src/Creational/FactoryMethod/README.md) | Let subclasses or specialized creators decide which product to instantiate. | Create different export formats | Select a provider-specific service | Extensible exporter or provider plug-in system | Planned |
| [Prototype](../src/Creational/Prototype/README.md) | Create new objects by copying existing instances. | Clone a game character configuration | Clone workflow templates | Safely copy complex object graphs and test shared mutable state | Planned |
| [Singleton](../src/Creational/Singleton/README.md) | Ensure a class has one instance and provide a controlled access point to it. | Demonstrate a thread-safe singleton | Compare a classic singleton with DI-managed lifetime | Explore concurrency and the limits of process-local singleton state | Planned |

## Structural Patterns

Patterns concerned with how objects and classes are composed.

| Pattern | Intent | Beginner project idea | Intermediate project idea | Advanced project idea | Status |
|---|---|---|---|---|---|
| [Adapter](../src/Structural/Adapter/README.md) | Make one interface usable through another expected interface. | Adapt a legacy logger | Normalize APIs from multiple shipping providers | Build tested adapters for several external providers | Planned |
| [Bridge](../src/Structural/Bridge/README.md) | Separate an abstraction from its implementation so both can vary independently. | Render a report in different formats | Combine report types and renderers | Extend reporting without multiplying class hierarchies | Planned |
| [Composite](../src/Structural/Composite/README.md) | Treat individual objects and compositions uniformly. | Model files and folders | Support product bundles in a cart | Evaluate a hierarchical organization or permission tree | Planned |
| [Decorator](../src/Structural/Decorator/README.md) | Add responsibilities to an object by wrapping it. | Add formatting to a text writer | Add logging and caching around a service | Compose audit, metrics, and resilience decorators with behavior tests | Planned |
| [Facade](../src/Structural/Facade/README.md) | Provide a simpler interface to a subsystem. | Simplify a multi-step text operation | Coordinate a checkout workflow | Encapsulate a complicated legacy subsystem | Planned |
| [Flyweight](../src/Structural/Flyweight/README.md) | Share common state across many objects to reduce memory usage. | Share immutable game assets | Render many map markers | Measure memory use with a large repeated data set | Planned |
| [Proxy](../src/Structural/Proxy/README.md) | Control access to another object through a substitute. | Lazy-load an image or document | Add caching or access checks to a remote client | Handle timeouts and remote failure through a proxy boundary | Planned |

## Behavioral Patterns

Patterns concerned with communication, responsibilities, and object behavior.

| Pattern | Intent | Beginner project idea | Intermediate project idea | Advanced project idea | Status |
|---|---|---|---|---|---|
| [Chain of Responsibility](../src/Behavioral/ChainOfResponsibility/README.md) | Pass a request through a sequence of handlers. | Validate an input through several checks | Process an order through approval steps | Build an ingestion pipeline with failure routing | Planned |
| [Command](../src/Behavioral/Command/README.md) | Encapsulate a request as an object. | Add undoable editor actions | Execute retryable background jobs | Handle durable commands with idempotency and failure recovery | Planned |
| [Interpreter](../src/Behavioral/Interpreter/README.md) | Represent and evaluate expressions in a small language. | Evaluate arithmetic expressions | Filter products with a small rule syntax | Build a constrained rule engine with a parser and grammar tests | Planned |
| [Iterator](../src/Behavioral/Iterator/README.md) | Traverse a collection without exposing its internal representation. | Implement a custom collection iterator | Paginate API results | Stream large asynchronous data sets with cancellation | Planned |
| [Mediator](../src/Behavioral/Mediator/README.md) | Centralize communication among collaborating components. | Coordinate components in a small application | Dispatch requests to application handlers | Decouple a complex workflow and test interactions independently | Planned |
| [Memento](../src/Behavioral/Memento/README.md) | Capture and restore an object's state without exposing its internals. | Undo text edits | Save and restore a form draft | Manage document snapshots and history size | Planned |
| [Observer](../src/Behavioral/Observer/README.md) | Notify dependents when a subject's state changes. | Notify subscribers of a value change | Send inventory-change notifications | Handle consumer failures and backpressure in event notification | Planned |
| [State](../src/Behavioral/State/README.md) | Change behavior when an object's internal state changes. | Model a traffic light | Model order or ticket status transitions | Validate concurrent state transitions and persistence boundaries | Planned |
| [Strategy](../src/Behavioral/Strategy/README.md) | Make a family of algorithms interchangeable. | Choose a discount algorithm | Select a shipping calculation strategy | Configure pricing policies and benchmark alternatives | Planned |
| [Template Method](../src/Behavioral/TemplateMethod/README.md) | Define an algorithm's skeleton while letting selected steps vary. | Generate reports using fixed steps | Process different file formats through common stages | Process streamed input with stable workflow steps and extension points | Planned |
| [Visitor](../src/Behavioral/Visitor/README.md) | Define operations over an object structure without placing every operation in the element classes. | Calculate properties of geometric shapes | Run different operations over a document tree | Analyze and export an abstract syntax tree | Planned |

## Progress Rules

A pattern should not be marked complete simply because its core classes compile. Consider an implementation complete when:

- The example runs using the documented instructions.
- Tests demonstrate the intended behavior.
- The README explains the problem, participants, and trade-offs.
- The implementation identifies a simpler alternative and explains why the pattern is justified here.
- The documented status matches the actual state of the repository.
