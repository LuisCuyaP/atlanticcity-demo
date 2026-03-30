# Events Frontend (MVP)

Frontend mínimo para el reto técnico: registrar eventos (con zonas) y listar eventos.

## Requisitos

- Node.js 20+
- pnpm

## Configuración

Crea un archivo `.env.local` en la raíz con:

```bash
API_BASE_URL=http://localhost:5143
```

`API_BASE_URL` debe apuntar a tu backend que expone `POST /auth/login` y `GET/POST /events`.

## Ejecutar

```bash
pnpm install
pnpm dev
```

Abrir: http://localhost:3000

## Flujo de autenticación (JWT en cookie)

- `/login`: envía `{ userName, password }` a `POST /auth/login`. Se guarda el `accessToken` como cookie **HttpOnly** (no accesible por JavaScript).
- `/logout`: elimina la cookie.

## Consumo del backend (Bearer)

El frontend **no** llama directo al backend desde el navegador. En su lugar:

- `GET /api/events` y `POST /api/events` (Route Handlers) leen el JWT desde cookie HttpOnly y envían `Authorization: Bearer <jwt>` al backend.
- Si el backend retorna error (404/500/etc) se propaga al frontend con el mensaje del API.

## Pantallas

- `/events`: lista eventos.
- `/events/new`: formulario “Registrar Evento” con zonas editables y validación mínima.

## Nota

Este MVP asume que ya tienes un JWT válido para el backend; el login no hace intercambio usuario/clave.

## Payload POST /events

El frontend envía el body alineado al backend:

```json
{
	"Name": "Mi evento",
	"EventDate": "2026-03-29T23:42",
	"Place": "Lima",
	"Zones": [
		{ "Name": "General", "Price": 0, "Capacity": 100 }
	]
}
```
