# Learning Roadmap

The repository explores the 23 classic GoF patterns through three learning levels. Complete one pattern end to end before starting too many others; a small set of finished, well-tested examples is more useful than dozens of unfinished folders.

## Learning Levels

### Beginner: Understand the Intent

**Goal:** Demonstrate the essential problem and structure of the pattern without infrastructure noise.

Expected scope:

- One small, runnable application.
- The minimal interfaces and concrete types necessary to demonstrate the pattern.
- A client that uses the pattern.
- Unit tests for the behavior.
- A short explanation of the design before and after introducing the pattern.
- At least one example where the pattern would be unnecessary.

Avoid databases, message brokers, and external services unless they are genuinely necessary to explain the pattern.

### Intermediate: Apply It in an Application

**Goal:** Show how the pattern fits with application boundaries, dependency injection, configuration, and testing.

Expected scope:

- A realistic use case with clear acceptance criteria.
- Separation between domain or application logic and infrastructure where appropriate.
- Unit tests and integration tests where useful.
- Configuration and error handling.
- One or more infrastructure components only when they support the scenario, such as Redis for distributed caching or file processing for an import workflow.
- Documentation of relevant alternatives and trade-offs.

### Advanced: Investigate Difficult Cases

**Goal:** Explore the limits of the pattern under realistic engineering constraints.

Possible concerns include:

- Concurrency and thread safety.
- Failure recovery and retry behavior.
- Idempotency and duplicate handling.
- Performance and memory use.
- Observability and diagnostics.
- External service integration and configuration.
- Backward compatibility and extensibility.

Do not add a concern solely to make the example look advanced. For some patterns, a carefully tested in-process program is a better advanced example than a distributed system.

## Recommended Workflow

1. **Describe the problem.** Write acceptance criteria before selecting the pattern.
2. **Build the simplest working version.** Implement the basic use case without prematurely adding abstractions.
3. **Identify the design pressure.** Determine what is difficult to change, test, or extend.
4. **Introduce the pattern.** Keep only the abstractions that address that pressure.
5. **Compare the alternatives.** Explain why direct construction, a simple interface, or another pattern would or would not fit.
6. **Test behavior.** Cover the normal path, important edge cases, and failure modes appropriate to the level.
7. **Document the trade-offs.** State the additional complexity introduced by the pattern.
8. **Automate verification.** Run formatting, build, and tests locally and through CI.
9. **Review and release.** Verify setup instructions from a clean checkout before marking the example complete.

## Definition of Done

Do not mark a pattern implementation complete until:

- [ ] The code builds with the documented SDK.
- [ ] The example runs using the documented instructions.
- [ ] Tests pass and demonstrate the intended behavior.
- [ ] The README explains the intent, problem, participants, and flow.
- [ ] A decision record or equivalent section explains why the pattern was chosen.
- [ ] At least one alternative and the pattern's trade-offs are documented.
- [ ] Configuration contains no real credentials or secrets.
- [ ] The status in the pattern catalog reflects the actual state of the example.

## Suggested First Patterns

A useful first group is:

1. **Abstract Factory** — families of related products.
2. **Factory Method** — delegated creation of a product.
3. **Builder** — step-by-step construction.
4. **Strategy** — interchangeable algorithms.
5. **Adapter** — translating an existing interface.
6. **Decorator** — composing additional behavior.

These examples provide good opportunities to compare patterns with similar-looking structures. After this group, continue through the three GoF categories rather than skipping randomly based on technology interest.

## Versioning and Progress

- Keep each example independently runnable and testable.
- Update the catalog only after the corresponding code and documentation exist.
- Prefer small, focused commits with descriptive messages.
- Use releases to mark meaningful milestones, such as completion of a pattern category.
- Do not claim that the full collection is complete while examples remain planned or untested.
