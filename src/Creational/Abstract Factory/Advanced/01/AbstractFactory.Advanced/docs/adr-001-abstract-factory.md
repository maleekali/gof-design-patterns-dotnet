# ADR-001: Use Abstract Factory for broker product families

- **Status:** Accepted for this learning project
- **Context:** The ingestion pipeline supports RabbitMQ and Kafka. Broker choice affects the publisher, event destination encoding, and failure publishing destination.
- **Decision:** Introduce `IMessageBrokerFactory` and concrete `RabbitMqFactory` / `KafkaFactory` implementations. Select one factory at application composition time.
- **Consequences:** The Worker is insulated from broker-specific classes. Adding a broker requires a transport, an encoder, a failure publisher, a factory, configuration, and tests. This is extra code, justified here because the repository explicitly demonstrates family creation and broker substitution.
- **Alternatives:** If only a single publisher implementation were needed, injecting `IMessagePublisher` directly would be simpler. Abstract Factory would then be unnecessary ceremony.
- **Known limitation:** The factory does not make cross-system publication and Redis state updates atomic. Delivery is at least once; consumers must be idempotent.
