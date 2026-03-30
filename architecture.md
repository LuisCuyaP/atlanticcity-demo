# Arquitectura (events.backend + notification.backend)

Este documento describe la arquitectura del sistema de eventos y notificaciones.

## Componentes / servicios

- **events.backend.API** (HTTP)
  - Endpoints principales:
    - `POST /auth/login` (anónimo): emite JWT.
    - `GET /events` (JWT requerido): consulta eventos (cache-aside en Redis).
    - `POST /events` (JWT requerido + rate limiting): crea eventos.
  - Middleware:
    - Rate limiting (por IP) aplicado a `POST /events`.
    - Health check en `GET /health`.
  - Publica eventos de integración a RabbitMQ vía MassTransit.

- **notification.backend** (worker/consumer, no expone API HTTP)
  - Consume el evento de integración `EventCreatedIntegrationEvent` desde RabbitMQ.
  - Ejecuta `EventCreatedIntegrationEventHandler` para procesar la notificación (p.ej. email) y deduplicar mensajes.

- **RabbitMQ** (Message Broker)
  - Transporte de eventos asíncronos (publish/consume) entre servicios.

- **SQL Server (EventsDb)**
  - Persistencia del agregado `Event` y sus `Zones`.

- **Redis (distributedcache)**
  - Cache distribuida para optimizar lecturas (`GET /events`).

- **Proveedor de Email (SMTP/Email Gateway)**
  - Usado por `notification.backend` para enviar notificaciones.

## Microservicios

- **events.backend**
  - Servicio API para administración/consulta de eventos.
- **notification.backend**
  - Servicio worker para notificaciones (consume eventos y ejecuta handlers).


## Flujos

### 1) HTTP síncrono (consulta)

1. El cliente llama `GET /events` con JWT.
2. `events.backend.API` intenta resolver desde Redis (cache-aside).
3. Si no existe en cache, consulta SQL Server, responde y guarda en Redis.

### 2) HTTP síncrono (creación)

1. El cliente llama `POST /events` con JWT.
2. La API valida el request y persiste el agregado `Event` en SQL Server.
3. Se invalida cache relacionada a listados de eventos.
4. Se levanta un **domain event** y desde Application se mapea a un **integration event**.

### 3) Eventos asíncronos (notificación)

1. `events.backend` publica `EventCreatedIntegrationEvent` en RabbitMQ (MassTransit).
2. `notification.backend` consume el mensaje.
3. `EventCreatedIntegrationEventHandler` procesa la notificación (por ejemplo, envío de correo) y aplica idempotencia/deduplicación.

## Seguridad (notas)

- **JWT Bearer**
  - `POST /auth/login` emite `accessToken`.
  - Endpoints de eventos requieren `Authorization: Bearer <token>`.

- **Roles**
  - El JWT incluye claims de rol (`ClaimTypes.Role`).
  - Se pueden crear policies (p.ej. `RequireRole("Admin")`) para restringir creación/operaciones administrativas.

- **Boundaries / Trust boundaries**
  - `events.backend` es el límite de entrada (HTTP) y debe validar/authN/authZ.
  - `notification.backend` no expone HTTP; su frontera es el broker.
  - Entre servicios, el broker es **zona no confiable**: los consumers deben validar payloads y aplicar idempotencia.

- **Secretos**
  - `Jwt:Key`, credenciales de broker/DB y secretos SMTP deben provenir de secretos/variables de entorno en producción (evitar valores hardcodeados).

## Sustentación (breve)

- Se separa **comando** (crear evento) de **side-effects** (notificar) usando eventos asíncronos, mejorando desacoplamiento y escalabilidad.
- Redis reduce latencia y presión sobre SQL Server en lecturas frecuentes.
- RabbitMQ + MassTransit habilitan reintentos y tolerancia a fallos en el flujo de notificaciones.
- JWT centraliza autenticación para la API y permite autorización por roles con baja fricción.
