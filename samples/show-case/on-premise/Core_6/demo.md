
# Demo

The solution can be used to demo various features of NServiceBus. This demo uses NHibernate for durable storage.

- [Recoverability](https://docs.particular.net/nservicebus/recoverability/)
 - [Immediate](https://docs.particular.net/nservicebus/recoverability/#immediate-retries) and [delayed](https://docs.particular.net/nservicebus/recoverability/#delayed-retries) retries
- [Property encryption](https://docs.particular.net/nservicebus/security/property-encryption)
- Temporal uncoupling - Ability to still send messages while receiver is down.
- Pubsub on transport that doesn't natively support pubsub.
- Message patterns
 - Commands
 - Pubsub
 - Request/Response (correlation) (Reply)
- Handlers
 - Stateless
- Long running processes / workflows / orchestration
 - [Sagas](https://docs.particular.net/nservicebus/sagas/)
 - Timeouts
 - ReplyToOriginator
- Invoking multiple handlers and/or sagas for a single message

Monitoring

- ServicePulse
 - [Custom checks](https://docs.particular.net/servicepulse/intro-endpoints-custom-checks) to check for environment conditions while no messages are processed
 - [Heartbeats](https://docs.particular.net/servicepulse/intro-endpoints-heartbeats)
- [Windows Performance Counter](https://docs.particular.net/nservicebus/operations/performance-counters)

Diagnostics

- Conversations
- [ServiceInsight](https://docs.particular.net/serviceinsight/)
- [Saga audit plugin](https://docs.particular.net/servicecontrol/plugins/saga-audit) to capture saga state and [viewing it](https://docs.particular.net/serviceinsight/#the-saga-view)

Logging

- Making use of [NLog](https://docs.particular.net/nservicebus/logging/nlog) for [customized logging](https://docs.particular.net/nservicebus/logging/#custom-logging)