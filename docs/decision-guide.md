# Design Pattern Decision Guide

This guide is a starting point for design discussions, not a rule engine. First understand the problem, then choose the simplest design that solves it. A pattern is useful only when its trade-offs are justified.

## Start With the Problem

Ask these questions before choosing a pattern:

1. **Is object creation becoming complicated or conditional?** Consider a creational pattern.
2. **Do incompatible APIs need to work together, or is object composition getting complicated?** Consider a structural pattern.
3. **Are algorithms, state transitions, or communication between objects difficult to manage?** Consider a behavioral pattern.
4. **Can a small method, a direct constructor, or a simple interface solve the problem?** Prefer that when it is sufficient.

## Creational Patterns

| If the main problem is... | Consider | Important distinction |
|---|---|---|
| Creating several related products that must belong to the same family | Abstract Factory | The factory creates a family of product types. The products can have different interfaces. |
| Creating one product through a specialized creation method | Factory Method | Focuses on delegating the choice of product creation to a creator or subclass. |
| Constructing an object through a sequence of steps or optional parts | Builder | Focuses on the construction process, especially when constructors would become unwieldy. |
| Creating a new object by copying a configured existing object | Prototype | Focuses on copying, including the consequences of shallow versus deep copying. |
| Restricting a type to one process-local instance | Singleton | Check whether dependency injection and a managed service lifetime are simpler. It is not a distributed singleton. |

### Abstract Factory vs. Factory Method

Use Abstract Factory when the client needs several related product types from one selected family. Use Factory Method when the main variation is which product a creation method returns. The patterns can coexist; they are not mutually exclusive.

### Abstract Factory vs. Dependency Injection

Dependency Injection supplies a dependency from outside a class. Abstract Factory defines an interface for creating a family of related objects. DI can inject an Abstract Factory, but using DI alone does not mean Abstract Factory is present.

## Structural Patterns

| If the main problem is... | Consider | Important distinction |
|---|---|---|
| An existing API has the wrong interface for the client | Adapter | Translates one interface into another expected by the client. |
| Two independent dimensions of variation are multiplying subclasses | Bridge | Separates an abstraction from an implementation so both can vary independently. |
| Individual objects and groups should be treated uniformly | Composite | Represents part-whole trees. |
| Additional behavior should be layered around an object | Decorator | Adds responsibilities through wrapping while keeping the same general interface. |
| A subsystem is difficult to use and needs a simpler entry point | Facade | Simplifies access; it does not primarily adapt an incompatible API. |
| Many objects repeat expensive common state | Flyweight | Shares intrinsic state; object identity and mutable state need careful handling. |
| Access to an object should be controlled or deferred | Proxy | Stands in for another object, for example for lazy loading or access control. |

### Adapter vs. Facade vs. Proxy

- **Adapter** changes the interface a client sees.
- **Facade** offers a simpler entry point to a subsystem.
- **Proxy** represents another object and controls access to it.

Their implementations may look structurally similar, so classify them by intent rather than by the number of classes.

### Bridge vs. Strategy

Both involve composition and interchangeable implementations. Bridge is about separating two dimensions of a type's structure so both can evolve independently. Strategy is about selecting among interchangeable algorithms or behaviors.

## Behavioral Patterns

| If the main problem is... | Consider | Important distinction |
|---|---|---|
| A request should pass along a sequence of possible handlers | Chain of Responsibility | A handler may process, reject, or forward the request. |
| A request or action should be represented as an object | Command | Useful for queuing, logging, undo, or delayed execution when those needs exist. |
| A small language or expression grammar must be evaluated | Interpreter | Appropriate for a limited grammar, not automatically for a full programming language. |
| A collection should be traversed without exposing its internal representation | Iterator | .NET already provides `IEnumerable<T>` and `IEnumerator<T>` for many cases. |
| Many components should communicate through a coordinating object | Mediator | Centralizes interactions but can become an oversized coordinator if poorly designed. |
| Internal state must be captured and restored later | Memento | Defines a boundary for snapshots while preserving encapsulation. |
| Dependents need notification about changes | Observer | Decide who owns subscriptions and how subscriber failures and lifetimes are handled. |
| An object's behavior depends on its current state | State | Models state-specific behavior and transitions. |
| Several algorithms should be interchangeable | Strategy | Encapsulates an algorithm; it need not own or persist state transitions. |
| An algorithm has stable high-level steps but variable steps | Template Method | Uses inheritance to vary selected algorithm steps. |
| New operations must be added over a relatively stable object structure | Visitor | Makes it easier to add operations, but can make adding new element types harder. |

### Strategy vs. State vs. Template Method

- **Strategy:** choose an algorithm, such as a shipping-cost calculation.
- **State:** behavior changes because the object moves through states, such as Draft, Submitted, and Approved.
- **Template Method:** the high-level algorithm stays fixed while subclasses customize selected steps.

### Observer vs. Mediator vs. Chain of Responsibility

- **Observer:** one subject notifies multiple interested subscribers.
- **Mediator:** a coordinating object manages communication among colleagues.
- **Chain of Responsibility:** a request is passed between handlers, typically in sequence.

### Command vs. Strategy

A Strategy represents an interchangeable way to perform a calculation or behavior. A Command represents a request as an object. A command can use a strategy internally, and a command can be queued or recorded, but these are different intents.

## Signs a Pattern May Be Unnecessary

Consider simplifying the design when:

- There is only one implementation and no credible reason to expect alternatives.
- A factory only forwards a constructor call and adds no useful boundary.
- The abstraction requires more code and concepts than the behavior it protects.
- Adding one small feature requires changing many interfaces and all their implementations.
- The pattern was introduced only to make the project appear more sophisticated.

## A Practical Decision Record

For each pattern example, write down:

- The concrete design problem.
- The simplest alternative considered.
- Why the pattern is justified in this scenario.
- The costs and limitations introduced.
- What would make you remove or replace the pattern later.
